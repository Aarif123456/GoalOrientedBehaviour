namespace GameBrains.AI
{
    using UnityEngine;
    
    public class EvaluatorGetHealth : Evaluator
    {
        public EvaluatorGetHealth(float characterBias)
            : base(characterBias)
        {
        }
        public override float CalculateDesirability(Agent agent)
        {
            // first grab the distance to the closest instance of a health item
            float distance = Feature.DistanceToItem(agent, ItemTypes.Health);

            // if the distance feature is rated with a value of 1 it means that the
            // item is either not present on the map or too far away to be worth 
            // considering, therefore the desirability is zero
            if (distance == 1)
            {
                return 0;
            }

            // the desirability of finding a health item is proportional to the amount
            // of health remaining and inversely proportional to the distance from the
            // nearest instance of a health item.
            float desirability =
                Parameters.Instance.AgentHealthGoalTweaker *
                (1 - Feature.Health(agent)) / Feature.DistanceToItem(agent, ItemTypes.Health);
            
            // bias the value according to the personality of the bot
            desirability *= characterBias;

            // ensure the value is in the range 0 to 1
            desirability = Mathf.Clamp(desirability, 0, 1);

            return desirability;
        }
        
        public override void SetGoal(Agent agent)
        {
            agent.Brain.AddGoalGetItemOfType(ItemTypes.Health);
        }
    }
}
