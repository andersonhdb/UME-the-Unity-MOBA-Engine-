using UnityEngine;
using TMPro;
using UME.Characters;

namespace UME.UI
{
    /// <summary>
    /// Displays kill / death / assist statistics for the local player's hero.
    /// Can be placed in a scoreboard or the bottom HUD.
    /// </summary>
    public class KDATrackerUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI killsText;
        [SerializeField] private TextMeshProUGUI deathsText;
        [SerializeField] private TextMeshProUGUI assistsText;
        [SerializeField] private TextMeshProUGUI kdaRatioText;

        private HeroController trackedHero;

        /// <summary>
        /// Binds this tracker to a specific hero. Call whenever the local hero changes.
        /// </summary>
        public void BindHero(HeroController hero)
        {
            if (trackedHero != null)
            {
                trackedHero.OnKDAChanged -= UpdateDisplay;
            }

            trackedHero = hero;
            if (trackedHero == null) return;

            trackedHero.OnKDAChanged += UpdateDisplay;
            UpdateDisplay(trackedHero.Kills, trackedHero.Deaths, trackedHero.Assists);
        }

        private void UpdateDisplay(int kills, int deaths, int assists)
        {
            if (killsText   != null) killsText.text   = kills.ToString();
            if (deathsText  != null) deathsText.text  = deaths.ToString();
            if (assistsText != null) assistsText.text = assists.ToString();

            if (kdaRatioText != null)
            {
                float ratio = deaths > 0 ? (kills + assists) / (float)deaths : kills + assists;
                kdaRatioText.text = $"KDA: {ratio:F2}";
            }
        }

        private void OnDestroy()
        {
            if (trackedHero != null) trackedHero.OnKDAChanged -= UpdateDisplay;
        }
    }
}
