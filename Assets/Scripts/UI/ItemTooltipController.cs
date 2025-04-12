using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemTooltipController : MonoBehaviour
{
    public static ItemTooltipController Instance;

    [Header("UI Elements")]
    public GameObject tooltipPanel;
    public TextMeshProUGUI descriptionText;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        tooltipPanel.SetActive(false);
    }

    public void ShowTooltip(string description, Vector2 position)
    {
        tooltipPanel.SetActive(true);
        tooltipPanel.transform.position = position;
        descriptionText.text = description;
    }

    public void HideTooltip()
    {
        tooltipPanel.SetActive(false);
    }
}
