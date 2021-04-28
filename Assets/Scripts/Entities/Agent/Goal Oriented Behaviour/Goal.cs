using System;
using Common;
using UnityEngine;

namespace Entities.GoalOrientedBehaviour {
    public abstract class Goal {
        public enum StatusTypes {
            Active,
            Inactive,
            Completed,
            Failed
        }

        private const float timeBetweenDisplayUpdates = 0.5f;
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

        public Message GetHeadMessage(){
            return new Message{
                Text = Agent.shortName + " (" + Agent.SteeringBehaviours.Count + ")",
                Color = Agent.color
            };
        }

        /* Get the colour based on the type of goal */
        private Color GetGoalColor(){
            return Status switch{
                StatusTypes.Completed => Color.cyan,
                StatusTypes.Active => Color.blue,
                StatusTypes.Failed => Color.red,
                StatusTypes.Inactive => Color.black,
                _ => Color.black
            };
        }

        public virtual void StoreThoughtProcess(MessageManager messageManager){
            displayTimer -= Time.deltaTime;
            if (displayTimer > 0) return;
            displayTimer = timeBetweenDisplayUpdates;
            var indent = 0;
            StoreThoughtProcess(messageManager, ref indent);
        }

        public virtual void StoreThoughtProcess(MessageManager messageManager, ref int indent){
            var messageColor = GetGoalColor();
            var messageString = new string(' ', indent) + EnumUtility.GetDescription(GoalType);
            var message = new Message{
                Text = messageString,
                Color = messageColor
            };
            messageManager.AddMessage(Agent, message);
        }
    }
}