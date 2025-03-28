using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIButtonHandler : MonoBehaviour
{

    
    // Aufruf direkt über OnClick Methode auf dem Button
    
    // Zeigt ein GameObject an
    public void ShowObject(GameObject obj)
    {
        if (obj != null)
            obj.SetActive(true);
    }
    
    // Versteckt ein GameObject
    public void HideObject(GameObject obj)
    {
        if (obj != null)
            obj.SetActive(false);
    }
    
    // Wechselt den Zustand eines GameObjects
    public void ToggleObject(GameObject obj)
    {
        if (obj != null)
            obj.SetActive(!obj.activeSelf);
    }
}
