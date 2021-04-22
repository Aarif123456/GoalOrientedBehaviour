using UnityEngine;

namespace GameBrains.AI {
    public static partial class Math {
        public static float WrapAngle(float angle){
            while (angle <= -180f){
                angle += 360f;
            }

            while (angle >= 180f){
                angle += -360f;
            }

            return angle;
        }

        public static Vector3 WrapAngles(Vector3 v){
            return new Vector3(WrapAngle(v.x), WrapAngle(v.y), WrapAngle(v.z));
        }
    }
}