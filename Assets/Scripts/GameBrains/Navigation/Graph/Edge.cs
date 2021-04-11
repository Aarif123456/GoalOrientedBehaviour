using UnityEngine;

[ExecuteInEditMode]
public class Edge : MonoBehaviour
{
    public bool locked;
    public bool nameLocked;
    public Color color = Color.magenta;
    public float alpha = 0.25f;
    public Node fromNode;
    public Node toNode;
    public float cost;
    
    public Graph Graph
    {
        get
        {
            return EdgeCollection == null ? null : EdgeCollection.Graph;
        }
    }
    
    public EdgeCollection EdgeCollection
    {
        get
        {
            return transform.parent == null ? null : transform.parent.GetComponent<EdgeCollection>();
        }
    }
    

    public Node FromNode 
    { 
        get 
        { 
            return fromNode; 
        } 
        
        set 
        { 
            fromNode = value; 
        } 
    }
    
    public Node ToNode 
    { 
        get 
        { 
            return toNode; 
        }
        
        set 
        { 
            toNode = value; 
        } 
    }
    

    public float Cost 
    { 
        get 
        { 
            return cost; 
        } 
        
        set 
        { 
            cost = value; 
        } 
    }
    
    public bool IsLocked
    {
        get
        {
            return locked || (EdgeCollection != null && EdgeCollection.IsLocked);
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
    
    public void CalculateCost()
    {
        if (!IsLocked)
        {
            if (FromNode == null || ToNode == null)
            {
                Cost = float.MaxValue;
            }
            else
            {
                Cost = Vector3.Distance(FromNode.Position, ToNode.Position);
            }
        }
    }
    
    public void GenerateNameFromNodes()
    {
        if (!IsLocked && !nameLocked)
        {
            name = 
                "Edge (" + 
                (FromNode == null ? "NONE" : FromNode.name) +
                " --[" + Cost.ToString("F1") + "]--> " +
                (ToNode == null ? "NONE" : ToNode.name) +
                ")";    
        }
    }
    
    public void Update()
    {
        CalculateCost();
        UpdatePosition();
        UpdateRotation();
        UpdateScale();
        GenerateNameFromNodes();
    }
    
    public void UpdatePosition()
    {
        if (!IsLocked && FromNode != null && ToNode != null)
        {
            transform.position = (FromNode.Position + ToNode.Position) / 2;
        }
    }
    
    public void UpdateRotation()
    {
        if (!IsLocked && FromNode != null && ToNode != null)
        {
            transform.LookAt(ToNode.Position);
            transform.Rotate(Vector3.right, 90);
        }
    }
    
    public void UpdateScale()
    {
        if (!IsLocked && FromNode != null && ToNode != null)
        {
            Vector3 scale = transform.localScale;
            scale.y = Vector3.Distance(FromNode.Position, ToNode.Position) / 2;
            transform.localScale = scale;
        }
    }
}