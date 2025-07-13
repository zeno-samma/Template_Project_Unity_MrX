using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MrX.Name_Project
{
    public class PoolManager1 : MonoBehaviour
    {
        public static PoolManager1 Ins;
        public float m_CD_Nextwave;//Thời gian Cd mỗi vòng wave
        [SerializeField] private int amountEnemyPool;
        [SerializeField] private int amountBulletPool;
        // [SerializeField] private EnemyPool[] enemyPrefabs;
        // [SerializeField] private BulletObjectPool[] bulletPrefabs;
        public List<GameObject> activeEnemies = new List<GameObject>();
        // private MyPool[] myEnemyPools; // Thêm dòng này
        // private MyPool[] myBulletPools; // Thêm dòng này
        public Player player;
        // private int i = 0;
        // private int j = 0;
        // ==============
        [Header("SpawnState")]
        public int m_waveNumber = 0;
        // 1.Đặt trạng thái
        public enum SpawnState
        {
            SPAWNING,      // Trạng thái đang tạo địch
            WAITING,       // Trạng thái đang chờ người chơi diệt hết địch
            COUNTING_DOWN  // Trạng thái đang đếm ngược tới wave tiếp theo
        }
        // 2.Đặt biến
        public SpawnState m_state;
        // ===================
        private void Awake()
        {
            Ins = this;
        }
        // Start is called before the first frame update
        private void Start()
        {
            player = GetComponent<Player>();
            // Đặt trạng thái ban đầu khi game bắt đầu
            m_state = SpawnState.COUNTING_DOWN;
            // =================Khởi tạo Enemy Pool================
            // myEnemyPools = new MyPool[enemyPrefabs.Length]; // Khởi tạo mảng pool
            // // myBulletPools = new MyPool[bulletPrefabs.Length]; // Khởi tạo mảng pool
            // for (int i = 0; i < enemyPrefabs.Length; i++)
            // {
            //     myEnemyPools[i] = new MyPool(enemyPrefabs[i].gameObject);
            //     Debug.Log(myEnemyPools);
            // }
            // while (i < amountEnemyPool)// Giới hạn số lượng đối tượng được tạo ra
            // {
            //     int idx = Random.Range(0, enemyPrefabs.Length);
            //     // myEnemyPools[idx].Get(false);
            //     i++;
            // }
            // for (int j = 0; i < myBulletPools.Length; i++)
            // {
            //     myBulletPools[i] = new MyPool(bulletPrefabs[i].gameObject);
            //     Debug.Log(myBulletPools);
            // }
            // while (j < amountBulletPool)// Giới hạn số lượng đối tượng được tạo ra
            // {
            //     int idx = Random.Range(0, bulletPrefabs.Length);
            //     myBulletPools[idx].Get(false);
            //     i++;
            // }
            // =================Khởi tạo Bullet Pool================
        }
        private void OnEnable()
        {
            // Đăng ký lắng nghe sự thay đổi trạng thái từ GameManager
            // EventBus.Subscribe<StateUpdatedEvent>(SpawnEnemiesState); 
        }
        private void OnDisable()
        {
            // EventBus.Unsubscribe<StateUpdatedEvent>(SpawnEnemiesState);
            StopAllCoroutines();
        }


        // private void SpawnEnemiesState(StateUpdatedEvent gameState)
        // {
        //     if (gameState.CurState == GameManager.GameState.PLAYING)
        //     {

        //         StartNextWave();
        //     }
        //     else if (gameState.CurState == GameManager.GameState.GAMEOVER)
        //     {
        //         // Debug.Log("Vào đây");
        //         StopAllCoroutines();
        //         Time.timeScale = 0f;
        //     }
        // }
        private void Update()
        {

            // ====================Kiểm tra trạng thái
            // Chỉ thực hiện kiểm tra này khi spawner đã spawn xong và đang ở trạng thái chờ
            if (m_state == SpawnState.WAITING)
            {
                // Kiểm tra nếu danh sách kẻ thù trong EnemyManager đã rỗng
                // Đây là cách kiểm tra hiệu quả nhất
                if (activeEnemies.Count == 0)
                {
                    // Nếu không còn kẻ thù nào, wave đã hoàn thành!
                    if (m_waveNumber > 0 && m_waveNumber % 3 == 0)
                    {
                        UpgradePhase();
                    }
                    else
                    {
                        WaveCompleted();
                    }
                }
            }
        }

        private void UpgradePhase()
        {
            // EventBus.Publish(new UpgradePhaseEvent { });//Phát thông báo lần đầu thay đổi state
        }

        // public void RegisterEnemy(EnemyPool EnemyPool)
        // {
        //     // Thêm enemy vào danh sách nếu nó chưa có ở trong
        //     if (!activeEnemies.Contains(EnemyPool.gameObject))
        //     {
        //         activeEnemies.Add(EnemyPool.gameObject);
        //     }
        // }

        // public void UnregisterEnemy(EnemyPool EnemyPool)
        // {
        //     // Xóa enemy khỏi danh sách nếu nó có tồn tại
        //     if (activeEnemies.Contains(EnemyPool.gameObject))
        //     {
        //         activeEnemies.Remove(EnemyPool.gameObject);
        //     }
        // }
        // Hàm này được gọi khi wave trước đã hoàn thành và đến lúc bắt đầu wave mới
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
                    StartCoroutine(SpawnEnemies(5, 1, 2.5f));//count, level,Delaytime
                    // m_state = SpawnState.WAITING; // Spawn xong, chuyển sang chờ
                    break;
                case 2:
                    // --- Điều kiện cho Wave 2 ---"Sức Mạnh Của Đánh Xuyên"Chia làm 2 cụm, mỗi cụm 3 con đi sát nhau. Thời gian nghỉ giữa 2 cụm là 3 giây.
                    Debug.Log("Kịch bản Wave 2: 4 x Lính Thí Mạng (Cấp 1)."); //cách nhau 2.5s
                    StartCoroutine(SpawnEnemies(10, 1, 2f)); // Gọi một coroutine có kịch bản phức tạp hơn
                    // m_state = SpawnState.WAITING; // Spawn xong, chuyển sang chờ
                    break;

                case 3:
                    // --- Điều kiện cho Wave 3 ---"Thử Thách Bức Tường Thịt"2 Thiết Giáp xuất hiện trước. Sau 2 giây, 3 Sát Thủ xuất hiện ngay phía sau và di chuyển cùng tốc độ.
                    Debug.Log("Kịch bản Wave 3: 2 x Kẻ Cản Trở (Cấp 2) và 2 x Lính Thí Mạng (Cấp 3).");
                    StartCoroutine(SpawnMixedWave_03()); // Gọi một coroutine có kịch bản phức tạp hơn
                    // m_state = SpawnState.WAITING;
                    break;

                case 4:
                    // --- Điều kiện cho Wave 4 ---
                    Debug.Log("Kịch bản Wave 4: 1 x Mini-Boss, 1 x Kẻ Cản Trở (Cấp 2), 4 x Kẻ Rỉa Máu (Cấp 3).!");
                    StartCoroutine(SpawnMixedWave_04()); // Gọi một coroutine có kịch bản phức tạp hơn
                    // m_state = SpawnState.WAITING; // Chuyển sang chờ boss bị giết
                    break;

                default:
                    // --- Điều kiện cho các wave sau wave 4 ---
                    // Có thể dùng một logic chung để tăng độ khó tự động
                    Debug.Log("Kịch bản Wave " + m_waveNumber + ": Thử thách tăng dần!");
                    int enemyCount = 1 + m_waveNumber ; // Ví dụ: số lượng tăng theo wave
                    StartCoroutine(SpawnMixedWaveEnd(enemyCount, 2f));
                    // m_state = SpawnState.WAITING; // Chuyển sang chờ boss bị giết
                    break;
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
        // Coroutine để spawn một loại enemy cụ thể
        private IEnumerator SpawnEnemies(int count, int level, float spawnInterval)
        {
            // Debug.Log("count: " +count +"level: "+ level+"spawnInterval: "+spawnInterval);
            m_state = SpawnState.SPAWNING;
            for (int i = 0; i < count; i++)
            {
                // myEnemyPools[level - 1].Get(true);
                yield return new WaitForSeconds(spawnInterval);
            }
            m_state = SpawnState.WAITING;
            Debug.Log("Đã spawn xong, đang chờ người chơi dọn dẹp...");
            yield break; // Kết thúc coroutine cho wave này
        }

        // Coroutine ví dụ cho wave 2
        private IEnumerator SpawnMixedWave_03()
        {
            // m_state = SpawnState.SPAWNING;
            // 2 x Kẻ Cản Trở (Cấp 2)
            for (int i = 0; i < 1; i++)
            {
                StartCoroutine(SpawnEnemies(2, 2, 2f));//count, level,Delaytime
                yield return new WaitForSeconds(14f);
            }
            // 2 x Lính Thí Mạng (Cấp 3)
            for (int i = 0; i < 1; i++)
            {
                StartCoroutine(SpawnEnemies(2, 3, 2f));//count, level,Delaytime
                yield return new WaitForSeconds(1f);
            }
            m_state = SpawnState.WAITING;
            Debug.Log("Đã spawn xong, đang chờ người chơi dọn dẹp...");
            yield break; // Kết thúc coroutine cho wave này
        }        // Hàm công khai để bắt đầu đếm ngược từ một script khác
        // Coroutine ví dụ cho wave 3
        private IEnumerator SpawnMixedWave_04()
        {
            // m_state = SpawnState.SPAWNING;
            // Spawn 4 con lính cấp 1
            for (int i = 0; i < 1; i++)
            {
                StartCoroutine(SpawnEnemies(1, 4, 0f));//count, level,Delaytime
                yield return new WaitForSeconds(15f);
                StartCoroutine(SpawnEnemies(1, 2, 0f));//count, level,Delaytime
            }
            // Spawn 4 con lính cấp 1
            for (int i = 0; i < 1; i++)
            {
                StartCoroutine(SpawnEnemies(4, 1, 0f));//count, level,Delaytime
                yield return new WaitForSeconds(0f);
            }
            m_state = SpawnState.WAITING;
            Debug.Log("Đã spawn xong, đang chờ người chơi dọn dẹp...");
            yield break; // Kết thúc coroutine cho wave này
        }        // Hàm công khai để bắt đầu đếm ngược từ một script khác
        private IEnumerator SpawnMixedWaveEnd(int count, float spawnInterval)
        {
            // m_state = SpawnState.SPAWNING;
            // Spawn 4 con lính cấp 1
            for (int i = 0; i < count; i++)
            {
                int idx = Random.Range(1, 4);
                StartCoroutine(SpawnEnemies(1, idx, spawnInterval));//count, level,Delaytime
                yield return new WaitForSeconds(1f);
            }
 
            m_state = SpawnState.WAITING;
            Debug.Log("Đã spawn xong, đang chờ người chơi dọn dẹp...");
            yield break; // Kết thúc coroutine cho wave này
        }
        // Hàm công khai để bắt đầu đếm ngược từ một script khác
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
                timer -= Time.deltaTime;

                yield return null;
            }

            // Phát event báo hiệu đã hoàn thành
            // Debug.Log("Countdown Finished!");
            StartNextWave();
            yield break; // Kết thúc coroutine cho wave này
        }

    }
}