using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public void StartGame()
    {
        Debug.Log("✅ Start button clicked!");
        // Go to LevelScene (build index 5) where player chooses the level
        SceneManager.LoadScene(5);
    }

    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
    }

}
