using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ZombieMovement : MonoBehaviour
{
    public float moveSpeed = 1.5f;        // how fast the zombie moves randomly
    public float changeDirectionTime = 3f; // how often it changes direction

    private Rigidbody rb;
    private Vector3 moveDirection;
    private float timer;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; // prevent falling over
        ChangeDirection();
    }

    void Update()
    {
        timer += Time.deltaTime;

        // change direction after time
        if (timer >= changeDirectionTime)
        {
            ChangeDirection();
            timer = 0f;
        }

        // move forward continuously
        Vector3 movement = moveDirection * moveSpeed * Time.deltaTime;
        rb.MovePosition(transform.position + movement);
    }

    void ChangeDirection()
    {
        float randomX = Random.Range(-1f, 1f);
        float randomZ = Random.Range(-1f, 1f);

        moveDirection = new Vector3(randomX, 0, randomZ).normalized;

        // rotate to face that direction
        if (moveDirection != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(moveDirection);
    }
}
