using UnityEngine;

namespace GameWorld.Cameras {
    public class SelectableCamera : MonoBehaviour {
        private Camera _curCamera;
        public string CameraName { get; protected set; }

        public virtual void Awake(){
            _curCamera = GetComponent<Camera>();
        }

        public void Deactivate(){
            enabled = false;
            _curCamera.enabled = false;
        }

        public void Activate(){
            enabled = true;
            _curCamera.enabled = true;
        }

        public bool IsActive(){
            return enabled;
        }
    }
}