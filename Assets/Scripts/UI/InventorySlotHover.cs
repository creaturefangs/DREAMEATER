using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlotHover : MonoBehaviour
{
    public int slotIndex;

    public void OnPointerEnter(PointerEventData eventData)
    {
        SO_Items item = InventoryManager.Instance.GetItemAtIndex(slotIndex);

        if (item != null)
        {
            ItemTooltipController.Instance.ShowTooltip(item);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ItemTooltipController.Instance.HideTooltip();
    }
}
