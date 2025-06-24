using UnityEngine;

namespace MrX.Name_Project
{
    public class Player : MonoBehaviour
    {
        public PlayerConfigSO playerConfig; //

        // Dữ liệu động của người chơi
        private int healthLevel;
        private float damageLevel;
        private float cooldownLevel;
        private int currentGold;

        // --- Các thuộc tính (Properties) để tính toán chỉ số cuối cùng ---
        public float MaxHealth => playerConfig.initialHealth + (playerConfig.healthBonusPerLevel * healthLevel);
        public float CurrentDamage => playerConfig.initialDamage + (playerConfig.damageBonusPerLevel * damageLevel);
        public float CurrentCooldown => playerConfig.initialCooldown - (playerConfig.cooldownReductionPerLevel * cooldownLevel);

        // Hàm này sẽ được GameManager gọi khi load game xong
        public void ApplyLoadedData(PlayerData data)
        {
            healthLevel = data.healthUpgradeLevel;
            damageLevel = data.damageUpgradeLevel;
            cooldownLevel = data.cooldownUpgradeLevel;
            currentGold = data.gold;

            Debug.Log("Player data applied. Current Damage: " + CurrentDamage);
        }

        // Hàm này được GameManager gọi trước khi save game
        public PlayerData GetDataToSave()
        {
            PlayerData data = new PlayerData();
            data.healthUpgradeLevel = healthLevel;
            data.damageUpgradeLevel = damageLevel;
            data.cooldownUpgradeLevel = cooldownLevel;
            data.gold = currentGold;
            return data;
        }

        // Ví dụ về việc nâng cấp
        public void UpgradeHealth()
        {
            // (Kiểm tra xem có đủ vàng không...)
            healthLevel++;
            // (Trừ vàng...)
            // << BÁO HIỆU CHO GAMEMANAGER >>
            // GameManager.Ins.SaveGame();//Dùng cho ít lần thay đổi và các thay đổi quan trọng(Qua một chương, hoàn thành được thành tựu)
            // GameManager.Ins.MarkDataAsDirty();//Dùng cho trường hợp nhặt liên tục 10 coins
        }

    }
}

