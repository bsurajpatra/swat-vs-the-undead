using UnityEngine;
using UnityEngine.SceneManagement;

public class VictoryManager : MonoBehaviour
{
    // Go back to Main Menu (scene index 0)
    public void GoToMainMenu()
    {
        Debug.Log("🏠 Returning to Main Menu...");
        SceneManager.LoadScene(5); // make sure your Main Menu scene is at index 0
    }

    
}
