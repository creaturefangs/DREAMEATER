using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    [Header("Inventory UI")]
    public Image[] inventorySlots; // UI slots where items will be displayed

    private SO_Items[] inventory = new SO_Items[6]; // Max 6 items in inventory

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject); // Prevent duplicates
        }
    }

    // Add item to inventory (returns true if added, false if full)
    public bool AddItem(SO_Items item)
    {
        for (int i = 0; i < inventory.Length; i++)
        {
            if (inventory[i] == null)
            {
                inventory[i] = item;
                UpdateInventoryUI();
                return true;
            }
        }
        Debug.Log("Inventory is full!");
        return false;
    }

    // Remove item from inventory
    public void RemoveItem(SO_Items item)
    {
        for (int i = 0; i < inventory.Length; i++)
        {
            if (inventory[i] == item)
            {
                inventory[i] = null;
                UpdateInventoryUI();
                return;
            }
        }
    }

    // Update inventory UI based on current inventory state
    private void UpdateInventoryUI()
    {
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            if (inventory[i] != null)
            {
                inventorySlots[i].sprite = inventory[i].itemIcon; // Show item icon
                inventorySlots[i].enabled = true; // Enable slot image
            }
            else
            {
                inventorySlots[i].sprite = null; // Empty slot
                inventorySlots[i].enabled = false;
            }
        }
    }

    public SO_Items GetItemAtIndex(int index)
    {
        return inventory[index];
    }

    public void SetItemAtIndex(int index, SO_Items item)
    {
        inventory[index] = item;
        UpdateInventoryUI();
    }

    public string[] GetInventoryAsStringArray()
    {
        string[] ids = new string[inventory.Length];

        for (int i = 0; i < inventory.Length; i++)
        {
            ids[i] = inventory[i] != null ? inventory[i].itemID : "";
        }

        return ids;
    }

    public void LoadInventoryFromStrings(string[] ids)
    {
        for (int i = 0; i < inventory.Length; i++)
        {
            if (string.IsNullOrEmpty(ids[i]))
            {
                inventory[i] = null;
            }
            else
            {
                inventory[i] = ItemDatabase.Instance.GetItem(ids[i]);
            }
        }

        UpdateInventoryUI();
    }

    public SO_Items[] GetInventoryArray()
    {
        return inventory;
    }
}
