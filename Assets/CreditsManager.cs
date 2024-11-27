using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;


public class CreditsManager : MonoBehaviour
{
    [SerializeField] private float delayTime = 20f; // Time in seconds before switching scenes
    [SerializeField] private string menuSceneName = "Menu"; // Name of the Menu scene

    void Start()
    {
        // Start the coroutine to wait and load the menu scene
        StartCoroutine(TransitionToMenu());
    }

    private IEnumerator TransitionToMenu()
    {
        // Wait for the specified delay time
        yield return new WaitForSeconds(delayTime);

        // Load the menu scene
        SceneManager.LoadScene(menuSceneName);
    }
}
