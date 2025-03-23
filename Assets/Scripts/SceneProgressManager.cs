using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneProgressManager : MonoBehaviour
{
    private static SceneProgressManager instance;
    public static SceneProgressManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<SceneProgressManager>();
                if (instance == null)
                {
                    GameObject go = new GameObject("SceneProgressManager");
                    instance = go.AddComponent<SceneProgressManager>();
                }
            }
            return instance;
        }
    }
    
    //Dictionary zum Speichern verschiedener Werte anlegen
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

    //Levelabschluss-Key
    [Header("Level Completion")]
    public string levelIdentifier; // z.B. "Level1", "Tutorial" etc.
    private string SaveKey => $"Level_{levelIdentifier}_Completed";
    private bool isLevelCompleted = false;
    public System.Action<bool> OnLevelCompletionChanged;

    public bool IsLevelCompleted 
    { 
        get { return isLevelCompleted; }
        private set
        {
            if (isLevelCompleted != value)
            {
                SetLevelCompletionState(value);
            }
        }
    }

    private void SetLevelCompletionState(bool value) 
    {
        isLevelCompleted = value;
        PlayerPrefs.SetInt(SaveKey, value ? 1 : 0);
        PlayerPrefs.Save();
        OnLevelCompletionChanged?.Invoke(isLevelCompleted);
    }
    

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            // Lade den gespeicherten Zustand
            isLevelCompleted = PlayerPrefs.GetInt(SaveKey, 0) == 1;
            SetValue(SaveKey, isLevelCompleted);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    //Variablenwert speichern
    public void SetValue(string key, object value)
    {
        sceneValues[key] = value;
        Debug.Log($"Wert gespeichert - Key: {key}, Value: {value}");
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
    public bool HasValue(string key)
    {
        return sceneValues.ContainsKey(key);
    }

    // Level-Abschluss überprüfen
    private bool CheckCondition(LevelCondition condition, object value)
    {
        return condition.valueType switch
        {
            ConditionValueType.Boolean => value is bool val && val == condition.expectedBool,
            ConditionValueType.Integer => value is int val && val == condition.expectedInt,
            ConditionValueType.Float => value is float val && val == condition.expectedFloat,
            ConditionValueType.String => value is string val && val == condition.expectedString,
            _ => false
        };
    }

    public bool CheckLevelCompletion()
    {
        foreach (var condition in completionConditions)
        {
            if (!sceneValues.ContainsKey(condition.key))
            {
                Debug.Log($"Fehlender Wert für Key: {condition.key}");
                IsLevelCompleted = false;
                return false;
            }

            if (!CheckCondition(condition, sceneValues[condition.key]))
            {
                Debug.Log($"Bedingung nicht erfüllt für Key: {condition.key}");
                IsLevelCompleted = false;
                return false;
            }
        }
        
        Debug.Log("Alle Level-Bedingungen erfüllt!");
        IsLevelCompleted = true;
        return true;
    }

    public void SetLevelCompleted(bool completed)
    {
        IsLevelCompleted = completed;
    }

    public void ResetProgress(bool resetAllLevels = false)
    {
        if (resetAllLevels)
        {
            PlayerPrefs.DeleteAll();
        }
        else
        {
            PlayerPrefs.DeleteKey(SaveKey);
        }
        PlayerPrefs.Save();
        sceneValues.Clear();
        SetLevelCompletionState(false);
        
        if (resetAllLevels)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    public static void ResetAllLevels()
    {
        if (Instance != null)
        {
            Instance.ResetProgress(true);
        }
    }

    void OnDestroy()
    {
    if (instance == this)
        {
        instance = null;
        }
    }

}
