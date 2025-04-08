using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GoalSaver : MonoBehaviour
{
    [SerializeField] private List<Toggle> toggles;
    [SerializeField] private List<string> toggleKeys;
    [SerializeField] private List<string> toggleDisplayNames;
    [SerializeField] private Button nextButton;
    [SerializeField] private GameObject goalOverlayPanel;
    [SerializeField] private Transform listContainer;
    [SerializeField] private GameObject listItemPrefab;
    [SerializeField] private Button closeOverlayButton;
    [SerializeField] private GameObject goalFallback;
    [SerializeField] private Button fallbackBackButton;

    private void Start()
    {
        InitializeKeys();
        InitializeDisplayNames();
        SetupButtons();
        LoadToggleStates();
        
        // Overlay und Fallback initial ausblenden
        goalOverlayPanel.SetActive(false);
        if (goalFallback != null) 
            goalFallback.SetActive(false);

        // Fallback-Button initial ausblenden
        if (fallbackBackButton != null)
            fallbackBackButton.gameObject.SetActive(false);
    }

    // Initialisiert die Toggle-Keys
    private void InitializeKeys()
    {
        if (toggleKeys == null || toggleKeys.Count < toggles.Count)
        {
            toggleKeys = new List<string>();
            for (int i = 0; i < toggles.Count; i++)
                toggleKeys.Add("Toggle_" + i);
        }
    }

    // Initialisiert die Display-Namen
    private void InitializeDisplayNames()
    {
        if (toggleDisplayNames == null || toggleDisplayNames.Count < toggles.Count)
        {
            toggleDisplayNames = new List<string>();
            for (int i = 0; i < toggles.Count; i++)
            {
                TextMeshProUGUI tmpText = toggles[i].GetComponentInChildren<TextMeshProUGUI>();
                toggleDisplayNames.Add(tmpText != null ? tmpText.text : toggles[i].name);
            }
        }
    }

    // Richtet alle Button-Listener ein
    // Richtet alle Button-Listener ein
    private void SetupButtons()
    {
        nextButton.onClick.AddListener(WeiterButtonClicked);
        
        if (closeOverlayButton != null)
            closeOverlayButton.onClick.AddListener(() => SceneManager.LoadScene("MainMenu"));
            
        // FallbackBackButton-Listener einrichten
        if (fallbackBackButton != null)
        {
            fallbackBackButton.onClick.RemoveAllListeners();
            fallbackBackButton.onClick.AddListener(() => {
                // Overlay, Fallback und Button ausblenden
                goalOverlayPanel.SetActive(false);
                if (goalFallback != null)
                    goalFallback.SetActive(false);
                fallbackBackButton.gameObject.SetActive(false);
            });
        }
    }

    // Speichert Toggles und zeigt Overlay
    private void WeiterButtonClicked()
    {
        SaveToggleStates();
        ShowGoalOverlay();
    }
    
    // Speichert die Toggle-Zustände
    private void SaveToggleStates()
    {
        for (int i = 0; i < toggles.Count; i++)
        {
            if (toggles[i] != null && i < toggleKeys.Count)
            {
                string key = toggleKeys[i];
                PlayerPrefs.SetInt(key, toggles[i].isOn ? 1 : 0);
            }
        }
        PlayerPrefs.Save();
        Debug.Log("Toggle-Zustände gespeichert");
    }

    // Zeigt das Overlay mit den ausgewählten Zielen an
    private void ShowGoalOverlay()
    {
        if (goalOverlayPanel == null || listContainer == null)
        {
            Debug.LogError("Fehlende UI-Komponenten!");
            return;
        }
        
        // Overlay einblenden
        goalOverlayPanel.SetActive(true);
        
        // Bestehende Liste leeren
        foreach (Transform child in listContainer)
            Destroy(child.gameObject);
        
        // Prüfen, ob Toggles ausgewählt sind und anzeigen
        bool anyToggleSelected = CreateToggleList();
        
        // Fallback anzeigen, falls nichts ausgewählt wurde
        // Fallback anzeigen, falls nichts ausgewählt wurde
        if (!anyToggleSelected)
        {
            // Fallback aktivieren
            if (goalFallback != null)
            {
                goalFallback.SetActive(true);
                
                // Fallback zum Container hinzufügen, falls noch nicht erfolgt
                if (goalFallback.transform.parent != listContainer)
                    goalFallback.transform.SetParent(listContainer, false);
            }
            
            // Unabhängigen Fallback-Button anzeigen
            if (fallbackBackButton != null)
                fallbackBackButton.gameObject.SetActive(true);
        }
        else
        {
            // Wenn Toggles ausgewählt sind, Fallback und Button ausblenden
            if (goalFallback != null)
                goalFallback.SetActive(false);
                
            if (fallbackBackButton != null)
                fallbackBackButton.gameObject.SetActive(false);
        }
    }
    
    // Erstellt die Liste der ausgewählten Toggles und gibt zurück, ob welche ausgewählt wurden
    private bool CreateToggleList()
    {
        bool anySelected = false;
        
        for (int i = 0; i < toggles.Count; i++)
        {
            if (toggles[i] != null && toggles[i].isOn)
            {
                anySelected = true;
                
                // Listenelement erstellen
                GameObject listItem = Instantiate(listItemPrefab, listContainer);
                
                // Text setzen
                if (i < toggleDisplayNames.Count)
                {
                    TextMeshProUGUI itemText = listItem.GetComponentInChildren<TextMeshProUGUI>();
                    if (itemText != null)
                        itemText.text = toggleDisplayNames[i];
                }
            }
        }
        
        return anySelected;
    }

    // Lädt gespeicherte Toggle-Zustände
    private void LoadToggleStates()
    {
        for (int i = 0; i < toggles.Count; i++)
        {
            if (toggles[i] != null && i < toggleKeys.Count)
            {
                string key = toggleKeys[i];
                toggles[i].isOn = PlayerPrefs.GetInt(key, 0) == 1;
            }
        }
    }

    // Hilfsmethode zum Abrufen der Toggle-Zustände für Notifications
    public List<string> GetSelectedToggleKeys()
    {
        List<string> selectedKeys = new List<string>();
        for (int i = 0; i < toggleKeys.Count; i++)
        {
            if (PlayerPrefs.GetInt(toggleKeys[i], 0) == 1)
                selectedKeys.Add(toggleKeys[i]);
        }
        return selectedKeys;
    }
}
