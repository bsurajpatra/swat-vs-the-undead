using UnityEngine;
using System.Collections;

public class ZombieAI : MonoBehaviour
{
    [Header("Chase Settings")]
    public float detectRange = 15f;
    public float attackRange = 2.2f;
    public float moveSpeed = 2.5f;
    public float rotationSpeed = 5f;

    [Header("Audio")]
    public AudioSource chaseAudio;           // looped growl sound
    public float fadeDuration = 1.0f;        // fade speed in seconds

    private Transform player;
    private Animator animator;
    private ZombieHealth health;

    private bool isChasing = false;
    private bool isAttacking = false;
    private bool isDead = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        animator = GetComponent<Animator>();
        health = GetComponent<ZombieHealth>();

        if (chaseAudio != null)
        {
            chaseAudio.loop = true;
            chaseAudio.volume = 0f; // start silent
        }
    }

    void Update()
    {
        if (health != null && health.IsDead())
        {
            if (!isDead)
            {
                isDead = true;
                StopAllAnimations();
                StartCoroutine(FadeOutAudio());
            }
            return;
        }

        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= attackRange)
            StartAttack();
        else if (distance <= detectRange)
            ChasePlayer();
        else
            Idle();
    }

    void ChasePlayer()
    {
        if (!isChasing)
        {
            isChasing = true;
            isAttacking = false;

            animator.ResetTrigger("Attack");
            animator.SetBool("isWalking", true);

            // ✅ Fade in growl sound
            StartCoroutine(FadeInAudio());
        }

        Vector3 dir = (player.position - transform.position).normalized;
        dir.y = 0;
        Quaternion lookRot = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.deltaTime * rotationSpeed);
        transform.position += dir * moveSpeed * Time.deltaTime;
    }

    void StartAttack()
    {
        if (!isAttacking)
        {
            isAttacking = true;
            isChasing = false;

            animator.SetBool("isWalking", false);
            animator.SetTrigger("Attack");

            // ✅ Fade out growl when attacking
            StartCoroutine(FadeOutAudio());
        }

        Vector3 dir = (player.position - transform.position).normalized;
        dir.y = 0;
        transform.rotation = Quaternion.LookRotation(dir);
    }

    void Idle()
    {
        if (isChasing || isAttacking)
        {
            isChasing = false;
            isAttacking = false;
            animator.SetBool("isWalking", false);
            animator.ResetTrigger("Attack");

            // ✅ Fade out growl when idle
            StartCoroutine(FadeOutAudio());
        }
    }

    void StopAllAnimations()
    {
        isChasing = false;
        isAttacking = false;
        animator.SetBool("isWalking", false);
        animator.ResetTrigger("Attack");
    }

    // ---------------- AUDIO COROUTINES ----------------

    IEnumerator FadeInAudio()
    {
        if (chaseAudio == null) yield break;

        if (!chaseAudio.isPlaying)
            chaseAudio.Play();

        float startVol = chaseAudio.volume;
        float t = 0;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            chaseAudio.volume = Mathf.Lerp(startVol, 0.8f, t / fadeDuration);
            yield return null;
        }

        chaseAudio.volume = 0.8f;
    }

    IEnumerator FadeOutAudio()
    {
        if (chaseAudio == null) yield break;

        float startVol = chaseAudio.volume;
        float t = 0;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            chaseAudio.volume = Mathf.Lerp(startVol, 0f, t / fadeDuration);
            yield return null;
        }

        chaseAudio.volume = 0f;
        chaseAudio.Stop();
    }
}
