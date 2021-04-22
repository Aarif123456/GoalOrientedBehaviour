using System.Collections.Generic;

using GameBrains.AI;

using UnityEngine;

public class MovingEntity : Entity
{
    public float closeEnoughDistance = 1;
    
    protected Motor motor;
    
    protected CharacterController characterController; // optional
    
    public List<SteeringBehaviour> SteeringBehaviours { get; set; }
    
    public float CloseEnoughDistance 
    {
        get 
        {
            return closeEnoughDistance;
        }
        
        set 
        {
            closeEnoughDistance = value;
        }
    }
    
    public override void Awake()
    {
        base.Awake();
        
        motor = GetComponent<Motor>();
        characterController = GetComponent<CharacterController>();
        SteeringBehaviours = new List<SteeringBehaviour>();
        
        if (characterController != null)
        {
            Kinematic.CenterOffset = characterController.center;
            Kinematic.Radius = characterController.radius;
            Kinematic.Height = characterController.height;
        }
    }
    
    public override void Update()
    {
        base.Update();
        
        if (motor != null && motor.enabled)
        {
            motor.UpdateFromGameObject(this, Time.deltaTime);
        }
    }
    
    public void LateUpdate()
    {
        Think(Time.deltaTime);
            
        Act(Time.deltaTime);
        
        if (motor != null && motor.enabled)
        {
            motor.CalculatePhysics(this, Time.deltaTime);
            motor.ApplyPhysicsToGameObject(this, Time.deltaTime);
        }    
    }
    
    protected virtual void Think(float deltaTime)
    {
    }
    
    protected virtual void Act(float deltaTime)
    {
        foreach (SteeringBehaviour steeringBehaviour in SteeringBehaviours)
        {
            Kinematic.AccumulateSteering(steeringBehaviour.Steer());
        }
    }
    
    public bool IsAtPosition(Vector3 position)
    {
        return IsAtPosition(position, CloseEnoughDistance);
    }
    
    public bool IsAtPosition(Vector3 position, float satisfactionRadius)
    {
        return (Kinematic.Position - position).magnitude <= satisfactionRadius;
    }
    
    public bool HasLineOfSight(Vector3 position)
    {
        LayerMask obstacleLayers = 1 << LayerMask.NameToLayer("Walls");
        return !Physics.Raycast(Kinematic.Position, (position - Kinematic.Position).normalized, (position - Kinematic.Position).magnitude, obstacleLayers);
    }
    
    public virtual void Spawn(Vector3 spawnPoint)
    {
        State = States.Alive;
        transform.position = spawnPoint;
        transform.eulerAngles = Vector3.zero;
        Kinematic = new Kinematic { Position = transform.position, Rotation = transform.eulerAngles };
        
        if (characterController != null)
        {
            Kinematic.CenterOffset = characterController.center;
            Kinematic.Radius = characterController.radius;
            Kinematic.Height = characterController.height;
        }
    }
}