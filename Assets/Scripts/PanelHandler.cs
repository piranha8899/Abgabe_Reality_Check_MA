using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class PanelHandler : MonoBehaviour
{
    private static PanelHandler instance;
    public static PanelHandler Instance    
    {
        get
        {
        if (instance == null)
            instance = FindObjectOfType<PanelHandler>();
        return instance;
        }
    }

    [System.Serializable]
    public class LevelCompletionPair
    {
        public string levelId;         // z.B. "level_kitchen"
        public GameObject targetObject; // Das zu aktivierende GameObject
    }

    [System.Serializable]
    public class ProgressIndicator
    {
        public int requiredCompletions;
        public GameObject[] indicatorObjects = new GameObject[3];
        public void SetObjectsActive(bool state)
        {
            foreach (var obj in indicatorObjects)
            {
                if (obj != null)
                {
                    obj.SetActive(state);
                }
            }
        }
    }
    

    public LevelCompletionPair[] levelCompletionPairs;
    public ProgressIndicator[] progressIndicators;
    public GameObject CompletionOverlay;
    public GameObject PanelContinueButton;

    void Start()
    {
        // Prüfe beim Start alle Level-Status
        CheckAllLevelStatus();
        UpdateProgressIndicator();
        CompletionOverlay.SetActive(false);
        PanelContinueButton.SetActive(false);
        CheckTotalCompletion();
        
    }



     void OnEnable()
    {
       
        if (SceneProgressManager.Instance != null)
        {
            SceneProgressManager.Instance.OnLevelCompletionChanged += OnLevelStatusChanged;
        }
    }

    void OnDisable()
    {
       
        if (SceneProgressManager.Instance != null)
        {
            SceneProgressManager.Instance.OnLevelCompletionChanged -= OnLevelStatusChanged;
        }
    }

    // Diese Methode lädt die spezifizierte Szene
    public void LoadScene(string SceneName)
    {
        SceneManager.LoadScene(SceneName);
    }

     private void OnLevelStatusChanged(bool completed)
    {
        CheckAllLevelStatus();
        UpdateProgressIndicator();
        CheckTotalCompletion();
    }

    // Prüft den Status aller konfigurierten Level
    public void CheckAllLevelStatus()
    {
        foreach (var pair in levelCompletionPairs)
        {
            CheckLevelStatus(pair.levelId);
        }
    }

    // Prüft den Status eines spezifischen Levels
    public void CheckLevelStatus(string levelId)
    {
        // Prüfe ob das Level abgeschlossen ist
        bool isCompleted = PlayerPrefs.GetInt($"Level_{levelId}_Completed", 0) == 1;
        
        // Finde das passende GameObject für diese levelId
        foreach (var pair in levelCompletionPairs)
        {
            if (pair.levelId == levelId && pair.targetObject != null)
            {
                // Aktiviere/Deaktiviere das GameObject je nach Level-Status
                pair.targetObject.SetActive(isCompleted);
                break;
            }
        }
    }

     private int CountCompletedLevels()
    {
        int completedCount = 0;
        foreach (var pair in levelCompletionPairs)
        {
            if (PlayerPrefs.GetInt($"Level_{pair.levelId}_Completed", 0) == 1)
            {
                completedCount++;
            }
        }
        return completedCount;
    }

    private void UpdateProgressIndicator()
    {
        int completedLevels = CountCompletedLevels();
        
        // Deaktiviere zuerst alle Indikatoren
        foreach (var indicator in progressIndicators)
            {
            indicator.SetObjectsActive(false);
            }

        // Aktiviere die passenden Indikatoren
        foreach (var indicator in progressIndicators)
        {
            if (indicator.requiredCompletions == completedLevels)
            {
                indicator.SetObjectsActive(true);
                break;
            }
        }
    }

    //Totale Level Completion prüfen
    private void CheckTotalCompletion()
    {
        int completedCount = CountCompletedLevels();
        int requiredCompletions = levelCompletionPairs.Length; // Anzahl der Level
        if (completedCount >= requiredCompletions)
        {
            CompletionOverlay.SetActive(true);
            Debug.Log("Alle Level abgeschlossen!");
            PanelContinueButton.SetActive(true);
        }
        else
        {
           Debug.Log($"Fortschritt: {completedCount} Level abgeschlossen.");
           Debug.Log($"Noch {requiredCompletions - completedCount} Level zu absolvieren.");            
        }
    }


    // Alle Szenen zurücksetzen (kompletter Reset)
    public void ResetAllScenes()
    {

    foreach (var pair in levelCompletionPairs)
    {
        if (!string.IsNullOrEmpty(pair.levelId))
        {
            string keyToDelete = $"Level_{pair.levelId}_Completed";
            PlayerPrefs.DeleteKey(keyToDelete);
            Debug.Log($"Level-Fortschritt gelöscht: {keyToDelete}");
        }
    }
    
    PlayerPrefs.Save();

    // Aktualisiere die Anzeigen
        CheckAllLevelStatus();
        UpdateProgressIndicator();
        
    // Aktuelle Szene neu laden
    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public static void ResetAllScenesStatic()
    {
    if (Instance != null)
     {
        Instance.ResetAllScenes();
        Debug.Log("Alle Szenen zurückgesetzt");
     }
    }


}
