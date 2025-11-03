using UnityEngine;

public class AmmoPickup : MonoBehaviour
{
    [Header("Pickup Settings")]
    [SerializeField] private int ammoToGive = 20;
    [SerializeField] private AudioClip pickupSound; 

    // Unity's built-in collision detection for triggers
    private void OnTriggerEnter(Collider other)
    {
        // 1. Check if the object that entered the trigger is the player
        if (other.CompareTag("Player"))
        {
            // 2. Try to get the PlayerCombat script from the player object
            PlayerCombat playerCombat = other.GetComponent<PlayerCombat>();

            if (playerCombat != null)
            {
                // 3. Call the public method we just added
                bool wasPickedUp = playerCombat.AddReserveAmmo(ammoToGive);

                if (wasPickedUp)
                {
                    // 4. Play a sound and destroy the object if pickup was successful
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