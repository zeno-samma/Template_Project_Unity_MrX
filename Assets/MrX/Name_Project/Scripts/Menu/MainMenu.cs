using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Services.Authentication;
using UnityEngine.UI;
using Unity.Services.Friends;

public class MainMenu : Panel
{

    [SerializeField] public TextMeshProUGUI nameText = null;
    [SerializeField] private Button logoutButton = null;
    [SerializeField] private Button leaderboardsButton = null;
    [SerializeField] private Button friendsButton = null;

    private bool isFriendsServiceInitialized = false;

    public override void Initialize()
    {
        if (IsInitialized)
        {
            return;
        }
        logoutButton.onClick.AddListener(SignOut);
        leaderboardsButton.onClick.AddListener(Leaderboards);
        friendsButton.onClick.AddListener(Friends);
        base.Initialize();
    }

    public override void Open()
    {
        friendsButton.interactable = isFriendsServiceInitialized;
        UpdatePlayerNameUI();
        if (isFriendsServiceInitialized == false)
        {
            InitializeFriendsServiceAsync();
        }
        base.Open();
    }

    private async void InitializeFriendsServiceAsync()
    {
        try
        {
            await FriendsService.Instance.InitializeAsync();
            isFriendsServiceInitialized = true;
            friendsButton.interactable = true;
        }
        catch (Exception exception)
        {
            Debug.Log(exception.Message);
        }
    }

    private void SignOut()
    {
        MenuManager.Singleton.SignOut();
        isFriendsServiceInitialized = false;

        // Dùng Show() để chuyển về màn hình đăng nhập
        PanelManager.Show("auth");
    }

    private void UpdatePlayerNameUI()
    {
        nameText.text = AuthenticationService.Instance.PlayerName;
    }

    private void Leaderboards()
    {
        // Giờ đây chỉ cần một dòng lệnh duy nhất, an toàn và rõ ràng
        PanelManager.Show("leaderboards");
    }

    private void Friends()
    {
        // Tương tự, chỉ cần gọi Show()
        PanelManager.Show("friends");
    }

}