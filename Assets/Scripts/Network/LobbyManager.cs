using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobby;
using Unity.Services.Lobby.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

namespace UME.Network
{
    /// <summary>
    /// Manages UGS Lobby + Relay sessions for pre-game matchmaking.
    /// Call InitializeAsync() first, then either CreateLobbyAsync or JoinLobbyAsync.
    /// </summary>
    public class LobbyManager : MonoBehaviour
    {
        public static LobbyManager Instance { get; private set; }

        private const string KeyRelayCode   = "RelayCode";
        private const string KeyGameVersion = "GameVersion";

        [SerializeField] private int maxPlayers = 10;
        [SerializeField] private string gameVersion = "1.0";

        public Lobby CurrentLobby { get; private set; }
        public bool IsHost { get; private set; }
        public string LocalPlayerId { get; private set; }

        public event Action<Lobby>           OnLobbyUpdated;
        public event Action<string>          OnJoinedLobby;
        public event Action                  OnLeftLobby;
        public event Action<string>          OnError;

        private float heartbeatTimer;
        private const float HeartbeatInterval = 15f;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Update()
        {
            if (IsHost && CurrentLobby != null)
            {
                heartbeatTimer -= Time.deltaTime;
                if (heartbeatTimer <= 0f)
                {
                    heartbeatTimer = HeartbeatInterval;
                    _ = SendHeartbeatAsync();
                }
            }
        }

        /// <summary>
        /// Initialises Unity Game Services and signs in anonymously.
        /// Must be awaited before any other lobby operation.
        /// </summary>
        public async Task InitializeAsync()
        {
            await UnityServices.InitializeAsync();
            if (!AuthenticationService.Instance.IsSignedIn)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }
            LocalPlayerId = AuthenticationService.Instance.PlayerId;
        }

        /// <summary>
        /// Creates a new lobby and allocates a Relay server for the session.
        /// </summary>
        public async Task<string> CreateLobbyAsync(string lobbyName, bool isPrivate = false)
        {
            try
            {
                // Relay accepts maxConnections = total players minus the host (host uses a local connection slot).
                Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxPlayers - 1);
                string relayCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

                CreateLobbyOptions options = new CreateLobbyOptions
                {
                    IsPrivate = isPrivate,
                    Data = new Dictionary<string, DataObject>
                    {
                        { KeyRelayCode,   new DataObject(DataObject.VisibilityOptions.Member, relayCode) },
                        { KeyGameVersion, new DataObject(DataObject.VisibilityOptions.Public, gameVersion) }
                    }
                };

                CurrentLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, options);
                IsHost = true;
                heartbeatTimer = HeartbeatInterval;

                NetworkManager.Singleton.StartHost();
                OnJoinedLobby?.Invoke(CurrentLobby.Id);
                return CurrentLobby.LobbyCode;
            }
            catch (Exception e)
            {
                OnError?.Invoke(e.Message);
                return null;
            }
        }

        /// <summary>
        /// Joins an existing lobby using a lobby code.
        /// </summary>
        public async Task JoinLobbyAsync(string lobbyCode)
        {
            try
            {
                JoinLobbyByCodeOptions options = new JoinLobbyByCodeOptions();
                CurrentLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode, options);

                string relayCode = CurrentLobby.Data[KeyRelayCode].Value;
                JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(relayCode);

                IsHost = false;
                NetworkManager.Singleton.StartClient();
                OnJoinedLobby?.Invoke(CurrentLobby.Id);
            }
            catch (Exception e)
            {
                OnError?.Invoke(e.Message);
            }
        }

        /// <summary>
        /// Leaves the current lobby and disconnects from the network.
        /// </summary>
        public async Task LeaveLobbyAsync()
        {
            if (CurrentLobby == null) return;

            try
            {
                await LobbyService.Instance.RemovePlayerAsync(CurrentLobby.Id, LocalPlayerId);
                NetworkManager.Singleton.Shutdown();
                CurrentLobby = null;
                IsHost = false;
                OnLeftLobby?.Invoke();
            }
            catch (Exception e)
            {
                OnError?.Invoke(e.Message);
            }
        }

        private async Task SendHeartbeatAsync()
        {
            if (CurrentLobby == null) return;
            await LobbyService.Instance.SendHeartbeatPingAsync(CurrentLobby.Id);
        }
    }
}
