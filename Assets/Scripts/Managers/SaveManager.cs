using System;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;

    private const int MAX_SLOTS = 4;
    private int selectedSlot = 0;

    [Header("UI References")]
    public TextMeshProUGUI slotTitleText;
    public TextMeshProUGUI playerNameText;
    public TextMeshProUGUI playTimeText;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI lastSavePointText;

    [Header("Popups")]
    public GameObject createPopup;
    public TMP_InputField createNameInput;

    public GameObject deletePopup;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // -----------------------------
    // SLOT SWITCHING
    // -----------------------------
    public void ScrollLeft()
    {
        selectedSlot--;
        if (selectedSlot < 0) selectedSlot = MAX_SLOTS - 1;

        UpdateUISlotInfo();
    }

    public void ScrollRight()
    {
        selectedSlot++;
        if (selectedSlot >= MAX_SLOTS) selectedSlot = 0;

        UpdateUISlotInfo();
    }

    // -----------------------------
    // UI UPDATE FOR CURRENT SLOT
    // -----------------------------
    public void UpdateUISlotInfo()
    {
        SaveData data = LoadData(selectedSlot);

        slotTitleText.text = $"File {selectedSlot + 1}";

        if (data == null)
        {
            playerNameText.text = "Empty Slot";
            playTimeText.text = "--:--:--";
            healthText.text = "--";
            lastSavePointText.text = "--";
        }
        else
        {
            playerNameText.text = $"Name: {data.playerName}";
            healthText.text = $"Health: {data.health}";
            playTimeText.text = $"Play Time: {FormatTime(data.playTimeSeconds)}";
            lastSavePointText.text = $"Save Point: {data.lastSavePoint}";
        }
    }

    private string FormatTime(int totalSeconds)
    {
        TimeSpan t = TimeSpan.FromSeconds(totalSeconds);
        return $"{t.Hours:D2}:{t.Minutes:D2}:{t.Seconds:D2}";
    }

    // -----------------------------
    // BUTTONS — CREATE
    // -----------------------------
    public void OpenCreatePopup()
    {
        createPopup.SetActive(true);
        createNameInput.text = "";
    }

    public void ConfirmCreateFile()
    {
        string name = createNameInput.text;
        if (string.IsNullOrEmpty(name)) return;

        SaveData newData = new SaveData
        {
            playerName = name,
            health = 100,
            lastSavePoint = "None",
            playTimeSeconds = 0,
            inventory = new string[9],
            sceneName = "StartScene" // You can change this!
        };

        SaveDataToDisk(selectedSlot, newData);

        createPopup.SetActive(false);
        UpdateUISlotInfo();
    }

    public void CancelCreateFile()
    {
        createPopup.SetActive(false);
    }

    // -----------------------------
    // BUTTONS — LOAD
    // -----------------------------
    public void LoadSelectedFile()
    {
        SaveData data = LoadData(selectedSlot);

        if (data == null)
        {
            Debug.Log("No save file in this slot!");
            return;
        }

        GlobalLoadedData.loadedSaveData = data;

        // Load the saved scene
        UnityEngine.SceneManagement.SceneManager.LoadScene(data.sceneName);
    }

    // -----------------------------
    // BUTTONS — DELETE
    // -----------------------------
    public void OpenDeletePopup()
    {
        deletePopup.SetActive(true);
    }

    public void ConfirmDeleteFile()
    {
        string path = GetSavePath(selectedSlot);
        if (File.Exists(path)) File.Delete(path);

        deletePopup.SetActive(false);
        UpdateUISlotInfo();
    }

    public void CancelDeleteFile()
    {
        deletePopup.SetActive(false);
    }

    // -----------------------------
    // SAVE SYSTEM CORE
    // -----------------------------
    [Serializable]
    public class SaveData
    {
        public string playerName;
        public float health;
        public string lastSavePoint;
        public int playTimeSeconds;
        public string[] inventory;
        public string sceneName;   // NEW
    }

    public void SaveInventoryToSlot(string[] inventoryItems)
    {
        SaveData data = LoadData(selectedSlot);
        if (data == null)
        {
            Debug.LogError("Tried to save inventory but slot is empty!");
            return;
        }

        data.inventory = inventoryItems;
        SaveDataToDisk(selectedSlot, data);
    }

    public string[] LoadInventoryFromSlot()
    {
        SaveData data = LoadData(selectedSlot);
        if (data == null)
        {
            Debug.LogError("Tried to load inventory but slot is empty!");
            return null;
        }

        return data.inventory;
    }

    private string GetSavePath(int slot)
    {
        return Application.persistentDataPath + $"/save_{slot}.json";
    }

    public void SaveDataToDisk(int slot, SaveData data)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(GetSavePath(slot), json);
    }

    public SaveData LoadData(int slot)
    {
        string path = GetSavePath(slot);
        if (!File.Exists(path)) return null;

        string json = File.ReadAllText(path);
        return JsonUtility.FromJson<SaveData>(json);
    }
}

public static class GlobalLoadedData
{
    public static SaveManager.SaveData loadedSaveData;
}