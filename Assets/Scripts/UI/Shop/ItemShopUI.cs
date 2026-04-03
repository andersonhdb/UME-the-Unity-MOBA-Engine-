using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UME.Systems;
using UME.Characters;

namespace UME.UI
{
    /// <summary>
    /// In-game item shop UI controller.
    /// Displays items by category, shows build paths, and handles purchase/sell.
    /// </summary>
    public class ItemShopUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private ItemSystem itemSystem;

        [Header("Category Tabs")]
        [SerializeField] private Button[] categoryButtons;

        [Header("Item Grid")]
        [SerializeField] private Transform itemGridParent;
        [SerializeField] private ItemCardUI itemCardPrefab;

        [Header("Detail Panel")]
        [SerializeField] private Image              itemIconImage;
        [SerializeField] private TextMeshProUGUI    itemNameText;
        [SerializeField] private TextMeshProUGUI    itemDescText;
        [SerializeField] private TextMeshProUGUI    goldCostText;
        [SerializeField] private Button             purchaseButton;
        [SerializeField] private Button             sellButton;
        [SerializeField] private TextMeshProUGUI    purchaseFeedbackText;

        [Header("Search")]
        [SerializeField] private TMP_InputField searchField;

        [Header("Player Gold")]
        [SerializeField] private TextMeshProUGUI goldText;

        private HeroController localHero;
        private ItemData selectedItem;
        private ItemCategory currentCategory = ItemCategory.Basic;

        private void Start()
        {
            for (int i = 0; i < categoryButtons.Length; i++)
            {
                int idx = i;
                categoryButtons[i].onClick.AddListener(() => OnCategorySelected((ItemCategory)idx));
            }

            if (purchaseButton != null) purchaseButton.onClick.AddListener(OnPurchase);
            if (sellButton     != null) sellButton.onClick.AddListener(OnSell);
            if (searchField    != null) searchField.onValueChanged.AddListener(_ => RefreshGrid());

            RefreshGrid();
        }

        /// <summary>
        /// Binds the shop to the local player's hero. Call once the hero spawns.
        /// </summary>
        public void BindHero(HeroController hero)
        {
            localHero = hero;
            if (localHero != null)
            {
                localHero.OnGoldChanged += OnGoldChanged;
                OnGoldChanged(localHero.Gold);
            }
        }

        private void OnCategorySelected(ItemCategory category)
        {
            currentCategory = category;
            RefreshGrid();
        }

        private void RefreshGrid()
        {
            if (itemGridParent == null || itemCardPrefab == null || itemSystem == null) return;

            foreach (Transform child in itemGridParent) Destroy(child.gameObject);

            string query = searchField != null ? searchField.text.ToLower() : "";

            foreach (var item in itemSystem.AllItems)
            {
                if (item.category != currentCategory) continue;
                if (!string.IsNullOrEmpty(query) && !item.itemName.ToLower().Contains(query)) continue;

                var card = Instantiate(itemCardPrefab, itemGridParent);
                card.Initialise(item, SelectItem);
            }
        }

        private void SelectItem(ItemData item)
        {
            selectedItem = item;
            UpdateDetailPanel(item);
        }

        private void UpdateDetailPanel(ItemData item)
        {
            if (itemIconImage  != null) itemIconImage.sprite  = item.icon;
            if (itemNameText   != null) itemNameText.text     = item.itemName;
            if (itemDescText   != null) itemDescText.text     = item.description;
            if (goldCostText   != null) goldCostText.text     = $"{item.goldCost}g";
            if (purchaseButton != null) purchaseButton.interactable = localHero != null && localHero.Gold >= item.goldCost;
            if (sellButton     != null) sellButton.interactable = false; // enable if hero owns item
        }

        private void OnPurchase()
        {
            if (selectedItem == null || localHero == null || itemSystem == null) return;

            bool success = itemSystem.PurchaseItem(localHero, selectedItem);
            if (purchaseFeedbackText != null)
            {
                purchaseFeedbackText.text = success ? "Purchased!" : "Cannot purchase.";
            }
        }

        private void OnSell()
        {
            if (selectedItem == null || localHero == null || itemSystem == null) return;
            itemSystem.SellItem(localHero, selectedItem);
        }

        private void OnGoldChanged(int gold)
        {
            if (goldText != null) goldText.text = $"{gold}g";
        }

        private void OnDestroy()
        {
            if (localHero != null) localHero.OnGoldChanged -= OnGoldChanged;
        }
    }
}
