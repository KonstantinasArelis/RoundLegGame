using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("MainScene"); // Replace "GameScene" with the name of your game scene
    }

    public void OpenSettings()
    {
        // Code to open the settings screen or menu
    }

    public void QuitGame()
    {
        Application.Quit(); // Quits the game (works only in the built version)
    }
}
