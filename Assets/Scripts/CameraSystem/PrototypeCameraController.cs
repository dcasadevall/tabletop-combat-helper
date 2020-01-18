using System;
using Drawing;
using Drawing.UI;
using InputSystem;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace CameraSystem {
    /// <summary>
    /// This prototype camera movement script needs to die soon
    /// </summary>
    [RequireComponent(typeof(Camera))]
    public class PrototypeCameraController : MonoBehaviour, ICameraController {
        [SerializeField]
        private RegionHandler regionHandler;
        [SerializeField]
        private bool lerpToTargetPosition;

        [SerializeField]
        private float smoothTime;
        private Vector3 positionToMoveTo;

        private DiContainer _container;
        private Camera _camera;
        private EventSystem _eventSystem;
        private IInputLock _inputLock;

        [Inject]
        public void Construct(Camera camera,
                              EventSystem eventSystem,
                              IInputLock inputLock) {
            _camera = camera;
            _eventSystem = eventSystem;
            _inputLock = inputLock;
        }

        private Vector3? lastPosition;
        private void Update() {
            // Panning with WASD
            float speed = 5.0f;
            if(Input.GetKey(KeyCode.D)) {
                transform.Translate(new Vector3(speed * Time.deltaTime,0,0));
            }
            if(Input.GetKey(KeyCode.A))
            {
                transform.Translate(new Vector3(-speed * Time.deltaTime,0,0));
            }
            if(Input.GetKey(KeyCode.S))
            {
                transform.Translate(new Vector3(0,-speed * Time.deltaTime,0));
            }
            if(Input.GetKey(KeyCode.W))
            {
                transform.Translate(new Vector3(0,speed * Time.deltaTime,0));
            }

            positionToMoveTo = transform.position;
            float cameraAspectSize = _camera.orthographicSize * _camera.aspect;

            Vector3 newCameraPosition = Vector3.zero;

            if (regionHandler != null && regionHandler.Regions.Count > 0) {

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
                if (region.Height < _camera.orthographicSize * 2) {
                    newCameraPosition.y = region.p1.y + region.Height / 2;
                }
                else {
                    newCameraPosition.y = Mathf.Clamp(positionToMoveTo.y, region.p1.y + _camera.orthographicSize, region.p0.y - _camera.orthographicSize);
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

            // Check if we are hovering UI
            if (_eventSystem.IsPointerOverGameObject()) {
                return;
            }

            // Check for lock. Do nothing if not acquired.
            if (_inputLock.IsLocked) {
                return;
            }

            using (_inputLock.Lock()) {
                // Panning with mouse
                float mouseSensitivity = 0.02f;
                if (Input.GetMouseButtonDown(0)) {
                    // We can't inject IGridUnitManager or IGridInputManager since camera controller lives 
                    // in the project context, so we have to raycast :/.
                    if (Physics2D.Raycast(_camera.ScreenToWorldPoint(Input.mousePosition),
                                          Vector2.zero,
                                          1000.0f,
                                          LayerMask.GetMask("Units")).collider != null) {
                        return;
                    }

                    lastPosition = Input.mousePosition;
                }

                if (Input.GetMouseButton(0) && lastPosition != null) {
                    Vector3 delta = Input.mousePosition - lastPosition.Value;
                    transform.Translate(-delta.x * mouseSensitivity, -delta.y * mouseSensitivity, 0);
                    lastPosition = Input.mousePosition;
                }

                if (Input.GetMouseButtonUp(0)) {
                    lastPosition = null;
                }
            }
        }

        private void LateUpdate() {
            float sizeMin = 2f;
            float sizeMax = 35f;
            _camera.orthographicSize = Mathf.Clamp(_camera.orthographicSize - Input.mouseScrollDelta.y, sizeMin, sizeMax);
        }

        // Call this function to switch to a completely different region handler
        public void SetRegionHandler(RegionHandler regionHandler) {
            this.regionHandler = regionHandler;
        }
    }
}