using UnityEngine;
using UnityEngine.InputSystem;

public class Gun : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint;
    public Camera playerCamera;
    public float range = 100f;

    void OnCombat(InputValue value)
    {
        if (value.isPressed)
        {
            // Player default point
            Vector3 targetPoint = playerCamera.transform.position + playerCamera.transform.forward * range;

            if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out RaycastHit hit, range))
            {
                targetPoint = hit.point;
            }

            //Get direction from firePoint to targetPoint
            Vector3 direction = (targetPoint - firePoint.position).normalized;

            // Spawn bullet 
            Quaternion rotation = Quaternion.LookRotation(playerCamera.transform.forward);
            Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        }
    }
}
