using UnityEngine;

public class AmmoPickup : MonoBehaviour
{
    [Header("Pickup Settings")]
    [SerializeField] private int ammoToGive = 20;
    [SerializeField] private AudioClip pickupSound; 

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerCombat playerCombat = other.GetComponent<PlayerCombat>();

            if (playerCombat != null)
            {
                bool wasPickedUp = playerCombat.AddReserveAmmo(ammoToGive);

                if (wasPickedUp)
                {
                    if (pickupSound != null)
                    {
                        AudioSource.PlayClipAtPoint(pickupSound, transform.position);
                    }
                    Destroy(gameObject);
                }
            }
        }
    }
}