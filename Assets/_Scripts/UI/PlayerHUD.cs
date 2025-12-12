using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerHUD : MonoBehaviour
{
    [Header("Player HUD")]
    [SerializeField] private Slider healthBar;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private PlayerController player;
    [SerializeField] private TextMeshProUGUI moneyText;

    [Header("Death UI")]
    [SerializeField] private GameObject deathPanel;
    [SerializeField] private TextMeshProUGUI deathText;
    [SerializeField] private bool pauseOnDeath = true;

    private bool hasShownDeath = false;


    private void Awake()
    {
        if (player == null)
        {
            player = PlayerController.Instance;
        }
        if (player != null && healthBar != null)
        {
            healthBar.maxValue = player.MaxHealth;
        }

        if(deathPanel != null)
        {
            deathPanel.SetActive(false);
        }
            
        UpdateOnce();
    }
    // Update is called once per frame
    void Update()
    {
        UpdateOnce();

        if(!hasShownDeath && player != null && player.CurrentHealth <= 0)
        {
            ShowDeathScreen();
        }
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

    public void ShowDeathScreen()
    {
        hasShownDeath = true;

        if(deathPanel != null)
        {
            deathPanel.SetActive(true);
        }

        if(deathText != null)
        {
            deathText.text = "YOU DIED";
        }

        if (pauseOnDeath)
        {
            Time.timeScale = 0f;
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void OnRetryButton()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = true;

        Scene current = SceneManager.GetActiveScene();
        SceneManager.LoadScene(current.buildIndex);
    }
}
