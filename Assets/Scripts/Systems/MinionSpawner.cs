using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UME.Characters;

namespace UME.Systems
{
    /// <summary>
    /// Manages minion wave spawning for a single lane.
    /// Waves are sent at a configurable interval and scale in difficulty over time.
    /// </summary>
    public class MinionSpawner : MonoBehaviour
    {
        [Header("Wave Configuration")]
        [SerializeField] private MinionData meleeMinionData;
        [SerializeField] private MinionData rangedMinionData;
        [SerializeField] private MinionData siegeMinionData;

        [SerializeField] private int   meleePerWave  = 3;
        [SerializeField] private int   rangedPerWave = 3;
        [Tooltip("Every N waves a siege minion is added.")]
        [SerializeField] private int   siegeWaveInterval = 3;

        [Header("Timing")]
        [SerializeField] private float firstWaveDelay  = 90f;
        [SerializeField] private float waveCooldown    = 30f;

        [Header("Spawn Points")]
        [SerializeField] private Transform blueSpawnPoint;
        [SerializeField] private Transform redSpawnPoint;
        [SerializeField] private Transform[] blueLanePath;
        [SerializeField] private Transform[] redLanePath;

        [Header("Team")]
        [SerializeField] private TeamID spawnerTeam = TeamID.Blue;

        private int waveCount;
        private bool isActive;

        // Track handles so they can be released when minions are destroyed.
        //private Dictionary<GameObject, AsyncOperationHandle<GameObject>> spawnHandles
        //    = new Dictionary<GameObject, AsyncOperationHandle<GameObject>>();

        private void Start()
        {
            StartCoroutine(WaveLoop());
        }

        public void StopSpawning()
        {
            isActive = false;
            StopAllCoroutines();
        }

        private IEnumerator WaveLoop()
        {
            isActive = true;
            yield return new WaitForSeconds(firstWaveDelay);

            while (isActive)
            {
                waveCount++;
                SpawnWave(waveCount);
                yield return new WaitForSeconds(waveCooldown);
            }
        }

        private void SpawnWave(int wave)
        {
            Transform spawnPoint = spawnerTeam == TeamID.Blue ? blueSpawnPoint : redSpawnPoint;
            Transform[] lanePath = spawnerTeam == TeamID.Blue ? blueLanePath  : redLanePath;

            if (spawnPoint == null) return;

            for (int i = 0; i < meleePerWave; i++)
            {
                SpawnMinion(meleeMinionData, spawnPoint.position + Vector3.right * i * 1.5f, lanePath, wave);
            }

            for (int i = 0; i < rangedPerWave; i++)
            {
                SpawnMinion(rangedMinionData, spawnPoint.position + Vector3.forward * (i + 1) * 1.5f, lanePath, wave);
            }

            if (wave % siegeWaveInterval == 0)
            {
                SpawnMinion(siegeMinionData, spawnPoint.position, lanePath, wave);
            }
        }

        private void SpawnMinion(MinionData data, Vector3 position, Transform[] path, int wave)
        {
            if (data == null) return;

            if (!string.IsNullOrEmpty(data.minionPrefabKey))
            {
                Addressables.InstantiateAsync(data.minionPrefabKey, position, Quaternion.identity).Completed += h =>
                {
                    if (h.Status == AsyncOperationStatus.Succeeded)
                    {
                        GameObject minionObj = h.Result;
                        //spawnHandles[minionObj] = h;
                        InitMinion(minionObj, data, path, wave);
                    }
                    else
                    {
                        Addressables.Release(h);
                    }
                };
            }
        }

        private void InitMinion(GameObject minionObj, MinionData data, Transform[] path, int wave)
        {
            var minion = minionObj.GetComponent<MinionController>();
            if (minion == null) return;

            minion.InitialiseForWave(wave);

            // Release the Addressables handle when the minion is destroyed.
            minion.OnDeath += _ => ReleaseMinion(minionObj);
        }

        private void ReleaseMinion(GameObject minionObj)
        {
            // if (spawnHandles.TryGetValue(minionObj, out var handle))
            // {
            //    spawnHandles.Remove(minionObj);
            //    Addressables.ReleaseInstance(handle);
            // }

        }
    }
}
