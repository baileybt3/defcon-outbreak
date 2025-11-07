using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour
{
    [SerializeField] private Slider healthBar;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private PlayerController player;
    [SerializeField] private TextMeshProUGUI moneyText;


    private void Awake()
    {
        if (player == null) player = PlayerController.Instance; ;
        if (player != null && healthBar != null)
            healthBar.maxValue = player.MaxHealth;
        UpdateOnce();
    }
    // Update is called once per frame
    void Update()
    {
        UpdateOnce();
    }

    private void UpdateOnce()
    {
        if (player == null || healthBar == null) return;

        if (healthBar.maxValue != player.MaxHealth)
            healthBar.maxValue = player.MaxHealth;

        healthBar.value = player.CurrentHealth;

        if(healthText != null)
        {
            healthText.text = $"Health: {player.CurrentHealth} / {player.MaxHealth}";
        }

        if(moneyText != null)
        {
            moneyText.text = $"$ {player.Money}";
        }
    }
}
