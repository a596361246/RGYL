using UnityEngine;
using System;

[System.Serializable]
public class DialogueData
{
    [Header("角色信息")]
    public string characterName;

    [Header("对话内容")]
    [TextArea(3, 10)]
    public string dialogueText;

    [Header("语音文件")]
    public AudioClip audioClip;

    [Header("显示设置")]
    public float textSpeed = 0.05f; // 打字机效果速度
}

namespace XQ
{
    [CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue System/Dialogue")]
    public class DialogueScriptableObject : ScriptableObject
    {
        [Header("对话配置")]
        public DialogueData[] dialogues;

        [Header("回调事件名称")]
        public string callbackEventName;
    }
}
