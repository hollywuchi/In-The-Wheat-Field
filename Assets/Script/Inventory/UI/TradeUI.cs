using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TradeUI : MonoBehaviour
{
    public Image itemIcon;
    public TextMeshProUGUI itemName;
    public InputField itemAmount;
    public Button submitButton;
    public Button cancelButton;

    private ItemDetails item;
    private bool isCellTrade;

    void Awake()
    {
        cancelButton.onClick.AddListener(CancleTrade);
    }

    public void SetupTradeUI(ItemDetails item, bool isCell)
    {
        this.item = item;
        itemIcon.sprite = item.itemIcon;
        itemName.text = item.itemName;
        isCellTrade = isCell;
        itemAmount.text = string.Empty;
    }

    private void CancleTrade()
    {
        this.gameObject.SetActive(false);
    }
}
