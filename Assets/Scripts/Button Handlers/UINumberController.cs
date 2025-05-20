using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UINumberController : MonoBehaviour
{
    public string progressKey;

    public TextMeshProUGUI numberDisplay; // TMP Text component
    
    public Button plusButton;
   
    public Button minusButton;
    
    [Header("Number Settings")]
    public int currentNumber = 0; // Startwert

    public int stepSize = 1;  // Schrittgrösse für Erhöhung/Verringerung
    
    public int minValue = 1;
    
    public int maxValue = 100;

    // Start is called before the first frame update
    void Start()
    {
        // Prüfe ob alle Komponenten zugewiesen sind
        if (numberDisplay == null || plusButton == null || minusButton == null)
        {
            Debug.LogError("Nicht alle erforderlichen Komponenten sind zugewiesen!");
            return;
        }

        // Gespeicherten Wert laden falls vorhanden
        if (!string.IsNullOrEmpty(progressKey))
        {
            currentNumber = SceneProgressManager.Instance.GetValue(progressKey, currentNumber);
        }

        // Click Events registrieren
        plusButton.onClick.AddListener(IncreaseNumber);
        minusButton.onClick.AddListener(DecreaseNumber);

        UpdateDisplay();
    }

    void IncreaseNumber()
    {
        // Erhöhe um stepSize, aber nicht über maxValue
        currentNumber = Mathf.Min(currentNumber + stepSize, maxValue);
        UpdateDisplay();
    }

    void DecreaseNumber()
    {
        // Verringere um stepSize, aber nicht unter minValue
        currentNumber = Mathf.Max(currentNumber - stepSize, minValue);
        UpdateDisplay();
    }

    void UpdateDisplay()
    {
        numberDisplay.text = currentNumber.ToString();
        
        // Speichere Wert wenn Key gesetzt
        if (!string.IsNullOrEmpty(progressKey))
        {
            SceneProgressManager.Instance.SetValue(progressKey, currentNumber);
        }

        // Optional: Buttons deaktivieren wenn Grenzen erreicht
        if (plusButton != null)
            plusButton.interactable = currentNumber < maxValue;
        
        if (minusButton != null)
            minusButton.interactable = currentNumber > minValue;
    }

    // Setzen der Schrittgrösse (Runtime)
    public void SetStepSize(int newStepSize)
    {
        stepSize = Mathf.Max(1, newStepSize); // Mindestens 1
    }

    // Direktes Setzen des Wertes
    public void SetNumber(int newNumber)
    {
        currentNumber = Mathf.Clamp(newNumber, minValue, maxValue);
        UpdateDisplay();
    }

    // Getter für aktuellen Wert
    public int GetCurrentNumber()
    {
        return currentNumber;
    }

    void OnDestroy()
    {
        if (plusButton != null)
            plusButton.onClick.RemoveListener(IncreaseNumber);
        
        if (minusButton != null)
            minusButton.onClick.RemoveListener(DecreaseNumber);
    }
}
