using UnityEngine;

namespace MrX.Name_Project
{
    // Ví dụ một sự kiện không chứa dữ liệu
    public struct GameStartedEvent { }
        // Sự kiện thông báo từ GameManager
    public struct StateUpdatedEvent
    {
        public GameManager.GameState CurState;
    }
    public struct InitialUIDataReadyEvent
    {
        // public int defHealth;
        // public int maxHealth;
        // public int defScore;
    }
    
}