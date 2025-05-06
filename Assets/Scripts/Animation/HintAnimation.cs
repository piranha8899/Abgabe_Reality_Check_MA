using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HintAnimation : MonoBehaviour
{
    [System.Serializable]
    public class HintConfig
    {
        public string hintId; // ID für den Hinweis
        public GameObject hintObject;
        public float delay = 5f; // Zeit in Sekunden bis zur Einblendung
        public Button associatedButton;
        public string[] requiredHints;
        public List<GameObject> blockingObjects = new List<GameObject>(); // Objekte, die den Hinweis blockieren (z.B Overlays)
        public bool isShown = false;
        public bool isDismissed = false;
    }
    public List<HintConfig> availableHints = new List<HintConfig>();
    private Dictionary<string, HintConfig> hintsById = new Dictionary<string, HintConfig>();
    private List<Coroutine> activeHintCoroutines = new List<Coroutine>();
    private bool systemActive = true;
    

    void Start()
    {
        // Alle Hint-Objekte initial deaktivieren und in Dictionary eintragen
        foreach (var hint in availableHints)
        {
            if (hint.hintObject != null)
            {
                hint.hintObject.SetActive(false);
            }
            
            if (hint.associatedButton != null)
            {
                hint.associatedButton.onClick.AddListener(() => OnButtonClicked(hint));
            }

            // Hinweis im Dictionary registrieren
            hintsById[hint.hintId] = hint;
        }
        
        // Starte Überwachung für alle Hinweise
        StartHintMonitoring();
    }

    public void StartHintMonitoring()
    {
        systemActive = true;
        foreach (var hint in availableHints)
        {
            if (!hint.isDismissed)
            {
                var coroutine = StartCoroutine(MonitorHint(hint));
                activeHintCoroutines.Add(coroutine);
            }
        }
    }

    public void StopAllHints()
    {
        systemActive = false;
        foreach (var coroutine in activeHintCoroutines)
        {
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }
        }
        activeHintCoroutines.Clear();
        // Alle Hinweise ausblenden
        foreach (var hint in availableHints)
        {
            if (hint.hintObject != null)
            {
                hint.hintObject.SetActive(false);
            }
        }
    }

    // Überprüft kontinuierlich, ob ein Hinweis angezeigt werden sollte
    private IEnumerator MonitorHint(HintConfig hint)
    {
        // Kontinuierliche Überwachung, bis der Hinweis bestätigt wurde
        while (!hint.isDismissed && systemActive)
        {
            bool canShow = true;
            // requiredHints überprüfen
            if (hint.requiredHints != null && hint.requiredHints.Length > 0)
            {
                foreach (var requiredHintId in hint.requiredHints)
                {
                    if (hintsById.TryGetValue(requiredHintId, out HintConfig requiredHint))
                    {
                        if (!requiredHint.isShown || !requiredHint.isDismissed)
                        {
                            canShow = false;
                            break;
                        }
                    }
                }
            }

            CheckBlockingObjects(hint, ref canShow); // Überprüfe blockierende Objekte
            // Zeige den Hinweis an, wenn alle Bedingungen erfüllt sind
            if (canShow && !hint.isShown)
            {
                yield return new WaitForSeconds(hint.delay); // Warte auf die Verzögerung
                CheckBlockingObjects(hint, ref canShow); // Überprüfe blockierende Objekte nochmals nach Delay
                if(canShow && !hint.isShown &&systemActive && !hint.isDismissed)
                    ShowHint(hint);
            }
            yield return new WaitForSeconds(1f); // Regelmässige Überprüfung
        }
    }
    private void ShowHint(HintConfig hint)
    {
        if (hint.hintObject != null)
        {
            hint.hintObject.SetActive(true);
            hint.isShown = true;
        }
    }
    private void HideHint(HintConfig hint)
    {
        if (hint.hintObject != null)
        {
            hint.hintObject.SetActive(false);
        }
    }
    private void OnButtonClicked(HintConfig clickedHint)
    {
        // Hinweis als bestätigt markieren
        clickedHint.isDismissed = true;
        
        // Hinweis ausblenden
        HideHint(clickedHint);
    }
    private void CheckBlockingObjects(HintConfig hint, ref bool canShow)
    {if (canShow && hint.blockingObjects != null && hint.blockingObjects.Count > 0)
            {
                foreach (var blockingObject in hint.blockingObjects)
                {
                    if (blockingObject != null && blockingObject.activeInHierarchy)
                    {
                        canShow = false; // Hinweis kann nicht angezeigt werden, wenn ein blockierendes Objekt aktiv ist
                        break;
                    }
                }
            }
    }
}