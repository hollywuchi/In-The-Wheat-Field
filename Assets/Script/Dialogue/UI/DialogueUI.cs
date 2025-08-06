using System.Collections;
using System.Collections.Generic;
using System.IO;
using DG.Tweening;
using Farm.Dialogue;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class DialogueUI : MonoBehaviour
{
    public GameObject dialogueBox;
    public Text dialogueText;
    public Image faceRight, faceLeft;
    public Text nameRight, nameLeft;
    public GameObject continueBox;
    void Awake()
    {
        continueBox.SetActive(false);
    }
    void OnEnable()
    {
        EventHandler.ShowDialogueEvent += OnShowDialogueEvent;
    }
    void OnDisable()
    {
        EventHandler.ShowDialogueEvent -= OnShowDialogueEvent;
    }

    private void OnShowDialogueEvent(DialoguePiece piece)
    {
        StartCoroutine(ShowDialogue(piece));
    }

    private IEnumerator ShowDialogue(DialoguePiece piece)
    {
        if (piece != null)
        {
            piece.isDown = false;

            dialogueBox.SetActive(true);
            continueBox.SetActive(false);

            dialogueText.text = string.Empty;

            if (piece.name != string.Empty)
            {
                if (piece.onLeft)
                {
                    faceLeft.gameObject.SetActive(true);
                    faceRight.gameObject.SetActive(false);

                    faceLeft.sprite = piece.faceImage;
                    nameLeft.text = piece.name;
                }
                else
                {
                    faceLeft.gameObject.SetActive(false);
                    faceRight.gameObject.SetActive(true);

                    faceRight.sprite = piece.faceImage;
                    nameRight.text = piece.name;
                }
            }
            else
            {
                faceLeft.gameObject.SetActive(false);
                faceRight.gameObject.SetActive(false);
                nameLeft.gameObject.SetActive(false);
                nameRight.gameObject.SetActive(false);
            }

            yield return dialogueText.DOText(piece.dialogueText, 1f).WaitForCompletion();   // 等待完成
            piece.isDown = true;

            if (piece.hasToPause && piece.isDown)
                continueBox.SetActive(true);
        }
        else
        {
            dialogueBox.SetActive(false);
            yield break;
        }
    }
}
