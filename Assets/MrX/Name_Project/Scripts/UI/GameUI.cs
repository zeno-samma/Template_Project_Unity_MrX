using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;

namespace MrX.Name_Project
{
    public class GameUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private TextMeshProUGUI timerText;
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private TextMeshProUGUI playerNameText;
        [SerializeField] private GameObject leaderboardPanel;
        [SerializeField] private Transform leaderboardContent;
        [SerializeField] private GameObject leaderboardItemPrefab;
        [SerializeField] private GameObject gameOverPanel;
        [SerializeField] private TextMeshProUGUI winnerText;
        [SerializeField] private TextMeshProUGUI finalScoreText;
        
        [Header("Settings")]
        [SerializeField] private float leaderboardUpdateInterval = 1f;
        
        private float leaderboardTimer;
        private List<GameObject> leaderboardItems = new List<GameObject>();
        
        private void Start()
        {
            // Ẩn các panel không cần thiết
            if (gameOverPanel != null)
                gameOverPanel.SetActive(false);
                
            // Cập nhật tên người chơi
            if (playerNameText != null)
                playerNameText.text = Unity.Services.Authentication.AuthenticationService.Instance.PlayerName;
        }
        
        private void Update()
        {
            if (GameManager.Ins == null) return;
            
            // Cập nhật timer
            UpdateTimer();
            
            // Cập nhật leaderboard
            leaderboardTimer += Time.deltaTime;
            if (leaderboardTimer >= leaderboardUpdateInterval)
            {
                UpdateLeaderboard();
                leaderboardTimer = 0f;
            }
        }
        
        private void UpdateTimer()
        {
            if (timerText != null && GameManager.Ins != null)
            {
                float remainingTime = GameManager.Ins.GetRemainingTime();
                int minutes = Mathf.FloorToInt(remainingTime / 60f);
                int seconds = Mathf.FloorToInt(remainingTime % 60f);
                timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
                
                // Đổi màu khi gần hết thời gian
                if (remainingTime <= 30f)
                {
                    timerText.color = Color.red;
                }
                else
                {
                    timerText.color = Color.white;
                }
            }
        }
        
        private void UpdateLeaderboard()
        {
            if (leaderboardContent == null || leaderboardItemPrefab == null || GameManager.Ins == null) return;
            
            // Xóa leaderboard cũ
            foreach (var item in leaderboardItems)
            {
                Destroy(item);
            }
            leaderboardItems.Clear();
            
            // Lấy danh sách người chơi và sắp xếp theo điểm
            var players = GameManager.Ins.GetPlayers();
            var sortedPlayers = players.Values.OrderByDescending(p => p.Score).ToList();
            
            // Tạo leaderboard mới
            for (int i = 0; i < sortedPlayers.Count; i++)
            {
                var player = sortedPlayers[i];
                GameObject leaderboardItem = Instantiate(leaderboardItemPrefab, leaderboardContent);
                
                // Cập nhật thông tin
                var rankText = leaderboardItem.transform.Find("RankText")?.GetComponent<TextMeshProUGUI>();
                var nameText = leaderboardItem.transform.Find("NameText")?.GetComponent<TextMeshProUGUI>();
                var scoreText = leaderboardItem.transform.Find("ScoreText")?.GetComponent<TextMeshProUGUI>();
                
                if (rankText != null)
                    rankText.text = $"#{i + 1}";
                    
                if (nameText != null)
                    nameText.text = player.PlayerName;
                    
                if (scoreText != null)
                    scoreText.text = player.Score.ToString();
                
                // Đánh dấu người chơi hiện tại
                if (player.IsOwner)
                {
                    var background = leaderboardItem.GetComponent<UnityEngine.UI.Image>();
                    if (background != null)
                        background.color = new Color(1f, 1f, 0f, 0.3f); // Màu vàng nhạt
                }
                
                leaderboardItems.Add(leaderboardItem);
            }
        }
        
        public void ShowGameOver(string winnerName, int winnerScore)
        {
            if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(true);
                
                if (winnerText != null)
                    winnerText.text = $"Winner: {winnerName}";
                    
                if (finalScoreText != null)
                    finalScoreText.text = $"Final Score: {winnerScore}";
            }
        }
        
        public void UpdatePlayerScore(int score)
        {
            if (scoreText != null)
                scoreText.text = $"Score: {score}";
        }
        
        public void OnBackToLobbyClicked()
        {
            // Quay về lobby
            if (Unity.Netcode.NetworkManager.Singleton != null)
            {
                Unity.Netcode.NetworkManager.Singleton.Shutdown();
            }
            
            // Load lại scene lobby
            UnityEngine.SceneManagement.SceneManager.LoadScene("LobbyScene");
        }
        
        public void OnPlayAgainClicked()
        {
            // Restart game (có thể implement sau)
            Debug.Log("Play again clicked - implement restart logic");
        }
    }
} 