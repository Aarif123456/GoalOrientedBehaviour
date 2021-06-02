using Entities.Steering;
using GameWorld;
using UnityEngine;

namespace Entities {
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

        protected States State { get; set; }

        public bool IsDead => State == States.Dead;

        public bool IsAlive => State == States.Alive;

        public bool IsSpawning => State == States.Spawning;

        public bool IsAiControlled {
            get => isAiControlled;

            set => isAiControlled = value;
        }
        private Collider _collider;
        public Collider Collider => _collider;

        public Kinematic Kinematic { get; private set; }

        public bool IsLockedPositionX {
            get => isLockedPositionX;

            set {
                isLockedPositionX = value;

                if (Kinematic != null) Kinematic.IsLockedPositionX = isLockedPositionX;
            }
        }

        public bool IsLockedPositionY {
            get => isLockedPositionY;

            set {
                isLockedPositionY = value;

                if (Kinematic != null) Kinematic.IsLockedPositionY = isLockedPositionY;
            }
        }

        public bool IsLockedPositionZ {
            get => isLockedPositionZ;

            set {
                isLockedPositionZ = value;

                if (Kinematic != null) Kinematic.IsLockedPositionZ = isLockedPositionZ;
            }
        }

        public bool IsLockedRotationX {
            get => isLockedRotationX;

            set {
                isLockedRotationX = value;

                if (Kinematic != null) Kinematic.IsLockedRotationX = isLockedRotationX;
            }
        }

        public bool IsLockedRotationY {
            get => isLockedRotationY;

            set {
                isLockedRotationY = value;

                if (Kinematic != null) Kinematic.IsLockedRotationY = isLockedRotationY;
            }
        }

        public bool IsLockedRotationZ {
            get => isLockedRotationZ;

            set {
                isLockedRotationZ = value;

                if (Kinematic != null) Kinematic.IsLockedRotationZ = isLockedRotationZ;
            }
        }

        public EntityTypes EntityType {
            get => entityType;
            set => entityType = value;
        }

        public virtual void Awake(){
            id = _nextId++;
            EntityManager.Add(this);
            _collider = GetComponent<Collider>();
            State = States.Alive;

            Kinematic =
                new Kinematic{
                    IsLockedPositionX = isLockedPositionX,
                    IsLockedRotationX = isLockedRotationX,
                    IsLockedPositionY = isLockedPositionY,
                    IsLockedRotationY = isLockedRotationY,
                    IsLockedPositionZ = isLockedPositionZ,
                    IsLockedRotationZ = isLockedRotationZ
                };

            var transform1 = transform;
            Kinematic.Position = transform1.position;
            Kinematic.Rotation = transform1.rotation.eulerAngles;
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
}