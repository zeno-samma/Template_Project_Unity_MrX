using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Services.Friends;
using Unity.Services.Friends.Models;
using UnityEngine.UI;

public class FriendRequestSentItem : MonoBehaviour
{

    [SerializeField] public TextMeshProUGUI nameText = null;
    [SerializeField] private Button deleteButton = null;
    
    private string id = "";
    private string memberId = "";
    
    private void Start()
    {
        deleteButton.onClick.AddListener(DeleteRequest);
    }
    
    public void Initialize(Relationship relationship)
    {
        memberId = relationship.Member.Id;
        id = relationship.Id;
        nameText.text = relationship.Member.Profile.Name;
    }
    
    private async void DeleteRequest()
    {
        deleteButton.interactable = false;
        try
        {
            await FriendsService.Instance.DeleteOutgoingFriendRequestAsync(id);
            Destroy(gameObject);
        }
        catch
        {
            deleteButton.interactable = true;
            ErrorMenu panel = (ErrorMenu)PanelManager.Get("error");
            panel.Open(ErrorMenu.Action.None, "Failed to delete friend request.", "OK");
        }
    }
    
}