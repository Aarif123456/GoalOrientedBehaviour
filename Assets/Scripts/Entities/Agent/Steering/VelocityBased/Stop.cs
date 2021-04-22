namespace Entities.Steering {
    public class Stop : SteeringBehaviour {
        public Stop(Kinematic agentKinematic)
            : base(agentKinematic){
        }

        public Stop(Agent agent)
            : this(agent.Kinematic){
        }

        public override Steering Steer(){
            return new Steering{Type = Steering.Types.Velocities};
        }
    }
}