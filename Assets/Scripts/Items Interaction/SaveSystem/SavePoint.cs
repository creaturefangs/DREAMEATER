using UnityEngine;

public class SavePoint : MonoBehaviour
{
    [Header("Save Point Settings")]
    public string savePointID = "SavePoint_01";

    [Header("UI")]
    public GameObject saveMenuUI;   // Assign your save menu panel here
    public KeyCode interactKey = KeyCode.E;

    private bool playerInRange = false;

    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(interactKey))
        {
            OpenSaveMenu();
        }
    }

    private void OpenSaveMenu()
    {
        if (saveMenuUI != null)
        {
            saveMenuUI.SetActive(true);
            Time.timeScale = 0f; // Optional: pause game
        }
    }

    public void CloseSaveMenu()
    {
        if (saveMenuUI != null)
        {
            saveMenuUI.SetActive(false);
            Time.timeScale = 1f;
        }
    }

    // -----------------------------
    // BUTTON: Save Game
    // -----------------------------
    public void SaveGameButton()
    {
        HealthBarManager player = FindObjectOfType<HealthBarManager>();
        if (player == null)
        {
            Debug.LogError("SavePoint: Could not find PlayerHealth!");
            return;
        }

        // Save last save point
        SaveManager.Instance.currentSave.lastSavePointID = savePointID;

        // Save inventory first
        SaveManager.Instance.SaveInventory();

        // Save health
        SaveManager.Instance.currentSave.health = player.currentHealth;

        // Write to file
        SaveManager.Instance.SaveGame();

        Debug.Log("Game saved at: " + savePointID);
    }

    // -----------------------------
    // Detect player entering trigger
    // -----------------------------
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInRange = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInRange = false;
    }
}
