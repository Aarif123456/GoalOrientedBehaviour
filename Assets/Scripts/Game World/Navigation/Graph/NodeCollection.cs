using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GameWorld.Navigation.Graph {
    public class NodeCollection : MonoBehaviour {
        public bool locked;
        public Color color = Color.magenta;
        public float alpha = 0.25f;
        public float radius = 0.5f;
        public float lookAheadDistance = 2;
        public bool useCapsuleCast;
        public float raycastMaximumDistance = 15;
        public float raycastPathRadius = 0.5f;
        public float surfaceOffset = 1.1f;
        public LayerMask raycastObstaclesLayerMask;
        public Graph graph;
        private readonly Dictionary<Node, Renderer> _nodeRenderer = new Dictionary<Node, Renderer>();
        private bool _isVisible;

        public Graph Graph {
            get => graph;

            set => graph = value;
        }

        public Node[] Nodes => GetComponentsInChildren<Node>();

        public bool IsLocked => locked || Graph is{IsLocked: true};

        /* TODO: Make cache the node component renderer */
        public bool IsVisible {
            get => _isVisible;

            set {
                _isVisible = value;
                foreach (var node in Nodes){
                    if (!_nodeRenderer.ContainsKey(node)) _nodeRenderer.Add(node, node.GetComponent<Renderer>());
                    _nodeRenderer[node].enabled = value;
                }
            }
        }

        public void Awake(){
            raycastObstaclesLayerMask = 1 << LayerMask.NameToLayer("Walls");
            ApplyParametersToNodes();
        }

        public void ApplyParametersToNodes(){
            if (IsLocked) return;

            foreach (var node in Nodes){
                ApplyParametersToNode(node);
            }
        }

        public void ApplyParametersToNode(Node node){
            if (IsLocked || node.IsLocked) return;
            color.a = alpha;
            node.color = color;
            //node.renderer.sharedMaterial.shader = Shader.Find("Transparent/Diffuse");
            node.renderer.sharedMaterial.color = color;
            node.lookAheadDistance = lookAheadDistance;
            node.radius = radius;
            node.useCapsuleCast = useCapsuleCast;
            node.raycastMaximumDistance = raycastMaximumDistance;
            node.raycastPathRadius = raycastPathRadius;
            node.surfaceOffset = surfaceOffset;
            node.raycastObstaclesLayerMask = raycastObstaclesLayerMask;
//            Vector3 position = node.transform.position;
//            position.y += 6;
//            node.transform.position = position;
        }

        public GameObject AddNode(Camera curCamera){
            if (IsLocked || ReferenceEquals(graph, null) || ReferenceEquals(graph.nodePrefab, null)) return null;

#if UNITY_EDITOR
            var nodeObject = PrefabUtility.InstantiatePrefab(graph.nodePrefab) as GameObject;
#else
        GameObject nodeObject = Instantiate(graph.nodePrefab) as GameObject;
#endif

            if (ReferenceEquals(nodeObject, null)) return null;
            var node = nodeObject.GetComponent<Node>();

            if (!ReferenceEquals(node, null)){
                if (!ReferenceEquals(curCamera, null)){
                    var transform1 = curCamera.transform;
                    node.CastToCollider(transform1.position, transform1.forward, 5f, 20f);
                }

                node.GenerateNameFromPosition();
                ApplyParametersToNode(node);
            }

            nodeObject.transform.parent = transform;

            return nodeObject;
        }

        public void DropToSurface(){
            if (IsLocked) return;

            foreach (var node in Nodes){
                if (!node.locked) node.DropToSurface();
            }
        }

        public void GenerateNamesFromPosition(){
            if (IsLocked) return;

            foreach (var node in Nodes){
                if (!node.locked) node.GenerateNameFromPosition();
            }
        }

        public void RaycastNodes(){
            if (IsLocked) return;
            foreach (var fromNode in Nodes){
                if (fromNode.locked) continue;
                fromNode.RemoveAllConnections();
                fromNode.RaycastNeighbours(Nodes, false, true);
            }
        }
    }
}