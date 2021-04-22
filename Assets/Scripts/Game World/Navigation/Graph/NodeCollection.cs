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
        private bool isVisible;

        public Graph Graph {
            get => graph;

            set => graph = value;
        }

        public Node[] Nodes => GetComponentsInChildren<Node>();

        public bool IsLocked => locked || Graph != null && Graph.IsLocked;

        public bool IsVisible {
            get => isVisible;

            set {
                isVisible = value;
                foreach (var node in Nodes){
                    node.GetComponent<Renderer>().enabled = value;
                }
            }
        }

        public void Awake(){
            raycastObstaclesLayerMask = 1 << LayerMask.NameToLayer("Walls");
            ApplyParametersToNodes();
        }

        public void ApplyParametersToNodes(){
            if (!IsLocked){
                foreach (var node in Nodes){
                    ApplyParametersToNode(node);
                }
            }
        }

        public void ApplyParametersToNode(Node node){
            if (!IsLocked && !node.IsLocked){
                color.a = alpha;
                node.color = color;
                //node.renderer.sharedMaterial.shader = Shader.Find("Transparent/Diffuse");
                node.GetComponent<Renderer>().sharedMaterial.color = color;
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
        }

        public GameObject AddNode(Camera camera){
            if (IsLocked || graph == null || graph.nodePrefab == null){
                return null;
            }

#if UNITY_EDITOR
            var nodeObject = PrefabUtility.InstantiatePrefab(graph.nodePrefab) as GameObject;
#else
        GameObject nodeObject = Instantiate(graph.nodePrefab) as GameObject;
#endif

            if (nodeObject != null){
                var node = nodeObject.GetComponent<Node>();

                if (node != null){
                    if (camera != null){
                        node.CastToCollider(camera.transform.position, camera.transform.forward, 5f, 20f);
                    }

                    node.GenerateNameFromPosition();
                    ApplyParametersToNode(node);
                }

                nodeObject.transform.parent = transform;
            }

            return nodeObject;
        }

        public void DropToSurface(){
            if (!IsLocked){
                foreach (var node in Nodes){
                    if (!node.locked){
                        node.DropToSurface();
                    }
                }
            }
        }

        public void GenerateNamesFromPosition(){
            if (!IsLocked){
                foreach (var node in Nodes){
                    if (!node.locked){
                        node.GenerateNameFromPosition();
                    }
                }
            }
        }

        public void RaycastNodes(){
            if (!IsLocked){
                foreach (var fromNode in Nodes){
                    if (!fromNode.locked){
                        fromNode.RemoveAllConnections();
                        fromNode.RaycastNeighbours(Nodes, false, true);
                    }
                }
            }
        }
    }
}