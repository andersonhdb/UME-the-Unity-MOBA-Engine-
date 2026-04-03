using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UME.Characters;

namespace UME.UI
{
    /// <summary>
    /// Hero selection screen UI controller.
    /// Populates a grid of hero cards, handles search/filter, and confirms the pick.
    /// </summary>
    public class HeroSelectionUI : MonoBehaviour
    {
        [Header("Hero Data")]
        [SerializeField] private List<HeroData> availableHeroes = new List<HeroData>();

        [Header("Grid")]
        [SerializeField] private Transform heroGridParent;
        [SerializeField] private HeroCardUI heroCardPrefab;

        [Header("Preview Panel")]
        [SerializeField] private Image    splashArtImage;
        [SerializeField] private TextMeshProUGUI heroNameText;
        [SerializeField] private TextMeshProUGUI heroTitleText;
        [SerializeField] private TextMeshProUGUI descriptionText;
        [SerializeField] private TextMeshProUGUI primaryRoleText;

        [Header("Buttons")]
        [SerializeField] private Button confirmButton;
        [SerializeField] private Button randomButton;

        [Header("Filter")]
        [SerializeField] private TMP_InputField searchField;
        [SerializeField] private TMP_Dropdown   roleFilterDropdown;

        [Header("Timer")]
        [SerializeField] private TextMeshProUGUI timerText;

        private HeroData selectedHero;
        private List<HeroData> bannedHeroes = new List<HeroData>();
        private float remainingTime;
        private bool timerActive;

        public System.Action<HeroData> OnHeroConfirmed;

        private void Start()
        {
            PopulateGrid(availableHeroes);

            if (confirmButton != null) confirmButton.onClick.AddListener(ConfirmSelection);
            if (randomButton  != null) randomButton.onClick.AddListener(SelectRandom);
            if (searchField   != null) searchField.onValueChanged.AddListener(FilterBySearch);
            if (roleFilterDropdown != null) roleFilterDropdown.onValueChanged.AddListener(_ => ApplyFilters());
        }

        private void Update()
        {
            if (!timerActive) return;
            remainingTime -= Time.deltaTime;
            if (timerText != null) timerText.text = Mathf.CeilToInt(Mathf.Max(0f, remainingTime)).ToString();

            if (remainingTime <= 0f)
            {
                timerActive = false;
                SelectRandom();
            }
        }

        /// <summary>
        /// Marks heroes as banned so they cannot be selected.
        /// </summary>
        public void SetBannedHeroes(List<HeroData> banned)
        {
            bannedHeroes = banned ?? new List<HeroData>();
            PopulateGrid(GetFilteredHeroes());
        }

        /// <summary>
        /// Starts the pick timer.
        /// </summary>
        public void StartTimer(float seconds)
        {
            remainingTime = seconds;
            timerActive   = true;
        }

        public void SelectHero(HeroData hero)
        {
            if (hero == null || bannedHeroes.Contains(hero)) return;
            selectedHero = hero;
            UpdatePreviewPanel(hero);
            if (confirmButton != null) confirmButton.interactable = true;
        }

        private void ConfirmSelection()
        {
            if (selectedHero == null) return;
            timerActive = false;
            OnHeroConfirmed?.Invoke(selectedHero);
        }

        private void SelectRandom()
        {
            List<HeroData> available = GetFilteredHeroes();
            if (available.Count == 0) return;
            SelectHero(available[Random.Range(0, available.Count)]);
            ConfirmSelection();
        }

        private void PopulateGrid(List<HeroData> heroes)
        {
            if (heroGridParent == null || heroCardPrefab == null) return;

            foreach (Transform child in heroGridParent)
            {
                Destroy(child.gameObject);
            }

            foreach (var hero in heroes)
            {
                var card = Instantiate(heroCardPrefab, heroGridParent);
                card.Initialise(hero, bannedHeroes.Contains(hero), SelectHero);
            }
        }

        private void UpdatePreviewPanel(HeroData hero)
        {
            if (splashArtImage   != null) splashArtImage.sprite   = hero.splashArt;
            if (heroNameText     != null) heroNameText.text        = hero.heroName;
            if (heroTitleText    != null) heroTitleText.text       = hero.heroTitle;
            if (descriptionText  != null) descriptionText.text     = hero.description;
            if (primaryRoleText  != null) primaryRoleText.text     = hero.primaryRole.ToString();
        }

        private void FilterBySearch(string query)
        {
            ApplyFilters();
        }

        private void ApplyFilters()
        {
            PopulateGrid(GetFilteredHeroes());
        }

        private List<HeroData> GetFilteredHeroes()
        {
            string query = searchField != null ? searchField.text.ToLower() : "";
            int roleFilter = roleFilterDropdown != null ? roleFilterDropdown.value : 0;

            List<HeroData> filtered = new List<HeroData>();
            foreach (var hero in availableHeroes)
            {
                if (bannedHeroes.Contains(hero)) continue;
                if (!string.IsNullOrEmpty(query) && !hero.heroName.ToLower().Contains(query)) continue;
                if (roleFilter > 0 && (int)hero.primaryRole != roleFilter) continue;
                filtered.Add(hero);
            }
            return filtered;
        }
    }
}
