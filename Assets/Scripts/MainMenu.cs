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
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private string gameSceneName = "GameScene";

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
            
        if (goalsButton != null)
            goalsButton.onClick.AddListener(OpenGoals);
            
        if (quitButton != null)
            quitButton.onClick.AddListener(QuitGame);

            settingsPanel.SetActive(false); // Setze das Einstellungs-Panel auf unsichtbar
            
        
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


}
