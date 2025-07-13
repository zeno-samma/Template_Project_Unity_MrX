using System.Collections;
using MrX.Name_Project;
using UnityEngine;

namespace MrX.Name_Project
{
    public class WaveSpawner : MonoBehaviour
    {
        [SerializeField] private Transform[] spawnPoints;
        [SerializeField] private int m_waveNumber = 0;
        [SerializeField] private int m_CD_Nextwave;
        public enum SpawnState
        {
            SPAWNING,      // Trạng thái đang tạo địch
            WAITING,       // Trạng thái đang chờ người chơi diệt hết địch
            COUNTING_DOWN  // Trạng thái đang đếm ngược tới wave tiếp theo
        }
        public SpawnState m_state;
        private void OnEnable()
        {

            // Đăng ký lắng nghe sự thay đổi trạng thái từ GameManager
            EventBus.Subscribe<StateUpdatedEvent>(SpawnEnemiesState);//Lắng nghe trạng thái game do gamemanager quản lý
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<StateUpdatedEvent>(SpawnEnemiesState);
        }
        void Start()
        {
            m_state = SpawnState.COUNTING_DOWN;
        }
        void Update()
        {
            if (m_state == SpawnState.WAITING)
            {
                if (EnemyManager.Ins.activeEnemies.Count == 0)
                {
                    // Nếu không còn kẻ thù nào, wave đã hoàn thành!
                    if (m_waveNumber > 0 && m_waveNumber % 3 == 0)
                    {
                        // UpgradePhase();
                        Debug.Log("UpgradePhase");
                        WaveCompleted();//Nếu không có UpgradePhase thì lập tức vào dòng này
                    }
                    else
                    {
                        WaveCompleted();
                    }
                }
            }

        }
        private void SpawnEnemiesState(StateUpdatedEvent Value)
        {
            if (Value.CurState == GameManager.GameState.PLAYING)
            {
                Debug.Log("GameStart...!");
                StartNextWave();
            }
            else if (Value.CurState == GameManager.GameState.GAMEOVER)
            {
                // Debug.Log("Vào đây");
                StopAllCoroutines();
                Time.timeScale = 0f;
            }
        }
        // Hàm được gọi khi wave được xác nhận là đã sạch
        void WaveCompleted()
        {
            Debug.Log("Wave " + m_waveNumber + " đã hoàn thành!");
            // EventBus.Publish(new CountdownNextWave {cooldownDuration = m_CD_Nextwave});
            // Chuyển sang trạng thái đếm ngược cho wave tiếp theo
            m_state = SpawnState.COUNTING_DOWN;
            // Bắt đầu bộ đếm ngược (ví dụ, gọi hệ thống CountdownTimer đã thiết kế)
            StartNewCountdown(m_CD_Nextwave);
        }
        private void StartNewCountdown(float duration)////duration thời gian cd mỗi wave enemy
        {
            StartCoroutine(CountdownCoroutine(duration));
        }
        private IEnumerator CountdownCoroutine(float duration)
        {
            float timer = duration;

            // Bắt đầu vòng lặp đếm ngược
            while (timer > 0)
            {
                // Giảm thời gian
                Debug.Log($"Time: {timer}");
                timer -= Time.deltaTime;
                yield return null;
            }

            // Phát event báo hiệu đã hoàn thành
            // Debug.Log("Countdown Finished!");
            StartNextWave();
            yield break; // Kết thúc coroutine cho wave này
        }
        public void StartNextWave()
        {
            m_waveNumber++;
            // OnNewWave?.Invoke(m_waveNumber); // Phát event cho UI (nếu có)
            // EventBus.Publish(new WaveUpdatedEvent { waveNumber = m_waveNumber});//Phát thông báo lần đầu thay đổi state

            Debug.Log("Chuẩn bị cho Wave: " + m_waveNumber);

            // DÙNG SWITCH...CASE ĐỂ QUYẾT ĐỊNH LOGIC CHO TỪNG WAVE
            switch (m_waveNumber)
            {
                case 1:
                    // --- Điều kiện cho Wave 1 ---"Làm Quen Vũ Khí"
                    Debug.Log("Kịch bản Wave 1: 3 x Lính Thí Mạng (Cấp 1)."); //cách nhau 4s
                    StartCoroutine(SpawnEnemies("GiantSlimeGreen", 5, 2.5f));//count, level,Delaytime
                    m_state = SpawnState.SPAWNING;
                    break;
                case 2:
                    // --- Điều kiện cho Wave 2 ---"Sức Mạnh Của Đánh Xuyên"Chia làm 2 cụm, mỗi cụm 3 con đi sát nhau. Thời gian nghỉ giữa 2 cụm là 3 giây.
                    Debug.Log("Kịch bản Wave 2: 4 x Lính Thí Mạng (Cấp 1)."); //cách nhau 2.5s
                    StartCoroutine(SpawnEnemies("GiantSlimeBlue", 5, 2f)); // Gọi một coroutine có kịch bản phức tạp hơn
                    m_state = SpawnState.SPAWNING;
                    break;

                case 3:
                    // --- Điều kiện cho Wave 3 ---"Thử Thách Bức Tường Thịt"2 Thiết Giáp xuất hiện trước. Sau 2 giây, 3 Sát Thủ xuất hiện ngay phía sau và di chuyển cùng tốc độ.
                    Debug.Log("Kịch bản Wave 3: 2 x Kẻ Cản Trở (Cấp 2) và 2 x Lính Thí Mạng (Cấp 3).");
                    StartCoroutine(SpawnMixedWave_03()); // Gọi một coroutine có kịch bản phức tạp hơn
                    break;

                case 4:
                    // --- Điều kiện cho Wave 4 ---
                    Debug.Log("Kịch bản Wave 4: 1 x Mini-Boss, 1 x Kẻ Cản Trở (Cấp 2), 4 x Kẻ Rỉa Máu (Cấp 3).!");
                    StartCoroutine(SpawnMixedWave_04()); // Gọi một coroutine có kịch bản phức tạp hơn
                    break;

                default:
                    // --- Điều kiện cho các wave sau wave 4 ---
                    // Có thể dùng một logic chung để tăng độ khó tự động
                    Debug.Log("Kịch bản Wave " + m_waveNumber + ": Thử thách tăng dần!");
                    // int enemyCount = 1 + m_waveNumber; // Ví dụ: số lượng tăng theo wave
                    // StartCoroutine(SpawnMixedWaveEnd(enemyCount, 2f));
                    break;
            }
        }
        private IEnumerator SpawnEnemies(string name, int count, float spawnInterval)
        {
            m_state = SpawnState.SPAWNING;
            for (int i = 0; i < count; i++)
            {
                // Lấy một chỉ số ngẫu nhiên từ 0 đến số lượng điểm spawn
                int randomIndex = Random.Range(0, spawnPoints.Length);

                // Lấy Transform của điểm spawn ngẫu nhiên đó
                Transform randomSpawnPoint = spawnPoints[randomIndex];
                PoolManager.Ins.GetFromPool(name, randomSpawnPoint.position);
                Debug.Log("Spawn");
                yield return new WaitForSeconds(spawnInterval);
            }
            m_state = SpawnState.WAITING;
            Debug.Log("Đã spawn xong, đang chờ người chơi dọn dẹp...");
            yield break; // Kết thúc coroutine cho wave này
        }
        private IEnumerator SpawnMixedWave_03()
        {
            // 2 x Kẻ Cản Trở (Cấp 2)
            for (int i = 0; i < 1; i++)
            {
                StartCoroutine(SpawnEnemies("GiantSlimeGreen", 5, 2.5f));//count, level,Delaytime
                yield return new WaitForSeconds(2.5f);
            }
            // 2 x Lính Thí Mạng (Cấp 3)
            for (int i = 0; i < 1; i++)
            {
                StartCoroutine(SpawnEnemies("GiantFlam", 2, 2.5f));//count, level,Delaytime
                yield return new WaitForSeconds(1f);
            }
            m_state = SpawnState.WAITING;
            Debug.Log("Đã spawn xong, đang chờ người chơi dọn dẹp...");
            yield break; // Kết thúc coroutine cho wave này
        }        // Hàm công khai để bắt đầu đếm ngược từ một script khác
        private IEnumerator SpawnMixedWave_04()
        {
            // 2 x Kẻ Cản Trở (Cấp 2)
            for (int i = 0; i < 1; i++)
            {
                StartCoroutine(SpawnEnemies("GiantSlimeBlue", 5, 2.5f));//count, level,Delaytime
                yield return new WaitForSeconds(2.5f);
            }
            // 2 x Lính Thí Mạng (Cấp 3)
            for (int i = 0; i < 1; i++)
            {
                StartCoroutine(SpawnEnemies("GiantSpirit", 2, 2.5f));//count, level,Delaytime
                yield return new WaitForSeconds(1f);
            }
            m_state = SpawnState.WAITING;
            Debug.Log("Đã spawn xong, đang chờ người chơi dọn dẹp...");
            yield break; // Kết thúc coroutine cho wave này
        }        // Hàm công khai để bắt đầu đếm ngược từ một script khác
    }
}

