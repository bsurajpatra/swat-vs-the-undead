using UnityEngine;
using System.Collections;

public class ZombieHealth : MonoBehaviour
{
    [Header("Zombie Health Settings")]
    public int hitsToDie = 5;
    private int currentHits = 0;
    private bool isDead = false;

    [Header("Fade Settings")]
    public float fadeDelay = 2f;
    public float fadeDuration = 2f;

    [Header("Audio")]
    public AudioClip deathSound;
    private AudioSource audioSource;

    private Animator animator;
    private SkinnedMeshRenderer[] renderers;

    void Start()
    {
        animator = GetComponent<Animator>();
        renderers = GetComponentsInChildren<SkinnedMeshRenderer>();
        audioSource = GetComponent<AudioSource>();
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;

        currentHits++;

        if (currentHits >= hitsToDie)
        {
            Die();
        }
    }

    public bool IsDead()
    {
        return isDead;
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;

        // Trigger death animation
        if (animator != null)
            animator.SetTrigger("isDead");

        // Stop movement/AI
        ZombieAI ai = GetComponent<ZombieAI>();
        if (ai != null)
            ai.enabled = false;

        // Play death sound
        if (audioSource != null && deathSound != null)
            audioSource.PlayOneShot(deathSound, 1.0f);

        // Increment kill counter
        if (KillCounter.instance != null)
            KillCounter.instance.AddKill();

        // Notify ZombieManager that a zombie was killed (for victory condition)
        if (ZombieManager.instance != null)
            ZombieManager.instance.ZombieKilled();

        // Fade out
        StartCoroutine(FadeAndDestroy());
    }

    IEnumerator FadeAndDestroy()
    {
        yield return new WaitForSeconds(fadeDelay);

        float t = 0;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, t / fadeDuration);

            foreach (var r in renderers)
            {
                foreach (var mat in r.materials)
                {
                    if (mat.HasProperty("_Color"))
                    {
                        Color c = mat.color;
                        c.a = alpha;
                        mat.color = c;
                    }
                }
            }

            yield return null;
        }

        Destroy(gameObject);
    }
}
