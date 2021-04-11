using System.Collections.Generic;

using UnityEngine;
    
[ExecuteInEditMode]
public class Node : MonoBehaviour
{
    public bool locked;
    public bool nameLocked;
    public Color color = Color.magenta;
    public float alpha = 0.25f;
    public float radius = 0.5f;
    public float lookAheadDistance = 2f;
    public bool useCapsuleCast = false;
    public float raycastMaximumDistance = 10f;
    public float raycastPathRadius = 0.5f;
    public float surfaceOffset = 1.1f;
    public LayerMask raycastObstaclesLayerMask;
    public List<Node> neighbours;
    public List<Edge> outEdges;
    public List<Trigger> nearbyTriggers;
    
    public Graph Graph
    {
        get
        {
            return NodeCollection == null ? null : NodeCollection.Graph;
        }
    }
    
    public NodeCollection NodeCollection
    {
        get
        {
            return transform.parent == null ? null : transform.parent.GetComponent<NodeCollection>();
        }
    }
    
    public Vector3 Position
    {
        get
        {
            return transform.position;
        }
    }
    
    public bool IsLocked
    {
        get
        {
            return locked || (NodeCollection != null && NodeCollection.IsLocked);
        }
    }
    
    public List<Trigger> NearbyTriggers 
    {
        get 
        {
            return this.nearbyTriggers;
        }
        
        set 
        {
            nearbyTriggers = value;
        }
    }

    public void Awake()
    {
        if (!IsLocked)
        {
            color.a = alpha;
            GetComponent<Renderer>().sharedMaterial = new Material(GetComponent<Renderer>().sharedMaterial) { color = color };
        }
    }
    
    public void Update()
    {
        if (!IsLocked)
        {
            GenerateNameFromPosition();
        }
    }
    
    public GameObject AddConnectedNode(bool oneWay, Camera camera)
    {
        if (IsLocked || NodeCollection == null || Graph == null || Graph.nodePrefab == null)
        {
            return null;
        }
#if UNITY_EDITOR    
        GameObject connectedNodeObject = UnityEditor.PrefabUtility.InstantiatePrefab(Graph.nodePrefab) as GameObject;
#else
        GameObject connectedNodeObject = Instantiate(Graph.nodePrefab) as GameObject;
#endif
        
        if (connectedNodeObject != null)
        {
            Node connectedNode = connectedNodeObject.GetComponent<Node>();
            
            if (camera != null)
            {
                connectedNode.CastToCollider(camera.transform.position, camera.transform.forward, 5f, 20f);
            }
            
            connectedNode.GenerateNameFromPosition();
            NodeCollection.ApplyParametersToNode(connectedNode);
            connectedNodeObject.transform.parent = transform.parent;
            
            if (!oneWay)
            {
                connectedNode.AddConnection(this);
            }
            
            AddConnection(connectedNode);
        }
        
        return connectedNodeObject;
    }
    
    public void AddConnection(Node toNode)
    {
        if (!IsLocked && toNode != this && Graph != null && toNode.Graph == Graph && Graph.edgeCollection != null)
        {
            if (neighbours == null)
            {
                neighbours = new List<Node>();
            }
            
            if (outEdges == null)
            {
                outEdges = new List<Edge>();
            }
            
            if (!neighbours.Contains(toNode))
            {
                GameObject edgeObject = Graph.edgeCollection.AddEdge(this, toNode);
            
                if (edgeObject != null)
                {    
                    neighbours.Add(toNode);
                
                    Edge edge = edgeObject.GetComponent<Edge>();
                    
                    if (!outEdges.Contains(edge))
                    {
                        outEdges.Add(edge);
                    }
                }
            }
        }
    }
    
    public void CastToCollider(Vector3 fromPosition, Vector3 forward, float minDistance, float maxDistance)
    {
        if (!IsLocked)
        {
            RaycastHit hit;
            Ray ray = new Ray(fromPosition, forward);
            bool flag = false;
            
            if (maxDistance > 0f)
            {
                flag = Physics.SphereCast(ray, radius, out hit, maxDistance, raycastObstaclesLayerMask);
            }
            else
            {
                flag = Physics.SphereCast(ray, radius, out hit, float.MaxValue, raycastObstaclesLayerMask);
            }
            
            if (flag)
            {
                transform.position = hit.point + Vector3.up * surfaceOffset;
                
            }
            else if (minDistance > 0f)
            {
                transform.position = fromPosition + (forward.normalized * minDistance) + Vector3.up * surfaceOffset;
            }
        }
    }
    
    public static void ConnectNodes(Node[] nodes)
    {
        foreach (Node nodeFrom in nodes)
        {
            foreach (Node nodeTo in nodes)
            {
                if (!nodeFrom.IsLocked && nodeFrom != nodeTo)
                {
                    nodeFrom.AddConnection(nodeTo);
                }
            }
        }
    }
    
    public static void CycleConnection(Node fromNode, Node toNode)
    {
        if (!fromNode.IsLocked && !toNode.IsLocked)
        {
            bool oneConnectedToTwo = fromNode.IsConnectedTo(toNode);
            bool twoConnectedToOne = toNode.IsConnectedTo(fromNode);
            
            if (oneConnectedToTwo && twoConnectedToOne)
            {
                toNode.RemoveConnection(fromNode);
            }
            else if (oneConnectedToTwo)
            {
                toNode.AddConnection(fromNode);
                fromNode.RemoveConnection(toNode);
            }
            else if (twoConnectedToOne)
            {
                toNode.RemoveConnection(fromNode);
            }
            else
            {
                fromNode.AddConnection(toNode);
                toNode.AddConnection(fromNode);
            }
        }
    }
    
    public static void DisconnectNodes(Node[] nodes)
    {
        foreach (Node nodeFrom in nodes)
        {
            foreach (Node nodeTo in nodes)
            {
                if (!nodeFrom.IsLocked && nodeFrom != nodeTo)
                {
                    nodeFrom.RemoveConnection(nodeTo);
                }
            }
        }
    }
    
    public void DropToSurface()
    {
        if (!IsLocked)
        {
            Vector3 forward = new Vector3(0f, -1f, 0f);
            CastToCollider(transform.position, forward, 0f, 0f);
        }
    }
    
    public void GenerateNameFromPosition()
    {
        if (!IsLocked && !nameLocked)
        {
            Vector3 position = transform.position;
            name = 
                "Node (" + 
                position.x.ToString("F1") +
                ", " +
                position.y.ToString("F1") +
                ", " +
                position.z.ToString("F1") +
                ")";    
        }
    }
    
    public bool IsConnectedTo(Node toNode)
    {
        return neighbours.Contains(toNode);
    }
    
    public void RaycastNeighbours(Node[] potentialNeighbours, bool clearConnections, bool requireSymmetry)
    {
        if (!IsLocked)
        {
            if (clearConnections)
            {
                RemoveAllConnections();
            }
            
            if (potentialNeighbours != null)
            {
                for (int i = 0; i < potentialNeighbours.Length; i++)
                {
                    Node neighbour = potentialNeighbours[i];
                    
                    if ((neighbour != null) && (neighbour != this))
                    {
                        RaycastHit hitInfo;
                        Vector3 direction = neighbour.transform.position - transform.position;

                        if ((direction.magnitude <= raycastMaximumDistance) && 
                            ((useCapsuleCast &&
                            !Physics.CapsuleCast(
                                    transform.position - (1 - raycastPathRadius) * Vector3.up, 
                                    transform.position + (1 - raycastPathRadius) * Vector3.up,
                                    raycastPathRadius, 
                                    direction,
                                    out hitInfo,
                                    direction.magnitude,
                                    raycastObstaclesLayerMask)) ||
                             (!useCapsuleCast &&
                              !Physics.SphereCast(
                                    transform.position, 
                                    raycastPathRadius, 
                                    direction, 
                                    out hitInfo, 
                                    direction.magnitude,
                                    raycastObstaclesLayerMask))))
                        {
                            if (!requireSymmetry)
                            {
                                AddConnection(neighbour);
                            }
                            else 
                            {
                                AddConnection(neighbour);
                                neighbour.AddConnection(this);
                            }
                        }
                    }
                }
            }
        }
    }
    
    public void RemoveAllConnections()
    {
        if (!IsLocked)
        {
            neighbours.Clear();
            
            for (int i = 0; i < outEdges.Count; i++)
            {
                Edge edge = outEdges[i];
                
                if (edge != null)
                {
                    DestroyImmediate(edge.gameObject);
                }
            }
            
            outEdges.Clear();
        }
    }

    public void RemoveConnection(Node toNode)
    {
        if (!IsLocked)
        {
            neighbours.Remove(toNode);
            
            for (int i = 0; i < outEdges.Count; i++)
            {
                if (outEdges[i] != null && outEdges[i].ToNode == toNode)
                {
                    DestroyImmediate(outEdges[i].gameObject);
                    outEdges.RemoveAt(i);
                    break;
                }
            }
        }
    }
}