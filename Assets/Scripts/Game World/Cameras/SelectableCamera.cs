using UnityEngine;

namespace GameBrains.Cameras
{
    public class SelectableCamera : MonoBehaviour
    {
        public string CameraName { get; protected set; }

        public void Deactivate(){
            this.enabled = false;
            GetComponent<Camera>().enabled = false; 
        }

        public void Activate(){
            this.enabled = true;
            GetComponent<Camera>().enabled = true; 
        }

        public bool IsActive(){
            return this.enabled;
        }
        public virtual void Awake()
        {
        }
    }
}
