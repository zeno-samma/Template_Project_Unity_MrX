using UnityEngine;

namespace MrX.Name_Project
{
    public class Player : MonoBehaviour
    {
        // Có thể biến nó thành Singleton để dễ truy cập toàn cục
        public static Player Instance { get; private set; }
        // "Bộ não" sẽ giữ tham chiếu đến tất cả các bộ phận chuyên môn
        public PlayerMovement Movement { get; private set; }
        public PlayerHealth Health { get; private set; }
        public WeaponManager Weapon { get; private set; }
        void Awake()
        {
            // Thiết lập Singleton
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
            }

            // Tự động lấy các "bộ phận" của mình
            Movement = GetComponent<PlayerMovement>();
            Health = GetComponent<PlayerHealth>();
            Weapon = GetComponentInChildren<WeaponManager>(); // Ví dụ nếu Weapon là con
        }
        void Start()
        {
            
        }
    }
}

