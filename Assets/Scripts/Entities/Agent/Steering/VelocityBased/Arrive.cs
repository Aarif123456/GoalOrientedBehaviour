using UnityEngine;

namespace Entities.Steering {
    public class Arrive : Seek {
        private readonly float _brakingRadius;
        private readonly float _timeToTarget;

        public Arrive(Kinematic agentKinematic, Kinematic targetKinematic, float timeToTarget, float brakingRadius,
            float satisfactionRadius)
            : base(agentKinematic, targetKinematic){
            _timeToTarget = timeToTarget;
            _brakingRadius = brakingRadius;
            SatisfactionRadius = satisfactionRadius;
        }

        public override Steering Steer(){
            var targetPosition = GetTargetPosition();
            var direction = GetMoveDirection(targetPosition);
            var distance = GetDistanceToTarget(direction);

            if (_timeToTarget <= 0f || distance > _brakingRadius) return base.Steer();

            Vector3 velocity;

            if (distance <= SatisfactionRadius)
                velocity = Vector3.zero;
            else
                velocity = direction / _timeToTarget;

            return new Steering{Type = Steering.Types.Velocities, Linear = velocity};
        }
    }
}