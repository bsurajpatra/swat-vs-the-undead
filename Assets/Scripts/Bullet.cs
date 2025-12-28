using UnityEngine;

public class Bullet : MonoBehaviour
{
    // How long the bullet exists before being destroyed (controls range)
    public float lifeTime = 6f; // increased from 3f for longer distance
    public GameObject hitEffect; // optional spark/dust prefab

    void Start()
    {
        // Destroy bullet after a few seconds to prevent memory clutter
        Destroy(gameObject, lifeTime);
    }

    void OnCollisionEnter(Collision collision)
    {
        ZombieHealth zombie = collision.gameObject.GetComponent<ZombieHealth>();
        if (zombie != null)
        {
            zombie.TakeDamage(1f); // each bullet = 1 hit
        }

        Destroy(gameObject);
    }

}
