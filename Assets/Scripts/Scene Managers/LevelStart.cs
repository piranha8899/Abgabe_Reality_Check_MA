using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelStart : MonoBehaviour
{

    [SerializeField] private List<TextMeshProUGUI> introText;
    [SerializeField] private List<Image> introImages; // Falls Bilder im Intro verwendet werden sollen
    [SerializeField] private Image introBackgroundImage;
    [SerializeField] private float displayDuration = 3.0f;
    [SerializeField] private float fadeDuration = 1.0f;
    [SerializeField] private Button instructionButton;
    [SerializeField] private GameObject completionOverlay; // Referenz auf das Overlay-Objekt

    private bool isCoroutineRunning = false;

    //ACHTUNG: Skript muss zwingend auf demselben Objekt liegen (aufgrund GameObject.SetActive).

    // Start is called before the first frame update
    void Start()
    {
        if ((introText == null || introText.Count == 0) && 
            (introImages == null || introImages.Count == 0) && 
            introBackgroundImage == null)
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
        yield return new WaitForEndOfFrame();

        if(!completionOverlay.activeSelf)
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
    }

    // Fade UI ein- und ausblenden
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
            foreach (TextMeshProUGUI text in introText)
            {
                Color textColor = text.color;
                textColor.a = alpha;
                text.color = textColor;
            }
        }

        // Bilder
        if (introImages != null)
        {
            foreach (Image image in introImages)
            {
                Color imgColor = image.color;
                imgColor.a = alpha;
                image.color = imgColor;
            }
        }
        
        // Hintergrund
        if (introBackgroundImage != null)
        {
            Color bgColor = introBackgroundImage.color;
            bgColor.a = alpha;
            introBackgroundImage.color = bgColor;
        }
    }

    // Darstellung setzen
    public void SetRaycasts(bool enableRaycasts)
    {
        if (introBackgroundImage != null)
        {
            introBackgroundImage.raycastTarget = enableRaycasts;
        }
        if (introText != null)
        {
            foreach (TextMeshProUGUI text in introText)
            {
                text.raycastTarget = enableRaycasts;
            }
        }
        if (introImages != null)
        {
            foreach (Image image in introImages)
            {
                image.raycastTarget = enableRaycasts;
            }
        }
    }

    
}
