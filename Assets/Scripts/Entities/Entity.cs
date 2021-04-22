using GameBrains.AI;
using UnityEngine;

public class Entity : MonoBehaviour {
    public enum States {
        Alive,
        Dead,
        Spawning
    }

    private static int _nextId;

    public int id;
    public bool isObstacle;
    public bool isLockedPositionX;
    public bool isLockedPositionY;
    public bool isLockedPositionZ;
    public bool isLockedRotationX;
    public bool isLockedRotationY;
    public bool isLockedRotationZ;
    public bool isAiControlled;

    public EntityTypes entityType = EntityTypes.DefaultEntityType;

    protected Kinematic kinematic;

    public States State { get; set; }

    public bool IsDead => State == States.Dead;

    public bool IsAlive => State == States.Alive;

    public bool IsSpawning => State == States.Spawning;

    public bool IsAiControlled {
        get => isAiControlled;

        set => isAiControlled = value;
    }

    public Collider Collider => GetComponent<Collider>();

    public Kinematic Kinematic {
        get => kinematic;

        set => kinematic = value;
    }

    public bool IsLockedPositionX {
        get => isLockedPositionX;

        set {
            isLockedPositionX = value;

            if (kinematic != null){
                kinematic.IsLockedPositionX = isLockedPositionX;
            }
        }
    }

    public bool IsLockedPositionY {
        get => isLockedPositionY;

        set {
            isLockedPositionY = value;

            if (kinematic != null){
                kinematic.IsLockedPositionY = isLockedPositionY;
            }
        }
    }

    public bool IsLockedPositionZ {
        get => isLockedPositionZ;

        set {
            isLockedPositionZ = value;

            if (kinematic != null){
                kinematic.IsLockedPositionZ = isLockedPositionZ;
            }
        }
    }

    public bool IsLockedRotationX {
        get => isLockedRotationX;

        set {
            isLockedRotationX = value;

            if (kinematic != null){
                kinematic.IsLockedRotationX = isLockedRotationX;
            }
        }
    }

    public bool IsLockedRotationY {
        get => isLockedRotationY;

        set {
            isLockedRotationY = value;

            if (kinematic != null){
                kinematic.IsLockedRotationY = isLockedRotationY;
            }
        }
    }

    public bool IsLockedRotationZ {
        get => isLockedRotationZ;

        set {
            isLockedRotationZ = value;

            if (kinematic != null){
                kinematic.IsLockedRotationZ = isLockedRotationZ;
            }
        }
    }

    public EntityTypes EntityType {
        get => entityType;
        set => entityType = value;
    }

    public virtual void Awake(){
        id = _nextId++;
        EntityManager.Add(this);

        State = States.Alive;

        kinematic =
            new Kinematic{
                IsLockedPositionX = isLockedPositionX,
                IsLockedRotationX = isLockedRotationX,
                IsLockedPositionY = isLockedPositionY,
                IsLockedRotationY = isLockedRotationY,
                IsLockedPositionZ = isLockedPositionZ,
                IsLockedRotationZ = isLockedRotationZ
            };

        Kinematic.Position = transform.position;
        Kinematic.Rotation = transform.rotation.eulerAngles;
    }

    public virtual void Update(){
    }

    public virtual bool HandleEvent<T>(Event<T> eventArguments){
        return false;
    }

    public void SetSpawning(){
        State = States.Spawning;
    }

    public void SetDead(){
        State = States.Dead;
    }

    public void SetAlive(){
        State = States.Alive;
    }
}