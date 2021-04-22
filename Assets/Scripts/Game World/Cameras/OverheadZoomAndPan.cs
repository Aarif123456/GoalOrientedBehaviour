using UnityEngine;

namespace GameWorld.Cameras {
    public class OverheadZoomAndPan : SelectableCamera {
        private const string ForwardMoveAxis = "Vertical";
        private const string SideMoveAxis = "Horizontal";
        private const string ZoomAxis = "Mouse ScrollWheel";
        public Camera overheadCamera;
        public KeyCode homeKey = KeyCode.Home;
        public float moveSpeed = 0.001f;
        private Vector3 homePosition;

        private float maximumZoomOut;

        public override void Awake(){
            base.Awake();

            maximumZoomOut = overheadCamera.orthographicSize;
            homePosition = overheadCamera.transform.position;
            CameraName = "Overhead Zoom And Pan";
        }

        public void Update(){
            var zoomDirection = Input.GetAxis(ZoomAxis);
            var size = overheadCamera.orthographicSize;

            if (zoomDirection < 0)
                size /= 1 - 0.01f * zoomDirection;
            else if (zoomDirection > 0) size *= 1 + 0.01f * zoomDirection;

            overheadCamera.orthographicSize = Mathf.Clamp(size, 1, maximumZoomOut);

            var moveX = Input.GetAxis(SideMoveAxis);
            var moveY = Input.GetAxis(ForwardMoveAxis);

            var position = overheadCamera.transform.position;
            position.x += moveX * moveSpeed;
            position.z += moveY * moveSpeed;
            overheadCamera.transform.position = position;

            if (Input.GetKeyDown(homeKey)) overheadCamera.transform.position = homePosition;
        }
    }
}