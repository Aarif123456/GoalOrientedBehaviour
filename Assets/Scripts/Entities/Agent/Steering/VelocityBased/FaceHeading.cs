namespace GameBrains.AI {
    public class FaceHeading : Face {
        private readonly float minimumSpeed;

        public FaceHeading(Kinematic agentKinematic)
            : this(agentKinematic, 0.1f, 0.25f, 5){
        }

        public FaceHeading(Kinematic agentKinematic, float minimumSpeed, float timeToTarget, float satisfactionRadius)
            : base(agentKinematic, agentKinematic.Position, timeToTarget, satisfactionRadius){
            this.minimumSpeed = minimumSpeed;
        }

        public override Steering Steer(){
            var speed = AgentKinematic.Velocity.magnitude;

            if (speed > minimumSpeed){
                OtherKinematic.Position = AgentKinematic.Position + AgentKinematic.Velocity / speed;

                return base.Steer();
            }

            return new Steering{Type = Steering.Types.Velocities};
        }
    }
}