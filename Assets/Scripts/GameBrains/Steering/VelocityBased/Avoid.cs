namespace GameBrains.AI
{
    public class Avoid : SteeringBehaviour
    {
        public Avoid(Kinematic agentKinematic)
            : base(agentKinematic, new Kinematic())
        {
        }
        
        public override Steering Steer()
        {
            Steering steering = new Steering { Type = Steering.Types.Velocities };

            return steering;
        }
    }
}
