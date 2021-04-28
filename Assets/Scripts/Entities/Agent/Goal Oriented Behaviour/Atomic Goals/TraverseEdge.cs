using Entities.Steering;
using GameWorld.Navigation.Graph;
using UnityEngine;

namespace Entities.GoalOrientedBehaviour {
    public class TraverseEdge : Goal {
        private readonly Vector3 _destination;
        private readonly FaceHeading _look;
        private readonly Seek _seek;
        private Edge _edgeToTraverse;

        public TraverseEdge(Agent agent, Edge edgeToTraverse)
            : base(agent, GoalTypes.SeekToPosition){
            //this.edgeToTraverse = edgeToTraverse;
            _destination = edgeToTraverse.ToNode.Position;
            _seek = new Seek(agent.Kinematic, _destination);
            _look = new FaceHeading(agent.Kinematic);
        }

        public override void Activate(){
            Status = StatusTypes.Active;
            Agent.SteeringBehaviours.Add(_seek);
            Agent.SteeringBehaviours.Add(_look);
//            edgeToTraverse.renderer.enabled = true;
//            edgeToTraverse.ToNode.renderer.enabled = true;
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
//            edgeToTraverse.renderer.enabled = edgeToTraverse.EdgeCollection.IsVisible;
//            edgeToTraverse.ToNode.renderer.enabled = edgeToTraverse.ToNode.NodeCollection.IsVisible;
        }

        private static bool IsStuck(){
            return false; //!Agent.PathPlanner.CanMoveBetween(Agent.Kinematic.Position, destination);
        }
    }
}