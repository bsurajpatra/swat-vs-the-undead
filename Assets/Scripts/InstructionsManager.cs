using UnityEngine;
using UnityEngine.SceneManagement;

public class InstructionsManager : MonoBehaviour
{
    public void LoadLevelScene()
    {
        SceneManager.LoadScene("LevelScene");
    }
}
