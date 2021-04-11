using GameBrains.AI;

using UnityEngine;
    
public class Entity : MonoBehaviour
{    
    private static int _nextId = 0;

    public int id;
    public bool isObstacle;
    public bool isLockedPositionX;    
    public bool isLockedPositionY;
    public bool isLockedPositionZ;
    public bool isLockedRotationX;
    public bool isLockedRotationY;
    public bool isLockedRotationZ;
    public bool isAiControlled;
    
    protected Kinematic kinematic;    

    public enum States
    {
        Alive,
        Dead,
        Spawning
    }
    
    public States State { get; set; }
    
    public bool IsDead
    {
        get { return State == States.Dead; }
    }

    public bool IsAlive
    {
        get { return State == States.Alive; }
    }

    public bool IsSpawning
    {
        get { return State == States.Spawning; }
    }
    
    public bool IsAiControlled 
    { 
        get 
        { 
            return isAiControlled; 
        } 
        
        set 
        { 
            isAiControlled = value; 
        } 
    }
    
    public Collider Collider
    {
        get
        {
            return GetComponent<Collider>();
        }
    }

    public Kinematic Kinematic
    {
        get
        {
            return kinematic;
        }
        
        set
        {
            kinematic = value;
        }
    }
    
    public bool IsLockedPositionX 
    { 
        get 
        { 
            return isLockedPositionX; 
        } 
        
        set 
        { 
            isLockedPositionX = value; 
            
            if (kinematic != null)
            {
                kinematic.IsLockedPositionX = isLockedPositionX;
            }
        } 
    }
    
    public bool IsLockedPositionY 
    { 
        get 
        { 
            return isLockedPositionY; 
        } 
        
        set 
        { 
            isLockedPositionY = value;  
            
            if (kinematic != null)
            {
                kinematic.IsLockedPositionY = isLockedPositionY;
            }
        } 
    }
    
    public bool IsLockedPositionZ 
    { 
        get 
        { 
            return isLockedPositionZ; 
        } 
        
        set 
        { 
            isLockedPositionZ = value;  
            
            if (kinematic != null)
            {
                kinematic.IsLockedPositionZ = isLockedPositionZ;
            }
        } 
    }
    
    public bool IsLockedRotationX 
    { 
        get 
        { 
            return isLockedRotationX; 
        } 
        
        set 
        { 
            isLockedRotationX = value;  
            
            if (kinematic != null)
            {
                kinematic.IsLockedRotationX = isLockedRotationX;
            }
        } 
    }
    
    public bool IsLockedRotationY 
    { 
        get 
        { 
            return isLockedRotationY; 
        } 
        
        set 
        { 
            isLockedRotationY = value;  
            
            if (kinematic != null)
            {
                kinematic.IsLockedRotationY = isLockedRotationY;
            }
        } 
    }
    
    public bool IsLockedRotationZ 
    { 
        get 
        { 
            return isLockedRotationZ; 
        } 
        
        set 
        { 
            isLockedRotationZ = value;  
            
            if (kinematic != null)
            {
                kinematic.IsLockedRotationZ = isLockedRotationZ;
            }
        } 
    }
    
    public EntityTypes entityType = EntityTypes.DefaultEntityType;
    public EntityTypes EntityType { get { return entityType; } set { entityType = value; } }
    
    public virtual void Awake()
    {
        id = _nextId++;
        EntityManager.Add(this);
        
        State = States.Alive;
        
        kinematic = 
            new Kinematic
            {
                IsLockedPositionX = isLockedPositionX,
                IsLockedRotationX = isLockedRotationX,
                IsLockedPositionY = isLockedPositionY,
                IsLockedRotationY = isLockedRotationY,
                IsLockedPositionZ = isLockedPositionZ,
                IsLockedRotationZ = isLockedRotationZ,
            };
        
        Kinematic.Position = transform.position;
        Kinematic.Rotation = transform.rotation.eulerAngles;
    }
    
    public virtual void Update()
    {
    }
    
    public virtual bool HandleEvent<T>(Event<T> eventArguments)
    {
        return false;
    }
    
    public void SetSpawning()
    {
        State = States.Spawning;
    }

    public void SetDead()
    {
        State = States.Dead;
    }

    public void SetAlive()
    {
        State = States.Alive;
    }
}