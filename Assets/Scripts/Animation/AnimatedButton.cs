//Dieses Skript auf jedem Button platzieren

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class AnimatedButton : MonoBehaviour
{
    [Header("Animation Settings")]
    public bool useAnimation = true;
    public Color pressedColor = new Color(0.8f, 0.8f, 0.8f);
    public float pressedScale = 0.9f;
    
    public Button Button { get; private set; }
    
    private void Awake()
    {
        Button = GetComponent<Button>();
    }

    private void OnEnable()
    {
        if (ButtonAnimationManager.Instance != null)
        {
            ButtonAnimationManager.Instance.RegisterButton(this);
        }
    }
}
