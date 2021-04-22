using UnityEngine;

namespace Entities.Steering {
    [RequireComponent(typeof(CharacterController))]
    public class CharacterMotor : Motor {
        private CharacterController characterController;

        public void Start(){
            characterController = GetComponent<CharacterController>();
        }

        public override void ApplyPhysicsToGameObject(MovingEntity movingEntity, float deltaTime){
            characterController.SimpleMove(movingEntity.Kinematic.Velocity);
            transform.rotation = Quaternion.Euler(movingEntity.Kinematic.Rotation);
        }

        public override void CalculatePhysics(MovingEntity movingEntity, float deltaTime){
            movingEntity.Kinematic.Update(deltaTime);
        }

        public override void UpdateFromGameObject(MovingEntity movingEntity, float deltaTime){
            movingEntity.Kinematic.Position = transform.position;
            movingEntity.Kinematic.Rotation = transform.rotation.eulerAngles;
            movingEntity.Kinematic.Velocity = characterController.velocity;
        }
    }
}