using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneProgressManager : MonoBehaviour
{
    private static SceneProgressManager instance;
    // Instanz erstellen
    public static SceneProgressManager Instance    
    {
        get
        {
        if (instance == null)
            instance = FindObjectOfType<SceneProgressManager>();
        return instance;
        }
    }
    private const string CURRENT_LEVEL_KEY = "CurrentLevelIdentifier";

    [SerializeField] private Button directCompleteButton; // Button für den direkten Levelabschluss (NUR FÜR TESTING)
    
    // Dictionary zum Speichern verschiedener Werte anlegen
    private Dictionary<string, object> sceneValues = new Dictionary<string, object>();

    public enum ConditionValueType
    {
        Boolean,
        Integer,
        Float,
        String
    }

    // Bedingungen für den Levelabschluss
    [System.Serializable]
    public class LevelCondition
    {
        public string key;
        public ConditionValueType valueType;
        public bool expectedBool;
        public int expectedInt;
        public float expectedFloat;
        public string expectedString;

    }
    
    public LevelCondition[] completionConditions;

    // Levelabschluss-Key
    [Header("Level Completion")]
    [SerializeField] private string levelIdentifier = ""; // z.B. "Achtung: Level-IDs immer in Inspector überprüfen, Merhfachverwendung!
    private string SaveKey => !string.IsNullOrEmpty(LevelIdentifier) ? $"Level_{LevelIdentifier}_Completed" : "";
    private bool isLevelCompleted = false;
    public System.Action<bool> OnLevelCompletionChanged;
    private int previousCompletedConditionsCount = 0;
    public System.Action OnProgressMade; // Event, wenn Fortschritt gemacht wurde
    [SerializeField] private Animator progressAnimator; // Animator
    [SerializeField] private string progressTriggerName = "progress"; // Trigger-Name für Animation
    [SerializeField] private string endprogressTriggerName = "endprogress"; // Trigger-Name für EndAnimation


    // Handelt den LevelIdentifier
    public string LevelIdentifier 
{
    get { return levelIdentifier; }
    set 
    {
        levelIdentifier = value;
        // Bei Änderung global speichern
        PlayerPrefs.SetString(CURRENT_LEVEL_KEY, levelIdentifier);
        PlayerPrefs.Save();
    }
}
    // Prüft, ob LevelIdentifier oder CompletionConditions gesetzt sind
    private bool IsLevelTrackingEnabled => !string.IsNullOrEmpty(LevelIdentifier) && 
                                    completionConditions != null && 
                                    completionConditions.Length > 0;

    public bool IsLevelCompleted 
    { 
        get { return isLevelCompleted; }
        private set
        {
            if (isLevelCompleted != value && IsLevelTrackingEnabled) //Status nur setzen, um Runtime Fehler zu vermeiden
            {
                SetLevelCompletionState(value);
            }
        }
    }

    private void SetLevelCompletionState(bool value) 
    {
        if(!IsLevelTrackingEnabled) return;

        isLevelCompleted = value;
        
        if (!string.IsNullOrEmpty(SaveKey))
        {
            PlayerPrefs.SetInt(SaveKey, value ? 1 : 0);
            PlayerPrefs.Save();
        }
        
        OnLevelCompletionChanged?.Invoke(isLevelCompleted);
    }
    

    void Awake()
    {
    if (instance == null)
     {
        instance = this;
        
        // WICHTIG: LevelIdentifier aus PlayerPrefs wiederherstellen, falls gesetzt
        if (string.IsNullOrEmpty(levelIdentifier))
         {
            levelIdentifier = PlayerPrefs.GetString(CURRENT_LEVEL_KEY, "");
         }
        else
          {
            // Aktuellen LevelIdentifier speichern
            PlayerPrefs.SetString(CURRENT_LEVEL_KEY, levelIdentifier);
            PlayerPrefs.Save();
         }
        
        // Lade den gespeicherten Zustand, nur wenn Level-Tracking aktiviert ist
        if (IsLevelTrackingEnabled && !string.IsNullOrEmpty(SaveKey))
         {
            isLevelCompleted = PlayerPrefs.GetInt(SaveKey, 0) == 1;
          }
    }   
        else if (instance != this)
        {
        Destroy(gameObject);
        }
    }

    void Start()
    { // Level Direkt abschliessen (NUR FÜR TESTING)
     if (directCompleteButton != null)
    {
        directCompleteButton.onClick.AddListener(() => {
            // Setzt isLevelCompleted direkt auf true
            SetLevelCompleted(true);
            Debug.Log("Level wurde manuell abgeschlossen (Test-Button)");
        });
    }   
    }

    // Variablenwert speichern, Schlüsselname wird definiert
    public void SetValue(string key, object value)
    {
    if (string.IsNullOrEmpty(key)) return;
    
    sceneValues[key] = value;
    
    // Nur prüfen wenn Level-Tracking aktiv
    if (IsLevelTrackingEnabled)
        CheckLevelCompletion();
    }

    // Wert abrufen
    public T GetValue<T>(string key, T defaultValue = default)
    {
        if (sceneValues.ContainsKey(key) && sceneValues[key] is T)
        {
            return (T)sceneValues[key];
        }
        return defaultValue;
    }
    // Prüfe, ob ein entsprechender Schlüssel existiert
    public bool HasValue(string key)
    {
        return sceneValues.ContainsKey(key);
    }

    // Level-Abschluss überprüfen
    private bool CheckCondition(LevelCondition condition, object value)
    {
    if (condition == null || value == null) return false;
    
    try
        {
        switch (condition.valueType)
            {
            case ConditionValueType.Boolean:
                return (value is bool b) && b == condition.expectedBool;
                
            case ConditionValueType.Integer:
                // Direkte Konvertierung für häufigste Fälle
                return (value is int i) ? i == condition.expectedInt : 
                       int.TryParse(value.ToString(), out int n) && n == condition.expectedInt;
                
            case ConditionValueType.Float:
                // Vereinfachte Float-Prüfung
                return (value is float f) ? Mathf.Approximately(f, condition.expectedFloat) : 
                       float.TryParse(value.ToString(), out float p) && Mathf.Approximately(p, condition.expectedFloat);
                
            case ConditionValueType.String:
                return value.ToString() == condition.expectedString;
                
            default:
                return false;
            }
        }
    catch { return false; }
    } 
    //Prüft alle definierten Bedingungen
    public bool CheckLevelCompletion()
    {
        // Wenn Level-Tracking nicht aktiviert ist, passiert nichts
        if (!IsLevelTrackingEnabled)
        {
            return false;
        }
        
        // Wenn keine Bedingungen definiert, wird false zurückgegeben
        if (completionConditions == null || completionConditions.Length == 0)
        {
            Debug.Log("Keine Level-Bedingungen definiert");
            IsLevelCompleted = false;
            return false;
        }

        int currentCompletedCount = 0;
        bool allConditionsMet = true;

        foreach (var condition in completionConditions)
        {
            // Überspringe null-Bedingungen
            if (condition == null || string.IsNullOrEmpty(condition.key))
            {
                continue;
            }
            
            if (!sceneValues.ContainsKey(condition.key))
            {
                Debug.Log($"Fehlender Wert für Key: {condition.key}");
                allConditionsMet = false;
                continue;
            }

            if (CheckCondition(condition, sceneValues[condition.key]))
            {
                currentCompletedCount++;
            }
            else
            {
                allConditionsMet = false;
            }
        }

        // Prüfen, ob Fortschritt gemacht wurde
        if (currentCompletedCount > previousCompletedConditionsCount)
        {
            // Animation triggern
            if (progressAnimator != null && !string.IsNullOrEmpty(progressTriggerName))
            {
                progressAnimator.SetTrigger(progressTriggerName);
                Debug.Log($"Fortschrittsanimation getriggert ({currentCompletedCount}/{completionConditions.Length})");
            }
            
            // Event auslösen
            OnProgressMade?.Invoke();
            
            // Neuen Zählerstand speichern
            previousCompletedConditionsCount = currentCompletedCount;
        }
        
        // Level ist nur abgeschlossen, wenn alle Bedingungen erfüllt sind
        if (allConditionsMet)
        {
            Debug.Log("Alle Level-Bedingungen erfüllt!");
            IsLevelCompleted = true;
            // Animation triggern, wenn alle Bedingungen erfüllt sind
            if (progressAnimator != null && !string.IsNullOrEmpty(endprogressTriggerName))
            {
                progressAnimator.SetTrigger(endprogressTriggerName);
                Debug.Log("Endanimation getriggert");
            }
            return true;
        }
        else
        {
            IsLevelCompleted = false;
            return false;
        }
    }

    // Setzt Completion manuell, wenn Bedingungen erfüllt
    public void SetLevelCompleted(bool completed)
    {
        if (IsLevelTrackingEnabled)
        {
            IsLevelCompleted = completed;
        }
    }

    public static string GetCurrentLevelIdentifier()
    {
    return PlayerPrefs.GetString(CURRENT_LEVEL_KEY, "");
    }

    public static void SetCurrentLevelIdentifier(string identifier)
    {
        PlayerPrefs.SetString(CURRENT_LEVEL_KEY, identifier);
        PlayerPrefs.Save();
    
        // Falls eine Instanz existiert, aktualisiere auch dort
        if (Instance != null)
        {
        Instance.levelIdentifier = identifier;
        }
    }

    public void HomeButton()
    {
        // Zurück zur Hauptszene
        SceneManager.LoadScene("Panel");
    }


    // Aktuelle Szene zurücksetzen (nur aktuelles Level)
    public void ResetCurrentScene()
    {
    // Variablen zurücksetzen
    sceneValues.Clear();
    isLevelCompleted = false;
    previousCompletedConditionsCount = 0;
    
    // Levelfortschritt löschen
    if (!string.IsNullOrEmpty(SaveKey))
        {
        PlayerPrefs.DeleteKey(SaveKey);
        PlayerPrefs.Save();
        }
    
    // Event auslösen
    OnLevelCompletionChanged?.Invoke(isLevelCompleted);
    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Statische Hilfsmethoden, können von überall mittel SceneProgressManager.ResetCurrentSceneStatic() aufgerufen werden
    public static void ResetCurrentSceneStatic()
    {
    if (Instance != null)
     {
        Instance.ResetCurrentScene();
        Debug.Log("Aktuelle Szene zurückgesetzt");
     }
    }

    // Objekt zerstören
    void OnDestroy()
    {
    if (instance == this)
        {
        instance = null;
        }
    }

}