using GameWorld;
using GameWorld.Navigation.Graph;
using UnityEngine;

namespace Entities.GoalOrientedBehaviour {
    public class Explore : CompositeGoal {
        private Vector3? _destination;

        public Explore(Agent agent)
            : base(agent, GoalTypes.Explore){
        }

        public override void Activate(){
            Status = StatusTypes.Active;

            EventManager.Instance.Subscribe<PathToPositionReadyEventPayload>(
                Events.PATH_TO_POSITION_READY,
                OnPathToPositionReady);

            EventManager.Instance.Subscribe<NoPathToPositionAvailableEventPayload>(
                Events.NO_PATH_TO_POSITION_AVAILABLE,
                OnNoPathToPositionAvailable);

            // if this goal is reactivated then there may be some existing
            // subgoals that must be removed
            RemoveAllSubgoals();

            if (!_destination.HasValue){
                var graph = GameObject.Find("Game").GetComponent<Graph>();
                var nodes = graph.nodeCollection.Nodes;
                var index = Random.Range(0, nodes.Length);

                // grab a random position
                _destination = nodes[index].Position;
            }

            EventManager.Instance.Enqueue(
                Events.PATH_TO_POSITION_REQUEST,
                new PathToPositionRequestEventPayload(Agent, _destination.Value));

            // the bot may have to wait a few update cycles before a path is
            // calculated so for appearances sake it simply SEEKS toward the
            // destination until a path has been found
            AddSubgoal(new SeekToPosition(Agent, _destination.Value));
        }

        public override StatusTypes Process(){
            ActivateIfInactive();

            Status = ProcessSubgoals();

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
                Debug.Log(Agent.name + " Explore Failed at " + Time.time);
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

            //Debug.Log(Agent.name + " Explore Got NO path at " + Time.time);
            Status = StatusTypes.Failed;
        }
    }
}