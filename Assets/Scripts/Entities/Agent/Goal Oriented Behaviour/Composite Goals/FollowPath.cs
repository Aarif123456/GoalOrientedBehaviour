using System.Collections.Generic;
using UnityEngine;

namespace GameBrains.AI {
    public class FollowPath : CompositeGoal {
        private readonly List<Edge> edgesToTraverse;
        private Vector3? destination;

        public FollowPath(Agent agent, List<Edge> edgesToTraverse)
            : base(agent, GoalTypes.FollowPath){
            this.edgesToTraverse = edgesToTraverse;
        }

        public override void Activate(){
            if (edgesToTraverse.Count == 0){
                Status = StatusTypes.Completed;
                return;
            }

            // if this goal is reactivated then there may be some existing
            // subgoals that must be removed
            RemoveAllSubgoals();

            Status = StatusTypes.Active;
            var edgeToTraverse = edgesToTraverse.Dequeue();
            AddSubgoal(new TraverseEdge(Agent, edgeToTraverse));
        }

        public override StatusTypes Process(){
            ActivateIfInactive();

            Status = ProcessSubgoals();

            // if there are no subgoals present check to see if the path still
            // has edges remaining. If it does then call activate to grab the
            // next edge.
            if (Status == StatusTypes.Completed){
                if (edgesToTraverse.Count > 0){
                    Activate();
                }
            }

            return Status;
        }

        public override void Terminate(){
        }
    }
}