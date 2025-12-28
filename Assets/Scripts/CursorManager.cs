using UnityEngine;

public class CursorManager : MonoBehaviour
{
    void Start()
    {
        // Show and unlock the cursor when entering this scene
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
