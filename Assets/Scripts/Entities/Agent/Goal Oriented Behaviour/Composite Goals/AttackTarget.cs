namespace GameBrains.AI
{
    public class AttackTarget : CompositeGoal
    {
        public AttackTarget(Agent agent)
            : base(agent, GoalTypes.AttackTarget)
        {
        }

        public override void Activate()
        {
            Status = StatusTypes.Active;

            // if this goal is reactivated then there may be some existing
            // subgoals that must be removed
            RemoveAllSubgoals();

            // it is possible for a agent's target to die while this goal is active
            // so we must test to make sure the agent always has an active target
            if (!Agent.TargetingSystem.IsTargetPresent)
            {
                Status = StatusTypes.Completed;
                return;
            }

            // if the agent is able to shoot the target (there is LOS between agent
            // and target), then select a tactic to follow while shooting
            if (Agent.TargetingSystem.IsTargetShootable)
            {
                // if the agent has space to strafe then do so
                if (Agent.CanStepLeft(0) || Agent.CanStepRight(0))
                {
                    AddSubgoal(new Strafe(Agent, Agent.TargetingSystem.Target));
                }

                // if not able to strafe, head directly at the target's position 
                else
                {
                    AddSubgoal(new AdjustRange(Agent, Agent.TargetingSystem.Target));
                }
            }

            // if the target is not visible, go hunt it.
            else
            {
                AddSubgoal(new HuntTarget(Agent));
            }
        }

        public override StatusTypes Process()
        {
            // if status is inactive, call Activate()
            ActivateIfInactive();

            // process the subgoals
            Status = ProcessSubgoals();

            ReactivateIfFailed();

            return Status;
        }

        public override void Terminate()
        {
            Status = StatusTypes.Completed;
        }
    }
}