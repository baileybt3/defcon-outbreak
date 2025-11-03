using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    [Header("Pickup Settings")]
    [SerializeField] private int healthToGive = 25; // How much health this pack restores
    [SerializeField] private AudioClip pickupSound; 

    // Unity's built-in collision detection for triggers
    private void OnTriggerEnter(Collider other)
    {
        // 1. Check if the object that entered the trigger is the player
        if (other.CompareTag("Player"))
        {
            // 2. Try to get the PlayerController script from the player object
            PlayerController playerController = other.GetComponent<PlayerController>();

            if (playerController != null)
            {
                // 3. Call the public Heal method
                bool wasHealed = playerController.Heal(healthToGive);

                if (wasHealed)
                {
                    // 4. Play a sound and destroy the object if healing was successful
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