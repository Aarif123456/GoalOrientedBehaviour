using Entities.Steering;

namespace Entities.GoalOrientedBehaviour {
    public class WanderAbout : Goal {
        private readonly Wander _wander;

        public WanderAbout(Agent agent)
            : base(agent, GoalTypes.WanderAbout){
            _wander = new Wander(agent.Kinematic);
        }

        public override void Activate(){
            Status = StatusTypes.Active;
            Agent.SteeringBehaviours.Add(_wander);
        }

        public override StatusTypes Process(){
            // if status is inactive, call Activate()
            ActivateIfInactive();

            return Status;
        }

        public override void Terminate(){
            Agent.SteeringBehaviours.Remove(_wander);
        }
    }
}