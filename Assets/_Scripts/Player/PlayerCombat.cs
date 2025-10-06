using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : MonoBehaviour
{
    [Header("Gun Settings")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 20f;
    public Camera playerCamera;

    void OnFire(InputValue value)
    {
        if (value.isPressed)
        {
            Shoot();
        }
    }

    void Shoot()
    {
        Vector3 shootDirection = playerCamera.transform.forward;

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        rb.linearVelocity = shootDirection * bulletSpeed;
    }
}