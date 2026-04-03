using UnityEngine;
using UnityEngine.AI;

namespace UME.Characters
{
    /// <summary>
    /// AI controller for minion units.
    /// Minions follow a lane path, attack enemies in range, and reward the killer.
    /// Requires a NavMeshAgent component.
    /// </summary>
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(Collider))]
    public class MinionController : UnitBase
    {
        [Header("Minion Setup")]
        [SerializeField] private MinionData minionData;
        [SerializeField] private Transform[] lanePath;

        private NavMeshAgent agent;
        private UnitBase currentTarget;
        private int waypointIndex;
        private float attackTimer;
        private int waveNumber;

        public MinionData MinionData => minionData;

        protected override void Awake()
        {
            if (minionData != null)
            {
                unitName   = minionData.minionName;
                baseStats  = minionData.baseStats;
            }
            base.Awake();
        }

        private void Start()
        {
            agent = GetComponent<NavMeshAgent>();
            if (minionData != null)
            {
                agent.speed = currentStats.moveSpeed;
            }
        }

        private void Update()
        {
            if (!IsAlive) return;

            attackTimer -= Time.deltaTime;

            if (currentTarget != null && currentTarget.IsAlive)
            {
                float distance = Vector3.Distance(transform.position, currentTarget.transform.position);
                if (distance <= minionData.attackRange)
                {
                    agent.ResetPath();
                    TryAttack();
                    return;
                }
                agent.SetDestination(currentTarget.transform.position);
                return;
            }

            currentTarget = FindNearestEnemy();
            if (currentTarget == null)
            {
                MoveAlongLane();
            }
        }

        private void MoveAlongLane()
        {
            if (lanePath == null || lanePath.Length == 0) return;

            if (!agent.hasPath || agent.remainingDistance < 0.5f)
            {
                waypointIndex = (waypointIndex + 1) % lanePath.Length;
                agent.SetDestination(lanePath[waypointIndex].position);
            }
        }

        private UnitBase FindNearestEnemy()
        {
            if (minionData == null) return null;

            Collider[] hits = Physics.OverlapSphere(transform.position, minionData.detectionRange);
            UnitBase nearest = null;
            float nearestDist = float.MaxValue;

            foreach (var hit in hits)
            {
                var unit = hit.GetComponent<UnitBase>();
                if (unit == null || !unit.IsAlive || unit.Team == team) continue;

                float dist = Vector3.Distance(transform.position, unit.transform.position);
                if (dist < nearestDist)
                {
                    nearestDist = dist;
                    nearest = unit;
                }
            }
            return nearest;
        }

        private void TryAttack()
        {
            if (attackTimer > 0f || currentTarget == null) return;
            currentTarget.TakeDamage(currentStats.attackDamage, this);
            attackTimer = minionData.attackCooldown;
        }

        /// <summary>
        /// Scales minion stats based on the current wave number for difficulty progression.
        /// </summary>
        public void InitialiseForWave(int wave)
        {
            waveNumber = wave;
            if (minionData == null) return;

            UnitStats scaled = minionData.baseStats;
            UnitStats growth = minionData.statsPerWave;
            scaled.maxHealth    += growth.maxHealth    * (wave - 1);
            scaled.armor        += growth.armor        * (wave - 1);
            scaled.attackDamage += growth.attackDamage * (wave - 1);
            currentStats = scaled;
            CurrentHealth = currentStats.maxHealth;
        }

        protected override void Die(UnitBase killer)
        {
            if (killer != null && minionData != null)
            {
                if (killer is HeroController hero)
                {
                    hero.AddGold(minionData.goldReward);
                    hero.AddExperience(minionData.experienceReward);
                }
            }
            base.Die(killer);
            gameObject.SetActive(false);
        }

        private void OnDrawGizmosSelected()
        {
            if (minionData == null) return;
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, minionData.attackRange);
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, minionData.detectionRange);
        }
    }
}
