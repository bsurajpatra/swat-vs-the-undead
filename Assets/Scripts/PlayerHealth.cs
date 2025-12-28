using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    public float currentHealth;
    public Slider healthSlider;

    [Header("Game Over Settings")]
    private bool isDead = false;

    [Header("Visual Feedback")]
    public DamageFlash damageFlash; // link to DamageFlash script in Inspector

    void Start()
    {
        currentHealth = maxHealth;

        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        if (currentHealth < 0) currentHealth = 0;

        if (healthSlider != null)
            healthSlider.value = currentHealth;

        Debug.Log($"💔 Player took {amount} damage! Remaining health: {currentHealth}");

        // Trigger red flash effect
        if (damageFlash != null)
            damageFlash.TriggerFlash();

        if (currentHealth <= 0 && !isDead)
        {
            isDead = true;
            Die();
        }
    }
    public bool Heal(float amount)
    {
        if (isDead) return false;

        // If already full health, do nothing
        if (currentHealth >= maxHealth)
            return false;

        currentHealth += amount;

        if (currentHealth > maxHealth)
            currentHealth = maxHealth;

        if (healthSlider != null)
            healthSlider.value = currentHealth;

        Debug.Log($"💚 Player healed by {amount}. Current health: {currentHealth}");

        return true; // healing successful
    }


    void Die()
    {
        Debug.Log("💀 Player has died. Loading GameOver scene...");
        SceneManager.LoadScene(2); // make sure scene index 2 = GameOver
    }
}
