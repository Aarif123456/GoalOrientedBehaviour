using UnityEngine;

namespace GameWorld.Cameras {
    public class TargetedCamera : SelectableCamera {
        // The camera's target.
        public Transform target;

        public override void Awake(){
        }
    }
}