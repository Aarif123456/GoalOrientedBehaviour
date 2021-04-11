using UnityEngine;

namespace GameBrains.Cameras
{
    public class TargetedCamera : SelectableCamera
    {
        //TODO: All targeted cameras should derive from this
        //TODO: Add code to find all targeted cameras and set the current camera.
        //TODO: Need to disable inactive camera scripts and camera gameObjects
        //TODO: Some of this should be done in SelectableCamera

        // The camera's target.
        public Transform target;
        
        public override void Awake()
        {
        }
    }
}
