using System.Transactions;
using UnityEngine;

public class Questable : MonoBehaviour
{
    public QuestDetails questDetails;


    void OnEnable()
    {
        EventHandler.AcceptQuest += OnAcceptQuest;
        EventHandler.DeliveryQuestItems += OnDeliveryQuestItems;
        EventHandler.RefreshQuestDetails += OnRefreshQuestDetails;
    }

    void OnDisable()
    {
        EventHandler.AcceptQuest -= OnAcceptQuest;
        EventHandler.DeliveryQuestItems -= OnDeliveryQuestItems;
        EventHandler.RefreshQuestDetails += OnRefreshQuestDetails;
    }

    private void OnAcceptQuest(QuestDetails currentDetials)
    {
        if (questDetails.questName == currentDetials.questName)
            questDetails = currentDetials;
    }

    private void OnDeliveryQuestItems(QuestDetails currnetDetails)
    {
        questDetails = currnetDetails;
    }

    /// <summary>
    /// 重新刷新任务，为了解决场景加载之前就获取旧版本的任务状态的BUG
    /// </summary>
    private void OnRefreshQuestDetails()
    {
        questDetails = QuestManager.Instance.RefreshQuestDetails(gameObject.name);
    }
}