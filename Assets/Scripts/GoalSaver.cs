using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;
using UnityEngine.SceneManagement; // TextMeshPro

public class GoalSaver : MonoBehaviour
{
    [SerializeField] private List<Toggle> toggles;
    [SerializeField] private Button nextButton;
    [SerializeField] private List<string> toggleKeys;
    [SerializeField] private List<string> toggleDisplayNames;
    [SerializeField] private GameObject goalOverlayPanel;
    [SerializeField] private Transform listContainer;
    [SerializeField] private GameObject listItemPrefab;

    [SerializeField] private Button closeOverlayButton;

    private void Start()
    {
        // Genügend Keys generieren, falls nicht vorhanden
        if (toggleKeys == null || toggleKeys.Count < toggles.Count)
        {
            toggleKeys = new List<string>();
            for (int i = 0; i < toggles.Count; i++)
            {
                toggleKeys.Add("Toggle_" + i);
            }
        }

        // Display-Namen initialisieren, falls nötig
        if (toggleDisplayNames == null || toggleDisplayNames.Count < toggles.Count)
        {
            toggleDisplayNames = new List<string>();
            for (int i = 0; i < toggles.Count; i++)
            {
                // Versuche, Text vom Toggle zu bekommen, sonst verwende den Namen
                TextMeshProUGUI tmpText = toggles[i].GetComponentInChildren<TextMeshProUGUI>();
                toggleDisplayNames.Add(tmpText != null ? tmpText.text : toggles[i].name);
            }
        }

        // Lade gespeicherte Werte und setze Toggles entsprechend
        LoadToggleStates();

        // Button-Listener hinzufügen
        nextButton.onClick.AddListener(WeiterButtonClicked);

        //Overlay ausblenden
        if (goalOverlayPanel != null)
        {
            goalOverlayPanel.SetActive(false);
        }

        // Close-Button für das Overlay
        if (closeOverlayButton != null)
        {
            closeOverlayButton.onClick.AddListener(() => SceneManager.LoadScene("Panel"));
        }

        
    }

    //Methode zum Speichern der Toggle-Zustände nach Weiter-Button-Klick
    private void WeiterButtonClicked()
    {
        for (int i = 0; i < toggles.Count; i++)
        {
            string key = toggleKeys[i];
            PlayerPrefs.SetInt(key, toggles[i].isOn ? 1 : 0);
        }
        PlayerPrefs.Save();
        Debug.Log("Toggle-Zustände gespeichert");

        ShowGoalOverlay();
    }

    private void ShowGoalOverlay()
    {
        //Prüfen, ob alles zugewiesen ist
        if (goalOverlayPanel == null || listContainer == null || listItemPrefab == null)
        {
            Debug.LogError("Overlay-Komponenten nicht zugewiesen!");
            return;
        }
        
        // Overlay einblenden
        goalOverlayPanel.SetActive(true);
        
        // Bestehende Liste leeren
        foreach (Transform child in listContainer)
        {
            Destroy(child.gameObject);
        }
        
        // Liste mit aktivierten Toggles füllen
        bool anyToggleSelected = false;
        for (int i = 0; i < toggles.Count; i++)
        {
            if (toggles[i].isOn)
            {
                anyToggleSelected = true;
                GameObject listItem = Instantiate(listItemPrefab, listContainer);
                
                // Text im ListItem-Prefab setzen (nur für TMP)
                TextMeshProUGUI itemText = listItem.GetComponentInChildren<TextMeshProUGUI>();
                if (itemText != null)
                {
                    // Verwende den vordefinierten Display-Namen
                    itemText.text = toggleDisplayNames[i];
                    Debug.Log($"Toggle ausgewählt: {toggleDisplayNames[i]}");
                }
                else
                {
                    Debug.LogError("Kein TextMeshProUGUI im ListItemPrefab gefunden!");
                }
            }
        }
        
        // Fallback, falls nichts ausgewählt wurde
        if (!anyToggleSelected)
        {
            GameObject listItem = Instantiate(listItemPrefab, listContainer);
            TextMeshProUGUI itemText = listItem.GetComponentInChildren<TextMeshProUGUI>();
            
            if (itemText != null)
            {
                itemText.text = "Du hast keine Ziele ausgewählt. Wenn du magst, kannst du nochmal spielen und dann deine Ziele auswählen.";
            }
        }
        
        
    }
    

    //Zustände Laden
    private void LoadToggleStates()
    {
        for (int i = 0; i < toggles.Count; i++)
        {
            string key = toggleKeys[i];
            if (PlayerPrefs.HasKey(key))
            {
                toggles[i].isOn = PlayerPrefs.GetInt(key) == 1;
            }
        }
    }

    // Hilfsmethode zum Abrufen der Toggle-Zustände für Notifications
    public List<string> GetSelectedToggleKeys()
    {
        List<string> selectedKeys = new List<string>();
        for (int i = 0; i < toggles.Count; i++)
        {
            if (PlayerPrefs.GetInt(toggleKeys[i], 0) == 1)
            {
                selectedKeys.Add(toggleKeys[i]);
            }
        }
        return selectedKeys;
    }


}
