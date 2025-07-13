using UnityEngine;

namespace MrX.Name_Project
{
    // Ví dụ một sự kiện không chứa dữ liệu
    public struct GameStartedEvent { }
    public struct PlayerDiedEvent{}
    public struct StateUpdatedEvent
    {
        public GameManager.GameState CurState;
    }
    public struct PlayerHealthChangedEvent
    {
        public float NewHealthPercentage;
    }
    public struct EnemyDiedEvent
    {
        public int diecoin;
    }																																																																																																																																																																	 
    public struct InitialUIDataReadyEvent
    {
        // public int defHealth;
        // public int maxHealth;
        // public int defScore;
    }
    
}