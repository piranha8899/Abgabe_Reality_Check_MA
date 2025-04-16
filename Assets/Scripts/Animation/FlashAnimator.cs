using System.Collections;
using UnityEngine;

public class FlashAnimator : MonoBehaviour
{

    [Header("Blink-Einstellungen")]
    [SerializeField] private string boolParameterName = "IsFlashing";  // Name des Trigger-Parameters im Animator
    [SerializeField] private float maxStartDelay = 0.5f;                 // Maximale Verzögerung für den Start

    [SerializeField] private float minTimeScale = 0.95f;  // Minimaler Zeitfaktor für Animatoren
    [SerializeField] private float maxTimeScale = 1.05f;  // Maximaler Zeitfaktor für Animatoren

    // Liste aller Animatoren (Child eines Objekts)
    private Animator[] childAnimators;

    void OnEnable()
    {
        childAnimators = GetComponentsInChildren<Animator>();
        Debug.Log($"{childAnimators.Length} Sprites gefunden.");
        
        // Jeder Animator bekommt eine zufällige Startverzögerung
        foreach (Animator animator in childAnimators)
        {
            float randomSpeed = Random.Range(minTimeScale, maxTimeScale);
            animator.speed = randomSpeed; // Setze die Animationsgeschwindigkeit, damit blinken nicht synchron ist
            animator.Play(0, 0, Random.value);
            float randomDelay = Random.Range(0f, maxStartDelay);
            StartCoroutine(StartBlinking(animator, randomDelay));
        }
    }

    void OnDisable()
    {
        StopAllCoroutines();

        // Alle Animationen auf false setzen
        if (childAnimators != null)
        {
            foreach (Animator animator in childAnimators)
            {
                if (animator != null)
                {
                    animator.SetBool(boolParameterName, false);
                }
            }
        }
    }

    private IEnumerator StartBlinking(Animator animator, float delay)

    {
        animator.SetBool(boolParameterName, false);
        
        // Verzögerung abwarten
        yield return new WaitForSeconds(delay);
        
        // Blinken aktivieren
        animator.SetBool(boolParameterName, true);
    }

}
