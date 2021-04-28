using UnityEngine;

namespace Entities.Steering {
    [RequireComponent(typeof(CharacterController))]
    public class CharacterMotor : Motor {
        private CharacterController _characterController;

        public void Start(){
            _characterController = GetComponent<CharacterController>();
        }

        public override void ApplyPhysicsToGameObject(MovingEntity movingEntity, float deltaTime){
            _characterController.SimpleMove(movingEntity.Kinematic.Velocity);
            transform.rotation = Quaternion.Euler(movingEntity.Kinematic.Rotation);
        }

        public override void CalculatePhysics(MovingEntity movingEntity, float deltaTime){
            movingEntity.Kinematic.Update(deltaTime);
        }

        public override void UpdateFromGameObject(MovingEntity movingEntity, float deltaTime){
            var transform1 = transform;
            movingEntity.Kinematic.Position = transform1.position;
            movingEntity.Kinematic.Rotation = transform1.rotation.eulerAngles;
            movingEntity.Kinematic.Velocity = _characterController.velocity;
        }
    }
}