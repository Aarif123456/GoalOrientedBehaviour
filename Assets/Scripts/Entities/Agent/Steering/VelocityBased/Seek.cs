namespace GameBrains.AI
{
    using UnityEngine;
    
    public class Seek : SteeringBehaviour
    {
        protected float satisfactionRadius = 1;
        
        public Seek(Kinematic agentKinematic)
            : base(agentKinematic, new Kinematic())
        {
        }
        
        public Seek(Kinematic agentKinematic, Vector3 targetPosition)
            : base(agentKinematic, targetPosition)
        {
        }
        
        public Seek(Kinematic agentKinematic, Kinematic targetKinematic)
            : base(agentKinematic, targetKinematic)
        {
        }
        
        public float SatisfactionRadius 
        {
            get 
            {
                return satisfactionRadius;
            }
            
            set 
            {
                satisfactionRadius = value;
            }
        }

        public override Steering Steer()
        {
            Vector3 direction = OtherKinematic.Position - AgentKinematic.Position;
            float distance = direction.magnitude;
            Steering steering = new Steering { Type = Steering.Types.Velocities };
            
            if (distance > SatisfactionRadius)
            {
                steering.Linear = direction / distance * AgentKinematic.MaximumSpeed;
            }

            return steering;
        }
    }
}