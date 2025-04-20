using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HintAnimation : MonoBehaviour
{
    [System.Serializable]
    public class HintConfig
    {
        public GameObject hintObject;     // GameObject mit Animation Controller
        public float delay = 5f; // Zeit in Sekunden bis zur Einblendung
        public Button associatedButton;
    }
    public List<HintConfig> hintSequence = new List<HintConfig>();
    private int currentHintIndex = -1;
    private Coroutine activeHintCoroutine;

    void Start()
    {
        // Alle Hint-Objekte initial deaktivieren
        foreach (var hint in hintSequence)
        {
            if (hint.hintObject != null)
            {
                hint.hintObject.SetActive(false);
            }
            
            if (hint.associatedButton != null)
            {
                hint.associatedButton.onClick.AddListener(() => OnButtonClicked(hint));
            }
        }
        
        StartHint();
    }

    public void StartHint()
    {
        // Vorherigen Hinweis stoppen
        if (activeHintCoroutine != null)
        {
            StopCoroutine(activeHintCoroutine);
        }
        
        if (currentHintIndex >= 0 && currentHintIndex < hintSequence.Count)
        {
            var currentHint = hintSequence[currentHintIndex];
            if (currentHint.hintObject != null)
            {
                currentHint.hintObject.SetActive(false);
            }
        }

        currentHintIndex++; // Nächster Hinweis

        // Fortschritt prüfen
        if (currentHintIndex >= hintSequence.Count)
        {
            currentHintIndex = -1;
            return;
        }
        
        // Nächsten Hinweis mit Verzögerung starten
        activeHintCoroutine = StartCoroutine(ShowHint(currentHintIndex));
    }

    private IEnumerator ShowHint(int hintIndex)
    {
        var hint = hintSequence[hintIndex];
        
        yield return new WaitForSeconds(hint.delay);
        
        // Nur anzeigen, wenn das noch der aktuelle Hinweis ist
        if (currentHintIndex == hintIndex && hint.hintObject != null)
        {
            hint.hintObject.SetActive(true);
        }
    }

    // Wird aufgerufen, wenn ein Button geklickt wird
    private void OnButtonClicked(HintConfig clickedHint)
    {
        // Den aktuellen Hinweis deaktivieren
        if (clickedHint.hintObject != null)
        {
            clickedHint.hintObject.SetActive(false);
        }
        
        // Nächster Hinweis
        StartHint();
    }

}
