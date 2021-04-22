using UnityEngine;

namespace GameBrains.AI {
    public class EvaluatorAttackTarget : Evaluator {
        public EvaluatorAttackTarget(float characterBias)
            : base(characterBias){
        }

        public override float CalculateDesirability(Agent agent){
            var desirability = 0.0f;

            // only do the calculation if there is a target present
            if (agent.TargetingSystem.IsTargetPresent){
                var tweaker = Parameters.Instance.AgentAggroGoalTweaker;

                desirability = tweaker *
                               Feature.Health(agent) *
                               Feature.TotalWeaponStrength(agent);

                // bias the value according to the personality of the bot
                desirability *= characterBias;

                // ensure the value is in the range 0 to 1
                desirability = Mathf.Clamp(desirability, 0.0f, 1.0f);
            }

            return desirability;
        }

        public override void SetGoal(Agent agent){
            agent.Brain.AddGoalAttackTarget();
        }
    }
}