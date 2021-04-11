namespace GameBrains.AI
{
    using UnityEngine;
    
    public sealed class Wander : SteeringBehaviour
    {
        private SteeringBehaviour move;
        private SteeringBehaviour look;
        private float wanderCircleOffset;
        private float wanderCircleRadius;
        private float maximumSlideDegrees;
        private float headingAngle;
        
        public Wander(Kinematic agentKinematic)
            : this(agentKinematic,
                   new Seek(agentKinematic, new Kinematic()), 
                   new Face(agentKinematic, new Kinematic()),
                   100.0f,
                   2.0f,
                   1)
        {
        }
        
        public Wander(
            Kinematic agentKinematic, 
            SteeringBehaviour move, 
            SteeringBehaviour look,
            float wanderCircleOffset,
            float wanderCircleRadius,
            float maximumSlideDegrees)
            : base(agentKinematic)
        {
            this.move = move;
            this.look = look;
            this.wanderCircleOffset = wanderCircleOffset;
            this.wanderCircleRadius = wanderCircleRadius;
            this.maximumSlideDegrees = maximumSlideDegrees;
        }
        
        public SteeringBehaviour Move 
        {
            get 
            {
                return move;
            }
            
            set 
            {
                move = value;
            }
        }
        
        public SteeringBehaviour Look 
        {
            get 
            {
                return look;
            }
            
            set 
            {
                look = value;
            }
        }
        
        public float WanderCircleRadius 
        {
            get 
            {
                return wanderCircleRadius;
            }
            
            set 
            {
                wanderCircleRadius = value;
            }
        }
        
        public float WanderCircleOffset 
        {
            get 
            {
                return wanderCircleOffset;
            }
            
            set 
            {
                wanderCircleOffset = value;
            }
        }
        
        public float MaximumSlideDegrees 
        {
            get 
            {
                return maximumSlideDegrees;
            }
            
            set 
            {
                maximumSlideDegrees = value;
            }
        }

        public override Steering Steer()
        {
            Steering steering = new Steering { Type = Steering.Types.Velocities };
            
            if (wanderCircleRadius <= 0)
            {
                return steering;
            }
            
            headingAngle += maximumSlideDegrees - (2 * Random.value * maximumSlideDegrees);
            headingAngle = Math.WrapAngle(headingAngle);
            
            Vector3 forwardDirection = new Vector3(AgentKinematic.Velocity.x, 0, AgentKinematic.Velocity.z);
            
            if (forwardDirection.magnitude > 0)
            {
                forwardDirection.Normalize();
            }
            else
            {
                forwardDirection = new Vector3(-Mathf.Sin(AgentKinematic.AngularVelocity.y), 0, -Mathf.Cos(AgentKinematic.AngularVelocity.y));
                forwardDirection.Normalize();
            }
            
            Vector3 rightDirection = new Vector3(-forwardDirection.z, 0, forwardDirection.x);
            forwardDirection *= wanderCircleOffset;
            rightDirection *= -Mathf.Sin(headingAngle) * wanderCircleRadius;
            
            Vector3 direction = forwardDirection + rightDirection;
            direction.Normalize();
            
            Kinematic targetKinematic = new Kinematic
            {
                Position = AgentKinematic.Position + direction * AgentKinematic.MaximumSpeed
            };
            
            if (move != null)
            {
                move.OtherKinematic = targetKinematic;
                steering.Linear = move.Steer().Linear;
            }

            if (look != null)
            {
                look.OtherKinematic = targetKinematic;
                steering.Angular = look.Steer().Angular;
            }
            
            return steering;
        }
    }
}