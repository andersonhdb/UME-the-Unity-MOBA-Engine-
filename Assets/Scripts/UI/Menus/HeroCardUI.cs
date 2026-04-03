using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UME.Characters;
using System;

namespace UME.UI
{
    /// <summary>
    /// A single hero card widget used in the hero selection and banning grids.
    /// </summary>
    public class HeroCardUI : MonoBehaviour
    {
        [SerializeField] private Image              portraitImage;
        [SerializeField] private TextMeshProUGUI    heroNameText;
        [SerializeField] private TextMeshProUGUI    roleText;
        [SerializeField] private Button             selectButton;
        [SerializeField] private GameObject         bannedOverlay;
        [SerializeField] private Image              difficultyIndicator;

        private HeroData heroData;
        private Action<HeroData> onSelected;

        /// <summary>
        /// Populates the card and wires up the click callback.
        /// </summary>
        public void Initialise(HeroData data, bool isBanned, Action<HeroData> selectionCallback)
        {
            heroData   = data;
            onSelected = selectionCallback;

            if (portraitImage != null) portraitImage.sprite = data.portrait;
            if (heroNameText  != null) heroNameText.text    = data.heroName;
            if (roleText      != null) roleText.text        = data.primaryRole.ToString();

            if (bannedOverlay != null) bannedOverlay.SetActive(isBanned);
            if (selectButton  != null)
            {
                selectButton.interactable = !isBanned;
                selectButton.onClick.AddListener(OnCardClicked);
            }
        }

        private void OnCardClicked()
        {
            onSelected?.Invoke(heroData);
        }
    }
}
