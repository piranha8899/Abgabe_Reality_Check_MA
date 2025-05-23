using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroHintAnimationManager : MonoBehaviour
{
    public GameObject requiredObject;
    public GameObject hintObject;
    public float delay = 3f; // Zeit in Sekunden bis zur Einblendung
    private Coroutine hintCoroutine;

    private void Start()
    {
        if (requiredObject != null && hintObject != null)
        {
            if (hintCoroutine != null)
            {
                StopCoroutine(hintCoroutine);
            }
            hintCoroutine = StartCoroutine(ShowHintAfterDelay());
        }
    }

    private IEnumerator ShowHintAfterDelay()
    {
        yield return new WaitForSeconds(delay);
        hintObject.SetActive(true);
        hintCoroutine = null;
    }

    private void OnMouseDown()
    {
        if(requiredObject != null && hintObject != null && hintObject.activeSelf)
        {
            hintObject.SetActive(false);
        }
        // Falls eine laufende Coroutine existiert, stoppe sie
            if (hintCoroutine != null)
            {
                StopCoroutine(hintCoroutine);
                hintCoroutine = null;
            }
    }
}
