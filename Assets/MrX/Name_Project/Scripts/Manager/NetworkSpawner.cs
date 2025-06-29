using Unity.Netcode;
using UnityEngine;

namespace MrX.Name_Project
{
    public class NetworkSpawner : NetworkBehaviour
    {
        [Header("Spawn Settings")]
        [SerializeField] private GameObject playerPrefab;
        [SerializeField] private Transform[] spawnPoints;
        
        public override void OnNetworkSpawn()
        {
            if (IsServer)
            {
                // Server sẽ spawn người chơi khi có client join
                NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            }
        }
        
        public override void OnNetworkDespawn()
        {
            if (IsServer)
            {
                NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
            }
        }
        
        private void OnClientConnected(ulong clientId)
        {
            // Spawn người chơi cho client mới
            SpawnPlayer(clientId);
        }
        
        private void SpawnPlayer(ulong clientId)
        {
            if (playerPrefab == null || spawnPoints.Length == 0) return;
            
            // Chọn spawn point ngẫu nhiên
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            
            // Spawn người chơi
            GameObject player = Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);
            NetworkObject networkObject = player.GetComponent<NetworkObject>();
            
            if (networkObject != null)
            {
                networkObject.SpawnAsPlayerObject(clientId);
            }
            
            Debug.Log($"Spawned player for client {clientId} at {spawnPoint.position}");
        }
        
        // Hàm để spawn người chơi cho host (nếu cần)
        public void SpawnHostPlayer()
        {
            if (IsServer && NetworkManager.Singleton.IsHost)
            {
                SpawnPlayer(NetworkManager.Singleton.LocalClientId);
            }
        }
    }
} 