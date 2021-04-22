using System.Collections.Generic;
using GameWorld;
using GameWorld.Navigation.Graph;
using UnityEngine;

namespace Entities.GoalOrientedBehaviour {
    public class MoveToPosition : CompositeGoal {
        private readonly Vector3 destination;

        public MoveToPosition(Agent agent, Vector2 destination)
            : base(agent, GoalTypes.MoveToPosition){
            this.destination = destination;
        }

        public override void Activate(){
            Status = StatusTypes.Active;

            // make sure the subgoal list is clear.
            RemoveAllSubgoals();

            EventManager.Instance.Subscribe<PathToPositionReadyEventPayload>(
                Events.PathToPositionReady,
                OnPathToPositionReady);

            EventManager.Instance.Subscribe<NoPathToPositionAvailableEventPayload>(
                Events.NoPathToPositionAvailable,
                OnNoPathToPositionAvailable);

            EventManager.Instance.Enqueue(
                Events.PathToPositionRequest,
                new PathToPositionRequestEventPayload(Agent, destination));

            AddSubgoal(new SeekToPosition(Agent, destination));
        }

        public override StatusTypes Process(){
            // if status is inactive, call Activate()
            ActivateIfInactive();

            // process the subgoals
            Status = ProcessSubgoals();

            // if any of the subgoals have failed then this goal re-plans
            ReactivateIfFailed();

            return Status;
        }

        public override void Terminate(){
            EventManager.Instance.Unsubscribe<PathToPositionReadyEventPayload>(
                Events.PathToPositionReady,
                OnPathToPositionReady);

            EventManager.Instance.Unsubscribe<NoPathToPositionAvailableEventPayload>(
                Events.NoPathToPositionAvailable,
                OnNoPathToPositionAvailable);
        }

        private void OnPathToPositionReady(Event<PathToPositionReadyEventPayload> eventArg){
            var payload = eventArg.EventData;

            if (payload.agent != Agent) // event not for us
            {
                return;
            }

            // clear any existing goals
            RemoveAllSubgoals();

            List<Edge> splicePath;
            Vector3? spliceTarget;
            if (!Agent.PathPlanner.SplicePath(payload.path, out splicePath, out spliceTarget)){
                //Debug.Log(Agent.name + " MoveToPosition Failed at " + Time.time);
                // QuickPath lead us astray!
                Status = StatusTypes.Failed;
            }
            else{
                // add in reverse order
                AddSubgoal(new SeekToPosition(Agent, payload.path.Destination));

                if (!Agent.CanMoveTo(payload.path.Destination)){
                    if (splicePath.Count > 0){
                        AddSubgoal(new FollowPath(Agent, splicePath));
                    }

                    if (spliceTarget.HasValue){
                        AddSubgoal(new SeekToPosition(Agent, spliceTarget.Value));
                    }
                }
            }
        }

        private void OnNoPathToPositionAvailable(Event<NoPathToPositionAvailableEventPayload> eventArg){
            var payload = eventArg.EventData;

            if (payload.agent != Agent) // event not for us
            {
                return;
            }

            //Debug.Log(Agent.name + " MoveToPosition Got NO path at " + Time.time");
            Status = StatusTypes.Failed;
        }
    }
}