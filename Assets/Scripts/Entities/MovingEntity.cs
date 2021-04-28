using System.Collections.Generic;
using Entities.Steering;
using UnityEngine;

namespace Entities {
    public class MovingEntity : Entity {
        public float closeEnoughDistance = 1;

        private CharacterController _characterController; // optional

        protected Motor motor;

        public List<SteeringBehaviour> SteeringBehaviours { get; private set; }

        public float CloseEnoughDistance {
            get => closeEnoughDistance;

            set => closeEnoughDistance = value;
        }

        public override void Awake(){
            base.Awake();

            motor = GetComponent<Motor>();
            _characterController = GetComponent<CharacterController>();
            SteeringBehaviours = new List<SteeringBehaviour>();

            if (_characterController == null) return;
            Kinematic.CenterOffset = _characterController.center;
            Kinematic.Radius = _characterController.radius;
            Kinematic.Height = _characterController.height;
        }

        public override void Update(){
            if (IsDead || IsSpawning) return;
            base.Update();

            if (motor is{enabled: true}) motor.UpdateFromGameObject(this, Time.deltaTime);
        }

        public void LateUpdate(){
            if (IsDead || IsSpawning) return;
            Think(Time.deltaTime);

            Act(Time.deltaTime);

            if (!(motor is{enabled: true})) return;
            motor.CalculatePhysics(this, Time.deltaTime);
            motor.ApplyPhysicsToGameObject(this, Time.deltaTime);
        }

        protected virtual void Think(float deltaTime){
        }

        protected virtual void Act(float deltaTime){
            foreach (var steeringBehaviour in SteeringBehaviours){
                Kinematic.AccumulateSteering(steeringBehaviour.Steer());
            }
        }

        public bool IsAtPosition(Vector3 position){
            return IsAtPosition(position, CloseEnoughDistance);
        }

        public bool IsAtPosition(Vector3 position, float satisfactionRadius){
            return (Kinematic.Position - position).magnitude <= satisfactionRadius;
        }

        public bool HasLineOfSight(Vector3 position){
            LayerMask obstacleLayers = 1 << LayerMask.NameToLayer("Walls");
            return !Physics.Raycast(Kinematic.Position, (position - Kinematic.Position).normalized,
                (position - Kinematic.Position).magnitude, obstacleLayers);
        }

        public virtual void Spawn(Vector3 spawnPoint){
            State = States.Alive;
            var transform1 = transform;
            if (_characterController != null){
                _characterController.enabled = false;
                transform1.position = spawnPoint;
                transform1.eulerAngles = Vector3.zero;
                // Kinematic = new Kinematic{Position = transform.position, Rotation = transform.eulerAngles};
                _characterController.enabled = true;
                // Kinematic.CenterOffset = characterController.center;
                // Kinematic.Radius = characterController.radius;
                // Kinematic.Height = characterController.height;
            }
            else{
                transform1.position = spawnPoint;
                transform1.eulerAngles = Vector3.zero;
            }
        }
    }
}