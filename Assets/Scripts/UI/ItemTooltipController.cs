using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ItemTooltipController : MonoBehaviour
{
    public static ItemTooltipController Instance;

    public GameObject tooltipPanel;
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI itemDescriptionText;

    private void Awake()
    {
        Instance = this;
        tooltipPanel.SetActive(false);
    }

    public void ShowTooltip(SO_Items item)
    {
        if (item == null) return;

        itemNameText.text = item.itemText;
        itemDescriptionText.text = item.itemDescription;

        tooltipPanel.SetActive(true);
    }

    public void HideTooltip()
    {
        tooltipPanel.SetActive(false);
    }

    private void Update()
    {
        // Make tooltip follow mouse (optional)
        tooltipPanel.transform.position = Input.mousePosition;
    }
}
