using UnityEngine;
using UnityEngine.UI;
using UME.Characters;

namespace UME.UI
{
    /// <summary>
    /// World-space health bar that floats above a unit and faces the camera.
    /// Attach to the unit prefab and assign the UnitBase reference.
    /// </summary>
    public class HealthBarUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private UnitBase trackedUnit;
        [SerializeField] private Slider   healthSlider;
        [SerializeField] private Slider   resourceSlider;

        [Header("Position Offset")]
        [SerializeField] private Vector3 worldOffset = new Vector3(0f, 2.5f, 0f);
        [SerializeField] private bool    faceCamera  = true;

        private Camera mainCamera;
        private Canvas canvas;

        private void Start()
        {
            mainCamera = Camera.main;
            canvas = GetComponentInParent<Canvas>();

            if (trackedUnit != null)
            {
                trackedUnit.OnHealthChanged += OnHealthChanged;
                OnHealthChanged(trackedUnit.CurrentHealth, trackedUnit.CurrentStats.maxHealth);
            }
        }

        private void LateUpdate()
        {
            if (trackedUnit == null) return;

            transform.position = trackedUnit.transform.position + worldOffset;

            if (faceCamera && mainCamera != null)
            {
                transform.forward = mainCamera.transform.forward;
            }
        }

        /// <summary>
        /// Binds the health bar to a unit at runtime.
        /// </summary>
        public void Bind(UnitBase unit)
        {
            if (trackedUnit != null)
            {
                trackedUnit.OnHealthChanged -= OnHealthChanged;
            }

            trackedUnit = unit;
            if (trackedUnit == null) return;

            trackedUnit.OnHealthChanged += OnHealthChanged;
            OnHealthChanged(trackedUnit.CurrentHealth, trackedUnit.CurrentStats.maxHealth);
        }

        private void OnHealthChanged(float current, float max)
        {
            if (healthSlider != null)
            {
                healthSlider.value = max > 0f ? current / max : 0f;
            }
        }

        private void OnDestroy()
        {
            if (trackedUnit != null)
            {
                trackedUnit.OnHealthChanged -= OnHealthChanged;
            }
        }
    }
}
