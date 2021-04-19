using UnityEngine;

public abstract class Trigger : Entity
{
    private bool isActive;
    
    public bool IsActive
    {
        get
        {
            return isActive;
        }

        protected set
        {
            isActive = value; 
            GetComponent<Renderer>().enabled = value;
        }
    }
    
    public Agent TriggeringAgent { get; protected set; }
    
    public override void Awake()
    {
        base.Awake();
        IsActive = true;
    }
    
    public override void Update()
    {
        base.Update();
    } 
}