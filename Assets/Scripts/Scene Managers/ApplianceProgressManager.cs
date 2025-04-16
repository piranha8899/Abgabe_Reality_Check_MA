using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ApplianceProgressManager : MonoBehaviour
{
    [SerializeField] private Image targetImage;
    
    [SerializeField] private List<string> requiredKeys = new List<string>();
    
    [SerializeField] private bool showWhenCompleted = true;
    
    [SerializeField] private bool checkContinuously = true; //Wenn false, dann nur bei Start prüfen
    
    void Start()
    {
        if (targetImage == null)
        {
            Debug.LogError("Kein Ziel-Image zugewiesen");
            return;
        }
        
        // Initialen Zustand prüfen
        UpdateImageVisibility();
    }
    
    void Update()
    {
        if (checkContinuously)
        {
            UpdateImageVisibility();
        }
    }
    
    public void UpdateImageVisibility()
    {
        bool conditionsFulfilled = AreSelectedConditionsMet();
        targetImage.gameObject.SetActive(showWhenCompleted ? conditionsFulfilled : !conditionsFulfilled);
    }
    
    public bool AreSelectedConditionsMet()
    {
        // SceneProgressManager existiert?
        if (SceneProgressManager.Instance == null)
        {
            Debug.LogWarning("Kein SceneProgressManager gefunden");
            return false;
        }
        
        // Keine Schlüssel angegeben?
        if (requiredKeys == null || requiredKeys.Count == 0)
        {
            return true; // Wenn keine Bedingungen angegeben sind, gilt es als erfüllt
        }
        
        // Zugriff auf die completionConditions des SceneProgressManager
        if (SceneProgressManager.Instance.completionConditions == null)
        {
            Debug.LogWarning("Keine Completion Conditions im SceneProgressManager definiert!");
            return false;
        }
        
        // Dictionary für schnellen Zugriff auf Bedingungen
        Dictionary<string, SceneProgressManager.LevelCondition> conditionsDict = new Dictionary<string, SceneProgressManager.LevelCondition>();
        foreach (var condition in SceneProgressManager.Instance.completionConditions)
        {
            if (!string.IsNullOrEmpty(condition.key))
            {
                conditionsDict[condition.key] = condition;
            }
        }
        
        // Überprüfen aller ausgewählten Schlüssel
        foreach (string key in requiredKeys)
        {
            if (string.IsNullOrEmpty(key))
                continue; // Leere Schlüssel überspringen
                
            // Prüfen, ob der Schlüssel als Bedingung definiert ist
            if (!conditionsDict.ContainsKey(key))
            {
                Debug.LogWarning($"Schlüssel '{key}' nicht in den Completion Conditions gefunden!");
                return false;
            }
            
            // Prüfen, ob der Wert im SceneProgressManager existiert
            if (!SceneProgressManager.Instance.HasValue(key))
            {
                return false; // Wert nicht gefunden
            }
            
            // Bedingung aus dem Dictionary holen
            var condition = conditionsDict[key];
            
            // Je nach Typ den Wert holen und mit der erwarteten Bedingung vergleichen
            bool isConditionMet = false;
            
            switch (condition.valueType)
            {
                case SceneProgressManager.ConditionValueType.Boolean:
                    bool boolValue = SceneProgressManager.Instance.GetValue<bool>(key, false);
                    isConditionMet = (boolValue == condition.expectedBool);
                    break;
                    
                case SceneProgressManager.ConditionValueType.Integer:
                    int intValue = SceneProgressManager.Instance.GetValue<int>(key, 0);
                    isConditionMet = (intValue == condition.expectedInt);
                    break;
                    
                case SceneProgressManager.ConditionValueType.Float:
                    float floatValue = SceneProgressManager.Instance.GetValue<float>(key, 0f);
                    isConditionMet = Mathf.Approximately(floatValue, condition.expectedFloat);
                    break;
                    
                case SceneProgressManager.ConditionValueType.String:
                    string stringValue = SceneProgressManager.Instance.GetValue<string>(key, "");
                    isConditionMet = (stringValue == condition.expectedString);
                    break;
            }
            
            // Wenn eine Bedingung nicht erfüllt ist, ist das Gesamtergebnis false
            if (!isConditionMet)
            {
                return false;
            }
        }
        
        // Alle geprüften Bedingungen sind erfüllt
        return true;
    }
    
    // Öffentliche Methode zum manuellen Auslösen der Überprüfung
    public void CheckConditionsManually()
    {
        UpdateImageVisibility();
    }
}
