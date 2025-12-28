using UnityEngine;

public class PickupVisual : MonoBehaviour
{
    [Header("Rotation")]
    public float rotationSpeed = 60f; // degrees per second

    [Header("Floating")]
    public bool enableFloating = true;
    public float floatAmplitude = 0.25f;
    public float floatSpeed = 2f;

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        // 🔄 Rotate horizontally (Y-axis)
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);

        // ⬆️⬇️ Floating motion
        if (enableFloating)
        {
            float newY = startPos.y + Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;
            transform.position = new Vector3(startPos.x, newY, startPos.z);
        }
    }
}
