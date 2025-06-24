using UnityEngine;

namespace MrX.Name_Project
{
    [CreateAssetMenu(fileName = "WeaponConfig", menuName = "Survivor/Weapon Config")]
    public class WeaponConfigSO : ScriptableObject
    {
        public string weaponName = "Basic Wand";
        public float fireRate = 1.0f; // Thời gian hồi giữa mỗi lần bắn (giây)

        [Header("Projectile")]
        public GameObject projectilePrefab; // Prefab của viên đạn
        public float projectileSpeed = 8f;
        public float projectileDamage = 10f;
        public float projectileLifetime = 3f; // Thời gian đạn tồn tại trước khi biến mất
    }
}