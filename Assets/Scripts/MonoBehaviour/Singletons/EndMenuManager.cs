using UnityEngine;
using UnityEngine.SceneManagement;

public class EndMenuManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject endMenu;

    [Header("Main Menu Scene")]
    [Tooltip("The name of the main menu scene to load.")]
    public string mainMenuSceneName = "MainMenu";

    public void PlayAgain()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainScene");
    }

    void Start()
    {
        // Disable the pause menu when the scene loads
        if (endMenu != null)
        {
            endMenu.SetActive(false);
        }
        else
        {
            Debug.LogWarning("PauseMenuUI is not assigned in the Inspector.");
        }
    }
    /// <summary>
    /// Loads the main menu scene.
    /// </summary>
    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
    }

    /// <summary>
    /// Exits the game.
    /// </summary>
    public void QuitGame()
    {
        Time.timeScale = 1f;
        Debug.Log("Quitting Game...");
        Application.Quit();
    }
}
