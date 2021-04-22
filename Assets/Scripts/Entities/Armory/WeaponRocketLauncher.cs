using GameWorld.Managers;
using UnityEngine;
using Utility.Fuzzy;

namespace Entities.Armory {
    public sealed class WeaponRocketLauncher : Weapon {
        public WeaponRocketLauncher(Agent agent)
            : base(
                WeaponTypes.RocketLauncher,
                Parameters.Instance.RocketLauncherDefaultRounds,
                Parameters.Instance.RocketLauncherMaximumRoundsCarried,
                Parameters.Instance.RocketLauncherFiringFrequency,
                Parameters.Instance.RocketLauncherIdealRange,
                Parameters.Instance.RocketMaximumSpeed,
                agent){
            // setup the fuzzy module
            InitializeFuzzyModule();
        }

        public override void ShootAt(Vector3 targetPosition){
            if (RoundsRemaining <= 0 || !IsReadyForNextShot()) return;

            AddRocket(Agent, targetPosition);

            DecrementRounds();

            UpdateTimeNextAvailable();

            AddSoundTrigger(Agent, Parameters.Instance.RocketLauncherSoundRange);
        }

        public override float GetDesirability(float distanceToTarget){
            if (RoundsRemaining == 0)
                LastDesirabilityScore = 0;
            else{
                // fuzzify distance and amount of ammo
                FuzzyModule.Fuzzify("distanceToTarget", distanceToTarget);
                FuzzyModule.Fuzzify("ammoStatus", RoundsRemaining);
                LastDesirabilityScore =
                    FuzzyModule.DeFuzzify("desirability", FuzzyModule.DefuzzifyMethod.MaxAv);
            }

            return LastDesirabilityScore;
        }

        protected override void InitializeFuzzyModule(){
            FzSet targetClose;
            FzSet targetMedium;
            FzSet targetFar;

            InitializeDistanceToTarget(out targetClose, out targetMedium, out targetFar);

            FzSet undesirable;
            FzSet desirable;
            FzSet veryDesirable;

            InitializeDesirability(out undesirable, out desirable, out veryDesirable);

            var ammoStatus = FuzzyModule.CreateFlv("ammoStatus");
            var ammoLoads = ammoStatus.AddRightShoulderSet("ammoLoads", 10, 12, 15);
            var ammoOkay = ammoStatus.AddTriangularSet("ammoOkay", 5, 10, 12);
            var ammoLow = ammoStatus.AddTriangularSet("ammoLow", 0, 2, 6);

            FuzzyModule.AddRule(new FzAnd(targetClose, ammoLoads), undesirable);
            FuzzyModule.AddRule(new FzAnd(targetClose, ammoOkay), undesirable);
            FuzzyModule.AddRule(new FzAnd(targetClose, ammoLow), undesirable);

            FuzzyModule.AddRule(new FzAnd(targetMedium, ammoLoads), veryDesirable);
            FuzzyModule.AddRule(new FzAnd(targetMedium, ammoOkay), veryDesirable);
            FuzzyModule.AddRule(new FzAnd(targetMedium, ammoLow), desirable);
            FuzzyModule.AddRule(new FzAnd(targetFar, ammoLoads), desirable);
            FuzzyModule.AddRule(new FzAnd(targetFar, ammoOkay), undesirable);
            FuzzyModule.AddRule(new FzAnd(targetFar, ammoLow), undesirable);
        }

        private void AddRocket(Agent shooter, Vector3 target){
            if (activeProjectileCount >= Parameters.Instance.MaximumActiveRockets) return;
            OnProjectileAdded();

            var rocketObject = Object.Instantiate(GameManager.Instance.rocketPrefab);
            rocketObject.GetComponent<Rigidbody>().mass = Parameters.Instance.RocketMass;
            var rocket = rocketObject.AddComponent<ProjectileRocket>();
            rocket.Spawn(this, shooter, target);
        }
    }
}