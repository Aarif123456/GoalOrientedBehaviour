namespace GameBrains.AI
{
    using System.Collections.Generic;
    
    using UnityEngine;
    
    public class Think : CompositeGoal
    {
        private readonly List<Evaluator> evaluators = new List<Evaluator>();    
        private float bestDesirability;    
        private Evaluator mostDesirable;
        //private Evaluator prevMostDesirable;
        private bool arbitrateTickTock;
        
        public Think(Agent agent)
            : base(agent, GoalTypes.Think)
        {
            //// these biases could be loaded in from a script on a per bot basis
            //// but for now we'll just give them some random values
            const float LOW_RANGE_OF_BIAS = 0.5f;
            const float HIGH_RANGE_OF_BIAS = 1.5f;

            float healthBias = Random.Range(LOW_RANGE_OF_BIAS, HIGH_RANGE_OF_BIAS);
            float exploreBias = Random.Range(LOW_RANGE_OF_BIAS, HIGH_RANGE_OF_BIAS);
            float attackBias = Random.Range(LOW_RANGE_OF_BIAS, HIGH_RANGE_OF_BIAS);

            ////float evadeBias =  Random.Range(lowRangeOfBias, highRangeOfBias);

            float shotgunBias = Random.Range(LOW_RANGE_OF_BIAS, HIGH_RANGE_OF_BIAS);
            float rocketLauncherBias = Random.Range(LOW_RANGE_OF_BIAS, HIGH_RANGE_OF_BIAS);
            float railgunBias = Random.Range(LOW_RANGE_OF_BIAS, HIGH_RANGE_OF_BIAS);
            
            //// create the evaluator objects
            evaluators.Add(new EvaluatorGetHealth(healthBias));
            evaluators.Add(new EvaluatorExplore(exploreBias));
            evaluators.Add(new EvaluatorAttackTarget(attackBias));
            ////evaluators.Add(new EvaluatorEvadeBot(evadeBias));
            evaluators.Add(new EvaluatorGetWeapon(shotgunBias, WeaponTypes.Shotgun));
            evaluators.Add(new EvaluatorGetWeapon(railgunBias, WeaponTypes.Railgun));
            evaluators.Add(new EvaluatorGetWeapon(rocketLauncherBias, WeaponTypes.RocketLauncher));
        }
        
        public override void Activate()
        {
            if (Agent.IsAiControlled)
            {
                Arbitrate();
            }

            Status = StatusTypes.Active;
        }
        
        public override StatusTypes Process()
        {
            ActivateIfInactive();

            StatusTypes subgoalStatus = ProcessSubgoals();

            if ((subgoalStatus == StatusTypes.Completed ||
                 subgoalStatus == StatusTypes.Failed) &&
                 Agent.IsAiControlled)
            {
                Status = StatusTypes.Inactive;
                
                if (!Subgoals.IsEmpty())
                {
                    Subgoals.Peek().Terminate();
                }
            }

            return Status;
        }
        
        public override void Terminate()
        {
        }
        
        public void Arbitrate()
        {        
            arbitrateTickTock = !arbitrateTickTock;
            //prevMostDesirable = mostDesirable;
            
            bestDesirability = 0.0f;
            mostDesirable = null;
            
            // iterate through all the evaluators to find the highest scoring one
            foreach (Evaluator evaluator in evaluators)
            {
                float desirabilty = evaluator.CalculateDesirability(Agent);

                if (bestDesirability < desirabilty)
                {
                    bestDesirability = desirabilty;
                    mostDesirable = evaluator;
                }
            }
            
            if (mostDesirable == null)
            {
                throw new System.Exception("Think.Arbitrate: no evaluator selected.");
            }
            else
            {
                mostDesirable.SetGoal(Agent);
            }
        }
        
        public bool NotPresent(GoalTypes goalType)
        {
            return Subgoals.Count <= 0 || Subgoals.Peek().GoalType != goalType;
        }
        
        public void AddGoalExplore()
        {
            if (!NotPresent(GoalTypes.Explore))
            {
                return;
            }

            RemoveAllSubgoals();
            AddSubgoal(new Explore(Agent));
        }
        
        public void AddGoalMoveToPosition(Vector3 destination)
        {
            AddSubgoal(new MoveToPosition(Agent, destination));
        }
        
        public void AddGoalGetItemOfType(ItemTypes itemType)
        {
            if (!NotPresent(EnumUtility.ItemTypeToGoalType(itemType)))
            {
                return;
            }

            RemoveAllSubgoals();
            AddSubgoal(new GetItemOfType(Agent, itemType));
        }
        
        public void AddGoalAttackTarget()
        {
            if (!NotPresent(GoalTypes.AttackTarget))
            {
                return;
            }
            
            RemoveAllSubgoals();
            AddSubgoal(new AttackTarget(Agent));
        }
    
        public void QueueGoalMoveToPosition(Vector3 destination)
        {
            Subgoals.Enqueue(new MoveToPosition(Agent, destination));
        }
    }
}
