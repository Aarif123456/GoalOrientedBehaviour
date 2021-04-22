namespace Entities.GoalOrientedBehaviour {
    public abstract class Evaluator {
        protected float characterBias;

        protected Evaluator(float characterBias){
            this.characterBias = characterBias;
        }

        public abstract float CalculateDesirability(Agent agent);

        public abstract void SetGoal(Agent agent);
    }
}