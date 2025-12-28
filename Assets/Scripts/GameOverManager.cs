using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    void Start()
    {
        // In GameOver scene, ensure mouse is free and game is running
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        AudioListener.pause = false;
    }

    // This will load the main menu scene (index 0)
    public void GoToMainMenu()
    {
        Debug.Log("🏠 Returning to Main Menu...");
        SceneManager.LoadScene(5); // loads scene at index 0
    }

}
