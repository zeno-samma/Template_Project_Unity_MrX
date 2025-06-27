using System.Threading.Tasks;
using TMPro;
using Unity.Services.Economy;
using UnityEngine;
using UnityEngine.UI;

namespace MrX.Name_Project
{
    public class InventoryMenu : Panel
    {
        [Header("UI References")]
        [SerializeField] private Button closeButton;
        [SerializeField] private RectTransform itemContainer; // Khu vực chứa danh sách item (cần có Vertical Layout Group)
        [SerializeField] private GameObject itemPrefab; // Kéo Prefab bạn tạo ở Bước 1 vào đây
        [SerializeField] private TextMeshProUGUI TxtGoldCounting; // Kéo Prefab bạn tạo ở Bước 1 vào đây

        private EconomyManager economyManager;

        public override void Initialize()
        {
            if (IsInitialized)
            {
                return;
            }
            closeButton.onClick.AddListener(ClosePanel);
            economyManager = FindFirstObjectByType<EconomyManager>();
            base.Initialize();
        }   // Khi panel này được mở, tự động làm mới danh sách
        public override async void Open()
        {
            base.Open();
            await RefreshInventoryDisplay();
            await RefreshUI();
        }
        private async Task RefreshInventoryDisplay()
        {
            // Bước A: Dọn dẹp danh sách cũ
            ClearInventoryDisplay();

            // Bước B: Gọi EconomyManager để lấy dữ liệu túi đồ
            var inventoryResult = await economyManager.GetPlayerInventoryAsync();

            if (inventoryResult != null && inventoryResult.PlayersInventoryItems != null)
            {
                Debug.Log($"Found {inventoryResult.PlayersInventoryItems.Count} items in inventory.");

                // Bước C: Tạo các đối tượng UI cho mỗi vật phẩm
                foreach (var item in inventoryResult.PlayersInventoryItems)
                {
                    // Tạo một instance mới từ Prefab
                    GameObject itemInstance = Instantiate(itemPrefab, itemContainer);

                    // Lấy script và gán tên cho nó
                    var itemUI = itemInstance.GetComponent<InventoryItemUI>();
                    if (itemUI != null)
                    {
                        // item.InventoryItemId sẽ là "HAT_IRON", v.v...
                        itemUI.SetItemName(item.InventoryItemId);
                    }
                }
            }
        }
        private async Task RefreshUI()
        {
            var balances = await economyManager.GetPlayerBalancesAsync();
            if (balances != null)
            {
                foreach (var currency in balances.Balances)
                {
                    if (currency.CurrencyId == EconomyConst.ID_GOLD_CURRENCY)//ID
                    {
                        TxtGoldCounting.text = $"Gold: {currency.Balance}";
                    }
                }
            }
        }
        private void ClearInventoryDisplay()
        {
            foreach (Transform child in itemContainer)
            {
                Destroy(child.gameObject);
            }
        }

        private void ClosePanel()
        {
            Close();
        }
    }

}
