using UnityEngine;
using System.Collections;

public class GunShoot : MonoBehaviour
{
    [Header("References")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public Camera playerCamera;
    public GameObject muzzleFlashEffect;
    public Light muzzleLight;
    public AudioClip shootSound;   // 🎧 new field for sound
    public GunAmmo gunAmmo;        // 🔫 ammo system reference

    private AudioSource audioSource; // 🎧 internal reference

    [Header("Gun Settings")]
    public float shootForce = 120f; // increased from 60f for higher bullet speed
    public float fireRate = 0.2f;
    private float nextTimeToFire = 0f;

    void Start()
    {
        // get the attached audio source
        audioSource = GetComponent<AudioSource>();

        // Auto-find GunAmmo if not assigned
        if (gunAmmo == null)
        {
            gunAmmo = GetComponent<GunAmmo>();
            if (gunAmmo == null)
                gunAmmo = FindObjectOfType<GunAmmo>();
        }

        if (gunAmmo == null)
            Debug.LogWarning("⚠️ GunShoot: GunAmmo component not found! Shooting will work but ammo won't be tracked.");
    }

    void Update()
    {
        if (Input.GetButton("Fire1") && Time.time >= nextTimeToFire)
        {
            // Check if we can shoot (has ammo and not reloading)
            if (gunAmmo != null && !gunAmmo.CanShoot())
            {
                // Stop any shoot sounds that might be playing
                if (audioSource != null && audioSource.isPlaying)
                    audioSource.Stop();
                
                // Try to consume bullet to trigger empty click sound
                if (gunAmmo != null)
                    gunAmmo.ConsumeBullet();
                return;
            }

            nextTimeToFire = Time.time + fireRate;
            Shoot();
        }
    }

    void Shoot()
    {
        if (bulletPrefab == null || firePoint == null) return;

        // Consume ammo before shooting
        if (gunAmmo != null)
        {
            gunAmmo.ConsumeBullet();
        }

        Vector3 shootDirection = playerCamera != null ? playerCamera.transform.forward : firePoint.forward;

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.LookRotation(shootDirection));

        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        Collider bulletCollider = bullet.GetComponent<Collider>();
        Collider gunCollider = GetComponentInParent<Collider>();

        if (bulletCollider != null && gunCollider != null)
        {
            Physics.IgnoreCollision(bulletCollider, gunCollider);
        }

        if (rb != null)
        {
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
            rb.AddForce(shootDirection * shootForce, ForceMode.Impulse);
        }

        // 💥 muzzle flash
        if (muzzleFlashEffect != null)
        {
            GameObject flash = Instantiate(muzzleFlashEffect, firePoint.position, firePoint.rotation);
            Destroy(flash, 0.05f);
        }

        // 🔊 play sound
        if (audioSource != null && shootSound != null)
        {
            audioSource.PlayOneShot(shootSound);
        }

        // 💡 short muzzle light burst
        if (muzzleLight != null)
            StartCoroutine(MuzzleLightFlash());
    }

    IEnumerator MuzzleLightFlash()
    {
        muzzleLight.enabled = true;
        yield return new WaitForSeconds(0.05f);
        muzzleLight.enabled = false;
    }
}
