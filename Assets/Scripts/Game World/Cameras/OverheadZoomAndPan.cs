using UnityEngine;

namespace GameWorld.Cameras {
    public class OverheadZoomAndPan : SelectableCamera {
        private const string _FORWARD_MOVE_AXIS = "Vertical";
        private const string _SIDE_MOVE_AXIS = "Horizontal";
        private const string _ZOOM_AXIS = "Mouse ScrollWheel";
        public Camera overheadCamera;
        public KeyCode homeKey = KeyCode.Home;
        public float moveSpeed = 0.001f;
        private Vector3 _homePosition;

        private float _maximumZoomOut;

        public override void Awake(){
            base.Awake();

            _maximumZoomOut = overheadCamera.orthographicSize;
            _homePosition = overheadCamera.transform.position;
            CameraName = "Overhead Zoom And Pan";
        }

        public void Update(){
            var zoomDirection = Input.GetAxis(_ZOOM_AXIS);
            var size = overheadCamera.orthographicSize;

            if (zoomDirection < 0)
                size /= 1 - 0.01f * zoomDirection;
            else if (zoomDirection > 0) size *= 1 + 0.01f * zoomDirection;

            overheadCamera.orthographicSize = Mathf.Clamp(size, 1, _maximumZoomOut);

            var moveX = Input.GetAxis(_SIDE_MOVE_AXIS);
            var moveY = Input.GetAxis(_FORWARD_MOVE_AXIS);

            var transform1 = overheadCamera.transform;
            var position = transform1.position;
            position.x += moveX * moveSpeed;
            position.z += moveY * moveSpeed;
            transform1.position = position;

            if (Input.GetKeyDown(homeKey)) overheadCamera.transform.position = _homePosition;
        }
    }
}