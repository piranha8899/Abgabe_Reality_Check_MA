using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Speech_Texttyping : MonoBehaviour
{

    [Tooltip("ID für externe Aufrufe (gültige Werte: 1-30)")]
    public string typerId = "";
    
    public TextMeshProUGUI textDisplay;
    public List<string> textLines;
    public float typingSpeed = 0.05f;
    public List<AudioClip> audioClips;
    private AudioSource audioSource;
    public GameObject continueButton;
    public GameObject[] additionalObjectsToShow; // Alle UI-Elemente, die angezeigt werden sollen
    public bool startAutomatically = true; // Automatischer Start?
    public bool playOnlyOnce = false; // Nur einmal abspielen?
    public float startDelay = 0.5f;
    private int currentLine = 0;
    private bool isTyping = false;
    private bool hasStarted = false;

    // Statischer Dictionary für alle Typer (Liste)
    private static Dictionary<string, Speech_Texttyping> allTypers = new Dictionary<string, Speech_Texttyping>();

    void Awake()
    {
        // Im Dictionary registrieren
        if (!string.IsNullOrEmpty(typerId))
        {
            allTypers[typerId] = this;
        }

        // Deaktivieren, wenn nicht automatisch gestartet
        if (!startAutomatically)
            gameObject.SetActive(false);

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

        void OnDestroy()
    {
        // Aus Dictionary entfernen
        if (!string.IsNullOrEmpty(typerId) && allTypers.ContainsKey(typerId))
            allTypers.Remove(typerId);
    }

    void Start()
    {
        // UI initialisieren
        HideUIElements();
        if (startAutomatically)
            BeginTypingSequence();
    }

    // UI-Elemente ausblenden
    private void HideUIElements()
    {
        if (continueButton != null)
            continueButton.SetActive(false);
            
        foreach (GameObject obj in additionalObjectsToShow)
        {
            if (obj != null)
                obj.SetActive(false);
        }
        
        if (textDisplay != null)
            textDisplay.text = "";
    }
    
    // Starten des Typers von anderen Skripts
    public void BeginTypingSequence()
    {
        if(playOnlyOnce && HasAlreadyPlayed())
        {
            Debug.Log($"Dialog {typerId} wurde gespielt und play only once ist gesetzt. ");
            return;
        }
        
        if (!hasStarted)
        {
            hasStarted = true;
            StartCoroutine(BeginSequence());
        }
    }
    
    // Starten der Sequenz
    private IEnumerator BeginSequence()
    {
        // Verzögerung
        yield return new WaitForSeconds(startDelay);
        
        // Zusätzliche Objekte einblenden
        foreach (GameObject obj in additionalObjectsToShow)
        {
            if (obj != null)
                obj.SetActive(true);
        }
        
        yield return new WaitForSeconds(0.5f);
        StartTyping();
    }
    
    // Startet die Textanimation
    private void StartTyping()
    {
        if (currentLine < textLines.Count)
        {
            isTyping = true;
            textDisplay.text = "";
            StartCoroutine(TypeText());
        }
        else
        {
            // Alle Texte wurden angezeigt
            FinishTyping();
        }
    }
    
    // Text-Animations-Coroutine
    private IEnumerator TypeText()
    {
        // Ton abspielen, wenn vorhanden
        if (currentLine < audioClips.Count && audioClips[currentLine] != null)
        {
            audioSource.clip = audioClips[currentLine];
            audioSource.Play();
        }
        
        string currentText = textLines[currentLine];
        
        // Text Buchstabe für Buchstabe anzeigen
        foreach (char letter in currentText)
        {
            textDisplay.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
        
        isTyping = false;
        
        // Warten auf Klick für nächste Zeile
        yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
        
        currentLine++;
        StartTyping();
    }
    
    // Beendet den Typing-Prozess
    private void FinishTyping()
    { 
        if (playOnlyOnce)
        {
        MarkAsPlayed();
        }
        // Button einblenden
        if (continueButton != null)
            continueButton.SetActive(true);
    }
    // Prüfen, ob Dialog schon gespielt wurde
    private bool HasAlreadyPlayed()
    {
    string speechkey = GetPrefKey();
    return PlayerPrefs.GetInt(speechkey, 0) == 1;
    }

    // Markiert dialog als abgespielt
    private void MarkAsPlayed()
    {
    string speechkey = GetPrefKey();
    PlayerPrefs.SetInt(speechkey, 1);
    PlayerPrefs.Save();
    Debug.Log($"Dialog '{typerId}' als abgespielt markiert: {speechkey}");
    }

    // Bestimme den zu verwendenden PlayerPrefs-Key
    private string GetPrefKey()
    {
        return $"Typer_{typerId}_Played";
    }
    
    // Sofortiges Anzeigen des kompletten Texts (beim Klick)
    void Update()
    {
        if (isTyping && Input.GetMouseButtonDown(0))
        {
            StopAllCoroutines();
            isTyping = false;
            textDisplay.text = textLines[currentLine];
            
            // Warten auf erneuten Klick für nächste Zeile
            StartCoroutine(WaitForNextLine());
        }
    }
    
    private IEnumerator WaitForNextLine()
    {
        yield return new WaitForSeconds(0.5f); // Kurze Pause
        yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
        
        currentLine++;
        StartTyping();
    }
    
    // Zurücksetzen der Sequenz (für erneute Verwendung)
    public void ResetTyper()
    {
        StopAllCoroutines();
        currentLine = 0;
        hasStarted = false;
        isTyping = false;
        HideUIElements(); //Alle Elemente ausblenden
        
    }

    //STATICS für externen Zugriff

    //Dialog via ID Starten
    public static void StartDialog(string id)
    {
        if (allTypers.ContainsKey(id))
        {
            Speech_Texttyping typer = allTypers[id];
            typer.gameObject.SetActive(true); //Aktivieren
            typer.ResetTyper(); //Zurücksetzen
            typer.BeginTypingSequence(); //Starten
        }
        else
        {
            Debug.LogWarning("Typer mit ID " + id + " nicht gefunden.");
        }
    }

    public static bool CheckPlayed(string id)
    {
        if (allTypers.ContainsKey(id))
        {
            Speech_Texttyping typer = allTypers[id];
            return typer.HasAlreadyPlayed();

        }
        return false;
    }

    // Prüfen ob Dialog existiert
    public static bool HasDialog(string id)
    {
        return allTypers.ContainsKey(id);
    }
}