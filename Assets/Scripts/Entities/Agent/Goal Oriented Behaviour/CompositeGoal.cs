using System.Collections.Generic;
using Utility.DataStructures;

namespace Entities.GoalOrientedBehaviour {
    public abstract class CompositeGoal : Goal {
        protected CompositeGoal(Agent agent, GoalTypes goalType)
            : base(agent, goalType){
            Subgoals = new List<Goal>();
        }

        protected List<Goal> Subgoals { get; }

        public override void AddSubgoal(Goal subgoal){
            Subgoals.Push(subgoal);
        }

        public override void RemoveAllSubgoals(){
            foreach (var goal in Subgoals){
                goal.RemoveAllSubgoals();
                goal.Terminate();
            }

            Subgoals.Clear();
        }

        protected StatusTypes ProcessSubgoals(){
            // remove all completed and failed goals from the front of the subgoal list
            while (Subgoals.Count > 0 && (Subgoals.Peek().IsComplete || Subgoals.Peek().HasFailed)){
                Subgoals.Peek().RemoveAllSubgoals();
                Subgoals.Peek().Terminate();
                Subgoals.Pop();
            }

            // no more subgoals to process - return 'completed'
            if (Subgoals.Count <= 0) return StatusTypes.Completed;
            // if any subgoals remain, process the one at the front of the list
            var statusOfSubGoals = Subgoals.Peek().Process();

            // we have to test for the special case where the front-most reports 
            // 'completed' *and* the subgoal list contains additional goals. When this
            // is the case, to ensure the parent keeps processing its subgoal list we
            // must return the 'active' status.
            if (statusOfSubGoals == StatusTypes.Completed && Subgoals.Count > 1) return StatusTypes.Active;

            return statusOfSubGoals;
        }

       public override void StoreThoughtProcess(MessageManager messageManager, ref int indent)
       {
            base.StoreThoughtProcess(messageManager, ref indent);
            indent++;
            foreach (Goal goal in Subgoals){
                goal.StoreThoughtProcess(messageManager, ref indent);
            }
       }
    }
}