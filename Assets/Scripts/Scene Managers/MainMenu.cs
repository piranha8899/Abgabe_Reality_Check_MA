using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    public static MainMenu Instance { get; private set; } // Singleton-Instanz für Zugriff von anderen Skripten

    [SerializeField] private Button playButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button settingsCloseButton;
    [SerializeField] private Button goalsButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private Button resetScenesButton;
    [SerializeField] private Button resetAllPrefsButton;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private string gameSceneName = "GameScene";
    [SerializeField] private string[] levelIds; // Liste aller möglichen Level-IDs, um LevelCompletion zu prüfen


    private void Awake()
    {
        // Singleton-Pattern: Es gibt nur eine Instanz
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }
    
    private void Start()
    {
        // Button-Listener hinzufügen
        if (playButton != null)
            playButton.onClick.AddListener(StartGame);
            
        if (settingsButton != null)
            settingsButton.onClick.AddListener(OpenSettings);
            bool anyLevelCompleted = CheckIfAnyLevelCompleted();
            settingsButton.interactable = anyLevelCompleted; // Button nur aktivieren, wenn mindestens ein Level abgeschlossen ist
            
        if (goalsButton != null)
        {
            goalsButton.onClick.AddListener(OpenGoals);
            goalsButton.interactable = anyLevelCompleted; // Button nur aktivieren, wenn mindestens ein Level abgeschlossen ist
        }
            
        if (quitButton != null)
            quitButton.onClick.AddListener(QuitGame);

        settingsPanel.SetActive(false);
            
        
    }

    public void StartGame()
    {
        Debug.Log("Spiel wird gestartet...");
        SceneManager.LoadScene(gameSceneName);
    }

    public void OpenSettings()
    {
        Debug.Log("Einstellungen werden geöffnet...");
        settingsPanel.SetActive(true);
        settingsCloseButton.onClick.AddListener(CloseSettings);
        resetScenesButton.onClick.AddListener(ResetAllScenes);
        resetAllPrefsButton.onClick.AddListener(ResetAllPrefs);
    }

    public void CloseSettings()
    {
        Debug.Log("Einstellungen werden geschlossen...");
        settingsPanel.SetActive(false);
    }

    public void OpenGoals()
    {
        Debug.Log("Ziele werden geladen...");
        SceneManager.LoadScene("Completion_Goals");
        
    }

    public void QuitGame()
    {
        Debug.Log("Spiel wird beendet...");
        
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    private bool CheckIfAnyLevelCompleted()
    {
        foreach (string levelId in levelIds)
        {
            if (PlayerPrefs.GetInt($"Level_{levelId}_Completed", 0) == 1) // Überprüfe, ob das Level abgeschlossen ist
            {
                return true; // Mindestens ein Level ist abgeschlossen
            }
        }
        return false; // Kein Level ist abgeschlossen
    }

    public void ResetAllScenes()
    {
        // Lösche alle Level-Completion-Keys
        foreach (string levelId in levelIds)
        {
            if (!string.IsNullOrEmpty(levelId))
            {
                string keyToDelete = $"Level_{levelId}_Completed";
                PlayerPrefs.DeleteKey(keyToDelete);
                Debug.Log($"Level-Fortschritt gelöscht: {keyToDelete}");
            }
        }

        // Lösche alle Typer-Keys (IDs von 1-30)
        for (int i = 1; i <= 30; i++)
        {
            string keyToDelete = $"Typer_{i}_Played";
            if (PlayerPrefs.HasKey(keyToDelete))
            {
                PlayerPrefs.DeleteKey(keyToDelete);
                Debug.Log($"Typer-Fortschritt gelöscht: {keyToDelete}");
            }
        }
        PlayerPrefs.Save();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Lade die aktuelle Szene neu
    }

    public void ResetAllPrefs()
    {
        PlayerPrefs.DeleteAll();
        Debug.Log("Alle Einstellungen wurden zurückgesetzt.");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Lade die aktuelle Szene neu
    }


}
