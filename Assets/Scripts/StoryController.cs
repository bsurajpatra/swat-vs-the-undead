using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class StoryController : MonoBehaviour
{
    public TMP_Text storyText;

    [Header("Typing Speed")]
    public float charDelay = 0.03f;   // FAST typing (lower = faster)
    public float lineDelay = 0.4f;    // pause between lines
    public float endDelay = 1.5f;     // pause before game starts

    private string[] lines =
    {
        "The world has fallen.",
        "The dead have risen.",
        "",
        "This arena is your final stand.",
        "No mercy. No backup.",
        "",
        "Survive the waves.",
        "Kill or be killed."
    };

    void Start()
    {
        storyText.text = "";
        StartCoroutine(TypeStory());
    }

    IEnumerator TypeStory()
    {
        foreach (string line in lines)
        {
            yield return StartCoroutine(TypeLine(line));
            storyText.text += "\n";
            yield return new WaitForSeconds(lineDelay);
        }

        yield return new WaitForSeconds(endDelay);
        SceneManager.LoadScene(1); // change if needed
    }

    IEnumerator TypeLine(string line)
    {
        foreach (char c in line)
        {
            storyText.text += c;
            yield return new WaitForSeconds(charDelay);
        }
    }

    public void SkipToGame()
    {
        Debug.Log("🏠 Returning to Main Menu...");
        SceneManager.LoadScene("SampleScene");
    }
}
