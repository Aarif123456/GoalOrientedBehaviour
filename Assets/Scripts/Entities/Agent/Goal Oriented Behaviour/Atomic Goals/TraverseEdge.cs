using Entities.Steering;
using GameWorld.Navigation.Graph;
using UnityEngine;

namespace Entities.GoalOrientedBehaviour {
    public class TraverseEdge : Goal {
        private readonly Vector3 destination;
        private readonly FaceHeading look;
        private readonly Seek seek;
        private Edge edgeToTraverse;

        public TraverseEdge(Agent agent, Edge edgeToTraverse)
            : base(agent, GoalTypes.SeekToPosition){
            //this.edgeToTraverse = edgeToTraverse;
            destination = edgeToTraverse.ToNode.Position;
            seek = new Seek(agent.Kinematic, destination);
            look = new FaceHeading(agent.Kinematic);
        }

        public override void Activate(){
            Status = StatusTypes.Active;
            Agent.SteeringBehaviours.Add(seek);
            Agent.SteeringBehaviours.Add(look);
//            edgeToTraverse.renderer.enabled = true;
//            edgeToTraverse.ToNode.renderer.enabled = true;
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
//            edgeToTraverse.renderer.enabled = edgeToTraverse.EdgeCollection.IsVisible;
//            edgeToTraverse.ToNode.renderer.enabled = edgeToTraverse.ToNode.NodeCollection.IsVisible;
        }

        private bool IsStuck(){
            return false; //!Agent.PathPlanner.CanMoveBetween(Agent.Kinematic.Position, destination);
        }
    }
}