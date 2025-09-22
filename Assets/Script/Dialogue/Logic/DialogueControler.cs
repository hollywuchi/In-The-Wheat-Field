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

        public UnityEvent OnFinishEvent;
        public List<DialoguePiece> dialogueList = new List<DialoguePiece>();

        private Stack<DialoguePiece> dialogueStack;
        private GameObject NPCButton;
        private bool canTalk;
        // private bool isTalking;

        void Awake()
        {
            FillDialogueStake();
            NPCButton = transform.GetChild(1).gameObject;
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
            if (dialogueStack.TryPop(out DialoguePiece result))
            {
                EventHandler.CallDialogueEvent(result);
                EventHandler.CallUpdateGameStateEvent(GameState.Pause);
                yield return new WaitUntil(() => result.isDown);
                // isTalking = false;
            }
            else
            {
                EventHandler.CallDialogueEvent(null);
                EventHandler.CallUpdateGameStateEvent(GameState.GamePlay);
                FillDialogueStake();
                // isTalking = false;

                if (OnFinishEvent != null)
                {
                    OnFinishEvent.Invoke();
                    canTalk = false;
                }
            }


        }
        private void FillDialogueStake()
        {
            dialogueStack = new Stack<DialoguePiece>();
            for (int i = dialogueList.Count - 1; i > -1; i--)
            {
                dialogueList[i].isDown = false;
                dialogueStack.Push(dialogueList[i]);
            }
        }

    }
}