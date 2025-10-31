using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
public class PlayerCombat : MonoBehaviour
{
    [Header("Gun Settings")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 20f;
    public float fireRate = 0.2f;
    public float reloadTime = 1.5f;

    [Header("Ammo Settings")]
    public int maxAmmo = 10;
    public int reserveAmmo = 30;
    private int currentAmmo;
    private bool isReloading = false;

    [Header("References")]
    public Camera playerCamera;

    private float lastShotTime;

    private void Start()
    {
        currentAmmo = maxAmmo;
    }

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
        if (isReloading || Time.time < lastShotTime + fireRate || currentAmmo <= 0)
            return;

        Shoot();
        lastShotTime = Time.time;
        currentAmmo--;
    }

    void Shoot()
    {
        Vector3 shootDirection = playerCamera.transform.forward;
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        rb.linearVelocity = shootDirection * bulletSpeed;
    }

    IEnumerator Reload()
    {
        if (isReloading || currentAmmo == maxAmmo || reserveAmmo <= 0)
            yield break;

        isReloading = true;
        yield return new WaitForSeconds(reloadTime);

        int ammoToReload = Mathf.Min(maxAmmo - currentAmmo, reserveAmmo);
        currentAmmo += ammoToReload;
        reserveAmmo -= ammoToReload;

        isReloading = false;
    }

    // Public properties for UI access
public int CurrentAmmo => currentAmmo;
public int ReserveAmmo => reserveAmmo;
public bool IsReloading => isReloading;

}
