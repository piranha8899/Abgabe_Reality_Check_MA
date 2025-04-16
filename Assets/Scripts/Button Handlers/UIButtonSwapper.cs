using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class ButtonPair
{
    public Button onButton;
    public Button offButton;
    public bool startWithOnButton = false;
}

public class UIButtonSwapper : MonoBehaviour
{
    [SerializeField] private List<ButtonPair> buttonPairs = new List<ButtonPair>();
    
    [SerializeField] private Button completionTargetButton; // Button, der den Level-Fortschritt bestimmt
    
    [SerializeField] private string levelCompletionKey = "LevelCompleted";

    void Start()
    {
        // Initialer Zustand für alle Button-Paare
        for (int i = 0; i < buttonPairs.Count; i++)
        {
            var pair = buttonPairs[i];
            
            // Nutze konfigurierten Startzustand
            bool isOnActive = pair.startWithOnButton;

            // Aktiviere entsprechenden Button
            pair.onButton.gameObject.SetActive(isOnActive);
            pair.offButton.gameObject.SetActive(!isOnActive);

            // Click Events registrieren
            int pairIndex = i;
            pair.onButton.onClick.AddListener(() => SwapToOff(pairIndex));
            pair.offButton.onClick.AddListener(() => SwapToOn(pairIndex));
        }
        
        // Level-Status prüfen
        CheckLevelCompletion();
    }

    void SwapToOn(int pairIndex)
    {
        if (pairIndex < 0 || pairIndex >= buttonPairs.Count)
            return;

        // Zuerst alle Paare auf "off" setzen
        for (int i = 0; i < buttonPairs.Count; i++)
        {
            buttonPairs[i].onButton.gameObject.SetActive(false);
            buttonPairs[i].offButton.gameObject.SetActive(true);
        }
        
        // Dann das ausgewählte Paar auf on setzen
        var pair = buttonPairs[pairIndex];
        pair.onButton.gameObject.SetActive(true);
        pair.offButton.gameObject.SetActive(false);
        
        // Level-Status prüfen
        CheckLevelCompletion();
    }

    void SwapToOff(int pairIndex)
    {
        if (pairIndex < 0 || pairIndex >= buttonPairs.Count)
            return;

        var pair = buttonPairs[pairIndex];
        pair.onButton.gameObject.SetActive(false);
        pair.offButton.gameObject.SetActive(true);
        
        // Level-Status prüfen
        CheckLevelCompletion();
    }
    
    private void CheckLevelCompletion()
    {
        if (completionTargetButton == null)
            return;
            
        // Prüfen ob der Ziel-Button aktiv ist
        bool isCompleted = completionTargetButton.gameObject.activeSelf;
        
        // Level-Fortschritt speichern
        SceneProgressManager.Instance.SetValue(levelCompletionKey, isCompleted);
    }

    void OnDestroy()
    {
        // Events wieder entfernen
        for (int i = 0; i < buttonPairs.Count; i++)
        {
            var pair = buttonPairs[i];
            int pairIndex = i;
            
            if (pair.onButton != null)
                pair.onButton.onClick.RemoveListener(() => SwapToOff(pairIndex));
            
            if (pair.offButton != null)
                pair.offButton.onClick.RemoveListener(() => SwapToOn(pairIndex));
        }
    }
}