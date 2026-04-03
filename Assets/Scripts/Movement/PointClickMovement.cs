using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

namespace UME.Movement
{
    /// <summary>
    /// Point-and-click movement for the local player's hero.
    /// Casts a ray from the cursor on right-click and directs the NavMeshAgent.
    /// </summary>
    [RequireComponent(typeof(NavMeshAgent))]
    public class PointClickMovement : MonoBehaviour
    {
        [Header("Input")]
        [Tooltip("Input action reference for the 'Move' command (right mouse button by default).")]
        [SerializeField] private InputActionReference moveAction;

        [Header("Visual Feedback")]
        [Tooltip("Optional marker spawned at the destination click point.")]
        [SerializeField] private GameObject destinationMarkerPrefab;
        [SerializeField] private float markerLifetime = 0.5f;

        [Header("Layer Masks")]
        [SerializeField] private LayerMask walkableLayer = ~0;

        private NavMeshAgent agent;
        private Camera mainCamera;
        private GameObject activeMarker;

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
        }

        private void OnEnable()
        {
            if (moveAction != null)
            {
                moveAction.action.performed += OnMovePerformed;
                moveAction.action.Enable();
            }
        }

        private void OnDisable()
        {
            if (moveAction != null)
            {
                moveAction.action.performed -= OnMovePerformed;
                moveAction.action.Disable();
            }
        }

        private void Start()
        {
            mainCamera = Camera.main;
        }

        private void OnMovePerformed(InputAction.CallbackContext ctx)
        {
            MoveToMousePosition();
        }

        /// <summary>
        /// Called from legacy input polling or tests to trigger movement.
        /// </summary>
        public void MoveToMousePosition()
        {
            if (mainCamera == null) return;
            if (Mouse.current == null) return;

            Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, walkableLayer))
            {
                SetDestination(hit.point);
                SpawnMarker(hit.point);
            }
        }

        /// <summary>
        /// Directly sets the NavMeshAgent destination.
        /// </summary>
        public void SetDestination(Vector3 destination)
        {
            if (NavMesh.SamplePosition(destination, out NavMeshHit navHit, 2f, NavMesh.AllAreas))
            {
                agent.SetDestination(navHit.position);
            }
        }

        private void SpawnMarker(Vector3 position)
        {
            if (destinationMarkerPrefab == null) return;

            if (activeMarker != null)
            {
                Destroy(activeMarker);
            }

            activeMarker = Instantiate(destinationMarkerPrefab, position, Quaternion.identity);
            Destroy(activeMarker, markerLifetime);
        }

        private void Update()
        {
            // Fallback for legacy input (when InputActionReference is not assigned).
            if (moveAction == null && Input.GetMouseButtonDown(1))
            {
                MoveToMousePosition();
            }
        }
    }
}
