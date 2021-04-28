using Entities.Steering;
using UnityEngine;

namespace Entities.GoalOrientedBehaviour {
    public class SeekToPosition : Goal {
        private readonly Vector3 _destination;
        private readonly FaceHeading _look;
        private readonly Seek _seek;

        public SeekToPosition(Agent agent, Vector3 destination)
            : base(agent, GoalTypes.SeekToPosition){
            _destination = destination;
            _seek = new Seek(agent.Kinematic, destination);
            _look = new FaceHeading(agent.Kinematic);
        }

        public override void Activate(){
            Status = StatusTypes.Active;
            Agent.SteeringBehaviours.Add(_seek);
            Agent.SteeringBehaviours.Add(_look);
        }

        public override StatusTypes Process(){
            // if status is inactive, call Activate()
            ActivateIfInactive();

            // test to see if the bot has become stuck
            if (IsStuck())
                Status = StatusTypes.Failed;
            else if (Vector3.Distance(Agent.Kinematic.Position, _destination) <= _seek.SatisfactionRadius)
                Status = StatusTypes.Completed;

            return Status;
        }

        public override void Terminate(){
            Agent.SteeringBehaviours.Remove(_seek);
            Agent.SteeringBehaviours.Remove(_look);
        }

        /* TODO: figure out if agent has been stuck */
        private static bool IsStuck(){
            return false;
        }
    }
}