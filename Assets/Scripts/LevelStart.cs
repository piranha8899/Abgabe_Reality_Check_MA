using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelStart : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI introText;
    [SerializeField] private Image introBackgroundImage;
    [SerializeField] private float displayDuration = 3.0f;
    [SerializeField] private float fadeDuration = 1.0f;
    [SerializeField] private Button instructionButton;

    private bool isCoroutineRunning = false;

    //ACHTUNG: Skript muss zwingend auf demselben Objekt liegen (aufgrund GameObject.SetActive).

    // Start is called before the first frame update
    void Start()
    {
        if (introText == null || introBackgroundImage == null)
        {
            Debug.LogError("Fehlendes Intro-Element.");
            return;
        }

        

        // Text und Hintergrund initial unsichtbar
        SetUIAlpha(0f);

        // Erste Einblendung direkt starten
        StartCoroutine(HandleTextDisplay());

        //Listener für Button
        if (instructionButton != null)
        {
            instructionButton.onClick.AddListener(TriggerInstruction);
        }
        
    }

    public void TriggerInstruction()
    {
        // Verhindert mehrfaches Starten während des laufenden Fades
        if (!isCoroutineRunning)
        {
            StartCoroutine(HandleTextDisplay());
        }
    }

    private System.Collections.IEnumerator HandleTextDisplay()
    {
        isCoroutineRunning = true;

        SetRaycasts(true);
        //Einblenden
        yield return StartCoroutine(FadeUI(0f, 1f, fadeDuration));
        // Warte für die eingestellte Zeit
        yield return new WaitForSeconds(displayDuration);
        //Ausblenden
        yield return StartCoroutine(FadeUI(1f, 0f, fadeDuration));
        SetRaycasts(false);

        isCoroutineRunning = false;
    }

     private System.Collections.IEnumerator FadeUI(float startAlpha, float endAlpha, float duration)
    {
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / duration);
            SetUIAlpha(alpha);
            yield return null;
        }
        SetUIAlpha(endAlpha);
    }

    private void SetUIAlpha(float alpha)
    {
        // Text
        if (introText != null)
        {
            Color textColor = introText.color;
            textColor.a = alpha;
            introText.color = textColor;
        }

        // Hintergrund
        if (introBackgroundImage != null)
        {
            Color bgColor = introBackgroundImage.color;
            bgColor.a = alpha;
            introBackgroundImage.color = bgColor;
        }
    }

    public void SetRaycasts(bool enableRaycasts)
    {
        if(introBackgroundImage != null)
        {
            introBackgroundImage.raycastTarget = enableRaycasts;
        }
        if(introText != null)
        {
            introText.raycastTarget = enableRaycasts;
        }
    }

    
}
