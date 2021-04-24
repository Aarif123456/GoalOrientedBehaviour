using Common;
using UnityEngine;

namespace Entities.GoalOrientedBehaviour {
    public class EvaluatorEvadeBot : Evaluator {

        public EvaluatorEvadeBot(float characterBias)
            : base(characterBias){
                GoalName = "Evade Target";
        }

        /* TODO FIX */
        public override float CalculateDesirability(Agent agent){
            var desirability = 0.0f;

            // only do the calculation if there is a target present
            if (!agent.TargetingSystem.IsTargetPresent) return desirability;
            /* Less aggressive agents will prefer to be cautious */
            var tweaker = 1-Parameters.Instance.AgentAggroGoalTweaker;

            /* The lower our health */
            desirability = tweaker *
                           (1-Feature.Health(agent)) ;

            // bias the value according to the personality of the bot
            desirability *= characterBias;

            // ensure the value is in the range 0 to 1
            desirability = Mathf.Clamp(desirability, 0.0f, 1.0f);

            return desirability;
        }

        public override void SetGoal(Agent agent){
            agent.Brain.AddGoalEvadeBot();
        }
    }
}