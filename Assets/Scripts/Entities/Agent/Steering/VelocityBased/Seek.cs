using UnityEngine;

namespace Entities.Steering {
    public class Seek : SteeringBehaviour {
        protected float satisfactionRadius = 1;

        public Seek(Kinematic agentKinematic)
            : base(agentKinematic, new Kinematic()){
        }

        public Seek(Kinematic agentKinematic, Vector3 targetPosition)
            : base(agentKinematic, targetPosition){
        }

        public Seek(Kinematic agentKinematic, Kinematic targetKinematic)
            : base(agentKinematic, targetKinematic){
        }

        public float SatisfactionRadius {
            get => satisfactionRadius;

            set => satisfactionRadius = value;
        }

        public override Steering Steer(){
            var direction = OtherKinematic.Position - AgentKinematic.Position;
            var distance = direction.magnitude;
            var steering = new Steering{Type = Steering.Types.Velocities};

            if (distance > SatisfactionRadius) steering.Linear = direction / distance * AgentKinematic.MaximumSpeed;

            return steering;
        }
    }
}