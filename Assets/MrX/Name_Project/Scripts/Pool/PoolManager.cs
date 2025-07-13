using System;
using System.Collections.Generic;
using UnityEngine;

namespace MrX.Name_Project
{
    public class PoolManager : MonoBehaviour
    {
        public static PoolManager Ins { get; private set; }
        // Một class nhỏ để cấu hình mỗi pool trong Inspector cho tiện
        [System.Serializable]
        public class PoolConfig
        {
            public string tag;          // Tên định danh cho pool (ví dụ: "PlayerBullet", "BasicEnemy")
            public GameObject prefab;   // Prefab của đối tượng
            public int initialSize;    // Số lượng đối tượng muốn tạo sẵn
        }

        [Header("Pool Configuration")]
        public List<PoolConfig> poolConfigs; // Danh sách tất cả các pool bạn muốn tạo

        // Dictionary để lưu trữ và truy cập các pool bằng tag
        private Dictionary<string, MyPool> pools;

        void Awake()
        {
            // Singleton Pattern
            if (Ins != null && Ins != this)
            {
                Destroy(gameObject);
                return; // Thêm return để dừng thực thi nếu đã có instance
            }
            else
            {
                Ins = this;
            }
            // Debug.Log("================ POOL MANAGER AWAKE START ================");
            pools = new Dictionary<string, MyPool>();
            // Kiểm tra xem có bao nhiêu cấu hình pool được thiết lập trong Inspector
            // Debug.Log($"Tìm thấy {poolConfigs.Count} cấu hình pool.");
            foreach (var config in poolConfigs)
            {
                if (config.prefab == null)
                {
                    continue; // Bỏ qua pool này nếu prefab bị null
                }
                MyPool newPool = new MyPool(config.prefab);
                // Bắt đầu quá trình "làm nóng" pool
                for (int i = 0; i < config.initialSize; i++)
                {
                    GameObject objToWarm = GameObject.Instantiate(config.prefab, Vector3.zero, Quaternion.identity);
                    objToWarm.SetActive(false);
                    var returnComponent = objToWarm.GetComponent<ReturnToMyPool>();
                    if (returnComponent == null)
                    {
                        returnComponent = objToWarm.AddComponent<ReturnToMyPool>();
                    }
                    returnComponent.pool = newPool;
                    newPool.AddToPool(objToWarm);
                }
                pools.Add(config.tag, newPool);
            }
        }

        public GameObject GetFromPool(string tag, Vector3 positon)
        {
            // Kiểm tra xem có pool nào với tag được yêu cầu không
            if (!pools.ContainsKey(tag))
            {
                Debug.LogWarning($"Pool with tag '{tag}' doesn't exist.");
                return null;
            }

            // Nếu có, lấy một object từ pool đó và trả về
            // Tham số 'true' đảm bảo object sẽ được SetActive(true)
            return pools[tag].Get(true, positon);
        }
    }
}

