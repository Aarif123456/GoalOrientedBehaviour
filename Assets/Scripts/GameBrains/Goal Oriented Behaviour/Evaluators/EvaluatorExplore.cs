namespace GameBrains.AI
{
    using UnityEngine;
    
    public class EvaluatorExplore : Evaluator
    {
        public EvaluatorExplore(float characterBias)
            : base(characterBias)
        {
        }
        public override float CalculateDesirability(Agent agent)
        {
            // small value so explore is desired if nothing else is
            float desirability = 0.05f;

            desirability *= characterBias;

            // ensure the value is in the range 0 to 1
            desirability = Mathf.Clamp(desirability, 0.0f, 1.0f);

            return desirability;
        }
        
        public override void SetGoal(Agent agent)
        {
            agent.Brain.AddGoalExplore();
        }
    }
}