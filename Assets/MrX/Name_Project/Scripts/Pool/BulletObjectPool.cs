using System;
using MRX.DefenseGameV1;
using UnityEngine;

namespace MrX.Name_Project
{
    public class BulletObjectPool : MonoBehaviour
    {
        public static BulletObjectPool Ins;
        public float speed;//Private sẽ không chạy  
        [SerializeField] public int maxHealth;
        private int currentHealth;

        // Event phát đi tỷ lệ máu (0.0 -> 1.0) khi máu thay đổi
        // public event Action<float> OnHealthChanged;
        // Event phát đi khi chết
        // public event Action OnDied;
        public int minCoinBonus;
        public int maxCoinBonus;
        private Rigidbody2D m_rb;
        private Animator m_anim;
        private bool m_canMove = true;
        private bool isDead;
        public bool IsComponentNull()
        {
            return m_rb == null;
        }
        private void OnEnable()
        {
            // Debug.Log("Reset kiếp sống" + gameObject.name);
            // ==================Rất quan trọng reset kiếp sống của một objectpool
            // Thông báo cho UI cập nhật lại thanh máu đầy
            currentHealth = maxHealth;
            m_canMove = true;
            if (m_anim != null)
            {
                // m_anim.SetBool(Const.ATTACK_ANIM, false);
            }
            // ===========================================================
            transform.position = new Vector3(8f, -1f, 0f);
        }
        private void OnDisable()
        {
        }
        private void Awake()
        {
            Ins = this;
            m_anim = GetComponent<Animator>();
            m_rb = GetComponent<Rigidbody2D>();
        }
        private void Start()
        {
            // gameObject.SetActive(false);
        }

        // Update is called once per frame
        private void Update()
        {
            if (m_canMove)
            {
                transform.position += Vector3.left * speed * Time.deltaTime; // Di chuyển đối tượng về bên trái
            }
            else if (!m_canMove)
            {

            }

        }
        private void OnCollisionStay2D(Collision2D colTarget)
        {
            if (IsComponentNull() || isDead) return;
            // Debug.Log("colTarget" + colTarget.gameObject.name);
            if (colTarget.gameObject.CompareTag(Const.PLAYER_TAG) && !isDead)//So sánh va chạm tag player
            {
                m_canMove = false;
                // m_anim.SetBool(Const.ATTACK_ANIM, true);

            }
        }
        /// Trả về tỷ lệ máu hiện tại (từ 0.0 đến 1.0).
        public float GetHealthPercentage()
        {
            // Tránh lỗi chia cho 0 nếu maxHealth chưa được thiết lập
            if (maxHealth <= 0) return 0;

            return (float)currentHealth / maxHealth;
        }
        // Phương thức nhận sát thương từ bên ngoài
        public void TakeDamage(int damage)
        {
            // Debug.Log("TakeDamage: " + damage);
            if (currentHealth <= 0) return; // Nếu đã chết rồi thì không nhận thêm sát thương

            currentHealth -= damage;
            // Debug.Log("currentHealth " + currentHealth);
            // Đảm bảo máu không âm
            if (currentHealth < 0)
            {
                currentHealth = 0;
            }

            // Tính toán tỷ lệ máu còn lại
            // float healthPercentage = (float)currentHealth / maxHealth;
            // Debug.Log("currentHealth " + healthPercentage);
            // Phát event cho UI
            // OnHealthChanged?.Invoke(healthPercentage);

            // Kiểm tra nếu đã chết
            if (currentHealth == 0)
            {
                int coinBonus = UnityEngine.Random.Range(minCoinBonus, maxCoinBonus);
                // EventBus.Publish(new EnemyDiedEvent { dieScore = coinBonus });
                // Debug.Log("Chết");
                gameObject.SetActive(false);
            }
        }
    }
}
