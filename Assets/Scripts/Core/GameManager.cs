using System;
using UnityEngine;

namespace UME.Core
{
    /// <summary>
    /// Central game manager that controls overall game state and lifecycle.
    /// Implemented as a persistent singleton accessible from any scene.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("Game Configuration")]
        [SerializeField] private GameConfig gameConfig;

        public GameConfig GameConfig => gameConfig;
        public GamePhase CurrentPhase { get; private set; } = GamePhase.None;

        public event Action<GamePhase> OnPhaseChanged;
        public event Action OnGameStarted;
        public event Action OnGameEnded;

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

        private void Start()
        {
            ChangePhase(GamePhase.Launch);
        }

        /// <summary>
        /// Transitions the game to a new phase, firing the OnPhaseChanged event.
        /// </summary>
        public void ChangePhase(GamePhase newPhase)
        {
            CurrentPhase = newPhase;
            OnPhaseChanged?.Invoke(newPhase);

            switch (newPhase)
            {
                case GamePhase.InGame:
                    OnGameStarted?.Invoke();
                    break;
                case GamePhase.PostGame:
                    OnGameEnded?.Invoke();
                    break;
            }
        }

        /// <summary>
        /// Quits the application. Safe to call from any context.
        /// </summary>
        public void QuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
