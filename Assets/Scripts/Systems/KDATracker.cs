using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Multiplayer;
using UME.Characters;

namespace UME.Systems
{
    /// <summary>
    /// Aggregates and broadcasts KDA statistics for all players in the match.
    /// Runs server-side; clients subscribe to NetworkPlayerData changes.
    /// </summary>
    public class KDATracker : MonoBehaviour
    {
        public static KDATracker Instance { get; private set; }

        private Dictionary<ulong, KDARecord> records = new Dictionary<ulong, KDARecord>();

        public event Action<ulong, KDARecord> OnRecordUpdated;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        public void RegisterPlayer(ulong clientId)
        {
            if (!records.ContainsKey(clientId))
            {
                records[clientId] = new KDARecord();
            }
        }

        public void RecordKill(ulong killerId, ulong victimId)
        {
            if (!records.ContainsKey(killerId)) RegisterPlayer(killerId);
            if (!records.ContainsKey(victimId)) RegisterPlayer(victimId);

            records[killerId].Kills++;
            records[victimId].Deaths++;

            OnRecordUpdated?.Invoke(killerId, records[killerId]);
            OnRecordUpdated?.Invoke(victimId, records[victimId]);
        }

        public void RecordAssist(ulong assisterId)
        {
            if (!records.ContainsKey(assisterId)) RegisterPlayer(assisterId);
            records[assisterId].Assists++;
            OnRecordUpdated?.Invoke(assisterId, records[assisterId]);
        }

        public KDARecord GetRecord(ulong clientId)
        {
            return records.TryGetValue(clientId, out var record) ? record : new KDARecord();
        }

        public IReadOnlyDictionary<ulong, KDARecord> GetAllRecords() => records;
    }

    [Serializable]
    public class KDARecord
    {
        public int Kills;
        public int Deaths;
        public int Assists;

        public float KDAValue => Deaths > 0 ? (Kills + Assists) / (float)Deaths : Kills + Assists;
        public override string ToString() => $"{Kills}/{Deaths}/{Assists}";
    }
}
