namespace Entities.GoalOrientedBehaviour {
    public class EvadeBot : CompositeGoal {
        public EvadeBot(Agent agent)
            : base(agent, GoalTypes.EvadeBot){
        }

        public override void Activate(){
            Status = StatusTypes.Active;

            // if this goal is reactivated then there may be some existing
            // subgoals that must be removed
            RemoveAllSubgoals();

            // it is possible for a agent's target to die while this goal is active
            // so we must test to make sure the agent always has an active target
            if (!Agent.TargetingSystem.IsTargetPresent){
                Status = StatusTypes.Completed;
                return;
            }

            AddSubgoal(new EvadeTarget(Agent, Agent.TargetingSystem.Target));

            /*var projectileMinRange = Agent.SensoryMemory.projectileMinRange;
            var projectileMaxRange = Agent.SensoryMemory.projectileMaxRange;
            var cases = new Vector3[]{
                new Vector3(projectileMinRange.x, 1.1f, projectileMinRange.z),
                new Vector3(projectileMinRange.x, 1.1f, projectileMaxRange.z),
                new Vector3(projectileMaxRange.x, 1.1f, projectileMinRange.z),
                new Vector3(projectileMaxRange.x, 1.1f, projectileMaxRange.z),
            };*/

            /*TODO: Make a new composite goal where bot will dodge bullets and check what ever is closest to us and get away from that projectile - the idea is to move away the extreme range of the projectile fire */
        }

        public override StatusTypes Process(){
            // if status is inactive, call Activate()
            ActivateIfInactive();

            // process the subgoals
            Status = ProcessSubgoals();

            ReactivateIfFailed();

            return Status;
        }

        public override void Terminate(){
            Status = StatusTypes.Completed;
        }
    }
}