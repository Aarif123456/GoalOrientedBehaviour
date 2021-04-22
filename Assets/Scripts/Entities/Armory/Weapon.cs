using Common;
using Entities.Triggers;
using GameWorld.Managers;
using UnityEngine;
using Utility.Fuzzy;

namespace Entities.Armory {
    public abstract class Weapon {
        protected int activeProjectileCount;

        protected Weapon(
            WeaponTypes weaponType,
            int roundsRemaining,
            int maximumRoundsCarried,
            float rateOfFire,
            float idealRange,
            float projectileSpeed,
            Agent agent){
            FuzzyModule = new FuzzyModule();
            WeaponType = weaponType;
            RoundsRemaining = roundsRemaining;
            Agent = agent;
            RateOfFire = rateOfFire;
            MaximumRoundsCarried = maximumRoundsCarried;
            LastDesirabilityScore = 0;
            IdealRange = idealRange;
            MaximumProjectileSpeed = projectileSpeed;
            TimeNextAvailable = Time.time;
        }

        public Agent Agent { get; }

        public WeaponTypes WeaponType { get; }

        public int RoundsRemaining { get; private set; }

        public float RateOfFire { get; }

        public int MaximumRoundsCarried { get; }

        public float MaximumProjectileSpeed { get; }

        public float IdealRange { get; }

        public float LastDesirabilityScore { get; set; }

        public float TimeNextAvailable { get; private set; }

        public FuzzyModule FuzzyModule { get; set; }

        public bool AimAt(Vector3 targetPosition){
            return Agent.RotateAimTowardPosition(targetPosition);
        }

        public abstract void ShootAt(Vector3 targetPosition);

        public abstract float GetDesirability(float distanceToTarget);

        public virtual void OnProjectileAdded(){
            activeProjectileCount++;
        }

        public virtual void OnProjectileRemoved(){
            activeProjectileCount--;

            if (activeProjectileCount < 0) activeProjectileCount = 0;
        }

        public void DecrementRounds(){
            if (RoundsRemaining > 0) RoundsRemaining -= 1;
        }

        public void IncrementRounds(int amount){
            RoundsRemaining = Mathf.Clamp(RoundsRemaining + amount, 0, MaximumRoundsCarried);
        }

        protected bool IsReadyForNextShot(){
            return Time.time > TimeNextAvailable;
        }

        protected void UpdateTimeNextAvailable(){
            TimeNextAvailable = Time.time + 1.0f / RateOfFire;
        }

        protected abstract void InitializeFuzzyModule();

        protected void InitializeDistanceToTarget(out FzSet targetClose, out FzSet targetMedium, out FzSet targetFar){
            var distanceToTarget = FuzzyModule.CreateFlv("distanceToTarget");

            targetClose = distanceToTarget.AddLeftShoulderSet("targetClose", 0, 10, 20);
            targetMedium = distanceToTarget.AddTriangularSet("targetMedium", 10, 30, 50);
            targetFar = distanceToTarget.AddRightShoulderSet("targetFar", 30, 60, 100);
        }

        protected void InitializeDesirability(out FzSet undesirable, out FzSet desirable, out FzSet veryDesirable){
            var desirability = FuzzyModule.CreateFlv("desirability");

            veryDesirable = desirability.AddRightShoulderSet("veryDesirable", 50, 75, 100);
            desirable = desirability.AddTriangularSet("Desirable", 25, 50, 75);
            undesirable = desirability.AddLeftShoulderSet("undesirable", 0, 25, 50);
        }

        protected void AddSoundTrigger(Agent soundSource, float range){
            var triggerObject = Object.Instantiate(GameManager.Instance.soundNotifyTriggerPrefab);
            var trigger = triggerObject.GetComponent<TriggerSoundNotify>();
            trigger.GetComponent<MeshRenderer>().enabled = Parameters.Instance.SoundTriggerVisible;
            trigger.NoiseMakingAgent = soundSource;
            triggerObject.transform.position = soundSource.Kinematic.Position;
            triggerObject.transform.localScale =
                new Vector3(range, 5, range); // y limited to 5 because overhead camera is at 10???
        }
    }
}