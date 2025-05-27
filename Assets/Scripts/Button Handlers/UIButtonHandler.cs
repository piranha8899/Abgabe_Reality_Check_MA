using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIButtonHandler : MonoBehaviour
{
    // Aufruf direkt über OnClick Methode auf dem Button

    public void ShowObject(GameObject obj)
    {
        if (obj != null)
            obj.SetActive(true);
    }

    public void SwitchShowObject(GameObject obj)
    {
        if (obj != null)
        {
            bool isActive = obj.activeSelf;
            obj.SetActive(!isActive);
        }
    }
    
    public void HideObject(GameObject obj)
    {
        if (obj != null)
            obj.SetActive(false);
    }
    
    public void ToggleObject(GameObject obj)
    {
        if (obj != null)
            obj.SetActive(!obj.activeSelf);
    }
}
