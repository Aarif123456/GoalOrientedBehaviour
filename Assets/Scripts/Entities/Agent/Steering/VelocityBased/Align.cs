using UnityEngine;
using Utility.Math;

namespace Entities.Steering {
    public class Align : SteeringBehaviour {
        private readonly float satisfactionRadius;
        private readonly float timeToTarget;

        public Align(Kinematic agentKinematic, Kinematic targetKinematic, float timeToTarget, float satisfactionRadius)
            : base(agentKinematic, targetKinematic){
            this.timeToTarget = timeToTarget;
            this.satisfactionRadius = satisfactionRadius;
        }

        public override Steering Steer(){
            var angularDirection = Math.WrapAngles(OtherKinematic.Rotation - AgentKinematic.Rotation);

            Vector3 angularVelocity;

            if (angularDirection.magnitude <= satisfactionRadius){
                angularVelocity = Vector3.zero;
            }
            else if (timeToTarget <= 0f){
                angularVelocity = angularDirection;
            }
            else{
                angularVelocity = angularDirection / timeToTarget;
            }

            return new Steering{Type = Steering.Types.Velocities, Angular = angularVelocity};
        }
    }
}