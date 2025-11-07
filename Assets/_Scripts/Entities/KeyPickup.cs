using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider))]
public class KeyPickup : MonoBehaviour
{
    [SerializeField] private string nextSceneName = "Street";
    private bool pickedUp = false;

    private bool playerInRange;

    private void Reset()
    {
        var col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (pickedUp) return;

        if (other.GetComponent<PlayerController>() != null)
        {
            pickedUp = true;

            if(pickedUp)
            {
                LoadNextScene();
            }
        }
            playerInRange = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PlayerController>() != null)
            playerInRange = false;
    }

    private void Update()
    {
        if(playerInRange && PlayerController.Instance != null && PlayerController.Instance.InputActions.PlayerInputActions.Interact.triggered)
        {
            if (!string.IsNullOrEmpty(nextSceneName))
                SceneManager.LoadScene(nextSceneName);
            else
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }

    void LoadNextScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex + 1);
    }
}