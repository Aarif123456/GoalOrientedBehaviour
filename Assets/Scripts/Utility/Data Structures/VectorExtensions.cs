using UnityEngine;

namespace GameBrains.AI {
    public static class VectorExtensions {
        public static float ToAngle(this Vector3 vector){
            return (Mathf.Atan2(-vector.z, vector.x) - Mathf.PI / 2) * Mathf.Rad2Deg;
        }

        public static Vector3 VectorFromAngle(float angle){
            return -new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad), 0, Mathf.Cos(angle * Mathf.Deg2Rad));
        }

        public static void RotateAroundPivot(Vector3 pivot, Quaternion rotation, ref Vector3 point){
            point = rotation * (point - pivot) + pivot;
        }
    }
}