using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UME.Characters;

namespace UME.UI
{
    /// <summary>
    /// Banning phase UI controller.
    /// Teams alternate banning heroes in rounds defined by GameConfig.
    /// </summary>
    public class HeroBanningUI : MonoBehaviour
    {
        [Header("Hero Data")]
        [SerializeField] private List<HeroData> allHeroes = new List<HeroData>();

        [Header("Grids")]
        [SerializeField] private Transform heroPickGridParent;
        [SerializeField] private HeroCardUI heroCardPrefab;

        [Header("Banned Display")]
        [SerializeField] private Transform blueBansParent;
        [SerializeField] private Transform redBansParent;
        [SerializeField] private Image     bannedHeroImagePrefab;

        [Header("Status")]
        [SerializeField] private TextMeshProUGUI phaseText;
        [SerializeField] private TextMeshProUGUI timerText;
        [SerializeField] private Button          confirmBanButton;

        [Header("Timer")]
        [SerializeField] private float banTimeLimit = 30f;

        private List<HeroData> blueBans = new List<HeroData>();
        private List<HeroData> redBans  = new List<HeroData>();
        private HeroData pendingBan;
        private float remainingTime;
        private bool timerActive;
        private int currentTeam = 1; // 1 = Blue, 2 = Red
        private int bansPerTeam = 3;
        private int blueBanCount;
        private int redBanCount;

        public System.Action<List<HeroData>> OnBanPhaseComplete;

        private void Start()
        {
            PopulateGrid();
            if (confirmBanButton != null)
            {
                confirmBanButton.onClick.AddListener(ConfirmBan);
                confirmBanButton.interactable = false;
            }
            StartNextBan();
        }

        private void Update()
        {
            if (!timerActive) return;
            remainingTime -= Time.deltaTime;
            if (timerText != null) timerText.text = Mathf.CeilToInt(Mathf.Max(0f, remainingTime)).ToString();

            if (remainingTime <= 0f)
            {
                timerActive = false;
                AutoBan();
            }
        }

        public void SelectHeroToBan(HeroData hero)
        {
            if (hero == null) return;
            pendingBan = hero;
            if (confirmBanButton != null) confirmBanButton.interactable = true;
        }

        private void ConfirmBan()
        {
            if (pendingBan == null) return;
            ExecuteBan(pendingBan);
        }

        private void AutoBan()
        {
            List<HeroData> available = GetAvailable();
            if (available.Count > 0)
            {
                ExecuteBan(available[Random.Range(0, available.Count)]);
            }
        }

        private void ExecuteBan(HeroData hero)
        {
            timerActive = false;
            pendingBan = null;
            if (confirmBanButton != null) confirmBanButton.interactable = false;

            List<HeroData> targetBans = currentTeam == 1 ? blueBans : redBans;
            targetBans.Add(hero);

            AddBanDisplay(hero, currentTeam == 1 ? blueBansParent : redBansParent);

            if (currentTeam == 1) blueBanCount++; else redBanCount++;

            currentTeam = currentTeam == 1 ? 2 : 1;

            int totalBans = (bansPerTeam * 2);
            if (blueBanCount + redBanCount >= totalBans)
            {
                CompleteBanPhase();
            }
            else
            {
                StartNextBan();
            }

            PopulateGrid();
        }

        private void StartNextBan()
        {
            remainingTime = banTimeLimit;
            timerActive   = true;

            string teamName = currentTeam == 1 ? "Blue Team" : "Red Team";
            if (phaseText != null) phaseText.text = $"{teamName} — Select a Hero to Ban";
        }

        private void CompleteBanPhase()
        {
            List<HeroData> allBans = new List<HeroData>(blueBans);
            allBans.AddRange(redBans);
            OnBanPhaseComplete?.Invoke(allBans);
        }

        private void PopulateGrid()
        {
            if (heroPickGridParent == null || heroCardPrefab == null) return;
            foreach (Transform child in heroPickGridParent) Destroy(child.gameObject);

            List<HeroData> allBans = GetAllBans();
            foreach (var hero in allHeroes)
            {
                bool isBanned = allBans.Contains(hero);
                var card = Instantiate(heroCardPrefab, heroPickGridParent);
                card.Initialise(hero, isBanned, SelectHeroToBan);
            }
        }

        private void AddBanDisplay(HeroData hero, Transform parent)
        {
            if (bannedHeroImagePrefab == null || parent == null) return;
            var img = Instantiate(bannedHeroImagePrefab, parent);
            img.sprite = hero.portrait;
        }

        private List<HeroData> GetAllBans()
        {
            var all = new List<HeroData>(blueBans);
            all.AddRange(redBans);
            return all;
        }

        private List<HeroData> GetAvailable()
        {
            List<HeroData> bans = GetAllBans();
            return allHeroes.FindAll(h => !bans.Contains(h));
        }
    }
}
