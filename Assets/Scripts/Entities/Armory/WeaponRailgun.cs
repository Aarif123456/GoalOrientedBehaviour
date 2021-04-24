using Common;
using GameWorld.Managers;
using UnityEngine;
using Utility.Fuzzy;

namespace Entities.Armory {
    public sealed class WeaponRailgun : Weapon {
        public WeaponRailgun(Agent agent)
            : base(
                WeaponTypes.Railgun,
                Parameters.Instance.RailgunDefaultRounds,
                Parameters.Instance.RailgunMaximumRoundsCarried,
                Parameters.Instance.RailgunFiringFrequency,
                Parameters.Instance.RailgunIdealRange,
                Parameters.Instance.SlugMaximumSpeed,
                agent){
            // setup the fuzzy module
            InitializeFuzzyModule();
        }

        public override void ShootAt(Vector3 targetPosition){
            if (RoundsRemaining <= 0 || !IsReadyForNextShot()) return;

            AddSlug(Agent, targetPosition);

            DecrementRounds();

            UpdateTimeNextAvailable();

            AddSoundTrigger(Agent, Parameters.Instance.RailgunSoundRange);
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
            InitializeDistanceToTarget(out var targetClose, out var targetMedium, out var targetFar);

            InitializeDesirability(out var undesirable, out var desirable, out var veryDesirable);

            var ammoStatus = FuzzyModule.CreateFlv("ammoStatus");
            var ammoLoads = ammoStatus.AddRightShoulderSet("ammoLoads", 8, 12, 15);
            var ammoOkay = ammoStatus.AddTriangularSet("ammoOkay", 0, 5, 10);
            var ammoLow = ammoStatus.AddTriangularSet("ammoLow", 0, 0, 4);

            FuzzyModule.AddRule(new FzAnd(targetClose, ammoLoads), new FzFairly(desirable));
            FuzzyModule.AddRule(new FzAnd(targetClose, ammoOkay), new FzFairly(desirable));
            FuzzyModule.AddRule(new FzAnd(targetClose, ammoLow), undesirable);

            FuzzyModule.AddRule(new FzAnd(targetMedium, ammoLoads), veryDesirable);
            FuzzyModule.AddRule(new FzAnd(targetMedium, ammoOkay), desirable);
            FuzzyModule.AddRule(new FzAnd(targetMedium, ammoLow), desirable);
            FuzzyModule.AddRule(new FzAnd(targetFar, ammoLoads), new FzVery(veryDesirable));
            FuzzyModule.AddRule(new FzAnd(targetFar, ammoOkay), new FzVery(veryDesirable));
            FuzzyModule.AddRule(new FzAnd(targetFar, new FzFairly(ammoLow)), veryDesirable);
        }

        private void AddSlug(Agent shooter, Vector3 target){
            if (activeProjectileCount >= Parameters.Instance.MaximumActiveSlugs) return;
            OnProjectileAdded();

            var slugObject = Object.Instantiate(GameManager.Instance.slugPrefab);
            slugObject.GetComponent<Rigidbody>().mass = Parameters.Instance.SlugMass;
            var slug = slugObject.AddComponent<ProjectileSlug>();
            slug.Spawn(this, shooter, target);
        }
    }
}