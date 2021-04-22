using System.Collections.Generic;
using Entities.Steering;
using UnityEngine;

namespace Entities {
    public class MovingEntity : Entity {
        public float closeEnoughDistance = 1;

        protected CharacterController characterController; // optional

        protected Motor motor;

        public List<SteeringBehaviour> SteeringBehaviours { get; set; }

        public float CloseEnoughDistance {
            get => closeEnoughDistance;

            set => closeEnoughDistance = value;
        }

        public override void Awake(){
            base.Awake();

            motor = GetComponent<Motor>();
            characterController = GetComponent<CharacterController>();
            SteeringBehaviours = new List<SteeringBehaviour>();

            if (characterController == null) return;
            Kinematic.CenterOffset = characterController.center;
            Kinematic.Radius = characterController.radius;
            Kinematic.Height = characterController.height;
        }

        public override void Update(){
            base.Update();

            if (motor is{enabled: true}) motor.UpdateFromGameObject(this, Time.deltaTime);
        }

        public void LateUpdate(){
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
            transform.position = spawnPoint;
            transform.eulerAngles = Vector3.zero;
            Kinematic = new Kinematic{Position = transform.position, Rotation = transform.eulerAngles};

            if (characterController == null) return;
            Kinematic.CenterOffset = characterController.center;
            Kinematic.Radius = characterController.radius;
            Kinematic.Height = characterController.height;
        }
    }
}