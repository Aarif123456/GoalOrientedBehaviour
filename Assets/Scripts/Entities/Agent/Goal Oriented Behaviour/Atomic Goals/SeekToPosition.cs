using Entities.Steering;
using UnityEngine;

namespace Entities.GoalOrientedBehaviour {
    public class SeekToPosition : Goal {
        private readonly Vector3 destination;
        private readonly FaceHeading look;
        private readonly Seek seek;

        public SeekToPosition(Agent agent, Vector3 destination)
            : base(agent, GoalTypes.SeekToPosition){
            this.destination = destination;
            seek = new Seek(agent.Kinematic, destination);
            look = new FaceHeading(agent.Kinematic);
        }

        public override void Activate(){
            Status = StatusTypes.Active;
            Agent.SteeringBehaviours.Add(seek);
            Agent.SteeringBehaviours.Add(look);
        }

        public override StatusTypes Process(){
            // if status is inactive, call Activate()
            ActivateIfInactive();

            // test to see if the bot has become stuck
            if (IsStuck())
                Status = StatusTypes.Failed;
            else if (Vector3.Distance(Agent.Kinematic.Position, destination) <= seek.SatisfactionRadius)
                Status = StatusTypes.Completed;

            return Status;
        }

        public override void Terminate(){
            Agent.SteeringBehaviours.Remove(seek);
            Agent.SteeringBehaviours.Remove(look);
        }

        private bool IsStuck(){
            return false;
        }
    }
}