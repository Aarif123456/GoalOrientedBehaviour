namespace GameBrains.AI
{
    using UnityEngine;
    
    public class HuntTarget : CompositeGoal
    {
        public HuntTarget(Agent agent)
            : base(agent, GoalTypes.HuntTarget)
        {
        }

        public override void Activate()
        {
            Status = StatusTypes.Active;
            
            // if this goal is reactivated then there may be some existing subgoals that must be removed
            RemoveAllSubgoals();

            // it is possible for the target to die while this goal is active so we
            // must test to make sure the agent always has an active target
            if (Agent.TargetingSystem.IsTargetPresent)
            {
                // grab a local copy of the last recorded position (LRP) of the target
                Vector3 lrp = Agent.TargetingSystem.LastRecordedPosition;

                // if the agent has reached the LRP and it still hasn't found the target
                // it starts to search by using the explore goal to move to random
                // map locations
                if (Epsilon.IsZero(lrp) || Agent.IsAtPosition(lrp))
                {
                    AddSubgoal(new Explore(Agent));
                }

                // else move to the LRP
                else
                {
                    AddSubgoal(new MoveToPosition(Agent, lrp));
                }
            }

            // if there is no active target then this goal can be removed from the queue
            else
            {
                Status = StatusTypes.Completed;
            }
        }

        public override StatusTypes Process()
        {
            // if status is inactive, call Activate()
            ActivateIfInactive();

            Status = ProcessSubgoals();

            // if target is in view this goal is satisfied
            if (Agent.TargetingSystem.IsTargetWithinFieldOfView)
            {
                Status = StatusTypes.Completed;
            }

            return Status;
        }

        public override void Terminate()
        {
        }
    }
}