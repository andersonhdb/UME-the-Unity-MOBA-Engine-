using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UME.Systems;
using System;

namespace UME.UI
{
    /// <summary>
    /// Clickable item card widget used in the shop grid.
    /// </summary>
    public class ItemCardUI : MonoBehaviour
    {
        [SerializeField] private Image           iconImage;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI costText;
        [SerializeField] private Button          selectButton;

        private ItemData itemData;
        private Action<ItemData> onSelected;

        public void Initialise(ItemData data, Action<ItemData> selectionCallback)
        {
            itemData   = data;
            onSelected = selectionCallback;

            if (iconImage  != null) iconImage.sprite = data.icon;
            if (nameText   != null) nameText.text    = data.itemName;
            if (costText   != null) costText.text    = $"{data.goldCost}g";
            if (selectButton != null) selectButton.onClick.AddListener(() => onSelected?.Invoke(itemData));
        }
    }
}
