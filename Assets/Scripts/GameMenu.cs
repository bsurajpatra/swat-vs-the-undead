using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMenu : MonoBehaviour
{
    public void BackToMainMenu()
    {
        Debug.Log("Returning to Main Menu...");
        // Ensure normal game speed and cursor before loading
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Load by scene name to avoid build index issues
        SceneManager.LoadScene(5);
    }

    public void QuitGame()
    {
        Debug.Log("Quitting Game...");
        Application.Quit();
    }
}
