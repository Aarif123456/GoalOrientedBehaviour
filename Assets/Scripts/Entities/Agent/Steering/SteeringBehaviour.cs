namespace GameBrains.AI
{
    using UnityEngine;
    
    public abstract class SteeringBehaviour
    {
        private Kinematic agentKinematic;
        private Kinematic otherKinematic;
        
        protected SteeringBehaviour(Agent agent, Vector3 otherPosition)
            : this(agent.Kinematic, otherPosition)
        {
        }
        
        protected SteeringBehaviour(Kinematic agentKinematic, Vector3 otherPosition)
            : this(agentKinematic, new Kinematic { Position = otherPosition })
        {
        }
        
        protected SteeringBehaviour(Kinematic agentKinematic)
            : this(agentKinematic, null)
        {
        }
        
        protected SteeringBehaviour(Kinematic agentKinematic, Kinematic otherKinematic)
        {
            this.agentKinematic = agentKinematic;
            this.otherKinematic = otherKinematic;
        }
        
        public Kinematic AgentKinematic 
        {
            get 
            {
                return agentKinematic;
            }
            
            set 
            {
                agentKinematic = value;
            }
        }

        public Kinematic OtherKinematic 
        {
            get 
            {
                return otherKinematic;
            }
            
            set 
            {
                otherKinematic = value;
            }
        }

        public abstract Steering Steer();
        
//        public virtual Steering Steer(Kinematic agentKinematic)
//        {
//            throw new System.NotSupportedException(
//                "The derived steering behaviour does not support this method.");
//        }
//        
//        public virtual Steering Steer(Kinematic agentKinematic, Kinematic targetKinematic)
//        {
//            throw new System.NotSupportedException(
//                "The derived steering behaviour does not support this method.");
//        }
//                
//        public virtual Steering Steer(Agent agent, Kinematic targetKinematic)
//        {
//            return Steer(agent.Kinematic, targetKinematic);
//        }
//        
//        public virtual Steering Steer(Agent agent, GameObject target)
//        {
//            Kinematic targetKinematic = new Kinematic 
//            {
//                Position = target.transform.position,
//                Rotation = target.transform.rotation.eulerAngles
//            };
//            
//            return Steer(agent, targetKinematic);
//        }
//        
//        public virtual Steering Steer(Agent agent, Vector3 targetPosition)
//        {
//            Kinematic targetKinematic = new Kinematic 
//            {
//                Position = targetPosition
//            };
//            
//            return Steer(agent, targetKinematic);
//        }
    }
}
