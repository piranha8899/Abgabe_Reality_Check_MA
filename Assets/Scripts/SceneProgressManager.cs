using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneProgressManager : MonoBehaviour
{
    private static SceneProgressManager instance;
    public static SceneProgressManager Instance
    {
        get { return instance; }
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

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
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
    public bool CheckLevelCompletion()
    {
        foreach (var condition in completionConditions)
        {
            if (!sceneValues.ContainsKey(condition.key))
            {
                Debug.Log($"Fehlender Wert für Key: {condition.key}");
                return false;
            }

            bool conditionMet = false;
            var value = sceneValues[condition.key];

            switch (condition.valueType)
            {
                case ConditionValueType.Boolean:
                    conditionMet = value is bool && (bool)value == condition.expectedBool;
                    break;
                case ConditionValueType.Integer:
                    conditionMet = value is int && (int)value == condition.expectedInt;
                    break;
                case ConditionValueType.Float:
                    conditionMet = value is float && (float)value == condition.expectedFloat;
                    break;
                case ConditionValueType.String:
                    conditionMet = value is string && (string)value == condition.expectedString;
                    break;
            }

            if (!conditionMet)
            {
                Debug.Log($"Bedingung nicht erfüllt für Key: {condition.key}");
                return false;
            }
        }
        
        Debug.Log("Alle Level-Bedingungen erfüllt!");
        return true;
    }

     
}
