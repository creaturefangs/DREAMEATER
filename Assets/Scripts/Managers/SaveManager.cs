using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;

    [Header("Save Settings")]
    public int activeSlot = -1;
    public SaveData currentSave;

    private string saveFolderPath;
    private string saveFileName = "saveSlot_";
    private float playTimer;

    public bool hasLoaded = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            saveFolderPath = Application.persistentDataPath + "/Saves/";
            if (!Directory.Exists(saveFolderPath))
                Directory.CreateDirectory(saveFolderPath);

            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        // Add playtime only when a save is loaded
        if (hasLoaded)
        {
            playTimer += Time.deltaTime;
        }
    }

    // ---------------------------------------------------------------------
    // SAVE / LOAD API
    // ---------------------------------------------------------------------

    public void CreateNewSave(int slot, string characterName, float startingHealth, string startingSavePoint)
    {
        activeSlot = slot;

        currentSave = new SaveData
        {
            characterName = characterName,
            health = startingHealth,
            lastSavePointID = startingSavePoint,
            playTimeSeconds = 0f,
            inventoryIDs = new string[6]
        };

        // Initialize empty inventory
        for (int i = 0; i < currentSave.inventoryIDs.Length; i++)
            currentSave.inventoryIDs[i] = "";

        hasLoaded = true;
        SaveGame();
    }

    public void SaveGame()
    {
        if (activeSlot < 0)
        {
            Debug.LogError("No save slot selected!");
            return;
        }

        // Update playtime into save data
        currentSave.playTimeSeconds += playTimer;
        playTimer = 0f;

        string filePath = saveFolderPath + saveFileName + activeSlot + ".json";
        string json = JsonUtility.ToJson(currentSave, true);
        File.WriteAllText(filePath, json);
    }

    public bool LoadGame(int slot)
    {
        activeSlot = slot;

        string filePath = saveFolderPath + saveFileName + slot + ".json";
        if (!File.Exists(filePath))
        {
            Debug.Log("Save file does not exist: " + filePath);
            return false;
        }

        string json = File.ReadAllText(filePath);
        currentSave = JsonUtility.FromJson<SaveData>(json);

        hasLoaded = true;
        playTimer = 0f;

        return true;
    }

    public void DeleteSave(int slot)
    {
        string filePath = saveFolderPath + saveFileName + slot + ".json";
        if (File.Exists(filePath))
            File.Delete(filePath);
    }

    // ---------------------------------------------------------------------
    // INVENTORY SAVE / LOAD
    // ---------------------------------------------------------------------

    public void SaveInventory()
    {
        if (currentSave == null)
            return;

        InventoryManager inv = InventoryManager.Instance;

        for (int i = 0; i < currentSave.inventoryIDs.Length; i++)
        {
            SO_Items item = inv.GetItemAtIndex(i);
            currentSave.inventoryIDs[i] = item != null ? item.itemID : "";
        }
    }

    public void LoadInventory()
    {
        if (!hasLoaded)
            return;

        InventoryManager inv = InventoryManager.Instance;

        for (int i = 0; i < currentSave.inventoryIDs.Length; i++)
        {
            string id = currentSave.inventoryIDs[i];
            SO_Items item = string.IsNullOrEmpty(id)
                ? null
                : ItemDatabase.Instance.GetItem(id);

            inv.SetItemAtIndex(i, item);
        }
    }

    // ---------------------------------------------------------------------
    // SCENE LOADING HOOK
    // ---------------------------------------------------------------------

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (hasLoaded)
        {
            // Load the inventory once the gameplay scene is loaded
            LoadInventory();
        }
    }
}

// -----------------------------------------------------------------------------
// SAVE DATA MODEL
// -----------------------------------------------------------------------------

[System.Serializable]
public class SaveData
{
    public string characterName;
    public float health;
    public string lastSavePointID;
    public float playTimeSeconds;

    // Inventory (6 slot)
    public string[] inventoryIDs;
}