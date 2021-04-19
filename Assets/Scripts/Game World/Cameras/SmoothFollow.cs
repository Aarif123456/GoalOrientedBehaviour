/*
This camera smooths out rotation around the y-axis and height.
Horizontal Distance to the target is always fixed.

There are many different ways to smooth the rotation but doing it this way gives you a lot of control over how the camera behaves.

For every of those smoothed values we calculate the wanted value and the current value.
Then we smooth it using the Lerp function.
Then we apply the smoothed values to the transform's position.
*/

using UnityEngine;

namespace GameBrains.Cameras
{
    public class SmoothFollow : TargetedCamera
    {
        // The distance in the x-z plane to the target
        public float distance = 10.0f;
        // the height we want the camera to be above the target
        public float height = 5.0f;
        // How much we 
        public float heightDamping = 2.0f;
        public float rotationDamping = 3.0f;
    
        public override void Awake()
        {
            base.Awake();
            
            if (target == null)
            {
                Debug.Log("Provide a target for the camera.");
            }
        }
        
        public void LateUpdate() 
        {
            // Early out if we don't have a target
            if (!target)
                return;
    
            // Calculate the current rotation angles
            var wantedRotationAngle = target.eulerAngles.y;
            var targetPosition = target.position;
            var wantedHeight = targetPosition.y + height;

            var thisTransform = transform;
            var currentRotationAngle = thisTransform.eulerAngles.y;
            var thisTransformPosition = thisTransform.position;
            var currentHeight = thisTransformPosition.y;
    
            // Damp the rotation around the y-axis
            currentRotationAngle = Mathf.LerpAngle (currentRotationAngle, wantedRotationAngle, rotationDamping * Time.deltaTime);

            // Damp the height
            currentHeight = Mathf.Lerp (currentHeight, wantedHeight, heightDamping * Time.deltaTime);

            // Convert the angle into a rotation
            var currentRotation = Quaternion.Euler (0, currentRotationAngle, 0);
    
            // Set the position of the camera on the x-z plane to:
            // distance meters behind the target
            thisTransformPosition = targetPosition;
            thisTransformPosition -= currentRotation * Vector3.forward * distance;

            // Set the height of the camera
            thisTransformPosition.y = currentHeight;
            transform.position = thisTransformPosition;

            // Always look at the target
            transform.LookAt (target);
        }
    }
}