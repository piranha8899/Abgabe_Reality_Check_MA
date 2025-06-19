using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Dieses Skript auf Button platzieren, der Audio abspielen soll

[RequireComponent(typeof(Button))]
public class AudioOnlyButton : MonoBehaviour
{
    [Header("Audio Settings")]
    public bool useAudio = true;
    public AudioClip buttonClickSound;
    [Range(0f, 1f)]
    public float clickVolume = 1f;

    public Button Button { get; private set; }
    private AudioSource audioSource;

    private void Awake()
    {
        Button = GetComponent<Button>();

        if (useAudio)
        {
            SetupAudioSource();
            SetupButtonClick();
        }
    }

    private void SetupAudioSource()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f; // 2D Audio
        audioSource.volume = clickVolume;

        if (buttonClickSound != null)
        {
            audioSource.clip = buttonClickSound;
        }
    }

    private void SetupButtonClick()
    {
        Button.onClick.AddListener(PlayClickSound);
    }
    
    private void PlayClickSound()
    {
        if (useAudio && buttonClickSound != null && audioSource != null && Button.interactable)
        {
            audioSource.PlayOneShot(buttonClickSound, clickVolume);
        }
    }
    
    private void OnDestroy()
    {
        if (Button != null)
        {
            Button.onClick.RemoveListener(PlayClickSound);
        }
    } 
}
