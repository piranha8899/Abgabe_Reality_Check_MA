using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor.Build;
#endif

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

    public string completionTyperId = "yz"; // ID wie aus Typer festlegen
    [System.Serializable]
    public class AnimatedProgressIndicator
    {
        public GameObject indicatorObject;
        public string triggerName = "NextState";
        public Animator animator;

        public void TriggerNextState()
        {
            if (indicatorObject != null && animator != null)
            {
                animator.SetTrigger(triggerName);
            }
        }
    }
    
    public LevelCompletionPair[] levelCompletionPairs;
    public AnimatedProgressIndicator progressIndicator;
    public GameObject CompletionOverlay;
    public float completionOverlayDelay = 5f; // Verzögerung für das Overlay
    private bool completionOverlayShown = false; // Flag, um zu verhindern, dass das Overlay mehrmals angezeigt wird
    public GameObject showCompletionOverlayButton; // Button, um das Overlay manuell anzuzeigen
    public GameObject PanelContinueButton;
    private int lastCompletedCount = 0; // Speichert den letzten bekannten Abschlussstand

    [System.Serializable]
    public class LevelCompletionPair
    {
        public string levelId;         // z.B. "level_kitchen"
        public GameObject targetObject; // Das zu aktivierende GameObject
    }

    void Awake()
    {
        DontDestroyOnLoad(gameObject); //Panel Handler über alle Szenen hinweg erhalten
        //Status des Overlays laden
        completionOverlayShown = PlayerPrefs.GetInt("CompletionOverlayShown", 0) == 1;
    }

    void Start()
    {
        // Prüfe beim Start alle Level-Status
        CheckAllLevelStatus();
        UpdateProgressIndicator();
        if (CompletionOverlay != null)
        {
            CompletionOverlay.SetActive(false);
        }

        if (showCompletionOverlayButton != null)
        {
            showCompletionOverlayButton.SetActive(false);
            showCompletionOverlayButton.GetComponent<Button>().onClick.AddListener(ReshowCompletionOverlay);
        }

        if( PanelContinueButton != null)
        {
            PanelContinueButton.SetActive(false);
        }
        

        // Initialisiere den Animator, falls nicht gesetzt
        if (progressIndicator.indicatorObject != null && progressIndicator.animator == null)
        {
            progressIndicator.animator = progressIndicator.indicatorObject.GetComponent<Animator>();
        }
        
        // Speichere den aktuellen Stand
        lastCompletedCount = CountCompletedLevels();

        if (CountCompletedLevels() >= levelCompletionPairs.Length && completionOverlayShown)
        {
            if (showCompletionOverlayButton != null) showCompletionOverlayButton.SetActive(true);
        }
        CheckTotalCompletion();
        StartCoroutine(CheckForTypingSequence());
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
        foreach (var pair in levelCompletionPairs) CheckLevelStatus(pair.levelId);
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
        int currentCompletedCount = CountCompletedLevels();
        // Prüfe, ob ein neues Level abgeschlossen wurde und Animation auslösen
        if (currentCompletedCount > lastCompletedCount)
        {
            // Aktualisiere die Animation nur, wenn tatsächlich etwas neues abgeschlossen wurde
            progressIndicator.TriggerNextState();
            lastCompletedCount = currentCompletedCount;
        }
    }

    //Totale Level Completion prüfen
    private void CheckTotalCompletion()
    {
        int completedCount = CountCompletedLevels();
        int requiredCompletions = levelCompletionPairs.Length; // Anzahl der Level
        if (completedCount >= requiredCompletions)
        {
            if(!completionOverlayShown)
            {
                StartCoroutine(ShowCompletionOverlay());
            }

            //Wenn Overlay schon angezeigt wurde, nur Button anzeigen
            else if (showCompletionOverlayButton != null)
            {
                showCompletionOverlayButton.SetActive(true);
            }
        }
        else
        {
           Debug.Log($"Fortschritt: {completedCount} Level abgeschlossen.");
           Debug.Log($"Noch {requiredCompletions - completedCount} Level zu absolvieren.");            
        }
    }

    private IEnumerator ShowCompletionOverlay()
    {
        yield return new WaitForSeconds(completionOverlayDelay); // Warten auf eingestellte Verzögerung
        ReshowCompletionOverlay();
    }

    public void ReshowCompletionOverlay()
    {
        if (CompletionOverlay != null)
        {
            CompletionOverlay.SetActive(true);
        }
        
        if (PanelContinueButton != null) 
        {
            PanelContinueButton.SetActive(true);
        }
        
        // Setze das Flag und speichere den Zustand
        completionOverlayShown = true;
        PlayerPrefs.SetInt("CompletionOverlayShown", 1);
        PlayerPrefs.Save();
        if(showCompletionOverlayButton != null) showCompletionOverlayButton.SetActive(false);
    }

    //Typing-Sequenz prüfen und abspielen
    private IEnumerator CheckForTypingSequence()
    {
        yield return new WaitForSeconds(0.2f); //Warten, bis alles geladen ist
        int completedCount = CountCompletedLevels();
        //Prüfen, ob Completion-Sequenz gespielt werden soll
        if (completedCount >= levelCompletionPairs.Length && !PlayerPrefs.HasKey($"Typer_{completionTyperId}_Played"))
        {
            if(Speech_Texttyping.HasDialog(completionTyperId))
            {
                Debug.Log($"Typing-ID {completionTyperId} wird abgespielt.");
                Speech_Texttyping.StartDialog(completionTyperId);
            }
            else
            {
                Debug.Log($"Typing-ID {completionTyperId} nicht gefunden.");
            }
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

        //Typer-IDs zurücksetzen
        for (int i = 1; i <= 30; i++)
        {
            string keyToDelete = $"Typer_{i}_Played";
            if(PlayerPrefs.HasKey(keyToDelete))
            {
                PlayerPrefs.DeleteKey(keyToDelete);
                Debug.Log($"Typer-Fortschritt gelöscht: {keyToDelete}");
            }
        }

        // Completion Overlay Status zurücksetzen
        PlayerPrefs.DeleteKey("CompletionOverlayShown");
        completionOverlayShown = false;
        PlayerPrefs.Save();
        // Zurücksetzen des Zählers
        lastCompletedCount = 0;
        // Aktualisiere die Anzeigen
        CheckAllLevelStatus();
        UpdateProgressIndicator();
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
