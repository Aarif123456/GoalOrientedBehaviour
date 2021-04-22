using UnityEngine;

namespace GameBrains.Cameras {
    public class SelectableCamera : MonoBehaviour {
        public string CameraName { get; protected set; }

        public virtual void Awake(){
        }

        public void Deactivate(){
            enabled = false;
            GetComponent<Camera>().enabled = false;
        }

        public void Activate(){
            enabled = true;
            GetComponent<Camera>().enabled = true;
        }

        public bool IsActive(){
            return enabled;
        }
    }
}