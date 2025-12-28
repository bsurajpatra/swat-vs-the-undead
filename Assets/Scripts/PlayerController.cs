using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TMPro;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 10f;
    public float gravity = -20f;
    public float jumpHeight = 5f;
    public float lookSensitivity = 1.2f;
    public Transform cameraTransform;
    public float crouchHeight = 0.5f;
    public float crouchSpeed = 3f;

    [Header("Spawn Settings")]
    public float startHeight = 140f;

    [Header("Landing Audio")]
    public AudioSource landingAudio;
    public float minFallVelocity = -15f;

    private CharacterController cc;
    private Vector2 moveInput;
    private Vector2 lookInput;
    private float yVel;
    private float camPitch;
    private float originalHeight;
    private bool isCrouching;
    private bool isAltPressed;
    private bool isPaused = false;

    private bool wasGrounded;
    private bool hasLandedOnce = false;

    private Animator animator;

    [Header("UI")]
    public TMP_Text pauseText;

    [Header("Weapon")]
    public GunAmmo gunAmmo;

    void Awake()
    {
        cc = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        originalHeight = cc.height;

        // 🔽 Start from height
        Vector3 pos = transform.position;
        pos.y = startHeight;
        transform.position = pos;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (pauseText != null)
            pauseText.gameObject.SetActive(false);

        // Auto-find GunAmmo if not assigned
        if (gunAmmo == null)
        {
            gunAmmo = GetComponent<GunAmmo>();
            if (gunAmmo == null)
                gunAmmo = FindObjectOfType<GunAmmo>();
        }
    }

    void Update()
    {
        if (isPaused) return;

        // --- Look ---
        float mouseX = lookInput.x * lookSensitivity;
        float mouseY = lookInput.y * lookSensitivity;

        camPitch = Mathf.Clamp(camPitch - mouseY, -85f, 85f);
        cameraTransform.localEulerAngles = new Vector3(camPitch, 0, 0);
        transform.Rotate(Vector3.up * mouseX);

        // --- Move ---
        Vector3 input = new Vector3(moveInput.x, 0, moveInput.y);
        float currentSpeed = isCrouching ? crouchSpeed : moveSpeed;

        if (isAltPressed && moveInput.y > 0.1f && !isCrouching)
            currentSpeed *= 2f;

        Vector3 world = transform.TransformDirection(input) * currentSpeed;

        if (cc.isGrounded && yVel < 0)
            yVel = -2f;

        yVel += gravity * Time.deltaTime;
        world.y = yVel;

        cc.Move(world * Time.deltaTime);

        // --- Landing Detection (play once) ---
        if (!wasGrounded && cc.isGrounded && !hasLandedOnce && yVel < minFallVelocity)
        {
            hasLandedOnce = true;

            if (landingAudio != null)
                landingAudio.Play();
        }

        wasGrounded = cc.isGrounded;

        // --- Animation ---
        bool isMoving = input.magnitude > 0.1f;
        animator.SetBool("isWalking", isMoving);
    }

    // ---------- INPUT SYSTEM ----------

    public void OnMove(InputValue value) => moveInput = value.Get<Vector2>();
    public void OnLook(InputValue value) => lookInput = value.Get<Vector2>();

    public void OnJump(InputValue value)
    {
        if (value.isPressed && cc.isGrounded)
            yVel = Mathf.Sqrt(jumpHeight * -2f * gravity);
    }

    public void OnFire(InputValue value)
    {
        if (value.isPressed)
        {
            // Only trigger shoot animation if we have ammo
            if (gunAmmo == null || gunAmmo.CanShoot())
            {
                animator.SetTrigger("Shoot");
            }
        }
    }

    public void OnReload(InputValue value)
    {
        if (value.isPressed && gunAmmo != null)
        {
            gunAmmo.Reload();
        }
    }

    public void OnCrouch(InputValue value)
    {
        isCrouching = value.isPressed;

        if (isCrouching)
        {
            cc.height = originalHeight * crouchHeight;
            cc.center = new Vector3(cc.center.x, cc.height / 2f, cc.center.z);
        }
        else
        {
            cc.height = originalHeight;
            cc.center = new Vector3(cc.center.x, originalHeight / 2f, cc.center.z);
        }
    }

    public void OnAlt(InputValue value)
    {
        isAltPressed = value.isPressed;
    }

    // ---------- GAME CONTROLS ----------

    public void OnQuit(InputValue value)
    {
        if (value.isPressed)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            SceneManager.LoadScene(5);
        }
    }

    public void OnPause(InputValue value)
    {
        if (!value.isPressed || isPaused) return;

        isPaused = true;
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        AudioListener.pause = true;

        if (pauseText != null)
        {
            pauseText.gameObject.SetActive(true);
            pauseText.text = "Game Paused!";
        }
    }

    public void OnUnpause(InputValue value)
    {
        if (!value.isPressed || !isPaused) return;

        isPaused = false;
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        AudioListener.pause = false;

        if (pauseText != null)
            pauseText.gameObject.SetActive(false);
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        // 🔫 Ammo Pickup
        if (hit.collider.CompareTag("Ammo"))
        {
            GunAmmo gunAmmo = GetComponentInChildren<GunAmmo>();
            if (gunAmmo == null)
                gunAmmo = FindObjectOfType<GunAmmo>();
            
            if (gunAmmo == null)
                return;

            Collider col = hit.collider;
            col.enabled = false; // Prevent multiple hits

            gunAmmo.AddReserveAmmo(30); // Fixed value: +30 reserve ammo

            // 🔊 Play pickup sound if available
            AudioSource audio = col.GetComponent<AudioSource>();
            if (audio != null && audio.clip != null)
            {
                // Play sound from ammo box AudioSource
                audio.Play();
                Destroy(col.gameObject, audio.clip.length);
            }
            else if (gunAmmo != null && gunAmmo.ammoPickupSound != null)
            {
                // Fallback: Play sound from GunAmmo component if ammo box has no AudioSource
                AudioSource gunAudio = gunAmmo.GetComponent<AudioSource>();
                if (gunAudio != null)
                {
                    gunAudio.PlayOneShot(gunAmmo.ammoPickupSound);
                }
                Destroy(col.gameObject);
            }
            else
            {
                Destroy(col.gameObject);
            }

            return;
        }

        // 💊 Health Pickup
        if (hit.collider.CompareTag("Medicine"))
        {
            PlayerHealth playerHealth = GetComponent<PlayerHealth>();
            if (playerHealth == null)
                return;

            // 🔒 Prevent multiple triggers
            Collider medCollider = hit.collider;
            medCollider.enabled = false;

            bool healed = playerHealth.Heal(5f);

            if (healed)
            {
                // 🔊 Play pickup sound if available
                AudioSource pickupAudio = hit.collider.GetComponent<AudioSource>();
                if (pickupAudio != null && pickupAudio.clip != null)
                {
                    pickupAudio.Play();
                    Destroy(hit.collider.gameObject, pickupAudio.clip.length);
                }
                else
                {
                    Destroy(hit.collider.gameObject);
                }
            }
            else
            {
                // If health was already full, re-enable collider
                medCollider.enabled = true;
            }
        }
    }

}
