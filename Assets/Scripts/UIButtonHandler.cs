using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIButtonHandler : MonoBehaviour
{
    public GameObject overlayImage;
    public Button closeButton;


    // Start is called before the first frame update
    void Start()
    {
        if (overlayImage != null)
        {
            overlayImage.SetActive(false);
        }
        
        Button button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(ShowOverlay);
        }

        if (closeButton != null)
        {
            closeButton.onClick.AddListener(HideOverlay);
        }
    }
   
    void ShowOverlay()
    {
        if (overlayImage != null)
        {
            overlayImage.SetActive(true);
        }
        
    }
    void HideOverlay()
    {
        overlayImage.SetActive(false);
    }

    void OnDestroy()
    {
        Button button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.RemoveListener(ShowOverlay);
        }
        
        if (closeButton != null)
        {
            closeButton.onClick.RemoveListener(HideOverlay);
        }
    }
}
