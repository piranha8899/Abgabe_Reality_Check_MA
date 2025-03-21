using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


  public class SceneLoader : MonoBehaviour
{
    [System.Serializable]
    public class LevelCompletionPair
    {
        public string levelId;         // z.B. "level_kitchen"
        public GameObject targetObject; // Das zu aktivierende GameObject
    }

    public LevelCompletionPair[] levelCompletionPairs;

    void Start()
    {
        // Prüfe beim Start alle Level-Status
        CheckAllLevelStatus();
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
}
