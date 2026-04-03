using Unity.Netcode;
using UnityEngine;

namespace UME.Network
{
    /// <summary>
    /// Represents a connected player's networked identity and chosen hero.
    /// Spawned by the server for each connecting client.
    /// </summary>
    public class NetworkPlayerData : NetworkBehaviour
    {
        public NetworkVariable<int>    TeamId        = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<int>    SelectedHeroIndex = new NetworkVariable<int>(-1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<bool>   IsReady       = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<FixedString64Bytes> PlayerName = new NetworkVariable<FixedString64Bytes>("", NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        // ── KDA (server authoritative) ────────────────────────────────────────
        public NetworkVariable<int> Kills   = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
        public NetworkVariable<int> Deaths  = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
        public NetworkVariable<int> Assists = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

        public override void OnNetworkSpawn()
        {
            if (IsOwner)
            {
                PlayerName.Value = $"Player_{OwnerClientId}";
            }
        }

        [ServerRpc]
        public void SetReadyServerRpc(bool ready)
        {
            IsReady.Value = ready;
        }

        [ServerRpc]
        public void SelectHeroServerRpc(int heroIndex)
        {
            SelectedHeroIndex.Value = heroIndex;
        }

        [ServerRpc]
        public void SetTeamServerRpc(int teamId)
        {
            TeamId.Value = teamId;
        }

        [ServerRpc(RequireOwnership = false)]
        public void RecordKillServerRpc()
        {
            Kills.Value++;
        }

        [ServerRpc(RequireOwnership = false)]
        public void RecordDeathServerRpc()
        {
            Deaths.Value++;
        }

        [ServerRpc(RequireOwnership = false)]
        public void RecordAssistServerRpc()
        {
            Assists.Value++;
        }
    }
}
