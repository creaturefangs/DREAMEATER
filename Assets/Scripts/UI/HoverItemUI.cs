using UnityEngine;
using UnityEngine.EventSystems;

public class HoverItemUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public SO_Items itemData;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (itemData != null)
        {
            Vector2 position = Input.mousePosition;
            ItemTooltipController.Instance.ShowTooltip(itemData.itemDescription, position);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ItemTooltipController.Instance.HideTooltip();
    }
}
