using UnityEngine;

namespace MrX.Name_Project
{
    [CreateAssetMenu(fileName = "PlayerConfig", menuName = "Survivor/Player Config")]
    public class PlayerConfigSO : ScriptableObject
    {
        public float moveSpeed = 5f;
        public float maxHealth = 100f;
        // Có thể thêm prefab của người chơi ở đây nếu muốn
        // public GameObject playerPrefab;
    }
}
