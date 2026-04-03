using System;
using System.Collections.Generic;
using UnityEngine;
using UME.Characters;

namespace UME.Systems
{
    /// <summary>
    /// ScriptableObject that defines a hero skill/ability.
    /// Create via Assets > Create > UME > Skill > SkillData.
    /// </summary>
    [CreateAssetMenu(fileName = "NewSkillData", menuName = "UME/Skill/SkillData")]
    public class SkillData : ScriptableObject
    {
        [Header("Identity")]
        public string skillName = "Skill";
        [TextArea(2, 4)]
        public string description = "";
        public Sprite icon;
        public SkillType skillType = SkillType.Active;
        public SkillTarget targetType = SkillTarget.Enemy;

        [Header("Cost & Cooldown")]
        public float resourceCost = 60f;
        public float cooldown = 8f;

        [Header("Damage / Heal")]
        public float baseDamage = 0f;
        public float damageScaling = 0f;
        [Tooltip("Base heal amount if this skill heals.")]
        public float baseHeal = 0f;

        [Header("Range & Area")]
        public float castRange = 6f;
        public float areaOfEffect = 0f;

        [Header("Projectile")]
        public bool isProjectile = false;
        public float projectileSpeed = 10f;
        [Tooltip("Addressable key for the projectile prefab.")]
        public string projectilePrefabKey = "";

        [Header("VFX / SFX")]
        [Tooltip("Addressable key for cast VFX.")]
        public string castVFXKey = "";
        [Tooltip("Addressable key for impact VFX.")]
        public string impactVFXKey = "";
        [Tooltip("Addressable key for cast sound.")]
        public string castSFXKey = "";

        [Header("Per-Level Values")]
        [Tooltip("Override values per level (index 0 = level 1). Leave empty to use base values.")]
        public float[] damagePerLevel;
        public float[] cooldownPerLevel;
        public float[] resourceCostPerLevel;

        public float GetDamageAtLevel(int level)
        {
            int idx = level - 1;
            if (damagePerLevel != null && idx < damagePerLevel.Length) return damagePerLevel[idx];
            return baseDamage;
        }

        public float GetCooldownAtLevel(int level)
        {
            int idx = level - 1;
            if (cooldownPerLevel != null && idx < cooldownPerLevel.Length) return cooldownPerLevel[idx];
            return cooldown;
        }

        public float GetCostAtLevel(int level)
        {
            int idx = level - 1;
            if (resourceCostPerLevel != null && idx < resourceCostPerLevel.Length) return resourceCostPerLevel[idx];
            return resourceCost;
        }
    }

    public enum SkillType
    {
        Passive,
        Active,
        Toggle,
        Channeled
    }

    public enum SkillTarget
    {
        Self,
        Ally,
        Enemy,
        AllUnits,
        GroundTarget,
        Direction
    }
}
