using UnityEngine;

namespace GameWorld.Navigation.Graph {
    [ExecuteInEditMode]
    public class Edge : MonoBehaviour {
        public bool locked;
        public bool nameLocked;
        public Color color = Color.magenta;
        public float alpha = 0.25f;
        public Node fromNode;
        public Node toNode;
        public float cost;

        public Graph Graph => EdgeCollection == null ? null : EdgeCollection.Graph;

        public EdgeCollection EdgeCollection =>
            transform.parent == null ? null : transform.parent.GetComponent<EdgeCollection>();


        public Node FromNode {
            get => fromNode;

            set => fromNode = value;
        }

        public Node ToNode {
            get => toNode;

            set => toNode = value;
        }


        public float Cost {
            get => cost;

            set => cost = value;
        }

        public bool IsLocked => locked || EdgeCollection != null && EdgeCollection.IsLocked;

        public void Awake(){
            if (IsLocked) return;
            color.a = alpha;
            GetComponent<Renderer>().sharedMaterial = new Material(GetComponent<Renderer>().sharedMaterial)
                {color = color};
        }

        public void Update(){
            CalculateCost();
            UpdatePosition();
            UpdateRotation();
            UpdateScale();
            GenerateNameFromNodes();
        }

        public void CalculateCost(){
            if (IsLocked) return;
            if (ReferenceEquals(FromNode, null) || ReferenceEquals(ToNode, null))
                Cost = float.MaxValue;
            else
                Cost = Vector3.Distance(FromNode.Position, ToNode.Position);
        }

        public void GenerateNameFromNodes(){
            if (!IsLocked && !nameLocked){
                name =
                    "Edge (" +
                    (ReferenceEquals(FromNode, null) ? "NONE" : FromNode.name) +
                    " --[" + Cost.ToString("F1") + "]--> " +
                    (ReferenceEquals(ToNode, null) ? "NONE" : ToNode.name) +
                    ")";
            }
        }

        public void UpdatePosition(){
            if (!IsLocked && !ReferenceEquals(FromNode, null) && !ReferenceEquals(ToNode, null))
                transform.position = (FromNode.Position + ToNode.Position) / 2;
        }

        public void UpdateRotation(){
            if (IsLocked || ReferenceEquals(FromNode, null) || ReferenceEquals(ToNode, null)) return;
            transform.LookAt(ToNode.Position);
            transform.Rotate(Vector3.right, 90);
        }

        public void UpdateScale(){
            if (IsLocked || ReferenceEquals(FromNode, null) || ReferenceEquals(ToNode, null)) return;
            var scale = transform.localScale;
            scale.y = Vector3.Distance(FromNode.Position, ToNode.Position) / 2;
            transform.localScale = scale;
        }
    }
}