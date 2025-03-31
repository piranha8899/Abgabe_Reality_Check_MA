using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CloseCompleteButton : MonoBehaviour
{

    [SerializeField] private GameObject overlayToClose;
    [SerializeField] private string scenetoLoad;
    
    // Start is called before the first frame update
    void Start()
    {
        Button button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(CloseOverlay);
            button.onClick.AddListener(CloseOverlayAndLoadScene);
        }
        
    }

    public void CloseOverlay()
    {
        if (overlayToClose != null)
        {
            overlayToClose.SetActive(false);
        }
    }

    public void CloseOverlayAndLoadScene()
    {
        if (overlayToClose != null)
        {
            overlayToClose.SetActive(false);
            SceneManager.LoadScene(scenetoLoad);
        }
    }
}
