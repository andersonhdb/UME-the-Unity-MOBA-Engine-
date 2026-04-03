using Unity.Netcode;
using UnityEngine;
using UME.Core;

namespace UME.Network
{
    /// <summary>
    /// Authoritative networked game state shared to all clients.
    /// Runs on the server; clients read NetworkVariables.
    /// </summary>
    public class NetworkGameState : NetworkBehaviour
    {
        public static NetworkGameState Instance { get; private set; }

        // ── Networked Variables ───────────────────────────────────────────────
        public NetworkVariable<int>   BlueScore      = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
        public NetworkVariable<int>   RedScore       = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
        public NetworkVariable<float> MatchTime      = new NetworkVariable<float>(0f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
        public NetworkVariable<bool>  MatchActive    = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
        public NetworkVariable<int>   CurrentPhaseNet = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        public override void OnNetworkSpawn()
        {
            if (IsServer)
            {
                MatchTime.Value = 0f;
                MatchActive.Value = false;
            }

            MatchActive.OnValueChanged     += OnMatchActiveChanged;
            CurrentPhaseNet.OnValueChanged += OnPhaseChanged;
        }

        public override void OnNetworkDespawn()
        {
            MatchActive.OnValueChanged     -= OnMatchActiveChanged;
            CurrentPhaseNet.OnValueChanged -= OnPhaseChanged;
        }

        // ── Server Methods ─────────────────────────────────────────────────────

        [ServerRpc(RequireOwnership = false)]
        public void StartMatchServerRpc()
        {
            MatchActive.Value = true;
            MatchTime.Value   = 0f;
        }

        [ServerRpc(RequireOwnership = false)]
        public void EndMatchServerRpc()
        {
            MatchActive.Value = false;
        }

        [ServerRpc(RequireOwnership = false)]
        public void AddScoreServerRpc(int teamId, int amount)
        {
            if (teamId == 1) BlueScore.Value += amount;
            else if (teamId == 2) RedScore.Value += amount;
        }

        [ServerRpc(RequireOwnership = false)]
        public void SetPhaseServerRpc(int phase)
        {
            CurrentPhaseNet.Value = phase;
        }

        private void Update()
        {
            if (!IsServer || !MatchActive.Value) return;
            MatchTime.Value += Time.deltaTime;
        }

        // ── Client Callbacks ──────────────────────────────────────────────────

        private void OnMatchActiveChanged(bool previous, bool current)
        {
            if (current)
                GameManager.Instance?.ChangePhase(GamePhase.InGame);
            else
                GameManager.Instance?.ChangePhase(GamePhase.PostGame);
        }

        private void OnPhaseChanged(int previous, int current)
        {
            GameManager.Instance?.ChangePhase((GamePhase)current);
        }
    }
}
