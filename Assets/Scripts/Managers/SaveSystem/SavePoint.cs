using UnityEngine;
using UnityEngine.SceneManagement;

public class SavePoint : MonoBehaviour
{
    [Header("Save Point Settings")]
    public string savePointID = "SavePoint_01";

    [Header("UI")]
    public GameObject saveMenuUI;
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
            Time.timeScale = 0f;
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

    // ----------------------------------------------------------
    // SAVE GAME BUTTON LOGIC
    // ----------------------------------------------------------
    public void SaveGameButton()
    {
        SaveManager mgr = SaveManager.Instance;

        if (mgr == null)
        {
            Debug.LogError("SaveManager not found.");
            return;
        }

        int slot = GetCurrentSlot();
        SaveManager.SaveData data = mgr.LoadData(slot);

        if (data == null)
        {
            Debug.LogError("This save slot is empty. Create a file in the main menu first.");
            return;
        }

        // --------------------------
        // UPDATE PLAYER HEALTH
        // --------------------------
        HealthBarManager player = FindObjectOfType<HealthBarManager>();
        if (player != null)
            data.health = player.currentHealth;

        // --------------------------
        // SAVE POINT ID
        // --------------------------
        data.lastSavePoint = savePointID;

        // --------------------------
        // SCENE NAME
        // --------------------------
        data.sceneName = SceneManager.GetActiveScene().name;

        // --------------------------
        // INVENTORY
        // --------------------------
        string[] inventoryStrings = InventoryManager.Instance.GetInventoryAsStringArray();
        data.inventory = inventoryStrings;

        // --------------------------
        // PLAY TIME
        // --------------------------
        if (GlobalLoadedData.loadedSaveData != null)
            data.playTimeSeconds = GlobalLoadedData.loadedSaveData.playTimeSeconds;

        // --------------------------
        // SAVE TO DISK
        // --------------------------
        mgr.SaveDataToDisk(slot, data);

        Debug.Log("Game saved at save point: " + savePointID);
    }

    // ----------------------------------------------------------
    // GET SELECTED SLOT FROM SAVEMANAGER (private field)
    // ----------------------------------------------------------
    private int GetCurrentSlot()
    {
        var field = typeof(SaveManager).GetField(
            "selectedSlot",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        return (int)field.GetValue(SaveManager.Instance);
    }

    // ----------------------------------------------------------
    // TRIGGER HANDLING
    // ----------------------------------------------------------
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