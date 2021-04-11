namespace GameBrains.AI
{
    using UnityEngine;

    public static partial class Math
    {
        public static Vector3 LimitMagnitude(Vector3 vector, float maximumMagnitude)
        {
            float magnitude = vector.magnitude;
            
            if (magnitude > maximumMagnitude)
            {
                // vector.normalized * maximumMagnitude
                vector *= maximumMagnitude / magnitude;
            }
            
            return vector;
        }
        
        public static Vector3 GetAngles(Vector3 v)
        {
            return new Vector3(
                               (float) -System.Math.Atan2(v.y, v.z) * Mathf.Rad2Deg, 
                               (float) -System.Math.Atan2(-v.x, v.z) * Mathf.Rad2Deg, 
                               (float) System.Math.Atan2(v.y, v.x) * Mathf.Rad2Deg);
        }
        
        public static Vector3 GetLookAtVector(Kinematic agentKinematic, Kinematic targetKinematic)
        {
            Vector3 direction = targetKinematic.Position - agentKinematic.Position;
            
            Vector3 angles = direction;
            angles.y = 0f;
            angles.z = angles.magnitude;
            angles.y = direction.y;
            angles.x = 0f;
            angles = GetAngles(angles);
            
            Vector3 lookAtVector = GetAngles(direction);
            lookAtVector.x = angles.x;
            lookAtVector.z = agentKinematic.Rotation.z;

            if (lookAtVector.x > 90f)
            {
                lookAtVector.x = 180f - lookAtVector.x;
            }

            if (lookAtVector.x < -90f)
            {
                lookAtVector.x = -180f - lookAtVector.x;
            }

            return lookAtVector;
        }
        
//        public static Vector3 NegateVector(Vector3 v)
//        {
//            return new Vector3(-v.x, -v.y, -v.z);
//        }
    }
}