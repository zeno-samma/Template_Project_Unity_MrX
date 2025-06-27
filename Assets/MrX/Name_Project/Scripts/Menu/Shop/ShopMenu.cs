
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
            // buyHelmetButton.onClick.AddListener(OnBuyHelmetClicked);
            addGoldButton.onClick.AddListener(OnAddGoldClicked);
            closeButton.onClick.AddListener(ClosePanel);
        }

        public override async void Open()
        {
            base.Open();
            await RefreshUI();
        }
        // ===========================================
        // Sửa lại hàm này để nhận vào ID, giúp tái sử dụng cho nhiều vật phẩm
        // private async void OnBuyItemClicked(string purchaseId)
        // {
        //     Debug.Log($"Attempting to purchase: {purchaseId}");
        //     // Có thể hiện panel loading ở đây
        //     // PanelManager.Show("loading");

        //     try
        //     {
        //         // Gọi hàm mua hàng từ EconomyManager
        //         var purchaseResult = await economyManager.MakePurchaseAsync(purchaseId);

        //         // Code dưới đây chỉ chạy khi giao dịch THÀNH CÔNG
        //         Debug.Log("Purchase successful!");
        //         await RefreshUI(); // Cập nhật lại UI
        //     }
        //     catch (EconomyException e)
        //     {
        //         // BƯỚC QUAN TRỌNG: BẮT LỖI KHÔNG ĐỦ TIỀN
        //         // Đây là "cái bẫy" đặc biệt chỉ dành cho lỗi "không đủ tiền".
        //         // Từ khóa "when" giúp chúng ta lọc chính xác lý do lỗi.
        //         Debug.LogWarning("Player does not have enough currency to make this purchase.");

        //         // Hiển thị thông báo thân thiện cho người chơi
        //         var errorPanel = (ErrorMenu)PanelManager.Get("error");
        //         errorPanel.Open(ErrorMenu.Action.None, "You do not have enough Gold!", "OK");
        //     }
        //     catch (Exception e)
        //     {
        //         // Đây là "cái bẫy" cuối cùng, bắt tất cả các loại lỗi khác
        //         // (mất mạng, ID sai sau khi đã sửa, lỗi server...)
        //         Debug.LogError($"Purchase failed for other reason: {e.Message}");

        //         var errorPanel = (ErrorMenu)PanelManager.Get("error");
        //         errorPanel.Open(ErrorMenu.Action.None, "Purchase failed. Please try again later.", "OK");
        //     }
        //     finally
        //     {
        //         // (Tùy chọn nhưng nên có)
        //         // Luôn ẩn panel loading ở đây để đảm bảo game không bị kẹt
        //         // PanelManager.Close("loading");
        //     }
        // }

        // Sửa lại hàm gọi sự kiện của nút Mũ Sắt
        // private void OnBuyHelmetClicked()
        // {
        //     // Gọi hàm chung với ID cụ thể
        //     OnBuyItemClicked(EconomyConst.PURCHASE_ITEM_SWORD_BASIC);
        // }

        // // Bạn cũng có thể dùng hàm chung này cho nút mua Kiếm
        // private void OnBuySwordClicked() // Giả sử bạn có hàm này
        // {
        //     OnBuyItemClicked(EconomyConst.PURCHASE_ITEM_SWORD_BASIC);
        // }

        // =================================================

        // private async void OnBuyHelmetClicked()
        // {
        //     Debug.Log("Attempting to buy helmet...");
        //     var result = await economyManager.MakePurchaseAsync(EconomyConst.PURCHASE_ITEM_SWORD_BASIC);//ID

        //     Debug.Log(EconomyConst.PURCHASE_ITEM_SWORD_BASIC);
        //     Debug.Log(result);
        //     if (result != null)
        //     {
        //         Debug.Log("Purchase successful!");
        //         await RefreshUI(); // Cập nhật lại UI sau khi mua thành công
        //     }
        //     else if (result == null)
        //     {
        //         Debug.Log($"Purchase failed!{result}");
        //     }
        // }
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