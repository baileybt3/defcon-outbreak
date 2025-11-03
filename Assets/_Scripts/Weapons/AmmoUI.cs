using UnityEngine;
using UnityEngine.UI;

public class AmmoUI : MonoBehaviour
{
    [Header("References")]
    public PlayerCombat playerCombat;
    public PlayerController playerController;
    
    [Header("Ammo Display")]
    public Text ammoText;
    public string reloadMessage = "Reloading...";
    
    [Header("Health Display")]
    public Text healthText;
    public GameObject gameOverPanel;

    private void Start()
    {
        // Ensure the Game Over panel is hidden at start
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
    }

    private void Update()
    {
        // --- Health Logic ---
        if (playerController != null && healthText != null)
        {
            if (playerController.IsAlive)
            {
                // Displaying health using the new public properties
                healthText.text = $"Health: {playerController.CurrentHealth} / {playerController.MaxHealth}";
            }
            else
            {
                // Player is dead, show game over screen
                if (gameOverPanel != null && !gameOverPanel.activeSelf)
                {
                    gameOverPanel.SetActive(true);
                }
                healthText.text = "DEAD";
            }
        }
        
        // --- Ammo Logic ---
        if (playerCombat == null || ammoText == null) return;

        if (playerCombat.IsReloading)
        {
            ammoText.text = reloadMessage;
        }
        else
        {
            ammoText.text = $"{playerCombat.CurrentAmmo} / {playerCombat.ReserveAmmo}";
        }
    }
}