using UnityEngine;
using UME.Characters;

namespace UME.Systems
{
    /// <summary>
    /// Defines an item purchasable from the in-game shop.
    /// Create via Assets > Create > UME > Item > ItemData.
    /// </summary>
    [CreateAssetMenu(fileName = "NewItemData", menuName = "UME/Item/ItemData")]
    public class ItemData : ScriptableObject
    {
        [Header("Identity")]
        public string itemName = "Item";
        [TextArea(2, 4)]
        public string description = "";
        public Sprite icon;

        [Header("Economy")]
        public int goldCost = 300;
        public int sellValue = 200;

        [Header("Components")]
        [Tooltip("Items that must be owned to build this item.")]
        public ItemData[] components;

        [Header("Stat Bonuses")]
        public UnitStats bonusStats;

        [Header("Classification")]
        public ItemCategory category = ItemCategory.Basic;
        public bool isConsumable = false;
        public bool isUnique = false;

        [Header("Addressables")]
        public string iconAddressableKey = "";
    }

    public enum ItemCategory
    {
        Basic,
        Advanced,
        Legendary,
        Starter,
        Consumable,
        Boot
    }
}
