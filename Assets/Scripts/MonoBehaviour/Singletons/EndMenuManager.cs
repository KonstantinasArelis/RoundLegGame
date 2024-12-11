using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;
public class EndMenuManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject endMenu;
    public TextMeshProUGUI scoreText; // Reference to the score text
    public GameObject creditsPanel;

    [Header("Play Again Scene")]
    [Tooltip("The name of the main menu scene to load.")]
    public string PlayAgainScene = "MainScene";


    [Header("Credits Settings")]
    public RectTransform creditsContent; // RectTransform of the scrolling text
    public float scrollSpeed = 50f;      // Speed of the scrolling credits

    private bool isScrolling = false;

    public void PlayAgain()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(PlayAgainScene);
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
            // Debug.LogWarning("PauseMenuUI is not assigned in the Inspector.");
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

        /// <summary>
    /// Displays the credits panel and starts scrolling.
    /// </summary>
    public void ShowCredits()
    {
        if (creditsPanel != null && creditsContent != null)
        {
            creditsPanel.SetActive(true);
            endMenu.SetActive(false); // Optionally hide the end menu
            StartCoroutine(ScrollCredits());
        }
    }

    /// <summary>
    /// Hides the credits panel and stops scrolling.
    /// </summary>
    public void HideCredits()
    {
        StopAllCoroutines(); // Stop scrolling
        isScrolling = false;
        creditsPanel.SetActive(false);
        endMenu.SetActive(true); // Optionally show the end menu
    }

    /// <summary>
    /// Displays end menu with player score.
    /// </summary>
    /// <param name="score"></param>
    public void ShowEndMenu(int score)
    {
        PauseMenuManager.isPaused = true;    // I'm sorry...
        Time.timeScale = 0f;

        GameObject.Find("SceneFade").GetComponent<SceneFadeController>().FadeOut(() =>
        {
            if (endMenu != null)
            {
                endMenu.SetActive(true);
                creditsPanel.SetActive(false);

                if (scoreText != null)
                {
                    scoreText.text = $"Final score: {score}";
                }
                else
                {
                    Debug.LogError("Score text is not assigned!");
                }
            }
            else
            {
                Debug.LogError("End menu object is not assigned!");
            }
        });
    }

    private IEnumerator ScrollCredits()
    {
        isScrolling = true;
        float startY = creditsContent.anchoredPosition.y;
        float endY = creditsContent.sizeDelta.y;

        while (isScrolling && creditsContent.anchoredPosition.y < endY)
        {
            creditsContent.anchoredPosition += Vector2.up * scrollSpeed * Time.deltaTime;
            yield return null;
        }

        // Reset position when finished
        creditsContent.anchoredPosition = new Vector2(creditsContent.anchoredPosition.x, startY);
        isScrolling = false;

        // Optionally hide credits panel when done
        HideCredits();
    }
}
