using UnityEngine;

namespace Entities.Steering {
    public class Flee : Seek {
        public Flee(Kinematic agentKinematic)
            : base(agentKinematic){
        }

        public Flee(Kinematic agentKinematic, Vector3 targetPosition)
            : base(agentKinematic, targetPosition){
        }

        public Flee(Kinematic agentKinematic, Kinematic targetKinematic)
            : base(agentKinematic, targetKinematic){
        }

        public override Steering Steer(){
            var steering = base.Steer();
            steering.Linear *= -1;
            return steering;
        }
    }
}