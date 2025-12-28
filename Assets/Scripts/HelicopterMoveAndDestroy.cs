using UnityEngine;

public class HelicopterMoveAndDestroy : MonoBehaviour
{
    public Vector3 targetPosition = new Vector3(110f, 100f, -387f);
    public float moveSpeed = 30f;
    public float destroyDelay = 0f; // delay after reaching target (optional)

    private bool reachedTarget = false;

    void Update()
    {
        if (reachedTarget) return;

        // Move towards target
        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            moveSpeed * Time.deltaTime
        );

        // Check if reached destination
        if (Vector3.Distance(transform.position, targetPosition) <= 0.1f)
        {
            reachedTarget = true;
            Destroy(gameObject, destroyDelay);
        }
    }
}
