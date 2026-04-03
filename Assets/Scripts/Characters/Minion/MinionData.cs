using UnityEngine;

namespace UME.Characters
{
    /// <summary>
    /// ScriptableObject that defines all customizable properties for a minion wave.
    /// Create via Assets > Create > UME > Minion > MinionData.
    /// </summary>
    [CreateAssetMenu(fileName = "NewMinionData", menuName = "UME/Minion/MinionData")]
    public class MinionData : ScriptableObject
    {
        [Header("Identity")]
        public string minionName = "Minion";
        public MinionType minionType = MinionType.Melee;

        [Header("Stats")]
        public UnitStats baseStats = UnitStats.Default;

        [Tooltip("Gold rewarded to the killing player.")]
        public int goldReward = 20;

        [Tooltip("Experience granted to nearby enemies on death.")]
        public float experienceReward = 50f;

        [Header("Wave Scaling")]
        [Tooltip("Additive stat increase applied per wave number.")]
        public UnitStats statsPerWave;

        [Header("Behaviour")]
        [Tooltip("Attack range in world units.")]
        public float attackRange = 2f;

        [Tooltip("Seconds between each attack.")]
        public float attackCooldown = 1.5f;

        [Tooltip("Sight range before the minion acquires a target.")]
        public float detectionRange = 8f;

        [Header("Addressables")]
        [Tooltip("Addressable key for the minion prefab.")]
        public string minionPrefabKey = "";
    }

    public enum MinionType
    {
        Melee,
        Ranged,
        Siege,
        Super
    }
}
