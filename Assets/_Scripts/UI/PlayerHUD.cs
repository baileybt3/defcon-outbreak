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

    [Header("Win/Lose UI")]
    [SerializeField] private GameObject endPanel;
    [SerializeField] private GameObject winText;
    [SerializeField] private GameObject loseText;
    [SerializeField] private bool pauseOnEnd = true;

    private bool hasShownEnd = false;


    private void Awake()
    {
        Time.timeScale = 1f;
        hasShownEnd = false;


        if (player == null)
        {
            player = PlayerController.Instance;
        }
        if (player != null && healthBar != null)
        {
            healthBar.maxValue = player.MaxHealth;
        }

        if (endPanel != null) endPanel.SetActive(false);
        if (winText != null) winText.SetActive(false);
        if (loseText != null) loseText.SetActive(false);

        UpdateOnce();
    }
    // Update is called once per frame
    void Update()
    {
        UpdateOnce();

        if(!hasShownEnd && player != null && player.CurrentHealth <= 0)
        {
            ShowEndScreen(false);
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

    public void ShowEndScreen(bool won)
    {
        if (hasShownEnd) return;
        hasShownEnd = true;

        if (endPanel != null) endPanel.SetActive(true);
        if (winText != null) winText.SetActive(won);
        if (loseText != null) loseText.SetActive(!won);

        if (player != null)
            player.InputActions.Disable();

        if (pauseOnEnd)
            Time.timeScale = 0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void OnRetryButton()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Scene current = SceneManager.GetActiveScene();
        SceneManager.LoadScene(current.buildIndex);
    }

    public  void OnQuitButton()
    {
        Time.timeScale = 1f;
        Application.Quit();
    }
}
