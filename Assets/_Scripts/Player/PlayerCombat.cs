using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class PlayerCombat : MonoBehaviour
{
    [Header("Gun Settings")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 35f;
    public float fireRate = 0.2f;
    public float reloadTime = 1.5f;
    
    [Header("Inaccuracy")]
    [SerializeField] private float maxSpreadAngle = 3f; // Max angle
    [SerializeField] private float firePointDistance = 10f; // Distance used to calculate the spread vector

    [Header("Ammo Settings")]
    public int maxAmmo = 6;
    public int reserveAmmo = 24;
    private int currentAmmo;
    private bool isReloading = false;

    [Header("References")]
    public Camera playerCamera;
    private AudioSource audioSource;
    private PlayerController playerController;

    [Header("Sound Effects")]
    public AudioClip shootSound;
    public AudioClip reloadStartSound;
    public AudioClip dryFireSound;
    float shootSoundVolume = 0.5f;
    float dryFireVolume = 0.5f;

    [Header("Visual Effects")]
    public ParticleSystem muzzleFlash;

    private float lastShotTime;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        playerController = GetComponent<PlayerController>(); 
        if (playerController == null)
        {
            Debug.LogError("PlayerCombat cannot find PlayerController component!"); 
        }
    }

    private void Start()
    {
        currentAmmo = maxAmmo;
    }

    // --- Input Callbacks ---
    void OnFire(InputValue value)
    {
        if (value.isPressed)
        {
            TryShoot();
        }
    }

    void OnReload(InputValue value)
    {
        if (value.isPressed)
        {
            StartCoroutine(Reload());
        }
    }

    void TryShoot()
    {
        if (playerController == null || !playerController.IsAlive)
            return;

        if (isReloading || Time.time < lastShotTime + fireRate)
            return;

        if (currentAmmo <= 0)
        {
            if (dryFireSound != null)
            {
                audioSource.PlayOneShot(dryFireSound, dryFireVolume);
            }
            StartCoroutine(Reload()); 
            return;
        }

        Shoot();
        lastShotTime = Time.time;
        currentAmmo--;
    }
    void Shoot()
    {
        // Play Sound
        if (shootSound != null)
        {
            audioSource.PlayOneShot(shootSound, shootSoundVolume);
        }

        // Trigger Recoil
        if (playerController != null)
        {
            playerController.DoRecoil();
        }

        // Play Muzzle Flash
        if (muzzleFlash != null)
        {
            muzzleFlash.Play();
        }

        Vector3 pointInFront = playerCamera.transform.position + playerCamera.transform.forward * firePointDistance;
        
        Vector3 randomSpread = Random.insideUnitSphere * Mathf.Tan(maxSpreadAngle * Mathf.Deg2Rad) * firePointDistance;

        Vector3 targetPoint = pointInFront + randomSpread;
        
        Vector3 shootDirection = (targetPoint - firePoint.position).normalized;


        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        rb.linearVelocity = shootDirection * bulletSpeed;
    }

    IEnumerator Reload()
    {
        if (isReloading || currentAmmo == maxAmmo || reserveAmmo <= 0)
            yield break;

        isReloading = true;
        
        if (reloadStartSound != null)
        {
            audioSource.PlayOneShot(reloadStartSound);
        }
        
        yield return new WaitForSeconds(reloadTime);

        int ammoToReload = Mathf.Min(maxAmmo - currentAmmo, reserveAmmo);
        currentAmmo += ammoToReload;
        reserveAmmo -= ammoToReload;

        isReloading = false;
    }

    public bool AddReserveAmmo(int amount)
    {
        if (reserveAmmo < maxAmmo * 5)
        {
            reserveAmmo += amount;
            Debug.Log($"Picked up {amount} reserve ammo. Total reserve: {reserveAmmo}");
            return true; // Pickup successful
        }
        
        Debug.Log("Reserve ammo is full!");
        return false; // Pickup failed (ammo full)
    }

    // Public properties for UI access
    public int CurrentAmmo => currentAmmo;
    public int ReserveAmmo => reserveAmmo;
    public bool IsReloading => isReloading;
}