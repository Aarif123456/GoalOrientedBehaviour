using GameWorld.Managers;
using UnityEngine;
using Utility.Fuzzy;

namespace Entities.Armory {
    public sealed class WeaponBlaster : Weapon {
        public WeaponBlaster(Agent agent)
            : base(
                WeaponTypes.Blaster,
                Parameters.Instance.BlasterDefaultRounds,
                Parameters.Instance.BlasterMaxRoundsCarried,
                Parameters.Instance.BlasterFiringFrequency,
                Parameters.Instance.BlasterIdealRange,
                Parameters.Instance.BlasterMaximumSpeed,
                agent){
            // setup the fuzzy module
            InitializeFuzzyModule();
        }

        public override void ShootAt(Vector3 targetPosition){
            if (!Parameters.Instance.UseBlaster || !IsReadyForNextShot()){
                return;
            }

            AddBolt(Agent, targetPosition);

            UpdateTimeNextAvailable();

            AddSoundTrigger(Agent, Parameters.Instance.BlasterSoundRange);
        }

        public override float GetDesirability(float distanceToTarget){
            // fuzzify distance and amount of ammo
            FuzzyModule.Fuzzify("distanceToTarget", distanceToTarget);

            LastDesirabilityScore =
                FuzzyModule.DeFuzzify("desirability", FuzzyModule.DefuzzifyMethod.MaxAv);

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

            FuzzyModule.AddRule(targetClose, desirable);
            FuzzyModule.AddRule(targetMedium, new FzVery(undesirable));
            FuzzyModule.AddRule(targetFar, new FzVery(undesirable));
        }


        private void AddBolt(Agent shooter, Vector3 target){
            if (activeProjectileCount < Parameters.Instance.MaximumActiveBolts){
                OnProjectileAdded();

                var boltObject = Object.Instantiate(GameManager.Instance.boltPrefab);
                boltObject.GetComponent<Rigidbody>().mass = Parameters.Instance.BoltMass;
                var bolt = boltObject.AddComponent<ProjectileBolt>();
                bolt.Spawn(this, shooter, target);
            }
        }
    }
}