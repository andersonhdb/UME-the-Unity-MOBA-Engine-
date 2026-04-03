using System;
using UnityEngine;

namespace UME.Characters
{
    /// <summary>
    /// Data container for a unit's core numeric stats.
    /// Used as a value type so stats can be safely copied and modified per-instance.
    /// </summary>
    [Serializable]
    public struct UnitStats
    {
        [Tooltip("Maximum hit points.")]
        public float maxHealth;

        [Tooltip("Hit-point regeneration per second.")]
        public float healthRegen;

        [Tooltip("Flat damage reduction applied before health loss.")]
        public float armor;

        [Tooltip("Magic resistance percentage (0-1).")]
        [Range(0f, 1f)]
        public float magicResist;

        [Tooltip("Physical attack damage.")]
        public float attackDamage;

        [Tooltip("Ability power used by skills.")]
        public float abilityPower;

        [Tooltip("Attack speed multiplier (1 = default).")]
        public float attackSpeed;

        [Tooltip("Movement speed in units per second.")]
        public float moveSpeed;

        [Tooltip("Maximum resource (mana/energy) pool.")]
        public float maxResource;

        [Tooltip("Resource regeneration per second.")]
        public float resourceRegen;

        /// <summary>
        /// Returns a default stat block suitable for a basic minion.
        /// </summary>
        public static UnitStats Default => new UnitStats
        {
            maxHealth    = 500f,
            healthRegen  = 1f,
            armor        = 10f,
            magicResist  = 0.1f,
            attackDamage = 25f,
            abilityPower = 0f,
            attackSpeed  = 1f,
            moveSpeed    = 3.5f,
            maxResource  = 0f,
            resourceRegen = 0f
        };
    }
}
