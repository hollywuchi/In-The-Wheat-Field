using System.Collections;
using System.Collections.Generic;
using Farm.Inventory;
using UnityEngine;
using UnityEngine.UI;

public class QuestInfoUI : MonoBehaviour
{
    public GameObject infoPanel;
    public Text infoQuestName;
    public Text infoQuestMessage;
    public GameObject rewardsList;
    public GameObject rewardSlotPrefab;
    void OnEnable()
    {
        EventHandler.ShowDetailOnInfoUI += ShowInformationOnPanel;
    }

    void OnDisable()
    {
        EventHandler.ShowDetailOnInfoUI -= ShowInformationOnPanel;
    }

    public void ShowInformationOnPanel(QuestDetails questDetails)
    {
        infoQuestName.text = questDetails.questName;
        infoQuestMessage.text = questDetails.questIntroduction;
        foreach (var reward in questDetails.rewards)
        {
            var rewardSlot = Instantiate(rewardSlotPrefab, rewardsList.transform);
            var currentItemDetal = InventoryManager.Instance.GetItemInDataBase(reward.itemID);
            // rewardSlot.GetComponent<SlotUI>().itemDetails = currentItemDetal;
            // rewardSlot.GetComponent<SlotUI>().itemAmount = reward.itemAmount;
            rewardSlot.GetComponent<SlotUI>().UpdateSolt(currentItemDetal,reward.itemAmount);
        }
    }
}
