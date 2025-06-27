
using System;
using System.Threading.Tasks;
using TMPro;
using Unity.Services.Economy;
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
            buyHelmetButton.onClick.AddListener(OnBuyItemClicked);
            addGoldButton.onClick.AddListener(OnAddGoldClicked);
            closeButton.onClick.AddListener(ClosePanel);
        }

        public override async void Open()
        {
            base.Open();
            await RefreshUI();
        }
        // ===========================================
        private async void OnBuyItemClicked()
        {
            try
            {
                var result = await economyManager.MakePurchaseAsync(EconomyConst.PURCHASE_ITEM_SWORD_BASIC);
                Debug.Log("✅ Purchase successful!");
                await RefreshUI();
            }
            catch (EconomyException e) when (e.ErrorCode == 10504) // Không đủ tiền
            {
                Debug.LogWarning("❌ Not enough currency!");
                var errorPanel = (ErrorMenu)PanelManager.Get("error");
                errorPanel.Open(ErrorMenu.Action.None, "You do not have enough currency!", "OK");
            }
            catch (EconomyException e)
            {
                Debug.LogWarning($"❌ Economy error: {e.Message}");
                var errorPanel = (ErrorMenu)PanelManager.Get("error");
                errorPanel.Open(ErrorMenu.Action.None, "Purchase failed (economy error).", "OK");
            }
            catch (Exception e)
            {
                Debug.LogError($"❌ Unknown error: {e.Message}");
                var errorPanel = (ErrorMenu)PanelManager.Get("error");
                errorPanel.Open(ErrorMenu.Action.None, "Unknown error occurred.", "OK");
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
                    Debug.Log($"4. Đang kiểm tra currency ID: '{currency.CurrencyId}' với số dư: {currency.Balance}");
                    if (currency.CurrencyId == EconomyConst.DIAMOND)//ID
                    {
                        goldBalanceText.text = $"Diamond: {currency.Balance}";
                    }
                    else
                    {
                        Debug.Log($"LỖI: {currency.CurrencyId}!");
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