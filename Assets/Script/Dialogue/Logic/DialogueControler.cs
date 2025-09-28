using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Farm.Dialogue
{
    [RequireComponent(typeof(NPCMovement))]
    [RequireComponent(typeof(BoxCollider2D))]
    public class DialogueControler : MonoBehaviour
    {
        private NPCMovement npcMovement => GetComponent<NPCMovement>();

        [Header("npc对话数据库")]
        public NPCDialogueList_SO dialogueDataList;
        public UnityEvent OnFinishEvent;
        public List<DialoguePiece> dialogueList = new List<DialoguePiece>();

        private Stack<DialoguePiece> dialogueStack;
        private GameObject NPCButton;
        private bool canTalk;

        private Questable questable => GetComponent<Questable>();
        // private bool isTalking;

        void Awake()
        {
            FillDialogueStake();
            NPCButton = transform.GetChild(1).gameObject;
            if (dialogueDataList != null)
                dialogueDataList.InitDialogueDic();
        }

        void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                canTalk = !npcMovement.isMoving && npcMovement.interactble;
            }
        }

        void OnTriggerExit2D(Collider2D collision)
        {
            canTalk = false;
        }

        void Update()
        {
            NPCButton.SetActive(canTalk);
            if (canTalk && Input.GetKeyDown(KeyCode.Space))
            {
                StartCoroutine(DialogueRoutine());
            }
        }

        private IEnumerator DialogueRoutine()
        {
            // isTalking = true;
            // BUG:现在问题是，触发之后只会重复一句话，而且不会停止
            if (questable != null)
            {
                dialogueList = dialogueDataList.InitDialogueDic()[questable.questDetails.questStates].dialogues;
                FillDialogueStake();
            }

            if (dialogueStack.TryPop(out DialoguePiece result))
            {
                EventHandler.CallShowDialogueEvent(result);
                EventHandler.CallUpdateGameStateEvent(GameState.Pause);
                yield return new WaitUntil(() => result.isDown);
                // isTalking = false;
            }
            // 如果首次堆栈中没有数据,那么就先压入
            else
            {
                EventHandler.CallShowDialogueEvent(null);
                EventHandler.CallUpdateGameStateEvent(GameState.GamePlay);
                FillDialogueStake();
                // isTalking = false;

                if (OnFinishEvent != null)
                {
                    OnFinishEvent.Invoke();
                    canTalk = false;
                }
            }
            // EventHandler.CallUpdateGameStateEvent(GameState.GamePlay);
            dialogueStack.Clear();
        }
        /// <summary>
        /// 将列表中的对话压入堆栈中
        /// </summary>
        private void FillDialogueStake()
        {
            dialogueStack = new Stack<DialoguePiece>();
            for (int i = dialogueList.Count - 1; i > -1; i--)
            {
                dialogueList[i].isDown = false;
                dialogueStack.Push(dialogueList[i]);
            }
        }


        // 根据任务不同状态切换不同对话的初步想法
        // 创建一个字典，任务状态为键，对话列表为值
        // 玩家对话时判断当前的任务状态，接着从对话字典SO中抽出当前状态的对话列表
        // 当然是一个NPC一个对话字典
    }
}