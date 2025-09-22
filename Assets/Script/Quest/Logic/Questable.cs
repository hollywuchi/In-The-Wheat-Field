using System.Transactions;
using UnityEngine;

public class Questable : MonoBehaviour
{
    public QuestDetails questDetails;

    void Awake()
    {
        questDetails = QuestManager.Instance.questDataBase.GetQuestData(this.gameObject.name);
    }

    void OnEnable()
    {
        EventHandler.AcceptQuest += OnAcceptQuest;
        EventHandler.DeliveryQuestItems += OnDeliveryQuestItems;
    }

    void OnDisable()
    {
        EventHandler.AcceptQuest -= OnAcceptQuest;
        EventHandler.DeliveryQuestItems -= OnDeliveryQuestItems;
    }


    private void OnAcceptQuest(QuestDetails currentDetials)
    {
        questDetails = currentDetials;
    }

    private void OnDeliveryQuestItems(QuestDetails currnetDetails)
    {
        questDetails = currnetDetails;
    }
}