using System;
using System.Collections.Generic;
using Common;
using Entities.Armory;
using UnityEngine;
using Utility.DataStructures;
using Random = UnityEngine.Random;

namespace Entities.GoalOrientedBehaviour {
    public class Think : CompositeGoal {
        public readonly List<Evaluator> evaluators = new List<Evaluator>();

        //private Evaluator prevMostDesirable;
        private bool arbitrateTickTock;
        private float bestDesirability;
        /* add a small boast to reduce the chance of flip flopping between goals */
        private const float consistencyBoast = 0.05f;
        private Evaluator mostDesirable;

        private static float CreateRandomValue(){
            const float LOW_RANGE_OF_BIAS = 0.5f;
            const float HIGH_RANGE_OF_BIAS = 1.5f;
            return Random.Range(LOW_RANGE_OF_BIAS, HIGH_RANGE_OF_BIAS);
        }
        public Think(Agent agent)
            : base(agent, GoalTypes.Think){
            var biases = new Biases{
                HealthBias = CreateRandomValue(),
                ExploreBias = CreateRandomValue(),
                AttackBias = CreateRandomValue(),
                EvadeBias = CreateRandomValue(),
                ShotgunBias = CreateRandomValue(),
                RailgunBias = CreateRandomValue(),
                RocketLauncherBias = CreateRandomValue()
            };
            //// create the evaluator objects
            evaluators.Add(new EvaluatorGetHealth(biases.HealthBias));
            evaluators.Add(new EvaluatorExplore(biases.ExploreBias));
            evaluators.Add(new EvaluatorAttackTarget(biases.AttackBias));
            /* TODO add in evaluate to do allow agent to evade */
            ////evaluators.Add(new EvaluatorEvadeBot(biases.EvadeBias));
            evaluators.Add(new EvaluatorGetWeapon(biases.ShotgunBias, WeaponTypes.Shotgun));
            evaluators.Add(new EvaluatorGetWeapon(biases.RailgunBias, WeaponTypes.Railgun));
            evaluators.Add(new EvaluatorGetWeapon(biases.RocketLauncherBias, 
                WeaponTypes.RocketLauncher));
        }

        public override void Activate(){
            if (Agent.IsAiControlled) Arbitrate();

            Status = StatusTypes.Active;
        }

        public override StatusTypes Process(){
            ActivateIfInactive();

            var subgoalStatus = ProcessSubgoals();

            if (subgoalStatus != StatusTypes.Completed && subgoalStatus != StatusTypes.Failed ||
                !Agent.IsAiControlled) return Status;
            Status = StatusTypes.Inactive;

            if (!Subgoals.IsEmpty()) Subgoals.Peek().Terminate();

            return Status;
        }

        public override void Terminate(){
        }

        public void Arbitrate(){
            arbitrateTickTock = !arbitrateTickTock;
            bestDesirability = 0.0f;
            var prevMostDesirable = mostDesirable;
            mostDesirable = null;

            // iterate through all the evaluators to find the highest scoring one
            foreach (var evaluator in evaluators){
                var desirability = evaluator.CalculateDesirability(Agent);

                if (!(bestDesirability < desirability)) continue;
                if(ReferenceEquals(evaluator, mostDesirable)) desirability += consistencyBoast;
                bestDesirability = desirability;
                mostDesirable = evaluator;
            }

            if (mostDesirable == null) throw new Exception("Think.Arbitrate: no evaluator selected.");

            mostDesirable.SetGoal(Agent);
        }

        public bool NotPresent(GoalTypes goalType){
            return Subgoals.Count <= 0 || Subgoals.Peek().GoalType != goalType;
        }

        public void AddGoalExplore(){
            if (Agent.IsAiControlled && !NotPresent(GoalTypes.Explore)) return;

            RemoveAllSubgoals();
            AddSubgoal(new Explore(Agent));
        }

        public void AddGoalMoveToPosition(Vector3 destination){
            AddSubgoal(new MoveToPosition(Agent, destination));
        }

        public void AddGoalGetItemOfType(ItemTypes itemType){
            if (Agent.IsAiControlled && !NotPresent(EnumUtility.ItemTypeToGoalType(itemType))) return;

            RemoveAllSubgoals();
            AddSubgoal(new GetItemOfType(Agent, itemType));
        }

        public void AddGoalAttackTarget(){
            if (Agent.IsAiControlled && !NotPresent(GoalTypes.AttackTarget)) return;

            RemoveAllSubgoals();
            AddSubgoal(new AttackTarget(Agent));
        }

        public void QueueGoalMoveToPosition(Vector3 destination){
            Subgoals.Enqueue(new MoveToPosition(Agent, destination));
        }
    }
}