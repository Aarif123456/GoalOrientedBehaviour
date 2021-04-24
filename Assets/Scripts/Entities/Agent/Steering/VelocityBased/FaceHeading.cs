namespace Entities.Steering {
    public class FaceHeading : Face {
        private readonly float minimumSpeed;

        public FaceHeading(Kinematic agentKinematic, float minimumSpeed = 0.1f, float timeToTarget = 0.25f, float satisfactionRadius = 5)
            : base(agentKinematic, agentKinematic.Position, timeToTarget, satisfactionRadius){
            this.minimumSpeed = minimumSpeed;
        }

        public override Steering Steer(){
            var speed = AgentKinematic.Velocity.magnitude;

            if (!(speed > minimumSpeed)) return new Steering{Type = Steering.Types.Velocities};
            OtherKinematic.Position = AgentKinematic.Position + AgentKinematic.Velocity / speed;

            return base.Steer();
        }
    }
}