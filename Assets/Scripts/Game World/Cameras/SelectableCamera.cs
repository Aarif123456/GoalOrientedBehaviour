using UnityEngine;

namespace GameWorld.Cameras {
    public class SelectableCamera : MonoBehaviour {
        private Camera curCamera;
        public string CameraName { get; protected set; }

        public virtual void Awake(){
            curCamera = GetComponent<Camera>();
        }

        public void Deactivate(){
            enabled = false;
            curCamera.enabled = false;
        }

        public void Activate(){
            enabled = true;
            curCamera.enabled = true;
        }

        public bool IsActive(){
            return enabled;
        }
    }
}