using UnityEngine;

namespace UME.Core
{
    /// <summary>
    /// Global game configuration settings stored as a ScriptableObject.
    /// Assign in the GameManager inspector.
    /// </summary>
    [CreateAssetMenu(fileName = "GameConfig", menuName = "UME/Core/GameConfig")]
    public class GameConfig : ScriptableObject
    {
        [Header("Match Settings")]
        [Tooltip("Maximum number of players per team.")]
        public int playersPerTeam = 5;

        [Tooltip("Maximum match duration in minutes. 0 = no limit.")]
        public float maxMatchDurationMinutes = 45f;

        [Tooltip("Starting gold amount for each player.")]
        public int startingGold = 500;

        [Tooltip("Base gold income per second.")]
        public float goldIncomePerSecond = 1f;

        [Header("Banning Phase")]
        [Tooltip("Number of heroes each team may ban.")]
        public int bansPerTeam = 3;

        [Tooltip("Seconds allowed per ban.")]
        public float banTimeLimitSeconds = 30f;

        [Header("Pick Phase")]
        [Tooltip("Seconds allowed per hero pick.")]
        public float pickTimeLimitSeconds = 30f;

        [Header("Respawn")]
        [Tooltip("Base respawn timer in seconds.")]
        public float baseRespawnTime = 5f;

        [Tooltip("Additional seconds added to respawn per level above 1.")]
        public float respawnTimePerLevel = 2.5f;

        [Header("Map")]
        [Tooltip("Default map scene name loaded for a standard match.")]
        public string defaultMapSceneName = "MainGame";
    }
}
