using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using Entities;
using Entities.Armory;
using GameWorld.Navigation.Graph;
using UnityEngine;
using Debug = System.Diagnostics.Debug;

namespace GameWorld {
    public sealed class PathPlanner : MonoBehaviour {
        public enum SearchTypes {
            Position,
            ItemType
        }

        private Entity itemEntity;

        public PathManager PathManager { get; set; }

        public Agent Agent { get; set; }

        public Vector3 Source { get; set; }

        public Vector3? Destination { get; set; }

        public ItemTypes? ItemType { get; set; }

        public Entity ItemEntity {
            get => itemEntity;
            set => itemEntity = value;
        }

        public TimeSlicedSearch CurrentSearch { get; set; }

        public SearchTypes? CurrentSearchType { get; set; }

        public void Awake(){
            Agent = gameObject.GetComponent<Agent>();
            PathManager = GameObject.Find("Game").GetComponent<PathManager>();
        }

        private void OnEnable(){
            EventManager.Instance.Subscribe<PathToPositionRequestEventPayload>(
                Events.PathToPositionRequest,
                OnPathToPositionRequest);

            EventManager.Instance.Subscribe<PathToItemRequestEventPayload>(
                Events.PathToItemRequest,
                OnPathToItemRequest);
        }

        private void OnDisable(){
            EventManager.Instance.Unsubscribe<PathToPositionRequestEventPayload>(
                Events.PathToPositionRequest,
                OnPathToPositionRequest);

            EventManager.Instance.Unsubscribe<PathToItemRequestEventPayload>(
                Events.PathToItemRequest,
                OnPathToItemRequest);
        }

        public SearchResults CycleOnce(){
            if (CurrentSearch == null) throw new Exception("PathPlanner.CycleOnce: No search object instantiated.");

            var searchResult = CurrentSearch.CycleOnce();

            switch (searchResult){
                case SearchResults.Failure:
                    if (CurrentSearchType != null){
                        switch (CurrentSearchType.Value){
                            case SearchTypes.Position:
                                EventManager.Instance.Enqueue(
                                    Events.NoPathToPositionAvailable,
                                    new NoPathToPositionAvailableEventPayload(Agent));
                                break;
                            case SearchTypes.ItemType:
                                EventManager.Instance.Enqueue(
                                    Events.NoPathToItemAvailable,
                                    new NoPathToItemAvailableEventPayload(Agent));
                                break;
                        }
                    }

                    break;
                case SearchResults.Success when CurrentSearchType.Value == SearchTypes.Position:
                    EventManager.Instance.Enqueue(
                        Events.PathToPositionReady,
                        new PathToPositionReadyEventPayload(Agent,
                            new Path(Source, CurrentSearch.Solution, Destination.Value)));
                    break;
                case SearchResults.Success:{
                    if (CurrentSearchType.Value == SearchTypes.ItemType){
                        EventManager.Instance.Enqueue(
                            Events.PathToItemReady,
                            new PathToItemReadyEventPayload(
                                Agent,
                                new Path(Source, CurrentSearch.Solution, ItemEntity.Kinematic.Position),
                                ItemEntity));
                    }

                    break;
                }
            }

            return searchResult;
        }

        private void OnPathToPositionRequest(Event<PathToPositionRequestEventPayload> eventArg){
            var payload = eventArg.EventData;

            if (payload.agent != Agent) // event not for us
                return;

            if (!RequestPathToPosition(payload.destination)){
                EventManager.Instance.Enqueue(
                    Events.NoPathToPositionAvailable,
                    new NoPathToPositionAvailableEventPayload(Agent));
            }
        }

        public bool RequestPathToPosition(Vector3 destination){
            GetReadyForNewSearch();

            if (Agent == null || PathManager == null || PathManager.graph == null) return false;

            CurrentSearchType = SearchTypes.Position;

            Source = Agent.Kinematic.Position;

            Destination = destination;

            // if the destination is walkable from the agent's position a path does
            // not need to be calculated, the agent can go straight to the position
            // by ARRIVING at the current waypoint (or using Quick Path)
            if (Agent.CanMoveBetween(Source, Destination.Value))
                // there will be no search
                return true;

            var closestNodeToAgent = GetClosestNodeToPosition(Source);

            if (ReferenceEquals(closestNodeToAgent, null)) return false;

            var closestNodeToDestination = GetClosestNodeToPosition(Destination.Value);

            if (closestNodeToDestination == null) return false;

            CurrentSearch =
                new TimeSlicedAStarSearch(
                    closestNodeToAgent,
                    closestNodeToDestination);

            PathManager.AddPathPlanner(this);

            return true;
        }

        private void OnPathToItemRequest(Event<PathToItemRequestEventPayload> eventArg){
            var payload = eventArg.EventData;

            if (payload.agent != Agent) // event not for us
                return;

            if (!RequestPathToItem(payload.itemType)){
                EventManager.Instance.Enqueue(
                    Events.NoPathToItemAvailable,
                    new NoPathToItemAvailableEventPayload(Agent));
            }
        }

        public bool RequestPathToItem(ItemTypes itemType){
            GetReadyForNewSearch();

            if (Agent == null || PathManager == null || PathManager.graph == null) return false;

            CurrentSearchType = SearchTypes.ItemType;

            Source = Agent.Kinematic.Position;

            ItemType = itemType;

            var closestNodeToAgent = GetClosestNodeToPosition(Source);

            if (ReferenceEquals(closestNodeToAgent, null)) return false;

            CurrentSearch =
                new TimeSlicedDijkstrasSearch(
                    closestNodeToAgent,
                    node => {
                        Debug.Assert(ItemType != null, nameof(ItemType) + " != null");
                        return NodeIsCloseToItemOfType(node, ItemType.Value, out itemEntity);
                    });

            PathManager.AddPathPlanner(this);

            return true;
        }

        public void GetReadyForNewSearch(){
            PathManager.RemovePathPlanner(this);
            CurrentSearch = null;
            CurrentSearchType = null;
            Destination = null;
            ItemType = null;
            ItemEntity = null;
        }

        public Node GetClosestNodeToPosition(Vector3 position){
            var closestSoFar = float.MaxValue;
            Node closestNode = null;

            foreach (var node in PathManager.graph.nodeCollection.Nodes){
                var distance = Vector3.Distance(node.Position, position);

                if (distance >= closestSoFar) continue;

                if (!Agent.CanMoveBetween(position, node.Position)) continue;
                closestNode = node;
                closestSoFar = distance;
            }

            return closestNode;
        }

        public float GetCostToClosestItem(ItemTypes giverType){
            var closestNodeToAgent = GetClosestNodeToPosition(Agent.Kinematic.Position);

            if (ReferenceEquals(closestNodeToAgent, null)) return -1;

            // TODO: should cache triggers??

            return (from node in PathManager.Graph.nodeCollection.Nodes
                from trigger in node.NearbyTriggers
                where trigger.IsActive &&
                      trigger.EntityType != EnumUtility.ItemTypeToEntityType(giverType)
                select CalculateCostBetweenNodes(closestNodeToAgent, node)).Prepend(float.MaxValue).Min();
        }

        public float CalculateCostBetweenNodes(Node sourceNode, Node destinationNode){
            return LeastCostPathTable.Cost(sourceNode, destinationNode);
        }

        public bool NodeIsCloseToItemOfType(Node node, ItemTypes itemType, out Entity itemEntity){
            foreach (var trigger in node.NearbyTriggers){
                if (trigger.EntityType != EnumUtility.ItemTypeToEntityType(itemType) || !trigger.IsActive) continue;
                itemEntity = trigger;
                return true;
            }

            itemEntity = null;
            return false;
        }

        public bool SplicePath(Path path, out List<Edge> edges, out Vector3? spliceTarget){
            edges = new List<Edge>();
            spliceTarget = null;

            if (Agent.CanMoveBetween(Agent.Kinematic.Position, path.Destination)) return true;

            for (var edgeIndex = path.Edges.Count - 1; edgeIndex >= 0; edgeIndex--){
                if (!Agent.CanMoveBetween(Agent.Kinematic.Position, path.Edges[edgeIndex].ToNode.Position))
                    edges.Add(path.Edges[edgeIndex]);
                else{
                    spliceTarget = path.Edges[edgeIndex].ToNode.Position;
                    edges.Reverse();
                    return true;
                }
            }

            edges.Reverse();

            if (path.Edges.Count <= 0 ||
                !Agent.CanMoveBetween(Agent.Kinematic.Position, path.Edges[0].FromNode.Position)) return false;
            spliceTarget = path.Edges[0].FromNode.Position;
            return true;

            // if we get here, probably QuickPath led us astray and now we can't get back on the FullPath
        }
    }
}