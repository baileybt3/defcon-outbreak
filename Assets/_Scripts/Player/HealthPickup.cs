using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    [Header("Pickup Settings")]
    [SerializeField] private int healthToGive = 25;
    [SerializeField] private AudioClip pickupSound; 

    
    private void OnTriggerEnter(Collider other)
    {
        // if the object that entered the trigger is the player
        if (other.CompareTag("Player"))
        {
            // Try to get the PlayerController script
            PlayerController playerController = other.GetComponent<PlayerController>();

            if (playerController != null)
            {
                // Call the public Heal method
                bool wasHealed = playerController.Heal(healthToGive);

                if (wasHealed)
                {
                    // Play a sound and destroy the object if healing was successful
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