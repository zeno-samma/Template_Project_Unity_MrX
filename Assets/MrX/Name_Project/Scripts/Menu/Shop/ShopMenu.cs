
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MrX.Name_Project
{


    public class ShopMenu : Panel // Giả sử kế thừa từ Panel của bạn
    {
        [SerializeField] private Button buyHelmetButton;
        [SerializeField] private TextMeshProUGUI goldBalanceText;
        [SerializeField] private Button addGoldButton;
        [SerializeField] private Button closeButton = null;

        private EconomyManager economyManager;

        public override void Initialize()
        {
            if (IsInitialized)
            {
                return;
            }
            base.Initialize();
            economyManager = FindFirstObjectByType<EconomyManager>(); // Tìm đến EconomyManager
            buyHelmetButton.onClick.AddListener(OnBuyHelmetClicked);
            addGoldButton.onClick.AddListener(OnAddGoldClicked);
            closeButton.onClick.AddListener(ClosePanel);
        }

        public override async void Open()
        {
            base.Open();
            await RefreshUI();
        }

        private async void OnBuyHelmetClicked()
        {
            Debug.Log("Attempting to buy helmet...");
            var result = await economyManager.MakePurchaseAsync(EconomyConst.ID_PURCHASE_ITEM_SWORD);//ID
            if (result != null)
            {
                Debug.Log("Purchase successful!");
                await RefreshUI(); // Cập nhật lại UI sau khi mua thành công
            }
            else
            {
                Debug.Log("Purchase failed!");
            }
        }
        // --- HÀM MỚI ĐỂ XỬ LÝ CLICK NÚT THÊM VÀNG ---
        private async void OnAddGoldClicked()
        {
            if (economyManager == null) return;

            // Gọi hàm từ EconomyManager để cộng 100 vàng (ví dụ)
            await economyManager.GrantGoldAsync(10);

            // Sau khi cộng xong, cập nhật lại UI để hiển thị số vàng mới
            await RefreshUI();
        }

        private async Task RefreshUI()
        {
            var balances = await economyManager.GetPlayerBalancesAsync();
            if (balances != null)
            {
                Debug.Log($"3. Phản hồi thành công, có {balances.Balances.Count} loại tiền tệ.");
                foreach (var currency in balances.Balances)
                {
                    // Debug.Log($"4. Đang kiểm tra currency ID: '{currency.CurrencyId}' với số dư: {currency.Balance}");
                    if (currency.CurrencyId == EconomyConst.ID_GOLD_CURRENCY)//ID
                    {
                        goldBalanceText.text = $"Gold: {currency.Balance}";
                    }
                    else
                    {
                        // Debug.Log($"LỖI: {currency.CurrencyId}!");
                    }
                }
            }
            else
            {
                // Debug.LogError("Lỗi: balancesResult là null! Không thể lấy được số dư.");
            }

            // Tương tự, bạn có thể gọi GetPlayerInventoryAsync và hiển thị các vật phẩm
        }
        private void ClosePanel()
        {
            Close();
        }
    }
}