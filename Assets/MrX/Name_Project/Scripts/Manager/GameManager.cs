using UnityEngine;
using System.IO;
using System;

namespace MrX.Name_Project
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Ins;
        [SerializeField] private int currentScore;
        private PlayerData playerData;
        public Player player; // Kéo đối tượng Hero trong Scene vào đây
        private string saveFilePath;
        private bool isDataDirty = false; // << "CỜ BẨN"
        public enum GameState//Giá trị mặc định của enum là đầu tiên.
        {
            NONE, //Rỗng
            PREPAIR, //Đang chuẩn bị
            PLAYING,      // Đang chơi
            PAUSE,       // Dừng game
            UPGRADEPHASE,//Nâng cấp.
            GAMEOVER  // Thua cuộc
        }
        public GameState CurrentState { get; private set; }
        void Awake()
        {
            saveFilePath = Path.Combine(Application.persistentDataPath, "savedata.json");
            // Singleton Pattern
            if (Ins != null && Ins != this)
            {
                Destroy(gameObject);
            }
            else
            {
                Ins = this;
                DontDestroyOnLoad(gameObject); // Giữ GameManager tồn tại giữa các scene
            }
            LoadGame();
        }
        // Hàm công khai để các script khác có thể "báo hiệu" có thay đổi
        public void MarkDataAsDirty()
        {
            isDataDirty = true;
        }
        void Start()
        {
            // Bắt đầu game bằng trạng thái khởi tạo
            UpdateGameState(GameState.PREPAIR);
        }
        public void UpdateGameState(GameState newState)
        {
            // Tránh gọi lại logic nếu không có gì thay đổi
            if (newState == CurrentState) return;
            CurrentState = newState;
            // Xử lý logic đặc biệt ngay khi chuyển sang state mới
            switch (newState)
            {
                case GameState.PREPAIR:
                    // Debug.Log("code chuẩn bị game");
                    // ... code chuẩn bị game ...
                    // Sau khi chuẩn bị xong, tự động chuyển sang Playing
                    // EventBus.Publish(new InitialUIDataReadyEvent { defScore = Pref.coins });//Phát thông báo lần đầu để ui cập nhật lên màn hình đầu game.
                    break;
                case GameState.PLAYING:
                    Time.timeScale = 1f;
                    // EventBus.Publish(new SendToPoolCtrlEvent {});//Phát thông báo lần đầu để ui cập nhật lên màn hình đầu game.
                    break;
                case GameState.PAUSE:
                    Time.timeScale = 0f;
                    break;
                case GameState.UPGRADEPHASE:
                    // EventBus.Publish(new StatPointEvent
                    // {
                    //     StatHealth = Player.Ins.maxHP, // maxHP của Player đã bao gồm Pref.Player_HP
                    //     StatDamage = Player.Ins.valueAtk, // valueAtk của Player đã bao gồm Pref.Player_ATK
                    //     StatCooldown = Player.Ins.m_Attack_CD,
                    //     PointHealth = Pref.Player_Point_HP,
                    //     PointDamage = Pref.Player_Point_ATK,
                    //     PointCooldown = Pref.Player_Point_Cd,
                    // });
                    Time.timeScale = 0f; // Dừng game để người chơi nâng cấp
                    break;
                case GameState.GAMEOVER:
                    Time.timeScale = 0f; // Dừng game
                    break;
            }

            // 4. Phát đi "báo cáo" về trạng thái mới cho các hệ thống khác lắng nghe
            // OnStateChanged?.Invoke(newState);
            // EventBus.Publish(new StateUpdatedEvent { CurState = newState });//Phát thông báo lần đầu thay đổi state
            Debug.Log("Game state changed to: " + newState);
        }
        public void SaveGame()
        {

            // Chỉ thực hiện lưu nếu có thay đổi
            if (!isDataDirty) return;
            Debug.Log("Data was dirty, SAVING GAME...");
            PlayerData dataToSave = player.GetDataToSave();
            dataToSave.version = Application.version; // << LƯU PHIÊN BẢN HIỆN TẠI
            dataToSave.gold = currentScore;

            string json = JsonUtility.ToJson(dataToSave, true);
            File.WriteAllText(saveFilePath, json);
            Debug.Log("Lưu game với version: " + dataToSave.version);
            // Sau khi lưu xong, reset cờ
            isDataDirty = false;
        }

        public void LoadGame()
        {
            if (File.Exists(saveFilePath))
            {
                string json = File.ReadAllText(saveFilePath);
                playerData = JsonUtility.FromJson<PlayerData>(json);

                // --- LOGIC SO SÁNH PHIÊN BẢN ---
                if (playerData.version != Application.version)
                {
                    // Phiên bản của file save khác với phiên bản của game
                    // -> Đây là bản build mới -> Reset dữ liệu
                    Debug.Log("Phát hiện phiên bản trò chơi mới (" + Application.version + "). Dữ liệu lưu cũ từ phiên bản " + playerData.version + " sẽ được đặt lại.");
                    ResetAndCreateNewData();
                }
                else
                {
                    // Cùng phiên bản, tải dữ liệu bình thường
                    currentScore = playerData.gold;
                    player.ApplyLoadedData(playerData);
                    Debug.Log("Đã tải trò chơi từ phiên bản: " + playerData.version);
                }
            }
            else
            {
                // Không có file save, tạo dữ liệu mới
                Debug.Log("Không tìm thấy tệp lưu, đang tạo dữ liệu mới.");
                player.ApplyLoadedData(new PlayerData());
                ResetAndCreateNewData();
            }
        }

        // Hàm tạo dữ liệu mới và lưu lại ngay lập tức
        private void ResetAndCreateNewData()
        {
            currentScore = 0;
            SaveGame(); // Gọi SaveGame để tạo file save mới với phiên bản hiện tại và score = 0
        }
        // Hàm đặc biệt của Unity
        void OnApplicationQuit()
        {
            Debug.Log("Application quitting...");
            SaveGame(); // Gọi hàm save lần cuối
        }
        public void PlayGame()///1.Sau khi ấn nút play
        {
            UpdateGameState(GameState.PLAYING);
            ActivePlayer();
        }
        public void ActivePlayer()
        {

            // if (m_curPlayer)
            //     Destroy(m_curPlayer.gameObject);

            // var shopItem = ShopController.Ins.items;
            // if (shopItem == null || shopItem.Length <= 0) return;

            // var newPlayerPb = shopItem[Pref.curPlayerId].playerPrefabs;
            // if (newPlayerPb)
            // {
            //     m_curPlayer = Instantiate(newPlayerPb, new Vector3(-6f, -1.7f, 0f), Quaternion.identity);
            // }
        }
        // public void AddScore(EnemyDiedEvent value)//Test UI
        // {
        //     // Debug.Log("DieGM");
        //     m_score += value.dieScore;
        //     // Debug.Log(value.dieScore);
        //     Pref.coins += value.dieScore;
        //     // Thay vì tự phát event, nó "gửi thông báo" đến EventBus
        //     EventBus.Publish(new ScoreUpdatedEvent { newScore = Pref.coins });//Phát thông báo kèm điểm khi tiêu diệt một enemy
        // }
        // public void GameOver(PlayerDiedEvent e)//
        // {
        //     // Debug.Log("Vào đây khi game over");
        //     // Gửi thông báo game over với dữ liệu điểm cuối cùng
        //     EventBus.Publish(new GameOverEvent { finalScore = m_score });
        //     UpdateGameState(GameState.GAMEOVER);
        // }
    }

}