using UnityEngine;

public class MiniGameTrigger : MonoBehaviour
{
    public GameObject miniGameCanvas;
    public GameObject interactionPrompt;

    private bool isPlayerNearby = false;

    private void Update()
    {
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.F))
        {
            StartMiniGame();
        }

        if (miniGameCanvas.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseMiniGame();
        }
    }

    private void StartMiniGame()
    {
        if (miniGameCanvas != null)
        {
            miniGameCanvas.SetActive(true);
        }
        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(false);
        }
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void CloseMiniGame()
    {
        if (miniGameCanvas != null)
        {
            miniGameCanvas.SetActive(false);
        }
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
            if (interactionPrompt != null)
            {
                interactionPrompt.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
            if (interactionPrompt != null)
            {
                interactionPrompt.SetActive(false);
            }
        }
    }
}
