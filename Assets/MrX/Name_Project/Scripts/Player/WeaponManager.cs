using UnityEngine;

namespace MrX.Name_Project
{
    public class WeaponManager : MonoBehaviour
    {
        private Camera mainCam;
        private Vector3 mousePos;
        [SerializeField] private GameObject bulletPrefabs;
        [SerializeField] private Transform firePos;
        [SerializeField] private float shotDelay = 0.15f;
        [SerializeField] private int maxAmo = 24;
        public int currentAmo;
        private float nextShot;
        void Start()
        {
            currentAmo = maxAmo;
            mainCam = Camera.main; // Lấy camera chính của game
        }
        // public bool IsComponentNull()
        // {
        //     return m_anim == null;
        // }
        void Update()
        {
            Shoot();
            Reload();
        }
        void Shoot()
        {
            // 1. Lấy vị trí chuột trên màn hình
            mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);
            if (Input.mousePosition.x < 0 || Input.mousePosition.x > Screen.width || Input.mousePosition.y < 0 || Input.mousePosition.y > Screen.height)
            {
                return;
            }

            // 2. Tính toán hướng từ vũ khí đến con trỏ chuột
            Vector3 direction = (mousePos - transform.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            // 3. Xoay vũ khí theo góc đã tính
            transform.rotation = Quaternion.Euler(0, 0, angle);

            // 4. Lật vũ khí để không bị lộn ngược
            Vector3 localScale = new Vector3(2, 2, 2);
            if (angle > 90 || angle < -90)
            {
                localScale.y = -2f; // Lật theo trục Y
            }
            else
            {
                localScale.y = 2f;
            }
            transform.localScale = localScale;
            // if (IsComponentNull()) return;
            if (Input.GetMouseButtonDown(0) && currentAmo > 0 && Time.time > nextShot)
            {
                nextShot = Time.time + shotDelay;
                GameObject bulletObj = PoolManager.Ins.GetFromPool("PlayerBullet", firePos.position);
                Bullet bulletScript = bulletObj.GetComponent<Bullet>();
                // 4. "Ra lệnh" cho viên đạn bay theo hướng đã tính
                bulletScript.SetDirection(direction);
                currentAmo--;
            }
        }
        void Reload()
        {
            if (Input.GetMouseButtonDown(1) && currentAmo <= 0)
            {
                currentAmo = maxAmo;
            }
        }
    }
}

