using UnityEngine;

namespace GameBrains.Cameras
{
    public class OverheadZoomAndPan : SelectableCamera 
    {
        public Camera overheadCamera;
        private string zoomAxis = "Mouse ScrollWheel";
        private string sideMoveAxis = "Horizontal";
        private string forwardMoveAxis = "Vertical";
        public KeyCode homeKey = KeyCode.Home;
        public float moveSpeed = 0.001f;
        private Vector3 homePosition;
    
        private float maximumZoomOut;
    
        public override void Awake()
        {
            base.Awake();
        
            maximumZoomOut = overheadCamera.orthographicSize;
            homePosition = overheadCamera.transform.position;
        }
    
        public void Update() 
        {
            float zoomDirection = Input.GetAxis(zoomAxis);
            float size = overheadCamera.orthographicSize;
        
            if (zoomDirection < 0)
            {
                size /= (1 - 0.01f * zoomDirection);
            }
            else if (zoomDirection > 0)
            {
                size *= (1 + 0.01f * zoomDirection);
            }
        
            overheadCamera.orthographicSize = Mathf.Clamp(size, 1, maximumZoomOut);
        
            float moveX = Input.GetAxis(sideMoveAxis);
            float moveY = Input.GetAxis(forwardMoveAxis);
        
            Vector3 position = overheadCamera.transform.position;
            position.x += moveX * moveSpeed;
            position.z += moveY * moveSpeed;
            overheadCamera.transform.position = position;
        
            if (Input.GetKeyDown(homeKey))
            {
                overheadCamera.transform.position = homePosition;
            }    
        }
    }
}