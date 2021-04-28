using UnityEngine;

namespace Entities.Steering {
    public abstract class Motor : MonoBehaviour {
        protected new Rigidbody rigidbody;

        protected void Awake(){
            rigidbody = GetComponent<Rigidbody>();
        }

        public abstract void ApplyPhysicsToGameObject(MovingEntity movingEntity, float deltaTime);
        public abstract void CalculatePhysics(MovingEntity movingEntity, float deltaTime);
        public abstract void UpdateFromGameObject(MovingEntity movingEntity, float deltaTime);
    }
}