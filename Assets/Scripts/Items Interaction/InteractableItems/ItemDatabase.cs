using UnityEngine;

public class ItemDatabase : MonoBehaviour
{
    public static ItemDatabase Instance;

    public SO_Items[] allItems;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public SO_Items GetItem(string id)
    {
        foreach (var item in allItems)
        {
            if (item.itemID == id)
                return item;
        }

        return null;
    }
}
