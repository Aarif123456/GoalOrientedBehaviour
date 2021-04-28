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

        protected virtual Vector3 GetTargetPosition(){
            if (!ReferenceEquals(OtherKinematic, null)) return OtherKinematic.Position;
            Debug.LogWarning("Steering does not have a target");
            return AgentKinematic.Position;
        }

        protected virtual Vector3 GetMoveDirection(Vector3 targetPosition){
            return targetPosition - AgentKinematic.Position;
        }

        protected virtual float GetDistanceToTarget(Vector3 moveDirection){
            return moveDirection.magnitude;
        }

        public override Steering Steer(){
            var targetPosition = GetTargetPosition();
            var direction = GetMoveDirection(targetPosition);
            var distance = GetDistanceToTarget(direction);

            var steering = new Steering{Type = Steering.Types.Velocities};

            if (distance > SatisfactionRadius) steering.Linear = direction / distance * AgentKinematic.MaximumSpeed;

            return steering;
        }
    }
}