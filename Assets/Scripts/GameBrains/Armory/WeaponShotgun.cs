namespace GameBrains.AI
{
    using System.Collections.Generic;
    
    using UnityEngine;
    
    public sealed class WeaponShotgun : Weapon
    {
        public WeaponShotgun(Agent agent)
            : base(
                WeaponTypes.Shotgun, 
                Parameters.Instance.ShotgunDefaultRounds, 
                Parameters.Instance.ShotgunMaximumRoundsCarried, 
                Parameters.Instance.ShotgunFiringFrequency, 
                Parameters.Instance.ShotgunIdealRange, 
                Parameters.Instance.PelletMaximumSpeed, 
                agent)
        {
            Targets = new List<Vector3>();
            BallsInShell = Parameters.Instance.ShotgunBallsInShell;
            Spread = Parameters.Instance.ShotgunSpread;

            // setup the fuzzy module
            InitializeFuzzyModule();
        }
        
        public int BallsInShell { get; private set; }
        
        public float Spread { get; private set; }
        
        public List<Vector3> Targets { get; private set; }
        
        public override void ShootAt(Vector3 targetPosition)
        {
            if (RoundsRemaining <= 0 || !IsReadyForNextShot())
            {
                return;
            }
            
            Targets.Clear();
            
            // a shotgun cartridge contains lots of tiny metal balls called
            // pellets. Therefore, every time the shotgun is discharged we 
            // have to calculate the spread of the pellets and add one for 
            // each trajectory
            for (int b = 0; b < BallsInShell; ++b)
            {
                // determine deviation from target using a bell curve type distribution
                float deviation =
                    Random.Range(0, Spread) +
                    Random.Range(0, Spread) - Spread;

                Vector3 adjustedTarget = targetPosition;
                
                // rotate the target vector by the deviation
                VectorExtensions.RotateAroundPivot(
                    Agent.Kinematic.Position, 
                    Quaternion.Euler(0, deviation, 0),
                    ref adjustedTarget);

                // add a pellet to the game world
                AddPellet(
                    Agent, 
                    adjustedTarget);

                // temp debugging code
                Targets.Add(adjustedTarget);
            }
            
            DecrementRounds();
            
            UpdateTimeNextAvailable();
            
            AddSoundTrigger(Agent, Parameters.Instance.ShotgunSoundRange);
        }
        
        public override float GetDesirability(float distanceToTarget)
        {
            if (RoundsRemaining == 0)
            {
                LastDesirabilityScore = 0;
            }
            else
            {
                // fuzzify distance and amount of ammo
                FuzzyModule.Fuzzify("distanceToTarget", distanceToTarget);
                FuzzyModule.Fuzzify("ammoStatus", RoundsRemaining);
                LastDesirabilityScore =
                    FuzzyModule.DeFuzzify("desirability", FuzzyModule.DefuzzifyMethod.MaxAv);
            }

            return LastDesirabilityScore;
        }
        
        protected override void InitializeFuzzyModule()
        {
            FzSet targetClose;
            FzSet targetMedium;
            FzSet targetFar;

            InitializeDistanceToTarget(out targetClose, out targetMedium, out targetFar);
            
            FzSet undesirable;
            FzSet desirable;
            FzSet veryDesirable;
                
            InitializeDesirability(out undesirable, out desirable, out veryDesirable);

            FuzzyVariable ammoStatus = FuzzyModule.CreateFlv("ammoStatus");
            FzSet ammoLoads = ammoStatus.AddRightShoulderSet("ammoLoads", 15, 20, 30);
            FzSet ammoOkay = ammoStatus.AddTriangularSet("ammoOkay", 0, 10, 20);
            FzSet ammoLow = ammoStatus.AddTriangularSet("ammoLow", 0, 0, 10);

            FuzzyModule.AddRule(new FzAnd(targetClose, ammoLoads), veryDesirable);
            FuzzyModule.AddRule(new FzAnd(targetClose, ammoOkay), veryDesirable);
            FuzzyModule.AddRule(new FzAnd(targetClose, ammoLow), veryDesirable);

            FuzzyModule.AddRule(new FzAnd(targetMedium, ammoLoads), veryDesirable);
            FuzzyModule.AddRule(new FzAnd(targetMedium, ammoOkay), desirable);
            FuzzyModule.AddRule(new FzAnd(targetMedium, ammoLow), undesirable);

            FuzzyModule.AddRule(new FzAnd(targetFar, ammoLoads), desirable);
            FuzzyModule.AddRule(new FzAnd(targetFar, ammoOkay), undesirable);
            FuzzyModule.AddRule(new FzAnd(targetFar, ammoLow), undesirable);
        }
        
        private void AddPellet(Agent shooter, Vector3 target)
        {
            if (activeProjectileCount < Parameters.Instance.MaximumActivePellets)
            {
                OnProjectileAdded();
                
                GameObject pelletObject = GameObject.Instantiate(GameManager.Instance.pelletPrefab) as GameObject;
                pelletObject.GetComponent<Rigidbody>().mass = Parameters.Instance.PelletMass;
                ProjectilePellet pellet = pelletObject.AddComponent<ProjectilePellet>();
                pellet.Spawn(this, shooter, target);
            }
        }
    }
}