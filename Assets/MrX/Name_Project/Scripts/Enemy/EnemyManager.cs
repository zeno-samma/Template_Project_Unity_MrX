using System.Collections.Generic;
using UnityEngine;

namespace MrX.Name_Project
{
    public class EnemyManager : MonoBehaviour
    {
        public static EnemyManager Ins { get; private set; }
        public List<Enemy> activeEnemies = new List<Enemy>();

        // Thuộc tính để WaveSpawner có thể kiểm tra xem còn bao nhiêu địch
        public int ActiveEnemyCount => activeEnemies.Count;
        public Transform playerTransform; // Kéo đối tượng Player vào đây
        void Awake()
        {
            // Singleton Pattern
            if (Ins != null && Ins != this)
            {
                Destroy(gameObject);
            }
            else
            {
                Ins = this;
            }
        }
        void Start()
        {

        }
        void Update()
        {
            // Đảm bảo Player tồn tại trước khi ra lệnh
            if (playerTransform == null) return;
            // Vòng lặp chỉ huy
            foreach (Enemy enemy in activeEnemies)
            {
                // 1. Manager tính toán hướng đi cho mỗi con Enemy
                Vector3 direction = (playerTransform.position - enemy.transform.position).normalized;

                // 2. Manager ra lệnh cho Enemy di chuyển theo hướng đó
                enemy.Move(direction);
            }
        }
        // Thay đổi kiểu tham số của hàm
        public void RegisterEnemy(Enemy enemy)
        {
            if (!activeEnemies.Contains(enemy))
            {
                activeEnemies.Add(enemy);
            }
        }

        // Thay đổi kiểu tham số của hàm
        public void UnregisterEnemy(Enemy enemy)
        {
            if (activeEnemies.Contains(enemy))
            {
                activeEnemies.Remove(enemy);
            }
        }
    }

}