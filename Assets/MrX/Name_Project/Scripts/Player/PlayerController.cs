using Unity.Netcode;
using UnityEngine;
using TMPro;
using Unity.Collections;

namespace MrX.Name_Project
{
    public class PlayerController : NetworkBehaviour
    {
        [Header("Player Settings")]
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private Color playerColor = Color.blue;
        
        [Header("UI References")]
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private TextMeshProUGUI nameText;
        
        private NetworkVariable<int> networkScore = new NetworkVariable<int>(0);
        private NetworkVariable<FixedString32Bytes> networkPlayerName = new NetworkVariable<FixedString32Bytes>("Player");
        private NetworkVariable<Vector3> networkPosition = new NetworkVariable<Vector3>();
        private NetworkVariable<Color> networkColor = new NetworkVariable<Color>();
        
        private Rigidbody2D rb;
        private SpriteRenderer spriteRenderer;
        private Vector2 moveInput;
        
        public int Score => networkScore.Value;
        public string PlayerName => networkPlayerName.Value.ToString();
        
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            
            rb = GetComponent<Rigidbody2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            
            // Đăng ký lắng nghe thay đổi network variables
            networkScore.OnValueChanged += OnScoreChanged;
            networkPlayerName.OnValueChanged += OnPlayerNameChanged;
            networkColor.OnValueChanged += OnColorChanged;
            networkPosition.OnValueChanged += OnPositionChanged;
            
            if (IsOwner)
            {
                // Thiết lập tên người chơi
                string playerName = Unity.Services.Authentication.AuthenticationService.Instance.PlayerName;
                SetPlayerNameServerRpc(playerName);
                
                // Thiết lập màu ngẫu nhiên
                Color randomColor = new Color(
                    Random.Range(0.5f, 1f),
                    Random.Range(0.5f, 1f),
                    Random.Range(0.5f, 1f)
                );
                SetPlayerColorServerRpc(randomColor);
                
                // Đăng ký với GameManager
                if (GameManager.Ins != null)
                {
                    GameManager.Ins.RegisterPlayer(OwnerClientId, this);
                }
            }
            
            // Cập nhật UI
            UpdateUI();
        }
        
        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            
            // Hủy đăng ký lắng nghe
            networkScore.OnValueChanged -= OnScoreChanged;
            networkPlayerName.OnValueChanged -= OnPlayerNameChanged;
            networkColor.OnValueChanged -= OnColorChanged;
            networkPosition.OnValueChanged -= OnPositionChanged;
            
            if (IsOwner && GameManager.Ins != null)
            {
                GameManager.Ins.UnregisterPlayer(OwnerClientId);
            }
        }
        
        private void Update()
        {
            if (!IsOwner) return;
            
            // Đọc input
            float moveX = Input.GetAxisRaw("Horizontal");
            float moveY = Input.GetAxisRaw("Vertical");
            moveInput = new Vector2(moveX, moveY).normalized;
        }
        
        private void FixedUpdate()
        {
            if (!IsOwner) return;
            
            // Di chuyển
            rb.linearVelocity = moveInput * moveSpeed;
            
            // Cập nhật vị trí network
            UpdatePositionServerRpc(transform.position);
        }
        
        private void UpdateUI()
        {
            if (scoreText != null)
                scoreText.text = $"Score: {networkScore.Value}";
            
            if (nameText != null)
                nameText.text = networkPlayerName.Value.ToString();
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!IsOwner) return;
            
            if (other.CompareTag("Collectible"))
            {
                // Thu thập item
                CollectItemServerRpc(other.GetComponent<NetworkObject>().NetworkObjectId);
            }
        }
        
        [ServerRpc]
        private void SetPlayerNameServerRpc(string name)
        {
            networkPlayerName.Value = new FixedString32Bytes(name);
        }
        
        [ServerRpc]
        private void SetPlayerColorServerRpc(Color color)
        {
            networkColor.Value = color;
        }
        
        [ServerRpc]
        private void UpdatePositionServerRpc(Vector3 position)
        {
            networkPosition.Value = position;
        }
        
        [ServerRpc]
        public void CollectItemServerRpc(ulong itemNetworkId)
        {
            // Tăng điểm
            networkScore.Value += 10;
            
            // Xóa item
            if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(itemNetworkId, out NetworkObject networkObject))
            {
                networkObject.Despawn();
            }
        }
        
        private void OnScoreChanged(int previousValue, int newValue)
        {
            UpdateUI();
        }
        
        private void OnPlayerNameChanged(FixedString32Bytes previousValue, FixedString32Bytes newValue)
        {
            UpdateUI();
        }
        
        private void OnColorChanged(Color previousValue, Color newValue)
        {
            if (spriteRenderer != null)
                spriteRenderer.color = newValue;
        }
        
        private void OnPositionChanged(Vector3 previousValue, Vector3 newValue)
        {
            if (!IsOwner)
                transform.position = newValue;
        }
    }
} 