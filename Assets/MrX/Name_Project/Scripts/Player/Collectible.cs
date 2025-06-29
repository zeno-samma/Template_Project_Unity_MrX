using Unity.Netcode;
using UnityEngine;

namespace MrX.Name_Project
{
    public class Collectible : NetworkBehaviour
    {
        [Header("Collectible Settings")]
        // [SerializeField] private int pointValue = 10;
        [SerializeField] private float rotationSpeed = 90f;
        [SerializeField] private float bobSpeed = 2f;
        [SerializeField] private float bobHeight = 0.5f;
        
        private Vector3 startPosition;
        private float bobTimer;
        
        public override void OnNetworkSpawn()
        {
            startPosition = transform.position;
            bobTimer = Random.Range(0f, 2f * Mathf.PI); // Random start phase
        }
        
        private void Update()
        {
            // Xoay collectible
            transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
            
            // Hiệu ứng nổi lên xuống
            bobTimer += bobSpeed * Time.deltaTime;
            float bobOffset = Mathf.Sin(bobTimer) * bobHeight;
            transform.position = startPosition + Vector3.up * bobOffset;
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                // Người chơi thu thập item
                PlayerController player = other.GetComponent<PlayerController>();
                if (player != null)
                {
                    // Gọi ServerRpc từ client để tăng điểm
                    if (player.IsOwner)
                    {
                        player.CollectItemServerRpc(GetComponent<NetworkObject>().NetworkObjectId);
                    }
                    
                    // Server sẽ xóa collectible
                    if (IsServer)
                    {
                        NetworkObject networkObject = GetComponent<NetworkObject>();
                        if (networkObject != null)
                        {
                            networkObject.Despawn();
                        }
                    }
                }
            }
        }
    }
} 