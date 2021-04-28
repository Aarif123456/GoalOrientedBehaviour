using System.Collections.Generic;
using UnityEngine;

namespace Entities.Armory {
    public sealed class WeaponSystem {
        private readonly Dictionary<WeaponTypes, Weapon> _weaponMap =
            new Dictionary<WeaponTypes, Weapon>();

        private float _bestDesirability;

        /// <summary>
        ///     A flag that is toggle each time weapon selection is called.
        /// </summary>
        private bool _weaponSelectionTickTock;
        //private WeaponTypes mostDesirable = WeaponTypes.Blaster;
        //private WeaponTypes previousMostDesirable = WeaponTypes.Blaster;

        public WeaponSystem(
            Agent agent,
            float reactionTime,
            float aimAccuracy,
            float aimPersistenceTime){
            Agent = agent;
            ReactionTime = reactionTime;
            AimAccuracy = aimAccuracy;
            AimPersistenceTime = aimPersistenceTime;
            Initialize();
        }

        public Agent Agent { get; }

        public Weapon CurrentWeapon { get; set; }

        public Vector3 AimingPosition { get; private set; }

        public float ReactionTime { get; }

        public float AimAccuracy { get; }

        public float AimPersistenceTime { get; }

        public void Initialize(){
            CurrentWeapon = new WeaponBlaster(Agent);

            _weaponMap.Clear();
            _weaponMap[WeaponTypes.Blaster] = CurrentWeapon;
            _weaponMap[WeaponTypes.Shotgun] = null;
            _weaponMap[WeaponTypes.Railgun] = null;
            _weaponMap[WeaponTypes.RocketLauncher] = null;
        }

        public void SelectWeapon(){
            // if a target is present use fuzzy logic to determine the most desirable weapon.
            if (Agent.TargetingSystem.IsTargetPresent){
                _weaponSelectionTickTock = !_weaponSelectionTickTock;
                //previousMostDesirable = mostDesirable;

                // calculate the distance to the target
                var distanceToTarget = Vector3.Distance(Agent.Kinematic.Position,
                    Agent.TargetingSystem.Target.Kinematic.Position);

                // for each weapon in the inventory calculate its desirability
                // given the current situation. The most desirable weapon is selected
                _bestDesirability = float.MinValue;

                foreach (var kvp in _weaponMap){
                    // grab the desirability of this weapon (desirability is
                    // based upon distance to target and ammo remaining)
                    if (kvp.Value == null) continue;

                    var score = kvp.Value.GetDesirability(distanceToTarget);

                    // if it is the most desirable so far select it
                    if (score <= _bestDesirability) continue;

                    _bestDesirability = score;

                    // place the weapon in the agent's hand.
                    CurrentWeapon = kvp.Value;
                    //mostDesirable = kvp.Key;
                }
            }
            else
                CurrentWeapon = _weaponMap[WeaponTypes.Blaster];
        }

        public void AddWeapon(WeaponTypes weaponType){
            // create an instance of this weapon

            Weapon weaponToAdd = weaponType switch{
                WeaponTypes.Railgun => new WeaponRailgun(Agent),
                WeaponTypes.Shotgun => new WeaponShotgun(Agent),
                WeaponTypes.RocketLauncher => new WeaponRocketLauncher(Agent),
                _ => null
            };

            if (weaponToAdd == null) return;

            // if the agent already holds a weapon of this type, just add its ammo
            var weaponInInventory = GetWeaponFromInventory(weaponType);

            if (weaponInInventory != null)
                weaponInInventory.IncrementRounds(weaponToAdd.RoundsRemaining);

            // if not already holding, add to inventory
            else
                _weaponMap[weaponType] = weaponToAdd;
        }

        public Weapon GetWeaponFromInventory(WeaponTypes weaponType){
            return _weaponMap[weaponType];
        }

        public void ChangeWeapon(WeaponTypes weaponType){
            var weaponInInventory = GetWeaponFromInventory(weaponType);

            if (weaponInInventory != null) CurrentWeapon = weaponInInventory;
        }

        public void TakeAimAndShoot(float deltaTime){
            // aim the weapon only if the current target is shootable or if it 
            // has only very recently gone out of view (this latter condition is
            // to ensure the weapon is aimed at the target even if it temporarily
            // dodges behind a wall or other cover)
            if (!Agent.TargetingSystem.IsTargetShootable &&
                !(Agent.TargetingSystem.TimeTargetOutOfView < AimPersistenceTime)) return;
            // the position the weapon will be aimed at
            AimingPosition = Agent.TargetingSystem.Target.Kinematic.Position;

            // if the current weapon is not an instant hit type gun the
            // target position must be adjusted to take into account the
            // predicted movement of the target
            if (CurrentWeapon.WeaponType == WeaponTypes.RocketLauncher ||
                CurrentWeapon.WeaponType == WeaponTypes.Blaster){
                AimingPosition = PredictFuturePositionOfTarget(deltaTime);

                // if the weapon is aimed correctly, there is line of sight between 
                // the agent and the aiming position and it has been in view for a
                // period longer than the agent's reaction time, shoot the weapon
                if (!(Agent.TargetingSystem.TimeTargetVisible > ReactionTime) ||
                    !Agent.HasLineOfSight(AimingPosition)) return;
                AddNoiseToAim();
                ShootAt(AimingPosition);
            }
            // no need to predict movement, aim directly at target
            else{
                // if the weapon is aimed correctly and it has been in view for
                // a period longer than the agent's reaction time, shoot the weapon
                if (!(Agent.TargetingSystem.TimeTargetVisible > ReactionTime)) return;
                AddNoiseToAim();
                ShootAt(AimingPosition);
            }
            // no target to shoot at so rotate aim to be parallel with the
            // agent's heading direction
        }

        public void AddNoiseToAim(){
            var direction = AimingPosition - Agent.Kinematic.Position;
            direction = Quaternion.Euler(0, Random.Range(-AimAccuracy, AimAccuracy), 0) * direction;
            AimingPosition = Agent.Kinematic.Position + direction;
        }

        public Vector3 PredictFuturePositionOfTarget(float deltaTime){
            var maximumSpeed = CurrentWeapon.MaximumProjectileSpeed;

            // if the target is ahead and facing the agent shoot at its current position
            var vectorToEnemy = Agent.TargetingSystem.Target.Kinematic.Position - Agent.Kinematic.Position;

            // the lookahead time is proportional to the distance between the
            // enemy and the pursuer; and is inversely proportional to the sum
            // of the agents' velocities

            var lookAheadTime = vectorToEnemy.magnitude /
                                (maximumSpeed + Agent.TargetingSystem.Target.Kinematic.MaximumSpeed);

            // return the predicted future position of the enemy
            return Agent.TargetingSystem.Target.Kinematic.Position +
                   Agent.TargetingSystem.Target.Kinematic.Velocity * (lookAheadTime * deltaTime);
        }

        public int GetRoundsRemaining(WeaponTypes weaponType){
            return _weaponMap[weaponType] != null ? _weaponMap[weaponType].RoundsRemaining : 0;
        }

        public void ShootAt(Vector3 targetPosition){
            CurrentWeapon.ShootAt(targetPosition);
        }
    }
}