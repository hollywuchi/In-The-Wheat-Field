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
        EventHandler.StartNewGameEvent += OnStartNewGameEvent;
    }

    void OnDisable()
    {
        EventHandler.AcceptQuest -= OnAcceptQuest;
        EventHandler.DeliveryQuestItems -= OnDeliveryQuestItems;
        EventHandler.RefreshQuestDetails -= OnRefreshQuestDetails;
        EventHandler.StartNewGameEvent -= OnStartNewGameEvent;
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

    /// <summary>
    /// 新游戏要执行的方法,解决了在进行新游戏时（没有存档数据的情况下）
    /// </summary>
    /// <param name="obj"></param>
    /// <exception cref="System.NotImplementedException"></exception>
    private void OnStartNewGameEvent(int obj)
    {
        questDetails = QuestManager.Instance.RefreshQuestDetails(gameObject.name);
    }
}