using UnityEngine;

namespace Entities.Steering {
    public class Arrive : Seek {
        private readonly float brakingRadius;
        private readonly float timeToTarget;

        public Arrive(Kinematic agentKinematic, Kinematic targetKinematic, float timeToTarget, float brakingRadius,
            float satisfactionRadius)
            : base(agentKinematic, targetKinematic){
            this.timeToTarget = timeToTarget;
            this.brakingRadius = brakingRadius;
            SatisfactionRadius = satisfactionRadius;
        }

        public override Steering Steer(){
            var targetPosition =  GetTargetPosition();
            var direction = GetMoveDirection(targetPosition);
            var distance = GetDistanceToTarget(direction);

            if (timeToTarget <= 0f || distance > brakingRadius) return base.Steer();

            Vector3 velocity;

            if (distance <= SatisfactionRadius)
                velocity = Vector3.zero;
            else
                velocity = direction / timeToTarget;

            return new Steering{Type = Steering.Types.Velocities, Linear = velocity};
        }
    }
}