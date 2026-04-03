using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UME.Core;

namespace UME.UI
{
    /// <summary>
    /// Main navigation menu controller.
    /// Handles button routing to Play, Training, Map Creator, Settings, and Quit.
    /// </summary>
    public class MainMenuUI : MonoBehaviour
    {
        [Header("Buttons")]
        [SerializeField] private Button playButton;
        [SerializeField] private Button trainingButton;
        [SerializeField] private Button mapCreatorButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button quitButton;

        [Header("Version")]
        [SerializeField] private TextMeshProUGUI versionText;

        private void Start()
        {
            if (versionText != null)
            {
                versionText.text = $"v{Application.version}";
            }

            if (playButton       != null) playButton.onClick.AddListener(OnPlay);
            if (trainingButton   != null) trainingButton.onClick.AddListener(OnTraining);
            if (mapCreatorButton != null) mapCreatorButton.onClick.AddListener(OnMapCreator);
            if (quitButton       != null) quitButton.onClick.AddListener(OnQuit);
        }

        private void OnPlay()
        {
            SceneLoader.Instance?.LoadScene(SceneLoader.LobbyScene);
        }

        private void OnTraining()
        {
            SceneLoader.Instance?.LoadScene(SceneLoader.TrainingScene);
        }

        private void OnMapCreator()
        {
            SceneLoader.Instance?.LoadScene(SceneLoader.MapCreatorScene);
        }

        private void OnQuit()
        {
            GameManager.Instance?.QuitGame();
        }
    }
}
