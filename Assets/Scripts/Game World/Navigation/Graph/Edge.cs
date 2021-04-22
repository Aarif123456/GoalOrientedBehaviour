using UnityEngine;

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
        if (!IsLocked){
            color.a = alpha;
            GetComponent<Renderer>().sharedMaterial = new Material(GetComponent<Renderer>().sharedMaterial)
                {color = color};
        }
    }

    public void Update(){
        CalculateCost();
        UpdatePosition();
        UpdateRotation();
        UpdateScale();
        GenerateNameFromNodes();
    }

    public void CalculateCost(){
        if (!IsLocked){
            if (FromNode == null || ToNode == null){
                Cost = float.MaxValue;
            }
            else{
                Cost = Vector3.Distance(FromNode.Position, ToNode.Position);
            }
        }
    }

    public void GenerateNameFromNodes(){
        if (!IsLocked && !nameLocked){
            name =
                "Edge (" +
                (FromNode == null ? "NONE" : FromNode.name) +
                " --[" + Cost.ToString("F1") + "]--> " +
                (ToNode == null ? "NONE" : ToNode.name) +
                ")";
        }
    }

    public void UpdatePosition(){
        if (!IsLocked && FromNode != null && ToNode != null){
            transform.position = (FromNode.Position + ToNode.Position) / 2;
        }
    }

    public void UpdateRotation(){
        if (!IsLocked && FromNode != null && ToNode != null){
            transform.LookAt(ToNode.Position);
            transform.Rotate(Vector3.right, 90);
        }
    }

    public void UpdateScale(){
        if (!IsLocked && FromNode != null && ToNode != null){
            var scale = transform.localScale;
            scale.y = Vector3.Distance(FromNode.Position, ToNode.Position) / 2;
            transform.localScale = scale;
        }
    }
}