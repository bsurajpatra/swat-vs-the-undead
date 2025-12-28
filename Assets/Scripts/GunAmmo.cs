using UnityEngine;
using TMPro;
using System.Collections;

public class GunAmmo : MonoBehaviour
{
    [Header("Magazine Settings")]
    public int magazineSize = 20;
    public int currentAmmo;

    [Header("Reserve Ammo")]
    public int reserveAmmo = 120;

    [Header("Reload")]
    public float reloadTime = 3f;
    private bool isReloading = false;

    [Header("UI")]
    public TMP_Text ammoText;

    [Header("Audio")]
    public AudioClip reloadSound;
    public AudioClip emptyClickSound;
    public AudioClip ammoPickupSound; // Optional: centralized ammo pickup sound
    private AudioSource audioSource;

    void Start()
    {
        // Initialize ammo based on current level
        InitializeAmmoForLevel();
        
        currentAmmo = magazineSize;
        UpdateUI();

        // Setup audio source
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    void InitializeAmmoForLevel()
    {
        // Get current level from LevelManager or LevelSelection
        int currentLevel = 1;
        
        if (LevelManager.instance != null)
        {
            currentLevel = LevelManager.instance.GetCurrentLevel();
        }
        else if (LevelSelection.SelectedLevel > 0)
        {
            currentLevel = LevelSelection.SelectedLevel;
        }

        // Set ammo values based on level
        switch (currentLevel)
        {
            case 1:
                // Level 1: 20 zombies, ~120 bullets needed + misses = ~140 total
                magazineSize = 20;
                reserveAmmo = 120;
                break;
            case 2:
                // Level 2: 30 zombies, ~180 bullets needed + misses = ~225 total
                magazineSize = 25;
                reserveAmmo = 225;
                break;
            case 3:
                // Level 3: 40 zombies, ~240 bullets needed + misses = ~360 total
                magazineSize = 30;
                reserveAmmo = 360;
                break;
            default:
                // Default to level 1 values
                magazineSize = 20;
                reserveAmmo = 120;
                break;
        }

        Debug.Log($"Level {currentLevel} Ammo Setup: Magazine={magazineSize}, Reserve={reserveAmmo}");
    }

    public bool CanShoot()
    {
        return currentAmmo > 0 && !isReloading;
    }

    public void ConsumeBullet()
    {
        if (!CanShoot()) 
        {
            // Play empty click sound if trying to shoot with no ammo
            if (currentAmmo == 0 && !isReloading)
            {
                if (audioSource != null)
                {
                    // Stop any other sounds that might be playing (like shoot sound)
                    if (audioSource.isPlaying)
                        audioSource.Stop();
                    
                    // Play empty click sound if assigned
                    if (emptyClickSound != null)
                    {
                        audioSource.PlayOneShot(emptyClickSound);
                    }
                    else
                    {
                        Debug.LogWarning("Empty Click Sound not assigned in GunAmmo component!");
                    }
                }
            }
            return;
        }

        currentAmmo--;
        UpdateUI();
    }

    public void Reload()
    {
        // Can't reload if already reloading, magazine is full, or no reserve ammo
        if (isReloading) return;
        if (currentAmmo == magazineSize) return;
        if (reserveAmmo <= 0)
        {
            Debug.Log("Warning: No reserve ammo left!");
            return;
        }

        StartCoroutine(ReloadRoutine());
    }

    IEnumerator ReloadRoutine()
    {
        isReloading = true;
        Debug.Log("Reloading...");

        // Play reload sound
        if (audioSource != null && reloadSound != null)
            audioSource.PlayOneShot(reloadSound);

        yield return new WaitForSeconds(reloadTime);

        // Calculate how much ammo to load
        int needed = magazineSize - currentAmmo;
        int loadAmount = Mathf.Min(needed, reserveAmmo);

        currentAmmo += loadAmount;
        reserveAmmo -= loadAmount;

        isReloading = false;
        UpdateUI();
        Debug.Log($"Reloaded! Current: {currentAmmo}/{magazineSize}, Reserve: {reserveAmmo}");
    }

    void UpdateUI()
    {
        if (ammoText != null)
            ammoText.text = currentAmmo + " / " + reserveAmmo;
    }

    // Add reserve ammo (for pickups)
    public void AddReserveAmmo(int amount)
    {
        reserveAmmo += amount;
        UpdateUI();
        Debug.Log($"Picked up {amount} ammo! Reserve: {reserveAmmo}");
    }

    // Public getters for UI or other scripts
    public int GetCurrentAmmo() => currentAmmo;
    public int GetReserveAmmo() => reserveAmmo;
    public int GetMagazineSize() => magazineSize;
    public bool IsReloading() => isReloading;
}
