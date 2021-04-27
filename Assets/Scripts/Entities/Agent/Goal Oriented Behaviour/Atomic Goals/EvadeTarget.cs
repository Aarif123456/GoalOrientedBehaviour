using Entities.Steering;

namespace Entities.GoalOrientedBehaviour {
    public class EvadeTarget : Goal {
        private readonly Evade evade;

        public EvadeTarget(Agent agent, Entity targetAgent)
            : base(agent, GoalTypes.EvadeBot){
            evade = new Evade(agent.Kinematic, targetAgent.Kinematic);
        }

        public override void Activate(){
            Status = StatusTypes.Active;
            Agent.SteeringBehaviours.Add(evade);
        }

        public override StatusTypes Process(){
            // if status is inactive, call Activate()
            ActivateIfInactive();
            // test to see if the bot has become stuck
            if (IsStuck())
                Status = StatusTypes.Failed;
            // if target goes out of view terminate
            else if (!Agent.TargetingSystem.IsTargetWithinFieldOfView)
                Status = StatusTypes.Completed;    
            return Status;
        }

        public override void Terminate(){
            Agent.SteeringBehaviours.Remove(evade);
        }

        /* TODO: figure out if agent has been stuck */
        private static bool IsStuck(){
            return false;
        }
    }
}