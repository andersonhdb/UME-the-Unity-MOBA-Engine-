using System;
using UnityEngine;
using UME.Characters;

namespace UME.Systems
{
    /// <summary>
    /// Runtime skill builder that manages a hero's equipped skills,
    /// cooldown tracking, and skill execution.
    /// Attach to the hero prefab alongside HeroController.
    /// </summary>
    [RequireComponent(typeof(HeroController))]
    public class SkillBuilder : MonoBehaviour
    {
        private HeroController hero;
        private SkillData[] skills = new SkillData[4];
        private int[] skillLevels = new int[4] { 1, 1, 1, 1 };
        private float[] cooldownTimers = new float[4];

        public event Action<int, SkillData> OnSkillEquipped;
        public event Action<int, float> OnSkillActivated;

        private void Awake()
        {
            hero = GetComponent<HeroController>();
        }

        private void Start()
        {
            if (hero.HeroData != null)
            {
                for (int i = 0; i < hero.HeroData.activeSkills.Count && i < skills.Length; i++)
                {
                    skills[i] = hero.HeroData.activeSkills[i];
                }
            }
        }

        private void Update()
        {
            for (int i = 0; i < cooldownTimers.Length; i++)
            {
                if (cooldownTimers[i] > 0f)
                {
                    cooldownTimers[i] -= Time.deltaTime;
                }
            }

            // Keyboard shortcuts: Q W E R
            if (Input.GetKeyDown(KeyCode.Q)) TryActivateSkill(0);
            if (Input.GetKeyDown(KeyCode.W)) TryActivateSkill(1);
            if (Input.GetKeyDown(KeyCode.E)) TryActivateSkill(2);
            if (Input.GetKeyDown(KeyCode.R)) TryActivateSkill(3);
        }

        /// <summary>
        /// Attempts to activate a skill by slot index (0-3 = Q/W/E/R).
        /// Returns true if the skill was successfully activated.
        /// </summary>
        public bool TryActivateSkill(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= skills.Length) return false;

            SkillData skill = skills[slotIndex];
            if (skill == null || skill.skillType == SkillType.Passive) return false;

            if (cooldownTimers[slotIndex] > 0f)
            {
                Debug.Log($"[SkillBuilder] Skill '{skill.skillName}' is on cooldown ({cooldownTimers[slotIndex]:F1}s remaining).");
                return false;
            }

            int level = skillLevels[slotIndex];
            float cost = skill.GetCostAtLevel(level);
            if (!hero.UseResource(cost))
            {
                Debug.Log($"[SkillBuilder] Not enough resource for '{skill.skillName}'.");
                return false;
            }

            cooldownTimers[slotIndex] = skill.GetCooldownAtLevel(level);
            OnSkillActivated?.Invoke(slotIndex, cooldownTimers[slotIndex]);

            ExecuteSkill(skill, level);
            return true;
        }

        /// <summary>
        /// Assigns a skill to a specific slot.
        /// </summary>
        public void EquipSkill(int slotIndex, SkillData skill)
        {
            if (slotIndex < 0 || slotIndex >= skills.Length) return;
            skills[slotIndex] = skill;
            OnSkillEquipped?.Invoke(slotIndex, skill);
        }

        /// <summary>
        /// Levels up a skill slot (max level is 5).
        /// </summary>
        public void LevelUpSkill(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= skillLevels.Length) return;
            skillLevels[slotIndex] = Mathf.Min(5, skillLevels[slotIndex] + 1);
        }

        public SkillData GetSkill(int slotIndex) => slotIndex >= 0 && slotIndex < skills.Length ? skills[slotIndex] : null;
        public int GetSkillLevel(int slotIndex) => slotIndex >= 0 && slotIndex < skillLevels.Length ? skillLevels[slotIndex] : 0;
        public float GetCooldownRemaining(int slotIndex) => slotIndex >= 0 && slotIndex < cooldownTimers.Length ? Mathf.Max(0f, cooldownTimers[slotIndex]) : 0f;

        private void ExecuteSkill(SkillData skill, int level)
        {
            // Base implementation: extend this or override per-skill via custom handlers.
            Debug.Log($"[SkillBuilder] Executing '{skill.skillName}' at level {level}.");
        }
    }
}
