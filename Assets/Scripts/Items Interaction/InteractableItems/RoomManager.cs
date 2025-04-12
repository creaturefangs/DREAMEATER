using UnityEngine;
using System.Collections;

public class RoomManager : MonoBehaviour
{
    public static RoomManager Instance;

    [Header("NPCs and Room Data")]
    public NPC[] npcs; // List of all NPCs
    public Door[] roomDoors; // List of corresponding doors for each NPC's room


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Call this method when an item is given to an NPC
    public void GiveItemToNPC(NPC npc, SO_Items item)
    {
        npc.AddItem(item);
        CheckNpcProgress(npc);
    }

    // Check if NPC has been given both items and unlock their door
    private void CheckNpcProgress(NPC npc)
    {
        if (npc.itemsGiven == 2)
        {
            UnlockRoomDoor(npc);
        }
    }

    // Unlock the room door when both items are given
    private void UnlockRoomDoor(NPC npc)
    {
        int npcIndex = System.Array.IndexOf(npcs, npc);
        if (npcIndex >= 0 && npcIndex < roomDoors.Length)
        {
            roomDoors[npcIndex].Unlock();
        }
    }
}

[System.Serializable]
public class NPC     
{
    public string npcName;
    public int itemsGiven = 0;

    // Add an item to this NPC's item count
    public void AddItem(SO_Items item)
    {
        itemsGiven++;
    }
}

public class Door : MonoBehaviour
{
    public LockableDoorTeleport lockedDoor;
    public void Unlock()
    {
        Debug.Log("Door unlocked!");

        lockedDoor.isLocked = false;
            // Logic to open the door or enable the exit
    }
}
