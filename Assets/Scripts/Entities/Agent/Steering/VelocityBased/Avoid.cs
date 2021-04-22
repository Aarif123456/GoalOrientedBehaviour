namespace GameBrains.AI {
    public class Avoid : SteeringBehaviour {
        public Avoid(Kinematic agentKinematic)
            : base(agentKinematic, new Kinematic()){
        }

        public override Steering Steer(){
            var steering = new Steering{Type = Steering.Types.Velocities};

            return steering;
        }
    }
}