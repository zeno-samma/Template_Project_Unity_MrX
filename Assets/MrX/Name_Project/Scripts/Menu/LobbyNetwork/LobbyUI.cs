using System.Collections.Generic;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

namespace MrX.Name_Project
{
    public class LobbyUI : Panel
    {
        [Header("Connect Panel")]
        [SerializeField] private GameObject connectPanel;
        [SerializeField] private TMP_InputField lobbyNameInput;
        [SerializeField] private TMP_InputField lobbyCodeInput;
        [SerializeField] private Button createLobbyButton;
        [SerializeField] private Button joinLobbyButton;
        [SerializeField] private Button closeButton;

        [Header("Lobby Panel")]
        [SerializeField] private GameObject lobbyPanel;
        [SerializeField] private TextMeshProUGUI lobbyNameText;
        [SerializeField] private TextMeshProUGUI lobbyCodeText;
        [SerializeField] private Button leaveLobbyButton;
        [SerializeField] private Button startGameButton;

        [Header("Player List")]
        [SerializeField] private RectTransform playerListContainer;
        [SerializeField] private LobbyPlayerItem playerItemPrefab;

        // MỚI: Dictionary để quản lý các item đang hiển thị, giúp truy cập nhanh
        private Dictionary<string, LobbyPlayerItem> _playerListItems = new Dictionary<string, LobbyPlayerItem>();
        // MỚI: Danh sách cache để tái sử dụng các item đã bị ẩn
        private List<LobbyPlayerItem> _cachedPlayerListItems = new List<LobbyPlayerItem>();

        private void Start()
        {
            
            // Gán sự kiện cho các nút bấm
            createLobbyButton.onClick.AddListener(OnCreateLobbyClicked);
            joinLobbyButton.onClick.AddListener(OnJoinLobbyClicked);
            leaveLobbyButton.onClick.AddListener(OnLeaveLobbyClicked);
            closeButton.onClick.AddListener(ClosePanel);
            startGameButton.onClick.AddListener(OnStartGameClicked);

            // --- PHẦN QUAN TRỌNG: ĐĂNG KÝ LẮNG NGHE SỰ KIỆN ---
            LobbyManager.Instance.OnLobbyUpdated += UpdateLobbyVisuals;

            // Ban đầu, chỉ hiển thị panel kết nối
            connectPanel.SetActive(true);
            lobbyPanel.SetActive(false);
            // Xóa hàm đăng ký sự kiện cũ, chúng ta sẽ cập nhật UI một cách chủ động
            // UpdateVisuals();
        }
        private void OnDestroy()
        {
            // Luôn hủy đăng ký sự kiện khi đối tượng bị phá hủy
            if (LobbyManager.Instance != null)
            {
                LobbyManager.Instance.OnLobbyUpdated -= UpdateLobbyVisuals;
            }
        }

        // Hàm này giờ đây được gọi bởi SỰ KIỆN từ LobbyManager
        private void UpdateLobbyVisuals(Lobby lobby)
        {
            UpdateLobbyUIVisibility();

            if (lobby == null) return;

            // Dọn dẹp danh sách cũ
            foreach (Transform child in playerListContainer) { Destroy(child.gameObject); }
            _playerListItems.Clear();

            // Vẽ lại danh sách người chơi mới
            foreach (Player player in lobby.Players)
            {
                LobbyPlayerItem playerItem = Instantiate(playerItemPrefab, playerListContainer);
                playerItem.Initialize(player);
                _playerListItems.Add(player.Id, playerItem);

                bool isHost = LobbyManager.Instance.IsLobbyHost();
                bool isSelf = player.Id == AuthenticationService.Instance.PlayerId;

                playerItem.SetKickButtonActive(isHost && !isSelf);
                playerItem.SetReadyButtonActive(!isHost && isSelf);

                playerItem.OnKick += async () => await LobbyManager.Instance.KickPlayer(player.Id);
                playerItem.OnReady += async () => await LobbyManager.Instance.SetPlayerReady();
                
                Debug.Log($"Player ID: {AuthenticationService.Instance.PlayerId}, IsHost: {isHost}, IsSelf: {isSelf}");
            }

            // Cập nhật các thông tin khác
            lobbyNameText.text = $"Lobby Name: {lobby.Name}";
            lobbyCodeText.text = $"Lobby Name: {lobby.LobbyCode}";
            // startGameButton.gameObject.SetActive(isHost);
            // ... (thêm logic kiểm tra all players ready cho startGameButton.interactable)
        }
        // Hàm chỉ để bật/tắt các panel chính
        private void UpdateLobbyUIVisibility()
        {
            bool inLobby = LobbyManager.Instance.JoinedLobby != null;
            connectPanel.SetActive(!inLobby);
            lobbyPanel.SetActive(inLobby);
        }
        // MỚI: Hàm cập nhật danh sách người chơi, áp dụng kỹ thuật caching
        private void UpdatePlayerList(Lobby lobby)
        {
            var playerIdsInLobby = new HashSet<string>();
            foreach (var player in lobby.Players)
            {
                playerIdsInLobby.Add(player.Id);
            }

            // Xóa và cache những người chơi đã rời đi
            var playersToRemove = new List<string>();

            foreach (var existingPlayer in _playerListItems)
            {
                if (!playerIdsInLobby.Contains(existingPlayer.Key))
                {
                    playersToRemove.Add(existingPlayer.Key);
                }
            }
            foreach (var playerId in playersToRemove)
            {
                var itemToCache = _playerListItems[playerId];
                itemToCache.gameObject.SetActive(false);
                _cachedPlayerListItems.Add(itemToCache);
                _playerListItems.Remove(playerId);
            }
            // ================================
            // --- PHẦN BỔ SUNG ĐỂ HOÀN THIỆN NÚT READY/KICK ---
            // Duyệt qua danh sách người chơi trong lobby để cập nhật hoặc tạo mới item
            foreach (Player player in lobby.Players)
            {
                LobbyPlayerItem playerItem;

                if (_playerListItems.TryGetValue(player.Id, out playerItem))
                {
                    // Nếu người chơi đã có trong danh sách, chỉ cập nhật lại giao diện của họ
                    playerItem.Initialize(player);
                }
                else
                {
                    // Nếu là người chơi mới, lấy một item từ cache hoặc tạo mới
                    playerItem = GetPlayerListItem();
                    playerItem.Initialize(player);
                    _playerListItems.Add(player.Id, playerItem);

                    // GÁN SỰ KIỆN - CHỈ LÀM MỘT LẦN KHI TẠO MỚI ITEM
                    // Lắng nghe tín hiệu OnKick từ item và gọi đến LobbyManager
                    // playerItem.OnKick += () => LobbyManager.Instance.KickPlayer(player.Id);
                    // Gán sự kiện bằng các hàm lambda bất đồng bộ
                    playerItem.OnKick += async () => await LobbyManager.Instance.KickPlayer(player.Id);


                    // Lắng nghe tín hiệu OnReady từ item và gọi đến LobbyManager
                    playerItem.OnReady += async () => await LobbyManager.Instance.SetPlayerReady();

                }

                // ĐIỀU KHIỂN HIỂN THỊ NÚT - LUÔN CẬP NHẬT MỖI LẦN
                // Xác định ngữ cảnh: Bạn là ai? Bạn có phải chủ phòng không?
                bool isHost = LobbyManager.Instance.IsLobbyHost();
                bool isSelf = player.Id == AuthenticationService.Instance.PlayerId;

                // Chỉ hiện nút "Kick" nếu BẠN là Host VÀ item này KHÔNG phải của bạn
                playerItem.SetKickButtonActive(isHost && !isSelf);

                // Chỉ hiện nút "Ready" nếu BẠN KHÔNG phải Host VÀ item này LÀ của chính bạn
                playerItem.SetReadyButtonActive(!isHost && isSelf);
            }
            // ===============================================
        }
        // MỚI: Hàm để lấy một item từ cache hoặc tạo mới
        private LobbyPlayerItem GetPlayerListItem()
        {
            if (_cachedPlayerListItems.Count > 0)
            {
                var item = _cachedPlayerListItems[0];
                _cachedPlayerListItems.RemoveAt(0);
                item.gameObject.SetActive(true);
                return item;
            }

            return Instantiate(playerItemPrefab, playerListContainer);
        }
        // Cập nhật: Các hàm gọi LobbyManager giờ đây sẽ chờ và cập nhật UI ngay sau đó
        private async void OnCreateLobbyClicked()
        {
            string lobbyName = string.IsNullOrEmpty(lobbyNameInput.text) ? $"Room_{Random.Range(1000, 9999)}" : lobbyNameInput.text;
            await LobbyManager.Instance.CreateLobby(lobbyName, 4, false);
            // UpdateVisuals(); // Cập nhật UI ngay sau khi tạo
        }

        private async void OnJoinLobbyClicked()
        {
            string lobbyCode = lobbyCodeInput.text;
            if (!string.IsNullOrEmpty(lobbyCode))
            {
                await LobbyManager.Instance.JoinLobbyByCode(lobbyCode);
                // UpdateVisuals(); // Cập nhật UI ngay sau khi tham gia
            }
        }

        private async void OnLeaveLobbyClicked()
        {
            await LobbyManager.Instance.LeaveLobby();
            // UpdateVisuals(); // Cập nhật UI ngay sau khi rời đi
        }

        // Cập nhật: Hàm UpdateVisuals giờ đây sẽ gọi UpdatePlayerList
        private void UpdateVisuals()
        {
            var joinedLobby = LobbyManager.Instance.JoinedLobby;
            bool inLobby = joinedLobby != null;

            connectPanel.SetActive(!inLobby);
            lobbyPanel.SetActive(inLobby);

            if (inLobby)
            {
                lobbyNameText.text = $"Lobby Name: {joinedLobby.Name}";
                lobbyCodeText.text = $"Lobby Code: {joinedLobby.LobbyCode}";
                UpdatePlayerList(joinedLobby);
            }
        }

        private void ClosePanel() { Close(); }
        private void OnStartGameClicked() { /* ... */ }
        private void Update()
        {
        }
    }
}

