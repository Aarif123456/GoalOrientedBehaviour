using UnityEngine;
using Utility.Math;

namespace Entities.Steering {
    public class Align : SteeringBehaviour {
        private readonly float _satisfactionRadius;
        private readonly float _timeToTarget;

        public Align(Kinematic agentKinematic, Kinematic targetKinematic, float timeToTarget, float satisfactionRadius)
            : base(agentKinematic, targetKinematic){
            _timeToTarget = timeToTarget;
            _satisfactionRadius = satisfactionRadius;
        }

        public override Steering Steer(){
            var angularDirection = Math.WrapAngles(OtherKinematic.Rotation - AgentKinematic.Rotation);

            Vector3 angularVelocity;

            if (angularDirection.magnitude <= _satisfactionRadius)
                angularVelocity = Vector3.zero;
            else if (_timeToTarget <= 0f)
                angularVelocity = angularDirection;
            else
                angularVelocity = angularDirection / _timeToTarget;

            return new Steering{Type = Steering.Types.Velocities, Angular = angularVelocity};
        }
    }
}