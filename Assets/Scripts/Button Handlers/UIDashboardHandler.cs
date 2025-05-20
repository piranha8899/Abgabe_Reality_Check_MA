

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIDashboardHandler : MonoBehaviour
{
    public GameObject overlayImage;
    public Button nextButton;
    private bool showOverlayAutomatically = true; // Option zum automatischen Einblenden
    // Start is called before the first frame update
    void Start()
    {
       if (overlayImage != null)
        {
            overlayImage.SetActive(false);
        }

        if (SceneProgressManager.Instance != null)
        {
            if (showOverlayAutomatically && SceneProgressManager.Instance.IsLevelCompleted)
            {
                ShowOverlay();
            }
        }

        Button button = GetComponent<Button>();

        if (nextButton != null)
        {
            nextButton.onClick.AddListener(HideOverlay);
        }
    }

    // Overlay über einen Button Event triggern. Muss auf Button gesetzt werden.
    public void TriggerOverlayCheck()
    {
        StartCoroutine(OverlayCheck());
    }

    private IEnumerator OverlayCheck()
    {
        // Warten, damit Overlay schliessen kann
        yield return new WaitForSeconds(0.3f);
        
        // Prüfe, ob das Level abgeschlossen ist und zeige ggf. das Completion Overlay
        if (SceneProgressManager.Instance != null && 
            SceneProgressManager.Instance.IsLevelCompleted && 
            showOverlayAutomatically)
        {
            ShowOverlay();
        }
    }

    void CheckStatus()
    {
    if (SceneProgressManager.Instance.IsLevelCompleted)
        {
        // Code für Abschlusscheck möglich
        }
    }


    void ShowOverlay()
    {
        //Debug.Log("Button clicked");
        if (overlayImage != null)
        {
            //Debug.Log("Show overlay");
            overlayImage.SetActive(true);
        }
        else
        {
            Debug.Log("Overlay ref not found");
        }
        
    }
    void HideOverlay()
    {
        overlayImage.SetActive(false);
        if(SceneProgressManager.Instance.IsLevelCompleted)
        {
            SceneManager.LoadScene("Panel");
        }
    }
    
}
