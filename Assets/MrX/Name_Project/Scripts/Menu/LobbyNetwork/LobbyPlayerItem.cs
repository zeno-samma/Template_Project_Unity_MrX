using System;
using System.Diagnostics;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

namespace MrX.Name_Project
{
    public class LobbyPlayerItem : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI playerNameText;
        [SerializeField] private TextMeshProUGUI playerStatusText;
        [SerializeField] private Button readyButton;
        [SerializeField] private Button kickButton;
        public event Action OnKick;
        public event Action OnReady;
        private Player _player;
        private void Awake()
        {
            // Gán sự kiện ngay từ đầu, khi nút được nhấn, nó sẽ phát sự kiện ra ngoài
            readyButton.onClick.AddListener(() => OnReady?.Invoke());
            kickButton.onClick.AddListener(() => OnKick?.Invoke());
        }
        public void Initialize(Player player)
        {
            _player = player;
            UpdateVisuals();

            // Gán sự kiện cho các nút
            // readyButton.onClick.AddListener(OnReadyButtonClicked); // Sẽ được quản lý bởi LobbyManager
            // kickButton.onClick.AddListener(OnKickButtonClicked); // Sẽ được quản lý bởi LobbyManager
        }

        // Cập nhật lại giao diện dựa trên dữ liệu Player mới nhất
        public void UpdateVisuals()
        {
            if (_player == null) return;

            // Hiển thị tên người chơi
            Debug.Log(_player.Data);
            if (_player.Data.ContainsKey("PlayerName"))
            {
                playerNameText.text = _player.Data["PlayerName"].Value;
            }

            // Hiển thị trạng thái Sẵn sàng
            if (_player.Data.ContainsKey("PlayerReady") && _player.Data["PlayerReady"].Value == "1")
            {
                playerStatusText.text = "Ready";
                playerStatusText.color = Color.green;
            }
            else
            {
                playerStatusText.text = "Not Ready";
                playerStatusText.color = Color.yellow;
            }
        }

        public string GetPlayerId()
        {
            return _player?.Id;
        }

        // Getter để các script khác có thể lấy thông tin Player
        public Player GetPlayer()
        {
            return _player;
        }
        // Hàm để LobbyUI điều khiển việc hiển thị các nút
        public void SetKickButtonActive(bool isActive)
        {
            kickButton.gameObject.SetActive(isActive);
        }

        public void SetReadyButtonActive(bool isActive)
        {
            readyButton.gameObject.SetActive(isActive);
        }
    }
}
