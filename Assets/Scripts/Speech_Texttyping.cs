using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SimpleTextTyper : MonoBehaviour
{

    public TextMeshProUGUI textDisplay;
    public List<string> textLines;
    public float typingSpeed = 0.05f;
    public List<AudioClip> audioClips;
    private AudioSource audioSource;
    public GameObject continueButton;
    public GameObject textContainer;
    public GameObject[] additionalObjectsToShow;
    public bool startAutomatically = true;
    public float startDelay = 0.5f;
    private int currentLine = 0;
    private bool isTyping = false;
    private bool hasStarted = false;

    void Awake()
    {
        gameObject.SetActive(true);
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    void Start()
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
        
        // Automatischer Start falls gewünscht
        if (startAutomatically)
        {
            BeginTypingSequence();
        }
    }
    
    // Öffentliche Methode zum Starten des Typers von anderen Skripts
    public void BeginTypingSequence()
    {
        if (!hasStarted)
        {
            hasStarted = true;
            StartCoroutine(BeginSequence());
        }
    }
    
    // Ablaufsequenz für Starten
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
        
        yield return new WaitForSeconds(0.5f); // Kurze Pause
            
        // Starte Textanimation
        StartTyping();
    }
    
    // Startet die Textanimation für die aktuelle Zeile
    public void StartTyping()
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
        // Button einblenden
        if (continueButton != null)
            continueButton.SetActive(true);
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
    
    // Geschwindigkeit während der Laufzeit ändern
    public void SetTypingSpeed(float speed)
    {
        typingSpeed = speed;
    }
    
    // Zurücksetzen der Sequenz (für erneute Verwendung)
    public void ResetTyper()
    {
        StopAllCoroutines();
        currentLine = 0;
        hasStarted = false;
        isTyping = false;
        
        if (textDisplay != null)
            textDisplay.text = "";
        
        if (continueButton != null)
            continueButton.SetActive(false);
        
        // Zusätzliche Objekte ausblenden
        foreach (GameObject obj in additionalObjectsToShow)
        {
            if (obj != null)
                obj.SetActive(false);
        }
    }
}