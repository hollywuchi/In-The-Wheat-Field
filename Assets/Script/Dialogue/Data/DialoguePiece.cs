using UnityEngine;

namespace Farm.Dialogue
{
    [System.Serializable]
    public class DialoguePiece
    {
        [Header("对话详情")]
        public string name;
        public Sprite faceImage;
        public bool onLeft;
        [TextArea]
        public string dialogueText;
        public bool hasToPause;     // 是否需要暂停
        [HideInInspector] public bool isDown;
    }
}