using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UME.Characters;
using UME.Systems;

namespace UME.UI
{
    /// <summary>
    /// Controls the bottom HUD bar shown during gameplay.
    /// Displays hero portrait, health/resource bars, skill icons, gold, and level.
    /// Wire up references in the Inspector.
    /// </summary>
    public class BottomBarUI : MonoBehaviour
    {
        [Header("Hero Info")]
        [SerializeField] private Image heroPortrait;
        [SerializeField] private TextMeshProUGUI heroNameText;
        [SerializeField] private TextMeshProUGUI levelText;

        [Header("Health & Resource Bars")]
        [SerializeField] private Slider healthBar;
        [SerializeField] private Slider resourceBar;
        [SerializeField] private TextMeshProUGUI healthText;
        [SerializeField] private TextMeshProUGUI resourceText;

        [Header("Skill Icons (Q W E R)")]
        [SerializeField] private SkillSlotUI[] skillSlots = new SkillSlotUI[4];

        [Header("Gold")]
        [SerializeField] private TextMeshProUGUI goldText;

        [Header("KDA")]
        [SerializeField] private TextMeshProUGUI kdaText;

        [Header("Experience Bar")]
        [SerializeField] private Slider experienceBar;

        private HeroController trackedHero;
        private SkillBuilder skillBuilder;

        /// <summary>
        /// Binds the bottom bar to a specific hero controller.
        /// </summary>
        public void BindHero(HeroController hero)
        {
            if (trackedHero != null)
            {
                trackedHero.OnHealthChanged -= OnHealthChanged;
                trackedHero.OnLevelUp       -= OnLevelUp;
                trackedHero.OnGoldChanged   -= OnGoldChanged;
                trackedHero.OnKDAChanged    -= OnKDAChanged;
                trackedHero.OnExperienceGained -= OnExperienceGained;
            }

            trackedHero = hero;
            if (trackedHero == null) return;

            trackedHero.OnHealthChanged    += OnHealthChanged;
            trackedHero.OnLevelUp         += OnLevelUp;
            trackedHero.OnGoldChanged     += OnGoldChanged;
            trackedHero.OnKDAChanged      += OnKDAChanged;
            trackedHero.OnExperienceGained += OnExperienceGained;

            skillBuilder = hero.GetComponent<SkillBuilder>();
            if (skillBuilder != null)
            {
                skillBuilder.OnSkillActivated += OnSkillActivated;
            }

            RefreshAll();
        }

        private void RefreshAll()
        {
            if (trackedHero == null) return;

            // Identity
            if (heroPortrait != null && trackedHero.HeroData?.portrait != null)
                heroPortrait.sprite = trackedHero.HeroData.portrait;
            if (heroNameText != null)
                heroNameText.text = trackedHero.HeroData?.heroName ?? trackedHero.UnitName;

            OnHealthChanged(trackedHero.CurrentHealth, trackedHero.CurrentStats.maxHealth);
            OnLevelUp(trackedHero.Level);
            OnGoldChanged(trackedHero.Gold);
            OnKDAChanged(trackedHero.Kills, trackedHero.Deaths, trackedHero.Assists);

            // Skills
            if (skillBuilder != null)
            {
                for (int i = 0; i < skillSlots.Length; i++)
                {
                    if (skillSlots[i] != null)
                    {
                        skillSlots[i].SetSkill(skillBuilder.GetSkill(i));
                    }
                }
            }
        }

        private void OnHealthChanged(float current, float max)
        {
            if (healthBar != null) healthBar.value = max > 0f ? current / max : 0f;
            if (healthText != null) healthText.text = $"{Mathf.CeilToInt(current)}/{Mathf.CeilToInt(max)}";

            float resource = trackedHero != null ? trackedHero.CurrentResource : 0f;
            float maxResource = trackedHero?.CurrentStats.maxResource ?? 0f;
            if (resourceBar != null) resourceBar.value = maxResource > 0f ? resource / maxResource : 0f;
            if (resourceText != null) resourceText.text = $"{Mathf.CeilToInt(resource)}/{Mathf.CeilToInt(maxResource)}";
        }

        private void OnLevelUp(int level)
        {
            if (levelText != null) levelText.text = $"Lv {level}";
            if (experienceBar != null && trackedHero != null)
            {
                float toNext = trackedHero.ExperienceToNextLevel;
                experienceBar.value = toNext > 0f ? trackedHero.Experience / toNext : 1f;
            }
        }

        private void OnGoldChanged(int gold)
        {
            if (goldText != null) goldText.text = gold.ToString("N0");
        }

        private void OnKDAChanged(int kills, int deaths, int assists)
        {
            if (kdaText != null) kdaText.text = $"{kills}/{deaths}/{assists}";
        }

        private void OnExperienceGained(float amount)
        {
            OnLevelUp(trackedHero?.Level ?? 1);
        }

        private void OnSkillActivated(int slotIndex, float cooldown)
        {
            if (slotIndex >= 0 && slotIndex < skillSlots.Length && skillSlots[slotIndex] != null)
            {
                skillSlots[slotIndex].StartCooldown(cooldown);
            }
        }

        private void OnDestroy()
        {
            if (trackedHero != null)
            {
                trackedHero.OnHealthChanged    -= OnHealthChanged;
                trackedHero.OnLevelUp         -= OnLevelUp;
                trackedHero.OnGoldChanged     -= OnGoldChanged;
                trackedHero.OnKDAChanged      -= OnKDAChanged;
                trackedHero.OnExperienceGained -= OnExperienceGained;
            }
            if (skillBuilder != null)
            {
                skillBuilder.OnSkillActivated -= OnSkillActivated;
            }
        }
    }
}
