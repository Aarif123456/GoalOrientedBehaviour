using Common;
using Entities.Armory;
using Entities.Triggers;
using GameWorld;

namespace Entities.GoalOrientedBehaviour {
    public class GetItemOfType : CompositeGoal {
        private readonly ItemTypes _itemTypeToGet;
        private Entity _itemEntity;
        private Trigger _itemTrigger;

        public GetItemOfType(Agent agent, ItemTypes itemTypeToGet)
            : base(agent, EnumUtility.ItemTypeToGoalType(itemTypeToGet)){
            _itemTypeToGet = itemTypeToGet;
        }

        public override void Activate(){
            Status = StatusTypes.Active;

            EventManager.Instance.Subscribe<PathToItemReadyEventPayload>(
                Events.PATH_TO_ITEM_READY,
                OnPathToItemReady);

            EventManager.Instance.Subscribe<NoPathToItemAvailableEventPayload>(
                Events.NO_PATH_TO_ITEM_AVAILABLE,
                OnNoPathToItemAvailable);

            _itemTrigger = null;
            _itemEntity = null;

            // request a path to the item
            EventManager.Instance.Enqueue(
                Events.PATH_TO_ITEM_REQUEST,
                new PathToItemRequestEventPayload(Agent, _itemTypeToGet));

            // the agent may have to wait a few update cycles before a path is
            // calculated so for appearances sake it just wanders
            AddSubgoal(new WanderAbout(Agent));
        }

        public override StatusTypes Process(){
            ActivateIfInactive();

            if (HasItemBeenStolen())
                Terminate();
            else
                // process the subgoals
                Status = ProcessSubgoals();

            return Status;
        }

        public override void Terminate(){
            EventManager.Instance.Unsubscribe<PathToItemReadyEventPayload>(
                Events.PATH_TO_ITEM_READY,
                OnPathToItemReady);

            EventManager.Instance.Unsubscribe<NoPathToItemAvailableEventPayload>(
                Events.NO_PATH_TO_ITEM_AVAILABLE,
                OnNoPathToItemAvailable);

            RemoveAllSubgoals();
            Status = StatusTypes.Completed;
        }

        private bool HasItemBeenStolen(){
            return _itemTrigger != null &&
                   !_itemTrigger.IsActive &&
                   _itemTrigger.TriggeringAgent != Agent &&
                   Agent.HasLineOfSight(_itemEntity.Kinematic.Position);
        }

        private void OnPathToItemReady(Event<PathToItemReadyEventPayload> eventArg){
            var payload = eventArg.EventData;

            if (payload.agent != Agent) // event not for us
                return;

            // clear any existing goals
            RemoveAllSubgoals();

            _itemEntity = payload.itemEntity;
            _itemTrigger = _itemEntity.GetComponent<Trigger>();

            if (!Agent.PathPlanner.SplicePath(payload.path, out var splicePath, out var spliceTarget)){
                //Debug.Log(Agent.name + " GetItem Failed at " + Time.time);
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

        private void OnNoPathToItemAvailable(Event<NoPathToItemAvailableEventPayload> eventArg){
            var payload = eventArg.EventData;

            if (payload.agent != Agent) // event not for us
                return;

            //Debug.Log(Agent.name + " GetItem Got NO path at " + Time.time");
            Status = StatusTypes.Failed;
        }
    }
}