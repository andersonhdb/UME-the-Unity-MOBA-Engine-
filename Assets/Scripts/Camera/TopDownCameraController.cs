using UnityEngine;
using Unity.Cinemachine;

namespace UME.Camera
{
    /// <summary>
    /// Controls the top-down Cinemachine camera for the main game view.
    /// Supports edge-scroll panning, zoom, and hero-follow toggle.
    /// Attach this component to a Camera Rig or empty root object in the scene.
    /// </summary>
    public class TopDownCameraController : MonoBehaviour
    {
        [Header("Camera Reference")]
        [SerializeField] private CinemachineCamera cinemachineCamera;

        [Header("Pan Settings")]
        [SerializeField] private float panSpeed = 20f;
        [SerializeField] private float edgeScrollThreshold = 20f;
        [SerializeField] private bool enableEdgeScroll = true;

        [Header("Zoom Settings")]
        [SerializeField] private float zoomSpeed = 5f;
        [SerializeField] private float minZoom = 10f;
        [SerializeField] private float maxZoom = 60f;

        [Header("Bounds")]
        [SerializeField] private bool clampToBounds = true;
        [SerializeField] private Vector2 minBounds = new Vector2(-50f, -50f);
        [SerializeField] private Vector2 maxBounds = new Vector2(50f, 50f);

        [Header("Follow")]
        [Tooltip("The transform the camera will follow when locked to hero.")]
        [SerializeField] private Transform heroFollowTarget;
        [SerializeField] private bool isFollowingHero = false;

        private CinemachineFollow followComponent;
        private float currentZoom;

        private void Start()
        {
            if (cinemachineCamera != null)
            {
                followComponent = cinemachineCamera.GetComponent<CinemachineFollow>();
                if (followComponent != null && heroFollowTarget != null)
                {
                    cinemachineCamera.Follow = heroFollowTarget;
                }
            }

            currentZoom = GetCurrentZoom();
        }

        private void Update()
        {
            if (isFollowingHero) return;

            HandleEdgeScroll();
            HandleKeyboardPan();
            HandleZoom();
        }

        private void HandleEdgeScroll()
        {
            if (!enableEdgeScroll) return;

            Vector3 direction = Vector3.zero;
            Vector3 mousePos  = Input.mousePosition;

            if (mousePos.x < edgeScrollThreshold)                    direction.x = -1f;
            else if (mousePos.x > Screen.width - edgeScrollThreshold) direction.x = 1f;

            if (mousePos.y < edgeScrollThreshold)                     direction.z = -1f;
            else if (mousePos.y > Screen.height - edgeScrollThreshold) direction.z = 1f;

            MoveCamera(direction);
        }

        private void HandleKeyboardPan()
        {
            Vector3 direction = new Vector3(
                Input.GetAxisRaw("Horizontal"),
                0f,
                Input.GetAxisRaw("Vertical")
            );
            MoveCamera(direction);
        }

        private void MoveCamera(Vector3 direction)
        {
            if (direction == Vector3.zero) return;

            Vector3 move = direction.normalized * (panSpeed * Time.deltaTime);
            Vector3 newPos = transform.position + move;

            if (clampToBounds)
            {
                newPos.x = Mathf.Clamp(newPos.x, minBounds.x, maxBounds.x);
                newPos.z = Mathf.Clamp(newPos.z, minBounds.y, maxBounds.y);
            }

            transform.position = newPos;
        }

        private void HandleZoom()
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (Mathf.Abs(scroll) < 0.01f) return;

            currentZoom = Mathf.Clamp(currentZoom - scroll * zoomSpeed * 10f, minZoom, maxZoom);
            SetZoom(currentZoom);
        }

        private void SetZoom(float zoom)
        {
            if (cinemachineCamera == null) return;

            // For an orthographic camera lens
            var lens = cinemachineCamera.Lens;
            if (lens.Orthographic)
            {
                lens.OrthographicSize = zoom;
                cinemachineCamera.Lens = lens;
            }
            else
            {
                // Adjust the follow offset Y for perspective cameras
                if (followComponent != null)
                {
                    var offset = followComponent.FollowOffset;
                    offset.y = zoom;
                    followComponent.FollowOffset = offset;
                }
            }
        }

        private float GetCurrentZoom()
        {
            if (cinemachineCamera == null) return (minZoom + maxZoom) / 2f;

            var lens = cinemachineCamera.Lens;
            return lens.Orthographic ? lens.OrthographicSize : (followComponent != null ? followComponent.FollowOffset.y : 30f);
        }

        /// <summary>
        /// Locks or unlocks the camera to follow the hero.
        /// </summary>
        public void SetHeroFollow(bool follow)
        {
            isFollowingHero = follow;
        }

        /// <summary>
        /// Instantly snaps the camera rig to a world position.
        /// </summary>
        public void SnapToPosition(Vector3 worldPos)
        {
            worldPos.y = transform.position.y;
            transform.position = worldPos;
        }

        /// <summary>
        /// Assigns the hero transform the camera should track when locked.
        /// </summary>
        public void SetFollowTarget(Transform target)
        {
            heroFollowTarget = target;
            if (cinemachineCamera != null)
            {
                cinemachineCamera.Follow = target;
            }
        }
    }
}
