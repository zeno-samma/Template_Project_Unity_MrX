using UnityEngine;

namespace MrX.Name_Project
{
    [CreateAssetMenu(fileName = "EnemyConfig", menuName = "Survivor/Enemy Config")]
    public class EnemyConfigSO : ScriptableObject
    {
        public string enemyName = "Slime";
        public float moveSpeed = 2f;
        public float health = 10f;
        public float damage = 5f; // Sát thương khi va chạm vào người chơi
        public int goldDropped = 1;
        // public Sprite enemySprite;
    }
}
