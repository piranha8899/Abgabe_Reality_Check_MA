using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


  public class SceneLoader : MonoBehaviour
{
    // Diese Methode lädt die spezifizierte Szene
    public void LoadScene(string SceneName)
    {
        SceneManager.LoadScene(SceneName);
    }
}
