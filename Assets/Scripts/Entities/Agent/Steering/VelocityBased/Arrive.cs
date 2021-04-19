namespace GameBrains.AI
{
    using UnityEngine;

    public class Arrive : Seek
    {
        private float brakingRadius;
        private float timeToTarget;

        public Arrive(Kinematic agentKinematic, Kinematic targetKinematic, float timeToTarget, float brakingRadius, float satisfactionRadius)
            : base(agentKinematic, targetKinematic)
        {
            this.timeToTarget = timeToTarget;
            this.brakingRadius = brakingRadius;
            SatisfactionRadius = satisfactionRadius;
        }

        public override Steering Steer()
        {
            Vector3 direction = OtherKinematic.Position - AgentKinematic.Position;
            float distance = direction.magnitude;

            if (timeToTarget <= 0f || distance > brakingRadius)
            {
                return base.Steer();
            }
            
            Vector3 velocity;
            
            if (distance <= SatisfactionRadius)
            {
                velocity = Vector3.zero;
            }
            else
            {
                velocity = direction / timeToTarget;
            }

            return new Steering { Type = Steering.Types.Velocities, Linear = velocity };
        }
    }
}