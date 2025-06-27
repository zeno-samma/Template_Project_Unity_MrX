using TMPro;
using UnityEngine;

namespace MrX.Name_Project
{
    public class InventoryItemUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI itemNameText;

        // Hàm này sẽ được gọi để gán thông tin cho item
        public void SetItemName(string name)
        {
            if (itemNameText != null)
            {
                itemNameText.text = name;
            }
        }
    }
}