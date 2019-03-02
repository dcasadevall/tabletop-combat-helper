using UnityEngine;

namespace Prototype {
    [RequireComponent(typeof(Camera))]
    public class PrototypeCameraController : MonoBehaviour {
        private Camera cam;
        [SerializeField]
        private RegionHandler regionHandler;
        [SerializeField]
        private bool lerpToTargetPosition;

        [SerializeField]
        private float smoothTime;
        private Vector3 positionToMoveTo;

        void Start() {
            if (regionHandler == null) {
                throw new System.Exception("RegionCamera needs a type of RegionHandler. Either create or assign a region handler object in the scene.");
            }

            cam = GetComponent<Camera>();
        }

        private void Update() {
            float speed = 5.0f;
            if(Input.GetKey(KeyCode.RightArrow)) {
                transform.Translate(new Vector3(speed * Time.deltaTime,0,0));
            }
            if(Input.GetKey(KeyCode.LeftArrow))
            {
                transform.Translate(new Vector3(-speed * Time.deltaTime,0,0));
            }
            if(Input.GetKey(KeyCode.DownArrow))
            {
                transform.Translate(new Vector3(0,-speed * Time.deltaTime,0));
            }
            if(Input.GetKey(KeyCode.UpArrow))
            {
                transform.Translate(new Vector3(0,speed * Time.deltaTime,0));
            }

            positionToMoveTo = transform.position;
            float cameraAspectSize = cam.orthographicSize * cam.aspect;

            Vector3 newCameraPosition = Vector3.zero;

            if (regionHandler.Regions.Count > 0) {

                Region region = regionHandler.ActiveRegion;

                // The following block holds the logic for moving the camera on the horizontal axis. If the current regions width is small than that of the width of the
                // camera, the camera stays fixed at the center of the region. Otherwise it'll move towards the target on the x-axis.
                if (region.Width < cameraAspectSize * 2) {
                    newCameraPosition.x = region.p0.x + region.Width / 2;
                }
                else {
                    newCameraPosition.x = Mathf.Clamp(positionToMoveTo.x, region.p0.x + cameraAspectSize, region.p1.x - cameraAspectSize);
                }

                // The same logic but this time for the vertical axis. If the active region is smaller in height than the height of the camera, the camera stays fixed in the
                // center of the region.
                if (region.Height < cam.orthographicSize * 2) {
                    newCameraPosition.y = region.p1.y + region.Height / 2;
                }
                else {
                    newCameraPosition.y = Mathf.Clamp(positionToMoveTo.y, region.p1.y + cam.orthographicSize, region.p0.y - cam.orthographicSize);
                }

                if (!region.Contains(positionToMoveTo)) {
                    regionHandler.SetActiveRegion(positionToMoveTo);
                }
            }
            else {
                newCameraPosition = positionToMoveTo;
            }

            // Restrict the camera to only move on the x- and y-axis. 
            newCameraPosition.z = transform.position.z;

            // Move towards the new target position that has been defined above. If lerpToTargetPosition is enabled, the camera lerps towards the target.
            if (lerpToTargetPosition) {
                transform.position = Vector3.Lerp(transform.position, newCameraPosition, smoothTime);
            } else {
                transform.position = newCameraPosition;
            }
        }

        private void LateUpdate() {
            float sizeMin = 2f;
            float sizeMax = 35f;
            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize - Input.mouseScrollDelta.y, sizeMin, sizeMax);
        }

        // Call this function to switch to a completely different region handler
        public void SetRegionHandler(RegionHandler regionHandler) {
            this.regionHandler = regionHandler;
        }
    }
}