using System;

namespace Entities.GoalOrientedBehaviour {
    public abstract class Goal {
        public enum StatusTypes {
            Active,
            Inactive,
            Completed,
            Failed
        }

        private const float timeBetweenDisplayUpdates = 1.0f;
        private float displayTimer;

        protected Goal(Agent agent, GoalTypes goalType){
            Agent = agent;
            GoalType = goalType;
            Status = StatusTypes.Inactive;
        }

        public bool IsComplete => Status == StatusTypes.Completed;

        public bool IsActive => Status == StatusTypes.Active;

        public bool IsInactive => Status == StatusTypes.Inactive;

        public bool HasFailed => Status == StatusTypes.Failed;

        public Agent Agent { get; protected set; }

        public GoalTypes GoalType { get; protected set; }

        public StatusTypes Status { get; protected set; }

        public abstract void Activate();

        public abstract StatusTypes Process();

        public abstract void Terminate();

        public virtual void AddSubgoal(Goal goal){
            throw new NotSupportedException("Cannot add goals to atomic goals.");
        }

        public virtual void RemoveAllSubgoals(){
        }

        protected void ReactivateIfFailed(){
            if (HasFailed) Status = StatusTypes.Inactive;
        }

        protected void ActivateIfInactive(){
            if (IsInactive) Activate();
        }

//        public void ShowOnDisplay(MessageManager messageManager, string messageDisplay)
//        {
//            displayTimer -= Time.deltaTime;
//            
//            if (displayTimer <= 0)
//            {
//                displayTimer = timeBetweenDisplayUpdates;
//                
//                int indent = 0;
//                messageManager.ClearMessages(messageDisplay);
//                messageManager.Message(messageDisplay, Agent.shortName + " (" + Agent.SteeringBehaviours.Count + ")", Agent.color);
//                ShowOnDisplay(messageManager, messageDisplay, ref indent);
//            }
//        }

//        public virtual void ShowOnDisplay(MessageManager messageManager, string messageDisplay, ref int indent)
//        {
//            Color textColor = Color.black;
//
//            if (IsComplete)
//            {
//                textColor = Color.cyan;
//            }
//
//            if (IsInactive)
//            {
//                textColor = Color.black;
//            }
//
//            if (HasFailed)
//            {
//                textColor = Color.red;
//            }
//
//            if (IsActive)
//            {
//                textColor = Color.blue;
//            }
//    
//            messageManager.Message(
//                messageDisplay,  
//                new string(' ', indent) + EnumUtility.GetDescription(GoalType),                 
//                textColor);
//        }
    }
}