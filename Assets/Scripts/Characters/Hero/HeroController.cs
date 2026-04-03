using System;
using UnityEngine;
using UME.Systems;

namespace UME.Characters
{
    /// <summary>
    /// Runtime controller for a hero unit. Manages levelling, experience, resources,
    /// item slots, skill references and KDA counters.
    /// Attach this to the hero prefab root object.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class HeroController : UnitBase
    {
        [Header("Hero Setup")]
        [SerializeField] private HeroData heroData;

        // ── Levelling ──────────────────────────────────────────────────────────
        public int Level        { get; private set; } = 1;
        public int MaxLevel     { get; } = 18;
        public float Experience { get; private set; }
        public float ExperienceToNextLevel => GetRequiredExperience(Level);

        // ── Resource (mana / energy) ───────────────────────────────────────────
        public float CurrentResource { get; private set; }

        // ── Economy ───────────────────────────────────────────────────────────
        public int Gold { get; private set; }

        // ── KDA ───────────────────────────────────────────────────────────────
        public int Kills   { get; private set; }
        public int Deaths  { get; private set; }
        public int Assists { get; private set; }

        // ── Items ──────────────────────────────────────────────────────────────
        private const int MaxItemSlots = 6;
        private ItemData[] equippedItems = new ItemData[MaxItemSlots];

        // ── Events ────────────────────────────────────────────────────────────
        public event Action<int>   OnLevelUp;
        public event Action<float> OnExperienceGained;
        public event Action<int>   OnGoldChanged;
        public event Action<int, int, int> OnKDAChanged;

        public HeroData HeroData => heroData;

        protected override void Awake()
        {
            if (heroData != null)
            {
                unitName    = heroData.heroName;
                baseStats   = heroData.baseStats;
            }
            base.Awake();
            CurrentResource = currentStats.maxResource;
        }

        // ── Experience & Levelling ─────────────────────────────────────────────

        public void AddExperience(float amount)
        {
            if (Level >= MaxLevel) return;

            Experience += amount;
            OnExperienceGained?.Invoke(amount);

            while (Level < MaxLevel && Experience >= GetRequiredExperience(Level))
            {
                Experience -= GetRequiredExperience(Level);
                LevelUp();
            }
        }

        private void LevelUp()
        {
            Level++;
            if (heroData != null)
            {
                UnitStats growth = heroData.statsPerLevel;
                currentStats.maxHealth    += growth.maxHealth;
                currentStats.healthRegen  += growth.healthRegen;
                currentStats.armor        += growth.armor;
                currentStats.attackDamage += growth.attackDamage;
                currentStats.moveSpeed    += growth.moveSpeed;
                currentStats.maxResource  += growth.maxResource;
            }
            CurrentHealth = currentStats.maxHealth;
            CurrentResource = currentStats.maxResource;
            OnLevelUp?.Invoke(Level);
        }

        private float GetRequiredExperience(int level)
        {
            return 100f + (level - 1) * 80f;
        }

        // ── Economy ───────────────────────────────────────────────────────────

        public void AddGold(int amount)
        {
            Gold += amount;
            OnGoldChanged?.Invoke(Gold);
        }

        public bool SpendGold(int amount)
        {
            if (Gold < amount) return false;
            Gold -= amount;
            OnGoldChanged?.Invoke(Gold);
            return true;
        }

        // ── Items ─────────────────────────────────────────────────────────────

        public bool EquipItem(ItemData item)
        {
            for (int i = 0; i < MaxItemSlots; i++)
            {
                if (equippedItems[i] == null)
                {
                    equippedItems[i] = item;
                    ApplyItemStats(item, add: true);
                    return true;
                }
            }
            return false;
        }

        public bool RemoveItem(ItemData item)
        {
            for (int i = 0; i < MaxItemSlots; i++)
            {
                if (equippedItems[i] == item)
                {
                    equippedItems[i] = null;
                    ApplyItemStats(item, add: false);
                    return true;
                }
            }
            return false;
        }

        public ItemData[] GetEquippedItems() => (ItemData[])equippedItems.Clone();

        private void ApplyItemStats(ItemData item, bool add)
        {
            float mul = add ? 1f : -1f;
            currentStats.maxHealth    += item.bonusStats.maxHealth    * mul;
            currentStats.armor        += item.bonusStats.armor        * mul;
            currentStats.attackDamage += item.bonusStats.attackDamage * mul;
            currentStats.abilityPower += item.bonusStats.abilityPower * mul;
            currentStats.moveSpeed    += item.bonusStats.moveSpeed    * mul;
            currentStats.maxResource  += item.bonusStats.maxResource  * mul;
        }

        // ── KDA ───────────────────────────────────────────────────────────────

        public void RecordKill()
        {
            Kills++;
            OnKDAChanged?.Invoke(Kills, Deaths, Assists);
        }

        public void RecordAssist()
        {
            Assists++;
            OnKDAChanged?.Invoke(Kills, Deaths, Assists);
        }

        protected override void Die(UnitBase killer)
        {
            Deaths++;
            OnKDAChanged?.Invoke(Kills, Deaths, Assists);

            if (killer is HeroController heroKiller)
            {
                heroKiller.RecordKill();
            }

            base.Die(killer);
            gameObject.SetActive(false);
        }

        // ── Resource ──────────────────────────────────────────────────────────

        public bool UseResource(float cost)
        {
            if (CurrentResource < cost) return false;
            CurrentResource -= cost;
            return true;
        }

        public void RestoreResource(float amount)
        {
            CurrentResource = Mathf.Min(currentStats.maxResource, CurrentResource + amount);
        }
    }
}
