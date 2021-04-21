using UnityEngine;

namespace GameBrains.Cameras
{
    public class TargetedCamera : SelectableCamera
    {
        // The camera's target.
        public Transform target;
        
        public override void Awake()
        {
        }
    }
}
