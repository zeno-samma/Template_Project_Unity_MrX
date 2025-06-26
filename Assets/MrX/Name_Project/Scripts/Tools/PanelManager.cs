using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelManager : MonoBehaviour
{
    
    private Dictionary<string, Panel> panels = new Dictionary<string, Panel>();
    private bool initialized = false;
    // private Canvas[] canvas = null;
    private static PanelManager singleton = null;
    
    public static PanelManager Singleton
    {
        get
        {
            if (singleton == null)
            {
                singleton = FindFirstObjectByType<PanelManager>();
                if (singleton == null)
                {
                    singleton = new GameObject("PanelManager").AddComponent<PanelManager>();
                }
                singleton.Initialize();
            }
            return singleton; 
        }
    }

    private void Initialize()
    {
        if (initialized) return;
        initialized = true;
        
        panels.Clear();
        // Tìm tất cả các Panel trong project, kể cả những cái đang bị tắt
        var allPanels = FindObjectsByType<Panel>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (var panel in allPanels)
        {
            if (!string.IsNullOrEmpty(panel.ID) && !panels.ContainsKey(panel.ID))
            {
                panels.Add(panel.ID, panel);
                panel.Initialize();
                panel.Close(); // Đóng tất cả các panel khi bắt đầu để đảm bảo trạng thái sạch
            }
        }
    }
    
    private void OnDestroy()
    {
        if (singleton == this)
        {
            singleton = null;
        }
    }
    
    public static Panel Get(string id)
    {
        if (Singleton.panels.ContainsKey(id))
        {
            return Singleton.panels[id];
        }
        return null;
    }
    
    public static void Open(string id)
    {
        var panel = Get(id);
        if (panel != null)
        {
            panel.Open();
        }
    }
    // --- HÀM MỚI QUAN TRỌNG ---
    // Hàm này sẽ thay thế cho Open() và Close() riêng lẻ
    public static void Show(string id)
    {
        // Đóng tất cả các panel khác
        foreach (var panel in Singleton.panels)
        {
            if (panel.Key != id && panel.Value.IsOpen)
            {
                panel.Value.Close();
            }
        }

        // Mở panel được yêu cầu
        var panelToShow = Get(id);
        if (panelToShow != null)
        {
            panelToShow.Open();
        }
        else
        {
            Debug.LogWarning($"PanelManager: Panel with ID '{id}' not found!");
        }
    }
    public static void Close(string id)
    {
        var panel = Get(id);
        if (panel != null)
        {
            panel.Close();
        }
    }
    
    public static bool IsOpen(string id)
    {
        if (Singleton.panels.ContainsKey(id))
        {
            return Singleton.panels[id].IsOpen;
        }
        return false;
    }
    
    public static void CloseAll()
    {
        foreach (var panel in Singleton.panels)
        {
            if (panel.Value != null)
            {
                panel.Value.Close();
            }
        }
    }
    
}