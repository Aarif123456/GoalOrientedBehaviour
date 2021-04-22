using System.Collections.Generic;
using Entities.Armory;
using Entities.Triggers;
using GameWorld;
using GameWorld.Navigation.Graph;
using UnityEngine;
using Utility;

namespace Entities.GoalOrientedBehaviour {
    public class GetItemOfType : CompositeGoal {
        private readonly ItemTypes itemTypeToGet;
        private Entity itemEntity;
        private Trigger itemTrigger;

        public GetItemOfType(Agent agent, ItemTypes itemTypeToGet)
            : base(agent, EnumUtility.ItemTypeToGoalType(itemTypeToGet)){
            this.itemTypeToGet = itemTypeToGet;
        }

        public override void Activate(){
            Status = StatusTypes.Active;

            EventManager.Instance.Subscribe<PathToItemReadyEventPayload>(
                Events.PathToItemReady,
                OnPathToItemReady);

            EventManager.Instance.Subscribe<NoPathToItemAvailableEventPayload>(
                Events.NoPathToItemAvailable,
                OnNoPathToItemAvailable);

            itemTrigger = null;
            itemEntity = null;

            // request a path to the item
            EventManager.Instance.Enqueue(
                Events.PathToItemRequest,
                new PathToItemRequestEventPayload(Agent, itemTypeToGet));

            // the agent may have to wait a few update cycles before a path is
            // calculated so for appearances sake it just wanders
            AddSubgoal(new WanderAbout(Agent));
        }

        public override StatusTypes Process(){
            ActivateIfInactive();

            if (HasItemBeenStolen()){
                Terminate();
            }
            else
                // process the subgoals
            {
                Status = ProcessSubgoals();
            }

            return Status;
        }

        public override void Terminate(){
            EventManager.Instance.Unsubscribe<PathToItemReadyEventPayload>(
                Events.PathToItemReady,
                OnPathToItemReady);

            EventManager.Instance.Unsubscribe<NoPathToItemAvailableEventPayload>(
                Events.NoPathToItemAvailable,
                OnNoPathToItemAvailable);

            RemoveAllSubgoals();
            Status = StatusTypes.Completed;
        }

        private bool HasItemBeenStolen(){
            return itemTrigger != null &&
                   !itemTrigger.IsActive &&
                   itemTrigger.TriggeringAgent != Agent &&
                   Agent.HasLineOfSight(itemEntity.Kinematic.Position);
        }

        private void OnPathToItemReady(Event<PathToItemReadyEventPayload> eventArg){
            var payload = eventArg.EventData;

            if (payload.agent != Agent) // event not for us
            {
                return;
            }

            // clear any existing goals
            RemoveAllSubgoals();

            itemEntity = payload.itemEntity;
            itemTrigger = itemEntity.GetComponent<Trigger>();

            List<Edge> splicePath;
            Vector3? spliceTarget;
            if (!Agent.PathPlanner.SplicePath(payload.path, out splicePath, out spliceTarget)){
                //Debug.Log(Agent.name + " GetItem Failed at " + Time.time);
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

        private void OnNoPathToItemAvailable(Event<NoPathToItemAvailableEventPayload> eventArg){
            var payload = eventArg.EventData;

            if (payload.agent != Agent) // event not for us
            {
                return;
            }

            //Debug.Log(Agent.name + " GetItem Got NO path at " + Time.time");
            Status = StatusTypes.Failed;
        }
    }
}