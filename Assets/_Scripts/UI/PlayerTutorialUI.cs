using UnityEngine;
using UnityEngine.UI;

public class PlayerTutorialUI : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject tutorialPromptPanel;         // Yes/No popup
    [SerializeField] private GameObject tutorialInstructionsPanel;   // Instructions + Close button

    [Header("Buttons")]
    [SerializeField] private Button promptYesButton;         // "Yes" on prompt
    [SerializeField] private Button promptNoButton;          // "No" on prompt
    [SerializeField] private Button instructionsCloseButton; // "Close" on instructions panel

    [Header("Settings")]
    [SerializeField] private bool pauseGameWhenOpen = true;
    [SerializeField] private bool showPromptOnSceneStart = true;
    [SerializeField] private bool manageCursor = true;

    private float previousTimeScale = 1f;

    private void Awake()
    {
        // Start with both panels hidden
        if (tutorialPromptPanel != null)
            tutorialPromptPanel.SetActive(false);

        if (tutorialInstructionsPanel != null)
            tutorialInstructionsPanel.SetActive(false);
    }

    private void OnEnable()
    {
        // Wire buttons via serialized references
        if (promptYesButton != null)
            promptYesButton.onClick.AddListener(OnPromptYesClicked);

        if (promptNoButton != null)
            promptNoButton.onClick.AddListener(OnPromptNoClicked);

        if (instructionsCloseButton != null)
            instructionsCloseButton.onClick.AddListener(OnCloseInstructionsClicked);
    }

    private void OnDisable()
    {
        // Clean up listeners
        if (promptYesButton != null)
            promptYesButton.onClick.RemoveListener(OnPromptYesClicked);

        if (promptNoButton != null)
            promptNoButton.onClick.RemoveListener(OnPromptNoClicked);

        if (instructionsCloseButton != null)
            instructionsCloseButton.onClick.RemoveListener(OnCloseInstructionsClicked);
    }

    private void Start()
    {
        if (showPromptOnSceneStart)
        {
            ShowPrompt();
        }
    }

    // ----- BUTTON METHODS -----

    // Yes button: hide prompt, show instructions
    private void OnPromptYesClicked()
    {
        HidePrompt();
        ShowInstructions();
    }

    // No button: hide prompt, resume game
    private void OnPromptNoClicked()
    {
        HidePrompt();
        RestoreTimeAndCursor();
    }

    // Close button on instructions panel: hide instructions, resume game
    private void OnCloseInstructionsClicked()
    {
        HideInstructions();
        RestoreTimeAndCursor();
    }

    // ----- INTERNAL HELPERS -----

    private void ShowPrompt()
    {
        if (pauseGameWhenOpen)
            PauseTime();

        if (manageCursor)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        if (tutorialPromptPanel != null)
            tutorialPromptPanel.SetActive(true);
    }

    private void HidePrompt()
    {
        if (tutorialPromptPanel != null)
            tutorialPromptPanel.SetActive(false);
    }

    private void ShowInstructions()
    {
        if (tutorialInstructionsPanel == null)
        {
            Debug.LogWarning("[PlayerTutorialUI] tutorialInstructionsPanel is not assigned!");
            return;
        }

        tutorialInstructionsPanel.SetActive(true);
    }

    private void HideInstructions()
    {
        if (tutorialInstructionsPanel != null)
            tutorialInstructionsPanel.SetActive(false);
    }

    private void PauseTime()
    {
        previousTimeScale = Time.timeScale;
        Time.timeScale = 0f;
    }

    private void RestoreTimeAndCursor()
    {
        if (pauseGameWhenOpen)
            Time.timeScale = previousTimeScale;

        if (manageCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}
