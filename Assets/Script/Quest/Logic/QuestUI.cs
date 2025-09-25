using Farm.Inventory;
using UnityEngine;
using UnityEngine.UI;

public class QuestUI : MonoBehaviour
{
    public QuestDetails questDetails;
    private Text questName;
    void Awake()
    {
        questName = transform.GetChild(0).GetComponent<Text>();
    }

    void OnEnable()
    {
        EventHandler.ShowQuestOnUI += OnShowQuestOnUI;
    }

    void OnDisable()
    {
        EventHandler.ShowQuestOnUI -= OnShowQuestOnUI;
    }

    private void OnShowQuestOnUI(QuestDetails details)
    {
        questDetails = details;
        questName.text = details.questName;
    }

    public void CallUI()
    {
        UIManager.Instance.WakeUpUI();
        EventHandler.CallShowDetailOnInfoUI(questDetails);
    }

}
