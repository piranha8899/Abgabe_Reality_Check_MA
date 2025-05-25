using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// Dieses Skript muss auf dem GameObject liegen, das den Animator hat.

public class OnClick_AnimationTrigger : MonoBehaviour
{

    [SerializeField] private string triggerName = "Clicked";
    [SerializeField] private Animator animator;

    private void Start()
    {
        // Animator automatisch finden
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        if (animator == null)
        {
            Debug.LogError("Kein Animator gefunden.");
        }
    }

    // Wird automatisch aufgerufen, wenn auf das Objekt geklickt wird
    public void OnPointerClick(PointerEventData eventData)
    {
        TriggerAnimation();
    }

    private void TriggerAnimation()
    {
        if (animator != null)
        {
            animator.SetTrigger(triggerName);
        }
    }

    // Öffentliche Methode, um die Animation manuell auszulösen
    public void PlayAnimation()
    {
        TriggerAnimation();
    }
}
