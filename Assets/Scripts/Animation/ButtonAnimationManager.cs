//Dieses Skript als Manager in Empty GameObject im Hauptmenü

using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class ButtonAnimationManager : MonoBehaviour
{
    public static ButtonAnimationManager Instance { get; private set; }

    [Header("Animation Default")]
    public float duration = 0.1f;

    // Verwaltete Buttons mit aktiven Overlays
    private Dictionary<Button, GameObject> managedButtons = new Dictionary<Button, GameObject>();
    private bool sceneLoad = false; // Flag für Szenenwechsel

    public void RegisterButton(AnimatedButton animatedButton)
    {
        if (animatedButton != null && animatedButton.useAnimation && animatedButton.Button != null && !managedButtons.ContainsKey(animatedButton.Button))
        {
            SetupButtonAnimation(animatedButton);
        }
    }
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
            ScanForAnimatedButtons();
        }
        
        else
        {
            Destroy(gameObject); // Zerstöre Duplikate
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        sceneLoad = true;
        ScanForAnimatedButtons();
    }
    
    // Manuelles Auslösen eines Scans
    public void ScanForAnimatedButtons()
    {
        //Bei Sceneload zuerst initialisieren, dann scannen
        if(sceneLoad)
        {
            StartCoroutine(DelayedScan());
        }
        else
        {
            NormalScan();
        }
    }

    private IEnumerator DelayedScan()
    {
        yield return null;
        yield return null;

        // Bestehende Button-Overlays entfernen
        foreach (var overlayObj in managedButtons.Values)
        {
            if (overlayObj != null)
            Destroy(overlayObj);
        }
        managedButtons.Clear();
        sceneLoad = false;
    
        // Scan nach dem Aufräumen durchführen
        NormalScan();
    }

    private void NormalScan()
    {
        // Finde alle AnimatedButtons in der aktuellen Szene, Skript muss auf Button liegen
        AnimatedButton[] buttons = FindObjectsOfType<AnimatedButton>();
    
        foreach (var animBtn in buttons)
        {
            if (animBtn.useAnimation && animBtn.Button != null && !managedButtons.ContainsKey(animBtn.Button))
            {
                SetupButtonAnimation(animBtn);
            }
        }
    }

    //Button-Animation aufsetzen
    private void SetupButtonAnimation(AnimatedButton animatedButton)
    {
        Button button = animatedButton.Button;
        Image targetImage = button.GetComponent<Image>();
        
        if (targetImage == null)
            return;
            
        Color originalColor = targetImage.color;
        Vector3 originalScale = button.transform.localScale;
        
        // Overlay erstellen
        GameObject overlay = new GameObject("AnimationOverlay");
        overlay.transform.SetParent(button.transform);
        overlay.transform.localPosition = Vector3.zero;
        
        //Overlay tranformieren auf richtige Grösse
        RectTransform rectTransform = overlay.AddComponent<RectTransform>();
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.offsetMin = rectTransform.offsetMax = Vector2.zero;
        
        Image overlayImage = overlay.AddComponent<Image>();
        overlayImage.color = new Color(0, 0, 0, 0); // Transparent
        
        Button overlayButton = overlay.AddComponent<Button>();
        overlayButton.navigation = button.navigation;
        
        // Klick-Handler
        overlayButton.onClick.AddListener(() => {
            if (button.interactable)
            {
                DOTween.Sequence()
                    .Append(targetImage.DOColor(animatedButton.pressedColor, duration))
                    .Join(button.transform.DOScale(originalScale * animatedButton.pressedScale, duration))
                    .Append(targetImage.DOColor(originalColor, duration))
                    .Join(button.transform.DOScale(originalScale, duration))
                    .OnComplete(button.onClick.Invoke);
            }
        });
        
        overlay.transform.SetAsLastSibling(); // Nach vorne bringen
        
        // In die Liste der verwalteten Buttons aufnehmen
        managedButtons[button] = overlay;
    }
    
    // Wenn ein Button gelöscht oder deaktiviert wird
    private void OnDestroy()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
        // Alle Overlays bereinigen
        foreach (var overlayObj in managedButtons.Values)
        {
            if (overlayObj != null)
                Destroy(overlayObj);
        }
        
        managedButtons.Clear();
        DOTween.KillAll();
    }
}