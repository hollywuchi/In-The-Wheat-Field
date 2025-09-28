using System.Collections.Generic;
using System.Data;
using Farm.Dialogue;
using UnityEngine;
using UnityEngine.Rendering.UI;

[System.Serializable]
public class dialogueList
{
    public List<DialoguePiece> dialogues;
}

[CreateAssetMenu(fileName = "Dialogue", menuName = "Dialogue/NPCDialogueList_SO")]
public class NPCDialogueList_SO : ScriptableObject
{
    public List<dialogueList> npcDialogueList;
    public Dictionary<QuestStates, dialogueList> npcDialogueDic = new Dictionary<QuestStates, dialogueList>();

    public Dictionary<QuestStates, dialogueList> InitDialogueDic()
    {
        foreach (QuestStates questStates in System.Enum.GetValues(typeof(QuestStates)))
        {
            if (!npcDialogueDic.ContainsKey(questStates))
            {
                npcDialogueDic.Add(questStates, npcDialogueList[(int)questStates]);
            }
        }

        return npcDialogueDic;
    }
}
