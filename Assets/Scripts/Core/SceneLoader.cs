using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UME.Core
{
    /// <summary>
    /// Handles asynchronous scene loading with optional loading screen support.
    /// </summary>
    public class SceneLoader : MonoBehaviour
    {
        public static SceneLoader Instance { get; private set; }

        [SerializeField] private string loadingSceneName = "LoadingScreen";

        public static readonly string LaunchScene      = "LaunchScreen";
        public static readonly string MainMenuScene    = "MainNavigation";
        public static readonly string LobbyScene       = "Lobby";
        public static readonly string GameConfigScene  = "GameConfig";
        public static readonly string MainGameScene    = "MainGame";
        public static readonly string TrainingScene    = "Training";
        public static readonly string MapCreatorScene  = "MapCreator";

        public bool IsLoading { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        /// <summary>
        /// Loads a scene by name asynchronously.
        /// </summary>
        public void LoadScene(string sceneName)
        {
            if (IsLoading) return;
            StartCoroutine(LoadSceneAsync(sceneName));
        }

        private IEnumerator LoadSceneAsync(string sceneName)
        {
            IsLoading = true;

            AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
            if (operation == null)
            {
                Debug.LogError($"[SceneLoader] Scene '{sceneName}' not found in build settings.");
                IsLoading = false;
                yield break;
            }

            operation.allowSceneActivation = false;

            while (!operation.isDone)
            {
                if (operation.progress >= 0.9f)
                {
                    operation.allowSceneActivation = true;
                }
                yield return null;
            }

            IsLoading = false;
        }
    }
}
