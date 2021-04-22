using System.Collections.Generic;
using Entities.Triggers;
using UnityEditor;
using UnityEngine;

namespace GameWorld.Navigation.Graph {
    [ExecuteInEditMode]
    public class Node : MonoBehaviour {
        public bool locked;
        public bool nameLocked;
        public Color color = Color.magenta;
        public float alpha = 0.25f;
        public float radius = 0.5f;
        public float lookAheadDistance = 2f;
        public bool useCapsuleCast;
        public float raycastMaximumDistance = 10f;
        public float raycastPathRadius = 0.5f;
        public float surfaceOffset = 1.1f;
        public LayerMask raycastObstaclesLayerMask;
        public List<Node> neighbours;
        public List<Edge> outEdges;
        public List<Trigger> nearbyTriggers;

        public Graph Graph => NodeCollection == null ? null : NodeCollection.Graph;

        public NodeCollection NodeCollection =>
            transform.parent == null ? null : transform.parent.GetComponent<NodeCollection>();

        public Vector3 Position => transform.position;

        public bool IsLocked => locked || NodeCollection != null && NodeCollection.IsLocked;

        public List<Trigger> NearbyTriggers {
            get => nearbyTriggers;

            set => nearbyTriggers = value;
        }

        public void Awake(){
            if (IsLocked) return;
            color.a = alpha;
            GetComponent<Renderer>().sharedMaterial = new Material(GetComponent<Renderer>().sharedMaterial)
                {color = color};
        }

        public void Update(){
            if (!IsLocked){
                GenerateNameFromPosition();
            }
        }

        public GameObject AddConnectedNode(bool oneWay, Camera camera){
            if (IsLocked || NodeCollection == null || Graph == null || Graph.nodePrefab == null){
                return null;
            }
#if UNITY_EDITOR
            var connectedNodeObject = PrefabUtility.InstantiatePrefab(Graph.nodePrefab) as GameObject;
#else
        GameObject connectedNodeObject = Instantiate(Graph.nodePrefab) as GameObject;
#endif

            if (ReferenceEquals(connectedNodeObject, null)) return null;
            var connectedNode = connectedNodeObject.GetComponent<Node>();

            if (!ReferenceEquals(camera, null)){
                connectedNode.CastToCollider(camera.transform.position, camera.transform.forward, 5f, 20f);
            }

            connectedNode.GenerateNameFromPosition();
            NodeCollection.ApplyParametersToNode(connectedNode);
            connectedNodeObject.transform.parent = transform.parent;

            if (!oneWay){
                connectedNode.AddConnection(this);
            }

            AddConnection(connectedNode);

            return connectedNodeObject;
        }

        public void AddConnection(Node toNode){
            if (IsLocked || toNode == this || ReferenceEquals(Graph, null) || toNode.Graph != Graph ||
                ReferenceEquals(Graph.edgeCollection, null)) return;
            neighbours ??= new List<Node>();
            outEdges ??= new List<Edge>();

            if (neighbours.Contains(toNode)) return;
            var edgeObject = Graph.edgeCollection.AddEdge(this, toNode);

            if (ReferenceEquals(edgeObject, null)) return;
            neighbours.Add(toNode);

            var edge = edgeObject.GetComponent<Edge>();

            if (!outEdges.Contains(edge)){
                outEdges.Add(edge);
            }
        }

        public void CastToCollider(Vector3 fromPosition, Vector3 forward, float minDistance, float maxDistance){
            if (IsLocked) return;
            RaycastHit hit;
            var ray = new Ray(fromPosition, forward);
            var flag = false;

            if (maxDistance > 0f){
                flag = Physics.SphereCast(ray, radius, out hit, maxDistance, raycastObstaclesLayerMask);
            }
            else{
                flag = Physics.SphereCast(ray, radius, out hit, float.MaxValue, raycastObstaclesLayerMask);
            }

            if (flag){
                transform.position = hit.point + Vector3.up * surfaceOffset;
            }
            else if (minDistance > 0f){
                transform.position = fromPosition + forward.normalized * minDistance + Vector3.up * surfaceOffset;
            }
        }

        public static void ConnectNodes(Node[] nodes){
            foreach (var nodeFrom in nodes)
            foreach (var nodeTo in nodes){
                if (!nodeFrom.IsLocked && nodeFrom != nodeTo){
                    nodeFrom.AddConnection(nodeTo);
                }
            }
        }

        public static void CycleConnection(Node fromNode, Node toNode){
            if (fromNode.IsLocked || toNode.IsLocked) return;
            var oneConnectedToTwo = fromNode.IsConnectedTo(toNode);
            var twoConnectedToOne = toNode.IsConnectedTo(fromNode);

            switch (oneConnectedToTwo){
                case true when twoConnectedToOne:
                    toNode.RemoveConnection(fromNode);
                    break;
                case true:
                    toNode.AddConnection(fromNode);
                    fromNode.RemoveConnection(toNode);
                    break;
                default:{
                    if (twoConnectedToOne){
                        toNode.RemoveConnection(fromNode);
                    }
                    else{
                        fromNode.AddConnection(toNode);
                        toNode.AddConnection(fromNode);
                    }

                    break;
                }
            }
        }

        public static void DisconnectNodes(Node[] nodes){
            foreach (var nodeFrom in nodes)
            foreach (var nodeTo in nodes){
                if (!nodeFrom.IsLocked && nodeFrom != nodeTo){
                    nodeFrom.RemoveConnection(nodeTo);
                }
            }
        }

        public void DropToSurface(){
            if (IsLocked) return;
            var forward = new Vector3(0f, -1f, 0f);
            CastToCollider(transform.position, forward, 0f, 0f);
        }

        public void GenerateNameFromPosition(){
            if (IsLocked || nameLocked) return;
            var position = transform.position;
            name =
                "Node (" +
                position.x.ToString("F1") +
                ", " +
                position.y.ToString("F1") +
                ", " +
                position.z.ToString("F1") +
                ")";
        }

        public bool IsConnectedTo(Node toNode){
            return neighbours.Contains(toNode);
        }

        public void RaycastNeighbours(Node[] potentialNeighbours, bool clearConnections, bool requireSymmetry){
            if (!IsLocked){
                if (clearConnections){
                    RemoveAllConnections();
                }

                if (potentialNeighbours == null) return;
                foreach (var neighbour in potentialNeighbours){
                    if (ReferenceEquals(neighbour, null) || neighbour == this) continue;
                    var direction = neighbour.transform.position - transform.position;

                    if (!(direction.magnitude <= raycastMaximumDistance) || ((!useCapsuleCast ||
                                                                              Physics.CapsuleCast(
                                                                                  transform.position - (1 - raycastPathRadius) * Vector3.up,
                                                                                  transform.position + (1 - raycastPathRadius) * Vector3.up,
                                                                                  raycastPathRadius,
                                                                                  direction,
                                                                                  out _,
                                                                                  direction.magnitude,
                                                                                  raycastObstaclesLayerMask)) && (useCapsuleCast || Physics.SphereCast(
                        transform.position,
                        raycastPathRadius,
                        direction,
                        out _,
                        direction.magnitude,
                        raycastObstaclesLayerMask)))) continue;
                    if (!requireSymmetry){
                        AddConnection(neighbour);
                    }
                    else{
                        AddConnection(neighbour);
                        neighbour.AddConnection(this);
                    }
                }
            }
        }

        public void RemoveAllConnections(){
            if (IsLocked) return;
            neighbours.Clear();

            foreach (var edge in outEdges){
                if (!ReferenceEquals(edge, null)){
                    DestroyImmediate(edge.gameObject);
                }
            }

            outEdges.Clear();
        }

        public void RemoveConnection(Node toNode){
            if (IsLocked) return;
            neighbours.Remove(toNode);

            for (var i = 0; i < outEdges.Count; i++){
                if (outEdges[i] == null || outEdges[i].ToNode != toNode) continue;
                DestroyImmediate(outEdges[i].gameObject);
                outEdges.RemoveAt(i);
                break;
            }
        }
    }
}