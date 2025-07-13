using UnityEngine;
using UnityEngine.UI;

namespace MrX.Name_Project
{
    [RequireComponent(typeof(Image))]
    public class PlayerHealthUI : MonoBehaviour
    {
        private Image healthBarImage;
        void Awake()
        {
            healthBarImage = GetComponent<Image>();
        }
        private void OnEnable()
        {
            EventBus.Subscribe<PlayerHealthChangedEvent>(UpdateHealthBar);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<PlayerHealthChangedEvent>(UpdateHealthBar);
        }

        private void UpdateHealthBar(PlayerHealthChangedEvent e)
        {
            // Cập nhật fillAmount dựa trên dữ liệu từ sự kiện
            healthBarImage.fillAmount = e.NewHealthPercentage;
        }
    }

}
