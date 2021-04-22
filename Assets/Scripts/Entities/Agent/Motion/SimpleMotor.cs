using UnityEngine;

namespace Entities.Steering {
    [RequireComponent(typeof(Rigidbody))]
    public class SimpleMotor : Motor {
        public override void ApplyPhysicsToGameObject(MovingEntity movingEntity, float deltaTime){
            GetComponent<Rigidbody>().velocity = movingEntity.Kinematic.Velocity;
            transform.rotation = Quaternion.Euler(movingEntity.Kinematic.Rotation);
        }

        public override void CalculatePhysics(MovingEntity movingEntity, float deltaTime){
            movingEntity.Kinematic.Update(deltaTime);
        }

        public override void UpdateFromGameObject(MovingEntity movingEntity, float deltaTime){
            movingEntity.Kinematic.Position = transform.position;
            movingEntity.Kinematic.Rotation = transform.rotation.eulerAngles;
            movingEntity.Kinematic.Velocity = GetComponent<Rigidbody>().velocity;
        }
    }
}