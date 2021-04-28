namespace Entities.GoalOrientedBehaviour {
    public abstract class Evaluator {
        protected readonly float characterBias;

        protected Evaluator(float characterBias){
            this.characterBias = characterBias;
        }

        public string GoalName { get; protected set; }

        public abstract float CalculateDesirability(Agent agent);

        public abstract void SetGoal(Agent agent);
    }
}