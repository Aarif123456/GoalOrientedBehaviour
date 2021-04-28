using GameWorld;
using UnityEngine;

namespace Entities.GoalOrientedBehaviour {
    public class MoveToPosition : CompositeGoal {
        private readonly Vector3 _destination;

        public MoveToPosition(Agent agent, Vector2 destination)
            : base(agent, GoalTypes.MoveToPosition){
            _destination = destination;
        }

        public override void Activate(){
            Status = StatusTypes.Active;

            // make sure the subgoal list is clear.
            RemoveAllSubgoals();

            EventManager.Instance.Subscribe<PathToPositionReadyEventPayload>(
                Events.PATH_TO_POSITION_READY,
                OnPathToPositionReady);

            EventManager.Instance.Subscribe<NoPathToPositionAvailableEventPayload>(
                Events.NO_PATH_TO_POSITION_AVAILABLE,
                OnNoPathToPositionAvailable);

            EventManager.Instance.Enqueue(
                Events.PATH_TO_POSITION_REQUEST,
                new PathToPositionRequestEventPayload(Agent, _destination));

            AddSubgoal(new SeekToPosition(Agent, _destination));
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
                Events.PATH_TO_POSITION_READY,
                OnPathToPositionReady);

            EventManager.Instance.Unsubscribe<NoPathToPositionAvailableEventPayload>(
                Events.NO_PATH_TO_POSITION_AVAILABLE,
                OnNoPathToPositionAvailable);
        }

        private void OnPathToPositionReady(Event<PathToPositionReadyEventPayload> eventArg){
            var payload = eventArg.EventData;

            if (payload.agent != Agent) // event not for us
                return;

            // clear any existing goals
            RemoveAllSubgoals();

            if (!Agent.PathPlanner.SplicePath(payload.path, out var splicePath, out var spliceTarget)){
                //Debug.Log(Agent.name + " MoveToPosition Failed at " + Time.time);
                // QuickPath lead us astray!
                Status = StatusTypes.Failed;
            }
            else{
                // add in reverse order
                AddSubgoal(new SeekToPosition(Agent, payload.path.Destination));

                if (Agent.CanMoveTo(payload.path.Destination)) return;
                if (splicePath.Count > 0) AddSubgoal(new FollowPath(Agent, splicePath));

                if (spliceTarget.HasValue) AddSubgoal(new SeekToPosition(Agent, spliceTarget.Value));
            }
        }

        private void OnNoPathToPositionAvailable(Event<NoPathToPositionAvailableEventPayload> eventArg){
            var payload = eventArg.EventData;

            if (payload.agent != Agent) // event not for us
                return;

            //Debug.Log(Agent.name + " MoveToPosition Got NO path at " + Time.time");
            Status = StatusTypes.Failed;
        }
    }
}