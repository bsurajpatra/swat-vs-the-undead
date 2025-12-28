using UnityEngine;

public class ZombieChase : MonoBehaviour
{
    [Header("Chase Settings")]
    public float detectionRange = 25f;     // how far the zombie can see
    public float stopDistance = 1.8f;      // how close before stopping (for smooth stop)
    public float chaseSpeed = 3f;          // chase speed
    public float rotationSpeed = 6f;       // how fast it turns to face player

    [Header("Attack Settings")]
    public float attackDistance = 3.5f;    // start attacking within this distance
    public float attackInterval = 2f;      // attack every 3 seconds
    public float attackDamage = 10f;        // reduce health by 5 per hit

    [Header("Audio")]
    public AudioClip chaseSound;
    public float chaseVolume = 0.8f;

    private AudioSource audioSource;
    private Transform player;
    private PlayerHealth playerHealth;
    private ZombieMovement wanderScript;
    private Animator animator;

    private bool isChasing = false;
    private bool isAttacking = false;
    private float attackTimer = 0f;

    void Start()
    {
        // Find player
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null)
        {
            player = p.transform;
            playerHealth = p.GetComponent<PlayerHealth>();
        }

        // Verify PlayerHealth reference
        if (playerHealth == null)
            Debug.LogError($"❌ [{gameObject.name}] PlayerHealth not found on Player object!");
        else
            Debug.Log($"✅ [{gameObject.name}] PlayerHealth reference found!");

        wanderScript = GetComponent<ZombieMovement>();
        animator = GetComponent<Animator>();

        // Setup audio
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = chaseSound;
        audioSource.playOnAwake = false;
        audioSource.loop = true;
        audioSource.spatialBlend = 1f;  // 3D sound
        audioSource.volume = chaseVolume;
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        // Log distance for near zombies only
        if (distance <= 10f)
            Debug.Log($"[{gameObject.name}] Distance: {distance:F2} | AttackRange: {attackDistance}");

        // Chase or attack depending on range
        if (distance <= detectionRange)
        {
            if (!isChasing)
            {
                isChasing = true;
                if (wanderScript) wanderScript.enabled = false;
                if (chaseSound != null && !audioSource.isPlaying)
                    audioSource.Play();
            }

            // ✅ FIX: Attack when within attackDistance (not stopDistance)
            if (distance > attackDistance)
            {
                MoveTowardPlayer();
            }
            else
            {
                FacePlayer();
                Debug.Log($"[{gameObject.name}] ❗ ENTERING AttackPlayer() call at distance {distance:F2}");
                AttackPlayer(distance);
            }
        }
        else
        {
            // Player escaped
            if (isChasing)
            {
                isChasing = false;
                if (wanderScript) wanderScript.enabled = true;
                if (audioSource.isPlaying)
                    audioSource.Stop();
            }
        }
    }

    void MoveTowardPlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0;

        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);

        transform.position += direction * chaseSpeed * Time.deltaTime;

        // Play walking animation if available
        if (animator != null)
        {
            animator.SetBool("isWalking", true);
        }
    }

    void FacePlayer()
    {
        Vector3 dir = (player.position - transform.position);
        dir.y = 0;
        if (dir != Vector3.zero)
        {
            Quaternion look = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, look, rotationSpeed * Time.deltaTime);
        }
    }

    void AttackPlayer(float distance)
    {
        if (playerHealth == null)
        {
            Debug.LogError($"[{gameObject.name}] ❌ PlayerHealth reference is null!");
            return;
        }

        // Debug check
        Debug.Log($"[{gameObject.name}] ⚔️ Checking attack logic. Distance={distance:F2}");

        if (distance <= attackDistance)
        {
            attackTimer += Time.deltaTime;
            Debug.Log($"[{gameObject.name}] 🧟 Within attack range! Timer={attackTimer:F2}");

            if (!isAttacking)
            {
                isAttacking = true;
                if (animator != null)
                {
                    animator.SetBool("isWalking", false);
                    animator.SetTrigger("Attack");
                }
            }

            if (attackTimer >= attackInterval)
            {
                playerHealth.TakeDamage(attackDamage);
                Debug.Log($"[{gameObject.name}] 💥 Hit! Player health reduced by {attackDamage}. Current: {playerHealth.currentHealth}");
                attackTimer = 0f;
            }
        }
        else
        {
            if (isAttacking)
            {
                Debug.Log($"[{gameObject.name}] 🔸 Player moved away, stopping attack.");
                isAttacking = false;
                if (animator != null)
                {
                    animator.ResetTrigger("Attack");
                    animator.SetBool("isWalking", true);
                }
            }
            attackTimer = 0f;
        }
    }

    // Draw detection and attack ranges for debugging
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackDistance);
    }
}
