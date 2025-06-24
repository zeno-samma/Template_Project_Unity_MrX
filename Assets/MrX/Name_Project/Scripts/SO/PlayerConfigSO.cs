using UnityEngine;

namespace MrX.Name_Project
{
    [CreateAssetMenu(fileName = "PlayerConfig", menuName = "Survivor/Player Config")]
    public class PlayerConfigSO : ScriptableObject
    {
        public float initialMoveSpeed = 5f;
        public int initialHealth = 100;
        public float initialDamage = 10f;
        public float initialCooldown = 2f;
        public int healthBonusPerLevel = 50;
        public float damageBonusPerLevel = 2f;
        public float cooldownReductionPerLevel = 0.2f;
        // Có thể thêm prefab của người chơi ở đây nếu muốn
        // public GameObject playerPrefab;
    }
}
