using UnityEngine;

namespace Entities.Steering {
    public class Evade : Flee {
        public Evade(Kinematic agentKinematic)
            : base(agentKinematic){
        }

        public Evade(Kinematic agentKinematic, Vector3 targetPosition)
            : base(agentKinematic, targetPosition){
        }

        public Evade(Kinematic agentKinematic, Kinematic targetKinematic)
            : base(agentKinematic, targetKinematic){
        }

        protected override Vector3 GetTargetPosition(){
            var targetPosition = base.GetTargetPosition();
            Debug.Log("Evading pre null check");
            if (ReferenceEquals(OtherKinematic, null)) return targetPosition;
            Debug.Log("Evading");
            var direction = GetMoveDirection(targetPosition);
            var distance = GetDistanceToTarget(direction);
            var prediction = distance / AgentKinematic.MaximumSpeed;
            var targetVelocity = OtherKinematic.Velocity;
            targetPosition += targetVelocity * prediction;
            return targetPosition;
        }
    }
}