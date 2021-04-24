using UnityEditor;
using UnityEngine;

namespace GameWorld.Navigation.Graph {
    public sealed class EdgeCollection : MonoBehaviour {
        public bool locked;
        public Graph graph;
        public Color color = Color.magenta;
        public float alpha = 0.25f;
        private bool isVisible;

        public Graph Graph {
            get => graph;

            set => graph = value;
        }

        public Edge[] Edges => GetComponentsInChildren<Edge>();

        public bool IsLocked => locked || Graph is{IsLocked: true};

        public bool IsVisible {
            get => isVisible;

            set {
                isVisible = value;
                foreach (var edge in Edges){
                    edge.GetComponent<Renderer>().enabled = value;
                }
            }
        }

        public void Awake(){
            ApplyParametersToEdges();
        }

        public GameObject AddEdge(Node fromNode, Node toNode){
            if (IsLocked || ReferenceEquals(fromNode, null) || fromNode.Graph != toNode.Graph ||
                ReferenceEquals(fromNode.Graph.edgePrefab, null))
                return null;

#if UNITY_EDITOR
            var edgeObject = PrefabUtility.InstantiatePrefab(fromNode.Graph.edgePrefab) as GameObject;
#else
        GameObject edgeObject = Instantiate(fromNode.Graph.edgePrefab) as GameObject;
#endif

            if (ReferenceEquals(edgeObject, null)) return edgeObject;
            var edge = edgeObject.GetComponent<Edge>();

            if (!ReferenceEquals(edge, null)){
                edge.FromNode = fromNode;
                edge.ToNode = toNode;
                edge.CalculateCost();
                edge.GenerateNameFromNodes();
                ApplyParametersToEdge(edge);
                edgeObject.transform.parent = transform;
            }
            else{
                Destroy(edgeObject);
                edgeObject = null;
            }

            return edgeObject;
        }

        public void ApplyParametersToEdges(){
            if (IsLocked) return;
            foreach (var edge in Edges){
                ApplyParametersToEdge(edge);
            }
        }

        private void ApplyParametersToEdge(Edge edge){
            if (IsLocked || edge.locked) return;
            color.a = alpha;
            edge.color = color;
            edge.GetComponent<Renderer>().sharedMaterial.color = color;
        }
    }
}