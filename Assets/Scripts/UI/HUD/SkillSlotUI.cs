using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UME.Systems;

namespace UME.UI
{
    /// <summary>
    /// Individual skill slot widget displayed in the bottom bar.
    /// Renders the skill icon and animates the cooldown overlay.
    /// </summary>
    public class SkillSlotUI : MonoBehaviour
    {
        [SerializeField] private Image iconImage;
        [SerializeField] private Image cooldownOverlay;
        [SerializeField] private TextMeshProUGUI cooldownText;
        [SerializeField] private TextMeshProUGUI keyLabel;

        private Coroutine cooldownCoroutine;

        private void Awake()
        {
            if (cooldownOverlay != null) cooldownOverlay.fillAmount = 0f;
            if (cooldownText != null)   cooldownText.gameObject.SetActive(false);
        }

        public void SetSkill(SkillData skill)
        {
            if (iconImage != null)
            {
                iconImage.sprite  = skill?.icon;
                iconImage.enabled = skill != null;
            }
        }

        public void StartCooldown(float duration)
        {
            if (cooldownCoroutine != null) StopCoroutine(cooldownCoroutine);
            cooldownCoroutine = StartCoroutine(CooldownRoutine(duration));
        }

        private IEnumerator CooldownRoutine(float duration)
        {
            if (cooldownOverlay != null) cooldownOverlay.fillAmount = 1f;
            if (cooldownText != null)   cooldownText.gameObject.SetActive(true);

            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float remaining = duration - elapsed;
                if (cooldownOverlay != null) cooldownOverlay.fillAmount = 1f - (elapsed / duration);
                if (cooldownText != null)   cooldownText.text = Mathf.CeilToInt(remaining).ToString();
                yield return null;
            }

            if (cooldownOverlay != null) cooldownOverlay.fillAmount = 0f;
            if (cooldownText != null)   cooldownText.gameObject.SetActive(false);
        }
    }
}
