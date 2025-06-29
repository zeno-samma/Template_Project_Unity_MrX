# 🎮 Multiplayer Minigame Setup Guide

## 📋 Tổng quan
Minigame "Color Capture" - người chơi di chuyển để thu thập các item và ghi điểm. Hỗ trợ 2-4 người chơi.

## 🚀 Tính năng đã thêm

### 1. **Lobby System**
- ✅ Kick người chơi (Host only)
- ✅ Ready/Not Ready system
- ✅ Start Game button (Host only)
- ✅ Tự động kiểm tra tất cả người chơi đã sẵn sàng

### 2. **Minigame Features**
- ✅ Multiplayer movement với WASD/Arrow keys
- ✅ Thu thập items để ghi điểm
- ✅ Real-time leaderboard
- ✅ Game timer (2 phút)
- ✅ Random spawn points
- ✅ Color-coded players

## 🛠️ Setup Instructions

### 1. **Tạo Game Scene**
1. Tạo scene mới tên "GameScene"
2. Thêm NetworkManager vào scene
3. Cấu hình NetworkManager với Unity Transport

### 2. **Tạo Player Prefab**
1. Tạo GameObject với các components:
   - `PlayerController` script
   - `NetworkObject` component
   - `Rigidbody2D` (Kinematic)
   - `Collider2D` (IsTrigger = false)
   - `SpriteRenderer`
   - Tag: "Player"

2. Thêm vào NetworkManager's Player Prefab list

### 3. **Tạo Collectible Prefabs**
1. Tạo GameObject với các components:
   - `Collectible` script
   - `NetworkObject` component
   - `Collider2D` (IsTrigger = true)
   - `SpriteRenderer`
   - Tag: "Collectible"

2. Tạo nhiều prefab với màu sắc khác nhau

### 4. **Setup GameManager**
1. Thêm `GameManager` vào scene
2. Cấu hình:
   - Player Prefab
   - Spawn Points (Transform array)
   - Collectible Prefabs (GameObject array)
   - Game Duration (120 seconds)
   - Spawn Interval (3 seconds)

### 5. **Setup NetworkSpawner**
1. Thêm `NetworkSpawner` vào scene
2. Cấu hình:
   - Player Prefab
   - Spawn Points

### 6. **Setup UI**
1. Tạo Canvas với GameUI script
2. Cấu hình các UI elements:
   - Timer Text
   - Score Text
   - Player Name Text
   - Leaderboard Panel
   - Game Over Panel

### 7. **Build Settings**
1. Thêm scenes vào Build Settings:
   - LobbyScene (index 0)
   - GameScene (index 1)

## 🎯 Game Rules

### **Mục tiêu:**
- Thu thập càng nhiều items càng tốt trong 2 phút
- Item = 10 điểm
- Người chơi có điểm cao nhất thắng

### **Điều khiển:**
- WASD hoặc Arrow keys để di chuyển
- Tự động thu thập khi chạm vào items

### **Lobby Rules:**
- Host có thể kick người chơi
- Tất cả phải Ready để Start Game
- Chỉ Host có thể Start Game

## 🔧 Troubleshooting

### **Lỗi thường gặp:**
1. **"Scene not found"**: Kiểm tra Build Settings
2. **"NetworkObject not found"**: Đảm bảo prefab có NetworkObject
3. **"Player not spawning"**: Kiểm tra NetworkSpawner setup

### **Debug Tips:**
- Sử dụng NetworkManager's Network Scene Manager
- Kiểm tra Console cho error messages
- Đảm bảo Unity Services đã được setup

## 📁 File Structure
```
Scripts/
├── Menu/LobbyNetwork/
│   ├── LobbyManager.cs (Updated)
│   ├── LobbyUI.cs (Updated)
│   └── LobbyPlayerItem.cs
├── Player/
│   ├── PlayerController.cs (New)
│   ├── Collectible.cs (New)
│   └── PlayerMovement.cs (Existing)
├── Manager/
│   ├── GameManager.cs (Updated)
│   └── NetworkSpawner.cs (New)
└── UI/
    └── GameUI.cs (New)
```

## 🎮 Test Instructions

1. **Build và Run:**
   - Build game cho 2-4 instances
   - Chạy từng instance

2. **Test Flow:**
   - Instance 1: Tạo lobby
   - Instance 2-4: Join bằng code
   - Tất cả Ready
   - Host Start Game
   - Chơi minigame
   - Kiểm tra leaderboard

3. **Test Features:**
   - Kick functionality
   - Ready system
   - Start game
   - Movement
   - Item collection
   - Score tracking
   - Timer
   - Game over

## 🚀 Next Steps

### **Có thể thêm:**
- Power-ups
- Obstacles
- Different game modes
- Sound effects
- Particle effects
- More complex scoring system
- Team modes

### **Optimization:**
- Object pooling cho collectibles
- Network optimization
- UI performance
- Mobile support 