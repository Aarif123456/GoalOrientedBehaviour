namespace GameBrains.AI
{
    public class WanderAbout : Goal
    {
        private Wander wander;
        
        public WanderAbout(Agent agent)
            : base(agent, GoalTypes.WanderAbout)
        {
            wander = new Wander(agent.Kinematic);
        }
        
        public override void Activate()
        {
            Status = StatusTypes.Active;
            Agent.SteeringBehaviours.Add(wander);
        }
        
        public override StatusTypes Process()
        {
            // if status is inactive, call Activate()
            ActivateIfInactive();

            return Status;
        }
        
        public override void Terminate()
        {
            Agent.SteeringBehaviours.Remove(wander);
        }
    }
}