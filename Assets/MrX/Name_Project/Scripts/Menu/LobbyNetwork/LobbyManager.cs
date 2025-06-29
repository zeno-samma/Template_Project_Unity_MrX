using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay; // Cần thiết cho Relay
using Unity.Services.Relay.Models; // Cần thiết cho các model của Relay
using Unity.Netcode; // Cần thiết cho NetworkManager
using Unity.Netcode.Transports.UTP; // Cần thiết cho UnityTransport
using UnityEngine;
using UnityEngine.SceneManagement; // Cần thiết cho LoadSceneMode
using Unity.Services.Lobbies.Models;
using Unity.Services.Lobbies;
using Unity.Collections;

namespace MrX.Name_Project
{


    public class LobbyManager : MonoBehaviour
    {
        // Sử dụng Singleton Pattern để dễ dàng truy cập từ bất cứ đâu
        public static LobbyManager Instance { get; private set; }

        // Biến lưu trữ Lobby hiện tại mà người chơi đã tham gia
        public Lobby JoinedLobby { get; private set; }
        // Kênh thông báo để UI lắng nghe
        public event Action<Lobby> OnLobbyUpdated;
        // Sự kiện để thông báo cho các phần khác của game khi có lỗi
        public event Action<string> OnLobbyError;

        private float _heartbeatTimer;

        private bool _isPlayerReady = false;
        private float _lobbyUpdateTimer;
        private bool _isKickingPlayer = false; // Thêm biến để tránh xung đột khi kick

        private NetworkVariable<FixedString32Bytes> networkPlayerName = new NetworkVariable<FixedString32Bytes>("Player");

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void Update()
        {
            HandleLobbyHeartbeat();
            HandleLobbyPolling(); // <<-- DÒNG QUAN TRỌNG ĐÃ BỊ THIẾU
        }
        private async void HandleLobbyHeartbeat()
        {
            if (IsLobbyHost() && JoinedLobby != null)
            {
                _heartbeatTimer -= Time.deltaTime;
                if (_heartbeatTimer <= 0f)
                {
                    _heartbeatTimer = 15f;// Dùng try-catch để đảm bảo an toàn
                    try { await LobbyService.Instance.SendHeartbeatPingAsync(JoinedLobby.Id); }
                    catch (LobbyServiceException e) { Debug.Log(e); }
                }
            }
        }
        // Hàm kiểm tra xem người chơi hiện tại có phải là chủ phòng không
        public bool IsLobbyHost()
        {
            return JoinedLobby != null && JoinedLobby.HostId == AuthenticationService.Instance.PlayerId;
        }
        // Hàm tiện ích để tạo đối tượng Player với các dữ liệu cần thiết
        private Player GetNewPlayer()
        {
            return new Player
            {
                Data = new Dictionary<string, PlayerDataObject>
        {
            // Key phải khớp với key bạn dùng để đọc ở LobbyPlayerItem
            { "PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, AuthenticationService.Instance.PlayerName) },
            { "PlayerReady", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, "0") } // Mặc định là "Chưa sẵn sàng"
        }
            };
        }
        // Hàm tạo Lobby và Relay cho Host
        public async Task CreateLobby(string lobbyName, int maxPlayers, bool isPrivate)
        {
            try
            {
                // === Bước 1: Tạo suất kết nối Relay (Allocation) cho Host ===
                Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxPlayers - 1); // Trừ 1 vì Host không cần slot

                // === Bước 2: Lấy mã tham gia (Join Code) từ Allocation ===
                string relayJoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
                // LẤY THÔNG TIN PLAYER MỚI VỚI ĐẦY ĐỦ DATA
                var player = GetNewPlayer();
                // === Bước 3: Tạo Lobby và nhúng Join Code vào dữ liệu của Lobby ===
                var createLobbyOptions = new CreateLobbyOptions
                {
                    IsPrivate = isPrivate,
                    Player = player, // GÁN THÔNG TIN PLAYER VÀO ĐÂY
                    // Lưu Join Code vào một key tên là "RelayJoinCode"
                    Data = new Dictionary<string, DataObject>
                {
                    { "RelayJoinCode", new DataObject(DataObject.VisibilityOptions.Member, relayJoinCode) }
                }
                };

                Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, createLobbyOptions);
                JoinedLobby = lobby;

                // === Bước 4: Cấu hình UnityTransport và khởi động Netcode ở chế độ Host ===
                // Gán thông tin Allocation vào UnityTransport của NetworkManager
                NetworkManager.Singleton.GetComponent<UnityTransport>().SetHostRelayData(
                    allocation.RelayServer.IpV4,
                    (ushort)allocation.RelayServer.Port,
                    allocation.AllocationIdBytes,
                    allocation.Key,
                    allocation.ConnectionData
                );

                // Khởi động Host
                NetworkManager.Singleton.StartHost();
                Debug.Log($"Created and hosted lobby '{lobby.Name}' with code '{lobby.LobbyCode}' and Relay code '{relayJoinCode}'");

            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to create lobby: {e.Message}");
                OnLobbyError?.Invoke("Failed to create lobby.");
            }

        }
        private async void HandleLobbyPolling()
        {
            // Chỉ polling khi đã ở trong lobby và không phải là host (host đã có heartbeat)
            // Hoặc có thể để cả host polling cũng không sao, tùy thiết kế. Ở đây ta để cả 2.
            if (JoinedLobby != null)
            {
                _lobbyUpdateTimer -= Time.deltaTime;
                if (_lobbyUpdateTimer <= 0f)
                {
                    _lobbyUpdateTimer = 1.1f;
                    try
                    {
                        Lobby lobby = await LobbyService.Instance.GetLobbyAsync(JoinedLobby.Id);

                        // Kiểm tra xem người chơi hiện tại có còn trong lobby không
                        bool playerStillInLobby = false;
                        foreach (var player in lobby.Players)
                        {
                            if (player.Id == AuthenticationService.Instance.PlayerId)
                            {
                                playerStillInLobby = true;
                                break;
                            }
                        }

                        // Chỉ cập nhật nếu người chơi vẫn còn trong lobby
                        if (playerStillInLobby)
                        {
                            JoinedLobby = lobby;
                            OnLobbyUpdated?.Invoke(JoinedLobby);
                        }
                        else
                        {
                            // Người chơi đã bị kick hoặc rời khỏi lobby
                            Debug.Log("Player was removed from lobby");
                            CleanupAndShutdown();
                        }
                    }
                    catch (LobbyServiceException e)
                    {
                        Debug.Log($"Lobby polling error: {e.Message} - Reason: {e.Reason}");

                        // Chỉ shutdown khi có lỗi nghiêm trọng
                        switch (e.Reason)
                        {
                            case LobbyExceptionReason.LobbyNotFound:
                            case LobbyExceptionReason.Unauthorized:
                                Debug.LogWarning($"Critical lobby error detected: {e.Reason}. Shutting down...");
                                CleanupAndShutdown();
                                break;

                            case LobbyExceptionReason.LobbyFull:
                            case LobbyExceptionReason.PlayerNotFound:
                                // Các lỗi này có thể xảy ra khi có người join/leave, không cần shutdown
                                Debug.Log($"Non-critical lobby error: {e.Reason}");
                                break;

                            default:
                                // Các lỗi khác, log nhưng không shutdown ngay
                                Debug.LogWarning($"Unknown lobby error: {e.Reason}. Continuing...");
                                break;
                        }
                    }
                }
            }
        }
        // --- HÀM KICK ĐÃ ĐƯỢC CẬP NHẬT ---
        public async Task KickPlayer(string playerIdToKick)
        {
            if (!IsLobbyHost())
            {
                Debug.LogWarning("Chỉ Host mới có quyền kick người chơi!");
                return; // Chỉ Host mới có quyền kick
            }

            if (playerIdToKick == AuthenticationService.Instance.PlayerId)
            {
                Debug.LogWarning("Host không thể tự kick mình!");
                return; // NGĂN không cho host tự kick mình
            }

            if (_isKickingPlayer)
            {
                Debug.LogWarning("Đang trong quá trình kick, vui lòng chờ...");
                return; // Tránh kick nhiều lần cùng lúc
            }

            _isKickingPlayer = true;

            try
            {
                Debug.Log($"Đang kick player {playerIdToKick}...");

                // Gửi yêu cầu kick người chơi lên server
                await LobbyService.Instance.RemovePlayerAsync(JoinedLobby.Id, playerIdToKick);

                // SAU KHI KICK THÀNH CÔNG:
                // Chủ động lấy lại dữ liệu lobby mới nhất ngay lập tức thay vì chờ polling.
                // Điều này đảm bảo UI của Host được cập nhật ngay lập tức.
                JoinedLobby = await LobbyService.Instance.GetLobbyAsync(JoinedLobby.Id);
                OnLobbyUpdated?.Invoke(JoinedLobby); // Phát tín hiệu để UI vẽ lại ngay

                Debug.Log($"✅ Successfully kicked player {playerIdToKick}");
            }
            catch (LobbyServiceException e)
            {
                Debug.LogError($"❌ Failed to kick player {playerIdToKick}: {e.Message} - Reason: {e.Reason}");

                // Phân loại lỗi để xử lý phù hợp
                switch (e.Reason)
                {
                    case LobbyExceptionReason.PlayerNotFound:
                        Debug.LogWarning($"Player {playerIdToKick} not found in lobby (may have already left)");
                        break;

                    case LobbyExceptionReason.Unauthorized:
                        Debug.LogError("Unauthorized to kick player - check host permissions");
                        break;

                    case LobbyExceptionReason.LobbyNotFound:
                        Debug.LogError("Lobby no longer exists");
                        CleanupAndShutdown();
                        break;

                    default:
                        Debug.LogError($"Unknown error while kicking player: {e.Reason}");
                        break;
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"❌ Unexpected error while kicking player {playerIdToKick}: {e.Message}");
            }
            finally
            {
                _isKickingPlayer = false; // Luôn reset trạng thái
            }
        }
        // Hàm tham gia Lobby và kết nối Relay cho Client
        public async Task JoinLobbyByCode(string lobbyCode)
        {
            try
            {
                // === Bước 1: Tham gia vào Lobby bằng mã code ===
                // LẤY THÔNG TIN PLAYER MỚI VỚI ĐẦY ĐỦ DATA
                var player = GetNewPlayer();
                // var joinOptions = new JoinLobbyByCodeOptions { Player = player };
                JoinLobbyByCodeOptions joinOptions = new JoinLobbyByCodeOptions { Player = player };
                Lobby lobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode, joinOptions);
                JoinedLobby = lobby;

                // === Bước 2: Lấy Join Code của Relay từ dữ liệu của Lobby ===
                string relayJoinCode = lobby.Data["RelayJoinCode"].Value;

                // === Bước 3: Dùng Join Code để tham gia vào suất kết nối Relay ===
                JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(relayJoinCode);

                // === Bước 4: Cấu hình UnityTransport và khởi động Netcode ở chế độ Client ===
                NetworkManager.Singleton.GetComponent<UnityTransport>().SetClientRelayData(
                    joinAllocation.RelayServer.IpV4,
                    (ushort)joinAllocation.RelayServer.Port,
                    joinAllocation.AllocationIdBytes,
                    joinAllocation.Key,
                    joinAllocation.ConnectionData,
                    joinAllocation.HostConnectionData
                );

                // Khởi động Client
                NetworkManager.Singleton.StartClient();
                Debug.Log($"✅ Successfully joined lobby with code '{lobbyCode}'");

            }
            catch (LobbyServiceException e) when (e.Reason == LobbyExceptionReason.LobbyFull)
            {
                // BẮT LỖI CỤ THỂ: Phòng đã đầy (Lobby is full)
                Debug.LogWarning("❌ Failed to join lobby: Lobby is full.");

                // Sử dụng ErrorMenu để hiển thị thông báo cho người chơi
                var errorPanel = PanelManager.Get("error") as ErrorMenu;
                if (errorPanel != null)
                {
                    errorPanel.Open(ErrorMenu.Action.None, "Lobby is full!", "OK");
                }
            }
            catch (LobbyServiceException e) when (e.Reason == LobbyExceptionReason.LobbyNotFound)
            {
                // BẮT LỖI CỤ THỂ: Không tìm thấy phòng (Lobby not found)
                Debug.LogWarning("❌ Failed to join lobby: Lobby not found (Invalid code).");

                var errorPanel = PanelManager.Get("error") as ErrorMenu;
                if (errorPanel != null)
                {
                    errorPanel.Open(ErrorMenu.Action.None, "Lobby not found. Please check the code.", "OK");
                }
            }
            catch (Exception e)
            {
                // Bắt tất cả các lỗi còn lại (mất mạng, lỗi không xác định...)
                Debug.LogError($"❌ Failed to join lobby for an unknown reason: {e.Message}");

                var errorPanel = PanelManager.Get("error") as ErrorMenu;
                if (errorPanel != null)
                {
                    errorPanel.Open(ErrorMenu.Action.None, "Failed to join lobby.", "OK");
                }
            }
            finally
            {
                // (Nên có) Luôn đảm bảo đóng panel loading nếu có
                // PanelManager.Close("loading");
            }
        }


        // Hàm để rời khỏi Lobby
        public async Task LeaveLobby()
        {
            if (JoinedLobby != null)
            {
                try
                {
                    string playerId = AuthenticationService.Instance.PlayerId;
                    await LobbyService.Instance.RemovePlayerAsync(JoinedLobby.Id, playerId);
                }
                catch (LobbyServiceException e)
                {
                    Debug.Log(e);
                }
                finally
                {
                    // Dọn dẹp trạng thái cục bộ sau khi đã gửi yêu cầu
                    CleanupAndShutdown();
                }
            }
        }
        // --- HÀM DỌN DẸP AN TOÀN ĐƯỢC TÁCH RIÊNG ---
        private void CleanupAndShutdown()
        {
            if (JoinedLobby != null)
            {
                Debug.Log("Cleaning up lobby and shutting down network...");
                JoinedLobby = null;

                // Luôn kiểm tra NetworkManager và trạng thái kết nối trước khi Shutdown
                if (NetworkManager.Singleton != null && (NetworkManager.Singleton.IsClient || NetworkManager.Singleton.IsHost))
                {
                    Debug.Log("Shutting down NetworkManager...");
                    NetworkManager.Singleton.Shutdown();
                }

                // Phát tín hiệu null để UI biết cần quay về màn hình kết nối
                OnLobbyUpdated?.Invoke(null);
            }
        }

        // Hàm mới để người chơi thay đổi trạng thái sẵn sàng
        public async Task SetPlayerReady()
        {
            _isPlayerReady = !_isPlayerReady;
            string readyStatus = _isPlayerReady ? "1" : "0";

            try
            {
                var options = new UpdatePlayerOptions
                {
                    Data = new Dictionary<string, PlayerDataObject>
                    {
                        { "PlayerReady", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, readyStatus) }
                    }
                };
                // Gửi yêu cầu cập nhật trạng thái của chính mình lên server
                await LobbyService.Instance.UpdatePlayerAsync(JoinedLobby.Id, AuthenticationService.Instance.PlayerId, options);
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }

        // Hàm mới để Host bắt đầu game
        public async Task StartGame()
        {
            if (!IsLobbyHost())
            {
                Debug.LogWarning("Chỉ Host mới có thể bắt đầu game!");
                return;
            }

            try
            {
                Debug.Log("Host đang bắt đầu game...");

                // Cập nhật trạng thái lobby để báo hiệu game đã bắt đầu
                var updateLobbyOptions = new UpdateLobbyOptions
                {
                    Data = new Dictionary<string, DataObject>
                    {
                        { "GameStarted", new DataObject(DataObject.VisibilityOptions.Member, "1") }
                    }
                };

                await LobbyService.Instance.UpdateLobbyAsync(JoinedLobby.Id, updateLobbyOptions);

                // Lấy thông tin lobby mới nhất
                JoinedLobby = await LobbyService.Instance.GetLobbyAsync(JoinedLobby.Id);
                OnLobbyUpdated?.Invoke(JoinedLobby);

                // Bắt đầu game scene
                StartGameScene();

                Debug.Log("✅ Game đã được bắt đầu!");
            }
            catch (LobbyServiceException e)
            {
                Debug.LogError($"❌ Failed to start game: {e.Message}");
            }
        }

        // Hàm để bắt đầu scene game
        private void StartGameScene()
        {
            // Tạo NetworkSceneManager để load scene
            var sceneManager = NetworkManager.Singleton.SceneManager;

            // Load scene game (bạn cần tạo scene này)
            sceneManager.LoadScene("GameScene", LoadSceneMode.Single);
        }

        // Hàm kiểm tra xem game đã bắt đầu chưa
        public bool IsGameStarted()
        {
            return JoinedLobby != null &&
                   JoinedLobby.Data.ContainsKey("GameStarted") &&
                   JoinedLobby.Data["GameStarted"].Value == "1";
        }

        // Hàm kiểm tra xem tất cả người chơi đã sẵn sàng chưa
        public bool AreAllPlayersReady()
        {
            if (JoinedLobby == null) return false;

            foreach (var player in JoinedLobby.Players)
            {
                // Bỏ qua host
                if (player.Id == JoinedLobby.HostId) continue;
                if (player.Data.ContainsKey("PlayerReady") && player.Data["PlayerReady"].Value != "1")
                {
                    return false;
                }
            }
            return true;
        }
    }
}
