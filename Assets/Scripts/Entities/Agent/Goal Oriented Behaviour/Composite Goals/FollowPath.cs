using System.Collections.Generic;
using GameWorld.Navigation.Graph;
using UnityEngine;
using Utility.DataStructures;

namespace Entities.GoalOrientedBehaviour {
    public class FollowPath : CompositeGoal {
        private readonly List<Edge> _edgesToTraverse;
        private Vector3? _destination;

        public FollowPath(Agent agent, List<Edge> edgesToTraverse)
            : base(agent, GoalTypes.FollowPath){
            _edgesToTraverse = edgesToTraverse;
        }

        public override void Activate(){
            if (_edgesToTraverse.Count == 0){
                Status = StatusTypes.Completed;
                return;
            }

            // if this goal is reactivated then there may be some existing
            // subgoals that must be removed
            RemoveAllSubgoals();

            Status = StatusTypes.Active;
            var edgeToTraverse = _edgesToTraverse.Dequeue();
            AddSubgoal(new TraverseEdge(Agent, edgeToTraverse));
        }

        public override StatusTypes Process(){
            ActivateIfInactive();

            Status = ProcessSubgoals();

            // if there are no subgoals present check to see if the path still
            // has edges remaining. If it does then call activate to grab the
            // next edge.
            if (Status != StatusTypes.Completed) return Status;
            if (_edgesToTraverse.Count > 0)
                Activate();

            return Status;
        }

        public override void Terminate(){
        }
    }
}