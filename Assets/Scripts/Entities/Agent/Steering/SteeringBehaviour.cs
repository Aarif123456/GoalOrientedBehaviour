using UnityEngine;

namespace Entities.Steering {
    public abstract class SteeringBehaviour {
        protected SteeringBehaviour(Entity agent, Vector3 otherPosition)
            : this(agent.Kinematic, otherPosition){
        }

        protected SteeringBehaviour(Kinematic agentKinematic, Vector3 otherPosition)
            : this(agentKinematic, new Kinematic{Position = otherPosition}){
        }

        protected SteeringBehaviour(Kinematic agentKinematic)
            : this(agentKinematic, null){
        }

        protected SteeringBehaviour(Kinematic agentKinematic, Kinematic otherKinematic){
            AgentKinematic = agentKinematic;
            OtherKinematic = otherKinematic;
        }

        public Kinematic AgentKinematic { get; set; }

        public Kinematic OtherKinematic { get; set; }

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