using System.Collections;
using Common;
using Entities.Armory;
using Entities.GoalOrientedBehaviour;
using Entities.Memory.SensoryMemory;
using GameWorld;
using UnityEngine;

namespace Entities {
    public class Agent : MovingEntity {
        public Color color;
        public string shortName;

        private readonly Color _flashColor = Color.yellow;
        private Material[] _normalMaterial, _flashMaterial;
        private Renderer _renderer;

        public Think Brain { get; private set; }

        public SensoryMemory SensoryMemory { get; private set; }

        public TargetingSystem TargetingSystem { get; private set; }

        public WeaponSystem WeaponSystem { get; private set; }

        public PathPlanner PathPlanner { get; private set; }

        public PathManager PathManager => PathPlanner.PathManager;

        public bool IsStrafing { get; set; }

        public float FieldOfView { get; protected set; }

        private Regulator GoalArbitrationRegulator { get; set; }

        private Regulator VisionUpdateRegulator { get; set; }

        private Regulator TargetSelectionRegulator { get; set; }

        private Regulator WeaponSelectionRegulator { get; set; }

        public int Score { get; set; }

        private float HitIndicatorTimer { get; set; }

        private bool Hit { get; set; }

        public int Health { get; set; }

        public int MaximumHealth { get; protected set; }

        public override void Awake(){
            base.Awake();
            _renderer = GetComponent<Renderer>();
            var parameters = Parameters.Instance;

            SensoryMemory = new SensoryMemory(this, parameters.AgentMemorySpan, parameters.FriendlyFire);
            TargetingSystem = new TargetingSystem(this);
            WeaponSystem =
                new WeaponSystem(
                    this,
                    parameters.AgentReactionTime,
                    parameters.AgentAimAccuracy,
                    parameters.AgentAimPersistenceTime);
            PathPlanner = GetComponent<PathPlanner>();

            MaximumHealth = parameters.AgentMaximumHealth;
            Health = 10;
            HitIndicatorTimer = (int) parameters.HitFlashTime;
            Hit = false;
            Score = 0;

            GoalArbitrationRegulator =
                new Regulator(parameters.AgentGoalAppraisalUpdateFrequency);
            VisionUpdateRegulator =
                new Regulator(parameters.AgentVisionUpdateFrequency);
            TargetSelectionRegulator =
                new Regulator(parameters.AgentTargetingUpdateFrequency);
            WeaponSelectionRegulator =
                new Regulator(parameters.AgentWeaponSelectionFrequency);

            Brain = new Think(this);

            FieldOfView = parameters.AgentFieldOfView;

            _normalMaterial = _renderer.materials;
            _flashMaterial = CreateFlashMaterial();
        }

        private void OnEnable(){
            EventManager.Instance.Subscribe<DamageInflictedEventPayload>(
                Events.DAMAGE_INFLICTED,
                OnDamageInflicted);

            EventManager.Instance.Subscribe<EntityDestroyedEventPayload>(
                Events.ENTITY_DESTROYED,
                OnEntityDestroyed);

            EventManager.Instance.Subscribe<WeaponSoundEventPayload>(
                Events.WEAPON_SOUND,
                OnWeaponSound);
        }

        private void OnDisable(){
            EventManager.Instance.Unsubscribe<DamageInflictedEventPayload>(
                Events.PATH_TO_POSITION_REQUEST,
                OnDamageInflicted);

            EventManager.Instance.Unsubscribe<EntityDestroyedEventPayload>(
                Events.ENTITY_DESTROYED,
                OnEntityDestroyed);

            EventManager.Instance.Unsubscribe<WeaponSoundEventPayload>(
                Events.WEAPON_SOUND,
                OnWeaponSound);
        }

        private Material[] CreateFlashMaterial(){
            var materials = new Material[_renderer.materials.Length];
            for (var i = 0; i < materials.Length; i++){
                materials[i] = new Material(_renderer.materials[i]){color = _flashColor};
            }

            return materials;
        }

        protected override void Think(float deltaTime){
            base.Think(deltaTime);

            Brain.Process();

            if (TargetSelectionRegulator.IsReady) TargetingSystem.Update();

            if (GoalArbitrationRegulator.IsReady) Brain.Arbitrate();

            if (VisionUpdateRegulator.IsReady) SensoryMemory.UpdateVision();

            if (WeaponSelectionRegulator.IsReady) WeaponSystem.SelectWeapon();
        }

        protected override void Act(float deltaTime){
            base.Act(deltaTime);

            WeaponSystem.TakeAimAndShoot(deltaTime);
        }

        public bool IsTargetInFieldOfView(Vector3 targetPosition){
            var angle = Vector3.Angle(targetPosition - Kinematic.Position, transform.forward);
            return angle < FieldOfView / 2;
        }

        public bool SameTeam(Agent agent){
            return color == agent.color;
        }

        public bool CanMoveTo(Vector3 endPosition){
            return CanMoveBetween(Kinematic.Position, endPosition);
        }

        public bool CanMoveTo(Vector3 endPosition, float satisfactionRadius){
            return CanMoveBetween(Kinematic.Position, endPosition, satisfactionRadius,
                PathManager.PathObstaclesLayerMask);
        }

        public bool CanMoveTo(Vector3 endPosition, float satisfactionRadius, LayerMask obstacleLayers){
            return CanMoveBetween(Kinematic.Position, endPosition, satisfactionRadius, obstacleLayers);
        }

        public bool CanMoveBetween(Vector3 startPosition, Vector3 endPosition){
            return CanMoveBetween(startPosition, endPosition, PathManager.CloseEnoughDistance,
                PathManager.PathObstaclesLayerMask);
        }

        public bool CanMoveBetween(Vector3 startPosition, Vector3 endPosition, float satisfactionRadius){
            return CanMoveBetween(startPosition, endPosition, satisfactionRadius, PathManager.PathObstaclesLayerMask);
        }

        public bool CanMoveBetween(Vector3 startPosition, Vector3 endPosition, float satisfactionRadius,
            LayerMask obstacleLayers){
            var distance = Vector3.Distance(startPosition, endPosition);

            if (distance <= satisfactionRadius) return true;

            var direction = (endPosition - startPosition) / distance;

            var blocked = Physics.CapsuleCast(
                Kinematic.Bottom + Kinematic.Radius * Vector3.up,
                Kinematic.Top - Kinematic.Radius * Vector3.up,
                Kinematic.Radius,
                direction,
                distance,
                obstacleLayers);

            return !blocked;
        }

        public bool CanStepRight(){
            return CanStepRight(PathManager.CloseEnoughDistance, PathManager.PathObstaclesLayerMask);
        }

        public bool CanStepRight(float satisfactionRadius){
            return CanStepRight(satisfactionRadius, PathManager.PathObstaclesLayerMask);
        }

        public bool CanStepRight(float satisfactionRadius, LayerMask obstacleLayers){
            return CanStepRight(satisfactionRadius, obstacleLayers, out _);
        }

        public bool CanStepRight(out Vector3 positionOfStep){
            return CanStepRight(PathManager.CloseEnoughDistance, PathManager.PathObstaclesLayerMask,
                out positionOfStep);
        }

        public bool CanStepRight(float satisfactionRadius, out Vector3 positionOfStep){
            return CanStepRight(satisfactionRadius, PathManager.PathObstaclesLayerMask, out positionOfStep);
        }

        public bool CanStepRight(float satisfactionRadius, LayerMask obstacleLayers, out Vector3 positionOfStep){
            var stepDistance = Kinematic.Radius * 2;

            positionOfStep = Kinematic.Position + stepDistance * transform.right;

            return CanMoveTo(positionOfStep, satisfactionRadius, obstacleLayers);
        }

        public bool CanStepLeft(){
            return CanStepLeft(PathManager.CloseEnoughDistance, PathManager.PathObstaclesLayerMask);
        }

        public bool CanStepLeft(float satisfactionRadius){
            return CanStepLeft(satisfactionRadius, PathManager.PathObstaclesLayerMask);
        }

        public bool CanStepLeft(float satisfactionRadius, LayerMask obstacleLayers){
            return CanStepLeft(satisfactionRadius, obstacleLayers, out _);
        }

        public bool CanStepLeft(out Vector3 positionOfStep){
            return CanStepLeft(PathManager.CloseEnoughDistance, PathManager.PathObstaclesLayerMask, out positionOfStep);
        }

        public bool CanStepLeft(float satisfactionRadius, out Vector3 positionOfStep){
            return CanStepLeft(satisfactionRadius, PathManager.PathObstaclesLayerMask, out positionOfStep);
        }

        public bool CanStepLeft(float satisfactionRadius, LayerMask obstacleLayers, out Vector3 positionOfStep){
            var stepDistance = Kinematic.Radius * 2;

            positionOfStep = Kinematic.Position - stepDistance * transform.right;

            return CanMoveTo(positionOfStep, satisfactionRadius, obstacleLayers);
        }

        public override void Spawn(Vector3 spawnPoint){
            base.Spawn(spawnPoint);

            Brain.RemoveAllSubgoals();
            TargetingSystem.ClearTarget();
            WeaponSystem.Initialize();
            RestoreHealthToMaximum();
        }

        public void IncrementScore(){
            ++Score;
        }

        /// <summary>
        ///     Restore health to <see cref="MaximumHealth" />.
        /// </summary>
        public void RestoreHealthToMaximum(){
            Health = MaximumHealth;
        }

        /// <summary>
        ///     Increase health by the given amount.
        /// </summary>
        /// <param name="amount">The amount to add.</param>
        public void IncreaseHealth(int amount){
            Health = Mathf.Clamp(Health + amount, 0, MaximumHealth);
        }


        private void ChangeColor(){
            _renderer.materials = Hit ? _flashMaterial : _normalMaterial;
        }

        private IEnumerator Flash(){
            ChangeColor();
            Hit = false;
            yield return new WaitForSeconds(HitIndicatorTimer);
            ChangeColor();
            Invoke(nameof(ChangeColor), HitIndicatorTimer);
        }

        /// <summary>
        ///     Reduce health by the given amount. Flash the hit indicator.
        /// </summary>
        /// <param name="amount">The amount to reduce health by.</param>
        private void ReduceHealth(int amount){
            Health -= amount;
            if (Health <= 0) SetDead();
            Hit = true;
            StartCoroutine(nameof(Flash));
        }

        public bool RotateAimTowardPosition(Vector3 target){
            var angle = Vector3.Angle(Kinematic.Position, target);
            const float weaponAimTolerance = 5; // degrees

            if (angle < weaponAimTolerance){
                Kinematic.Rotation = Quaternion.LookRotation(target).eulerAngles;
                return true;
            }

            Kinematic.Rotation =
                Quaternion.RotateTowards(
                    Quaternion.Euler(Kinematic.Rotation),
                    Quaternion.LookRotation(target),
                    Kinematic.MaximumAngularSpeed).eulerAngles;

            return false;
        }

        private void OnDamageInflicted(Event<DamageInflictedEventPayload> eventArg){
            var payload = eventArg.EventData;

            if (payload.victim != this) // event not for us
                return;

            if (IsDead || IsSpawning) return;

            ReduceHealth((int) payload.damageInflicted);

            if (IsDead){
                EventManager.Instance.Enqueue(
                    Events.ENTITY_DESTROYED,
                    new EntityDestroyedEventPayload(payload.shooter, payload.victim));
            }
        }

        private void OnEntityDestroyed(Event<EntityDestroyedEventPayload> eventArg){
            var payload = eventArg.EventData;

            if (payload.shooter != this) // event not for us
                return;

            IncrementScore();

            if (TargetingSystem.Target == payload.victim) TargetingSystem.ClearTarget();
        }

        private void OnWeaponSound(Event<WeaponSoundEventPayload> eventArg){
            var payload = eventArg.EventData;

            if (payload.noiseHearer != this) // event not for us
                return;

            SensoryMemory.UpdateWithSoundSource(payload.noiseMaker);
        }
    }
}