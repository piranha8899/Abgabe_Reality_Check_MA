using UnityEngine;
using UnityEngine.UI;

public class UIButtonSwapper : MonoBehaviour
{
    
    public Button firstButton;
        
    public Button secondButton;

    public string progressKey; //Key, der dann über SceneProgressManager aufgerufen wird
    public bool startWithSecondButton = false; //Wenn false, wird mit First Button gestartet

    void Start()
    {
        // Prüfe ob beide Buttons zugewiesen sind
        if (string.IsNullOrEmpty(progressKey))
        {
            Debug.LogError("Progress Key nicht gesetzt!");
            return;
        }

        // Prüfe ob gespeicherter Zustand existiert
        bool hasStoredState = SceneProgressManager.Instance.HasValue(progressKey);
        
        // Setze initialen Zustand
        bool isSecondActive;
        if (hasStoredState)
        {
            // Nutze gespeicherten Zustand
            isSecondActive = SceneProgressManager.Instance.GetValue(progressKey, false);
        }
        else
        {
            // Nutze konfigurierten Startzustand
            isSecondActive = startWithSecondButton;
            SceneProgressManager.Instance.SetValue(progressKey, isSecondActive);
        }
        // Aktiviere entsprechenden Button
        firstButton.gameObject.SetActive(!isSecondActive);
        secondButton.gameObject.SetActive(isSecondActive);

        // Click Events registrieren
        firstButton.onClick.AddListener(SwapToSecond);
        secondButton.onClick.AddListener(SwapToFirst);
    }

    void SwapToSecond()
    {
        firstButton.gameObject.SetActive(false);
        secondButton.gameObject.SetActive(true);
        //In Progress Manager schreiben, dass Key true ist
        SceneProgressManager.Instance.SetValue(progressKey, true);
    }

    void SwapToFirst()
    {
        secondButton.gameObject.SetActive(false);
        firstButton.gameObject.SetActive(true);
        //In Progress Manager schreiben, dass Key false ist
        SceneProgressManager.Instance.SetValue(progressKey, false);
    }

    void OnDestroy()
    {
        // Events wieder entfernen
        if (firstButton != null)
            firstButton.onClick.RemoveListener(SwapToSecond);
        
        if (secondButton != null)
            secondButton.onClick.RemoveListener(SwapToFirst);
    }
}
