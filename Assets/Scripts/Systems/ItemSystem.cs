using System.Collections.Generic;
using UnityEngine;
using UME.Characters;

namespace UME.Systems
{
    /// <summary>
    /// Manages the in-game item shop: purchase, sell, and build-tree validation.
    /// Place this on the GameManager or a dedicated Shop object in the game scene.
    /// </summary>
    public class ItemSystem : MonoBehaviour
    {
        [Header("Shop Inventory")]
        [SerializeField] private List<ItemData> allItems = new List<ItemData>();

        public IReadOnlyList<ItemData> AllItems => allItems;

        /// <summary>
        /// Attempts to purchase an item for the given hero.
        /// Returns true when the purchase succeeds.
        /// </summary>
        public bool PurchaseItem(HeroController hero, ItemData item)
        {
            if (hero == null || item == null) return false;
            if (!MeetsComponentRequirements(hero, item)) return false;
            if (!hero.SpendGold(item.goldCost)) return false;

            RemoveComponents(hero, item);
            bool equipped = hero.EquipItem(item);
            if (!equipped)
            {
                // Refund if no item slot available
                hero.AddGold(item.goldCost);
            }
            return equipped;
        }

        /// <summary>
        /// Sells an item from the hero's inventory for its sell value.
        /// </summary>
        public bool SellItem(HeroController hero, ItemData item)
        {
            if (hero == null || item == null) return false;
            if (!hero.RemoveItem(item)) return false;
            hero.AddGold(item.sellValue);
            return true;
        }

        private bool MeetsComponentRequirements(HeroController hero, ItemData item)
        {
            if (item.components == null || item.components.Length == 0) return true;

            ItemData[] equipped = hero.GetEquippedItems();
            List<ItemData> equippedList = new List<ItemData>(equipped);

            foreach (var component in item.components)
            {
                if (!equippedList.Remove(component)) return false;
            }
            return true;
        }

        private void RemoveComponents(HeroController hero, ItemData item)
        {
            if (item.components == null) return;
            foreach (var component in item.components)
            {
                hero.RemoveItem(component);
            }
        }

        /// <summary>
        /// Returns items filtered by category.
        /// </summary>
        public List<ItemData> GetItemsByCategory(ItemCategory category)
        {
            return allItems.FindAll(i => i.category == category);
        }
    }
}
