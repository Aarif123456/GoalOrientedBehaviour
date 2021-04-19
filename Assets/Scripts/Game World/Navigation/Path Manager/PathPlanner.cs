using System.Collections.Generic;

using GameBrains.AI;

using UnityEngine;

public sealed class PathPlanner : MonoBehaviour
{
    public enum SearchTypes { Position, ItemType };
    
    public PathManager PathManager { get; set; }
    
    public Agent Agent { get; set; }
    
    public Vector3 Source { get; set; }
    
    public Vector3? Destination { get; set; }
    
    public ItemTypes? ItemType { get; set; }
    
    private Entity itemEntity;
    public Entity ItemEntity { get { return itemEntity; } set { itemEntity = value; } }
    
    public TimeSlicedSearch CurrentSearch { get; set; }
    
    public SearchTypes? CurrentSearchType { get; set; }

    public void Awake()
    {
        Agent = gameObject.GetComponent<Agent>();
        PathManager = GameObject.Find("Game").GetComponent<PathManager>();
    }
    
    private void OnEnable()
    {
        EventManager.Instance.Subscribe<PathToPositionRequestEventPayload>(
            Events.PathToPositionRequest, 
            OnPathToPositionRequest);
        
        EventManager.Instance.Subscribe<PathToItemRequestEventPayload>(
            Events.PathToItemRequest, 
            OnPathToItemRequest);
    }
    
    private void OnDisable()
    {
        EventManager.Instance.Unsubscribe<PathToPositionRequestEventPayload>(
            Events.PathToPositionRequest, 
            OnPathToPositionRequest);
        
        EventManager.Instance.Unsubscribe<PathToItemRequestEventPayload>(
            Events.PathToItemRequest, 
            OnPathToItemRequest);
    }
    
    public SearchResults CycleOnce()
    {
        if (CurrentSearch == null)
        {
            throw new System.Exception("PathPlanner.CycleOnce: No search object instantiated.");
        }
        
        SearchResults searchResult = CurrentSearch.CycleOnce();
        
        if (searchResult == SearchResults.Failure)
        {
            if (CurrentSearchType.Value == PathPlanner.SearchTypes.Position)
            {
                EventManager.Instance.Enqueue<NoPathToPositionAvailableEventPayload>(
                    Events.NoPathToPositionAvailable,
                    new NoPathToPositionAvailableEventPayload(Agent));
            }
            else if (CurrentSearchType.Value == PathPlanner.SearchTypes.ItemType)
            {
                EventManager.Instance.Enqueue<NoPathToItemAvailableEventPayload>(
                    Events.NoPathToItemAvailable,
                    new NoPathToItemAvailableEventPayload(Agent));
            }
        }
        else if (searchResult == SearchResults.Success)
        {
            if (CurrentSearchType.Value == PathPlanner.SearchTypes.Position)
            {
                EventManager.Instance.Enqueue<PathToPositionReadyEventPayload>(
                    Events.PathToPositionReady,
                    new PathToPositionReadyEventPayload(Agent, new Path(Source, CurrentSearch.Solution, Destination.Value)));
            }
            else if (CurrentSearchType.Value == PathPlanner.SearchTypes.ItemType)
            {
                EventManager.Instance.Enqueue<PathToItemReadyEventPayload>(
                    Events.PathToItemReady,
                    new PathToItemReadyEventPayload(
                        Agent, 
                        new Path(Source, CurrentSearch.Solution, ItemEntity.Kinematic.Position), 
                        ItemEntity));
            }
        }
        
        return searchResult;
    }
    
    private void OnPathToPositionRequest(Event<PathToPositionRequestEventPayload> eventArg)
    {    
        PathToPositionRequestEventPayload payload = eventArg.EventData;
            
        if (payload.agent != Agent) // event not for us
        {
            return;
        }

        if (!RequestPathToPosition(payload.destination))
        {
            EventManager.Instance.Enqueue<NoPathToPositionAvailableEventPayload>(
                    Events.NoPathToPositionAvailable,
                    new NoPathToPositionAvailableEventPayload(Agent));
        }
    }
    
    public bool RequestPathToPosition(Vector3 destination)
    {
        GetReadyForNewSearch();
        
        if (Agent == null || PathManager == null || PathManager.graph == null)
        {
            return false;
        }
        
        CurrentSearchType = SearchTypes.Position;
        
        Source = Agent.Kinematic.Position;
        
        Destination = destination;
        
        // if the destination is walkable from the agent's position a path does
        // not need to be calculated, the agent can go straight to the position
        // by ARRIVING at the current waypoint (or using Quick Path)
        if (Agent.CanMoveBetween(Source, Destination.Value))
        {
            // there will be no search
            return true;
        }
        
        Node closestNodeToAgent = GetClosestNodeToPosition(Source);
        
        if (closestNodeToAgent == null)
        {
            return false;
        }
        
        Node closestNodeToDestination = GetClosestNodeToPosition(Destination.Value);
        
        if (closestNodeToDestination == null)
        {
            return false;
        }
        
        CurrentSearch =
            new TimeSlicedAStarSearch(
              closestNodeToAgent,
              closestNodeToDestination);
        
        PathManager.AddPathPlanner(this);
        
        return true;
    }
    
    private void OnPathToItemRequest(Event<PathToItemRequestEventPayload> eventArg)
    {    
        PathToItemRequestEventPayload payload = eventArg.EventData;
            
        if (payload.agent != Agent) // event not for us
        {
            return;
        }

        if (!RequestPathToItem(payload.itemType))
        {
            EventManager.Instance.Enqueue<NoPathToItemAvailableEventPayload>(
                    Events.NoPathToItemAvailable,
                    new NoPathToItemAvailableEventPayload(Agent));
        }
    }
    
    public bool RequestPathToItem(ItemTypes itemType)
    {
        GetReadyForNewSearch();
        
        if (Agent == null || PathManager == null || PathManager.graph == null)
        {
            return false;
        }
        
        CurrentSearchType = SearchTypes.ItemType;
        
        Source = Agent.Kinematic.Position;
        
        ItemType = itemType;
        
        Node closestNodeToAgent = GetClosestNodeToPosition(Source);
        
        if (closestNodeToAgent == null)
        {
            return false;
        }
        
        CurrentSearch =
            new TimeSlicedDijkstrasSearch(
              closestNodeToAgent,
              delegate(Node node) { return NodeIsCloseToItemOfType(node, ItemType.Value, out itemEntity); });
        
        PathManager.AddPathPlanner(this);
        
        return true;
    }
    
    public void GetReadyForNewSearch()
    {
        PathManager.RemovePathPlanner(this);
        CurrentSearch = null;
        CurrentSearchType = null;
        Destination = null;
        ItemType = null;
        ItemEntity = null;
    }
    
    public Node GetClosestNodeToPosition(Vector3 position)
    {
        float closestSoFar = float.MaxValue;
        Node closestNode = null;
        
        foreach (Node node in PathManager.graph.nodeCollection.Nodes)
        {
            float distance = Vector3.Distance(node.Position, position);
            
            if (distance >= closestSoFar)
            {
                continue;
            }
            
            if (Agent.CanMoveBetween(position, node.Position))
            {
                closestNode = node;
                closestSoFar = distance;
            }
        }
        
        return closestNode;
    }
    
    public float GetCostToClosestItem(ItemTypes giverType)
    {
        float closestSoFar = float.MaxValue;
        
        Node closestNodeToAgent = GetClosestNodeToPosition(Agent.Kinematic.Position);
        
        if (closestNodeToAgent == null)
        {
            return closestSoFar;
        }
        
        // TODO: should cache triggers??
        foreach (Node node in PathManager.Graph.nodeCollection.Nodes)
        {
            foreach (Trigger trigger in node.NearbyTriggers)
            {
                if (trigger.IsActive && trigger.EntityType != EnumUtility.ItemTypeToEntityType(giverType))
                {
                    float cost = CalculateCostBetweenNodes(closestNodeToAgent, node);
        
                    if (cost < closestSoFar)
                    {
                        closestSoFar = cost;
                    }
                }
            }
        }
        
        return closestSoFar;
    }
    
    public float CalculateCostBetweenNodes(Node sourceNode, Node destinationNode)
    {
        return LeastCostPathTable.Cost(sourceNode, destinationNode);
    }
    
    public bool NodeIsCloseToItemOfType(Node node, ItemTypes itemType, out Entity itemEntity)
    {
        foreach (Trigger trigger in node.NearbyTriggers)
        {
            if (trigger.EntityType == EnumUtility.ItemTypeToEntityType(itemType) && trigger.IsActive)
            {
                itemEntity = trigger;
                return true;
            }
        }
        
        itemEntity = null;
        return false;
    }
        
    public bool SplicePath(Path path, out List<Edge> edges, out Vector3? spliceTarget)
    {
        edges = new List<Edge>();
        spliceTarget = null;
        
        if (Agent.CanMoveBetween(Agent.Kinematic.Position, path.Destination))
        {
            return true;
        }
        
        for (int edgeIndex = path.Edges.Count - 1; edgeIndex >= 0; edgeIndex--)
        {
            if (!Agent.CanMoveBetween(Agent.Kinematic.Position, path.Edges[edgeIndex].ToNode.Position))
            {
                edges.Add(path.Edges[edgeIndex]);
            }
            else
            {
                spliceTarget = path.Edges[edgeIndex].ToNode.Position;
                edges.Reverse();
                return true;
            }
        }
        
        edges.Reverse();
        
        if (path.Edges.Count > 0 && Agent.CanMoveBetween(Agent.Kinematic.Position, path.Edges[0].FromNode.Position))
        {
            spliceTarget = path.Edges[0].FromNode.Position;
            return true;
        }
        
        // if we get here, probably QuickPath led us astray and now we can't get back on the FullPath
        return false;
    }
}