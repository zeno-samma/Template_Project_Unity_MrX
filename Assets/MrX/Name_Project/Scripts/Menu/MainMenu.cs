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
    [SerializeField] private Button renameButton = null;
    [SerializeField] private Button customizationButton = null;
    [SerializeField] private Button lobbyButton = null;
    [SerializeField] private Button shopButton = null;
    [SerializeField] private Button inventoryButton = null;

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
        renameButton.onClick.AddListener(RenamePlayer);
        customizationButton.onClick.AddListener(Customization);
        lobbyButton.onClick.AddListener(Lobby);
        shopButton.onClick.AddListener(Shop);
        inventoryButton.onClick.AddListener(Inventory);
        base.Initialize();
    }

    private void Lobby()
    {
        PanelManager.Open("lobby");
    }

    private void Inventory()
    {
        PanelManager.Open("inventory");
    }

    private void Shop()
    {
        PanelManager.Open("shop");
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

    private void Customization()
    {
        PanelManager.Open("customization");
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
        // MenuManager.Singleton.SignOut();
        // isFriendsServiceInitialized = false;

        // // Dùng Show() để chuyển về màn hình đăng nhập
        // PanelManager.Show("auth");
        ActionConfirmMenu panel = (ActionConfirmMenu)PanelManager.Get("action_confirm");
        panel.Open(SignOutResult, "Are you sure that you want to sign out?", "Yes", "No");
    }

    private void SignOutResult(ActionConfirmMenu.Result result)
    {
        if (result == ActionConfirmMenu.Result.Positive)
        {
            MenuManager.Singleton.SignOut();
            isFriendsServiceInitialized = false;
        }
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

    private void RenamePlayer()
    {
        GetInputMenu panel = (GetInputMenu)PanelManager.Get("input");
        panel.Open(RenamePlayerConfirm, GetInputMenu.Type.String, 20, "Enter a name for your account.", "Send", "Cancel");
    }

    private async void RenamePlayerConfirm(string input)
    {
        renameButton.interactable = false;
        try
        {
            await AuthenticationService.Instance.UpdatePlayerNameAsync(input);
            UpdatePlayerNameUI();
        }
        catch
        {
            ErrorMenu panel = (ErrorMenu)PanelManager.Get("error");
            panel.Open(ErrorMenu.Action.None, "Failed to change the account name.", "OK");
        }
        renameButton.interactable = true;
    }

}