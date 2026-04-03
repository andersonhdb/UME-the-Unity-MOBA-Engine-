using System;
using UnityEngine;

namespace UME.Characters
{
    /// <summary>
    /// Abstract base class for every unit in the game (heroes, minions, towers, etc.).
    /// Provides health, stats, team assignment, and death callbacks.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public abstract class UnitBase : MonoBehaviour
    {
        [Header("Unit Identity")]
        [SerializeField] protected string unitName = "Unit";
        [SerializeField] protected TeamID team = TeamID.Neutral;

        [Header("Stats")]
        [SerializeField] protected UnitStats baseStats;

        protected UnitStats currentStats;

        public string UnitName => unitName;
        public TeamID Team => team;
        public UnitStats CurrentStats => currentStats;

        public float CurrentHealth { get; protected set; }
        public bool IsAlive => CurrentHealth > 0f;

        public event Action<UnitBase> OnDeath;
        public event Action<float, float> OnHealthChanged;

        protected virtual void Awake()
        {
            currentStats = baseStats;
            CurrentHealth = currentStats.maxHealth;
        }

        /// <summary>
        /// Applies damage to the unit, clamping health to zero and firing death event.
        /// </summary>
        public virtual void TakeDamage(float amount, UnitBase source = null)
        {
            if (!IsAlive) return;

            float mitigated = Mathf.Max(0f, amount - currentStats.armor);
            CurrentHealth = Mathf.Max(0f, CurrentHealth - mitigated);
            OnHealthChanged?.Invoke(CurrentHealth, currentStats.maxHealth);

            if (CurrentHealth <= 0f)
            {
                Die(source);
            }
        }

        /// <summary>
        /// Restores health up to the unit's maximum.
        /// </summary>
        public virtual void Heal(float amount)
        {
            if (!IsAlive) return;
            CurrentHealth = Mathf.Min(currentStats.maxHealth, CurrentHealth + amount);
            OnHealthChanged?.Invoke(CurrentHealth, currentStats.maxHealth);
        }

        /// <summary>
        /// Called when the unit's health reaches zero.
        /// Override to add custom death behaviour (animation, loot, etc.).
        /// </summary>
        protected virtual void Die(UnitBase killer)
        {
            OnDeath?.Invoke(this);
        }

        /// <summary>
        /// Restores the unit to full health (e.g. after respawn).
        /// </summary>
        public virtual void Respawn(Vector3 position)
        {
            CurrentHealth = currentStats.maxHealth;
            transform.position = position;
            gameObject.SetActive(true);
            OnHealthChanged?.Invoke(CurrentHealth, currentStats.maxHealth);
        }
    }

    public enum TeamID
    {
        Neutral = 0,
        Blue    = 1,
        Red     = 2
    }
}
