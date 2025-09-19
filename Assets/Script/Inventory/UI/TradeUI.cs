using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Farm.Inventory
{
    public class TradeUI : MonoBehaviour
    {
        public Image itemIcon;
        public TextMeshProUGUI itemName;
        public InputField itemAmount;
        public Button submitButton;
        public Button cancelButton;
        public TextMeshProUGUI tradeText;

        private ItemDetails item;
        private bool isSellTrade;

        void Awake()
        {
            cancelButton.onClick.AddListener(CancleTrade);
            submitButton.onClick.AddListener(TradeItem);
            itemAmount.characterLimit = 2;
        }
        public void SetupTradeUI(ItemDetails item, bool isCell)
        {
            this.item = item;
            itemIcon.sprite = item.itemIcon;
            itemName.text = item.itemName;
            isSellTrade = isCell;
            itemAmount.text = "1";
        }

        public void TradeItem()
        {
            var amount = Convert.ToInt32(itemAmount.text);

            InventoryManager.Instance.TradeItem(item, amount, isSellTrade);
            CancleTrade();
        }
        public void AddItem()
        {
            int amount = Convert.ToInt32(itemAmount.text);
            amount++;
            itemAmount.text = amount.ToString();
        }
        public void ReduceItem()
        {
            int amount = Convert.ToInt32(itemAmount.text);
            if (amount > 1)
                amount--;
            itemAmount.text = amount.ToString();
        }
        private void CancleTrade()
        {
            this.gameObject.SetActive(false);
        }
    }
}