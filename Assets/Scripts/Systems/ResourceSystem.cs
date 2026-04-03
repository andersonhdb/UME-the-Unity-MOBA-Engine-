using System;
using System.Collections.Generic;
using UnityEngine;
using UME.Characters;

namespace UME.Systems
{
    /// <summary>
    /// Manages resource nodes on the map (gold camps, shrines, etc.) and
    /// distributes passive income to all registered heroes.
    /// </summary>
    public class ResourceSystem : MonoBehaviour
    {
        [Header("Passive Income")]
        [Tooltip("Gold awarded to every registered hero per tick.")]
        [SerializeField] private int passiveGoldPerTick = 3;

        [Tooltip("Seconds between each passive income tick.")]
        [SerializeField] private float tickInterval = 3f;

        [Header("Resource Nodes")]
        [SerializeField] private List<ResourceNode> resourceNodes = new List<ResourceNode>();

        private List<HeroController> registeredHeroes = new List<HeroController>();
        private float tickTimer;

        public event Action<HeroController, int> OnResourceCollected;

        private void Update()
        {
            tickTimer -= Time.deltaTime;
            if (tickTimer <= 0f)
            {
                tickTimer = tickInterval;
                DistributePassiveIncome();
            }
        }

        /// <summary>
        /// Registers a hero to receive passive gold income.
        /// </summary>
        public void RegisterHero(HeroController hero)
        {
            if (!registeredHeroes.Contains(hero))
            {
                registeredHeroes.Add(hero);
            }
        }

        /// <summary>
        /// Unregisters a hero from passive income (e.g. on disconnect).
        /// </summary>
        public void UnregisterHero(HeroController hero)
        {
            registeredHeroes.Remove(hero);
        }

        private void DistributePassiveIncome()
        {
            foreach (var hero in registeredHeroes)
            {
                if (hero == null || !hero.IsAlive) continue;
                hero.AddGold(passiveGoldPerTick);
                OnResourceCollected?.Invoke(hero, passiveGoldPerTick);
            }
        }

        /// <summary>
        /// Awards gold to a hero for capturing/clearing a resource node.
        /// </summary>
        public void CollectNode(ResourceNode node, HeroController collector)
        {
            if (node == null || collector == null) return;
            if (!node.IsAvailable) return;

            collector.AddGold(node.goldReward);
            collector.AddExperience(node.experienceReward);
            OnResourceCollected?.Invoke(collector, node.goldReward);
            node.SetCooldown();
        }
    }

    /// <summary>
    /// Represents a single resource node on the map (e.g. a jungle camp).
    /// </summary>
    [Serializable]
    public class ResourceNode
    {
        public string nodeName = "Camp";
        public int goldReward = 100;
        public float experienceReward = 150f;
        [Tooltip("Seconds before this node respawns after being cleared.")]
        public float respawnTime = 60f;

        public bool IsAvailable => cooldownTimer <= 0f;

        private float cooldownTimer;

        public void SetCooldown()
        {
            cooldownTimer = respawnTime;
        }

        public void Tick(float deltaTime)
        {
            if (cooldownTimer > 0f) cooldownTimer -= deltaTime;
        }
    }
}
