using UnityEngine;

namespace GameBrains.Cameras {
    public class OverheadZoomAndPan : SelectableCamera {
        public Camera overheadCamera;
        public KeyCode homeKey = KeyCode.Home;
        public float moveSpeed = 0.001f;
        private readonly string forwardMoveAxis = "Vertical";
        private readonly string sideMoveAxis = "Horizontal";
        private readonly string zoomAxis = "Mouse ScrollWheel";
        private Vector3 homePosition;

        private float maximumZoomOut;

        public override void Awake(){
            base.Awake();

            maximumZoomOut = overheadCamera.orthographicSize;
            homePosition = overheadCamera.transform.position;
            CameraName = "Overhead Zoom And Pan";
        }

        public void Update(){
            var zoomDirection = Input.GetAxis(zoomAxis);
            var size = overheadCamera.orthographicSize;

            if (zoomDirection < 0){
                size /= 1 - 0.01f * zoomDirection;
            }
            else if (zoomDirection > 0){
                size *= 1 + 0.01f * zoomDirection;
            }

            overheadCamera.orthographicSize = Mathf.Clamp(size, 1, maximumZoomOut);

            var moveX = Input.GetAxis(sideMoveAxis);
            var moveY = Input.GetAxis(forwardMoveAxis);

            var position = overheadCamera.transform.position;
            position.x += moveX * moveSpeed;
            position.z += moveY * moveSpeed;
            overheadCamera.transform.position = position;

            if (Input.GetKeyDown(homeKey)){
                overheadCamera.transform.position = homePosition;
            }
        }
    }
}