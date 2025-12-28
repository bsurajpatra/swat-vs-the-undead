using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Simple level selector used on the MainMenu scene.
/// - Attach this to a GameObject in MainMenu (e.g. "LevelSelectionController").
/// - Hook your L1 / L2 / L3 buttons to the public methods below.
/// - Stores the selected level config in static fields that other scenes can read.
/// </summary>
public class LevelSelection : MonoBehaviour
{
    public static int SelectedLevel = 1;
    public static int SelectedZombieCount = 20;
    public static float SelectedTimeLimit = 180f; // seconds

    private void StartLevel(int level, int zombies, float timeSeconds)
    {
        SelectedLevel = level;
        SelectedZombieCount = zombies;
        SelectedTimeLimit = timeSeconds;

        Debug.Log($"🎯 Starting Level {level}: {zombies} zombies, {timeSeconds} seconds");

        // First show the story, which then loads SampleScene (index 1) when finished
        SceneManager.LoadScene(4); // StoryScene (build index 4)
    }

    // Button hooks for MainMenu
    public void OnLevel1Button()
    {
        // Level 1 - 20 zombies, 3 minutes
        StartLevel(1, 20, 180f);
    }

    public void OnLevel2Button()
    {
        // Level 2 - 30 zombies, 4 minutes
        StartLevel(2, 30, 240f);
    }

    public void OnLevel3Button()
    {
        // Level 3 - 40 zombies, 5 minutes
        StartLevel(3, 40, 300f);
    }

    // Load back to MainMenu (scene index 0)
    public void LoadMainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void LoadInstructionsScene()
    {
        SceneManager.LoadScene("InstructionsScene");
    }
}


