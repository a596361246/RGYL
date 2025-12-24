using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using System;
namespace XQ
{
    public class DialogueManager : MonoBehaviour
    {
        [Header("UI组件")]
        public GameObject dialoguePanel;
        public TMP_Text characterNameText;
        public TMP_Text dialogueText;
        public KeyCode nextDialogueKey = KeyCode.Space;

        [Header("打字机效果")]
        public bool useTypewriterEffect = true;

        AudioSource audioSource;
        private DialogueScriptableObject currentDialogue;
        private int currentDialogueIndex = 0;
        private bool isTyping = false;
        private Coroutine typingCoroutine;

        // 对话结束回调事件
        public Action<string> OnDialogueComplete;

        // 单例模式
        public static DialogueManager Instance { get; private set; }

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            if (Instance == null)
            {
                Instance = this;
                //DontDestroyOnLoad(gameObject);
            }
        }

        private void OnDestroy()
        {
            Instance = null;
        }

        private void Start()
        {
            //if (dialoguePanel != null)
            //dialoguePanel.SetActive(false);
        }

        //private void Update()
        //{
        //    if (currentDialogue != null)//&& Input.GetKeyDown(nextDialogueKey))
        //    {
        //        if (isTyping)
        //        {
        //            // 如果正在打字，立即显示完整文本
        //            CompleteCurrentText();
        //        }
        //        else
        //        {
        //            // 显示下一段对话
        //            ShowNextDialogue();
        //        }
        //    }
        //}

        /// <summary>
        /// 显示对话
        /// </summary>
        public void ShowDialogue()
        {
            if (isTyping)
            {
                // 如果正在打字，立即显示完整文本
                CompleteCurrentText();
            }
            else
            {
                // 显示下一段对话
                ShowNextDialogue();
            }
        }

        /// <summary>
        /// 开始对话
        /// </summary>
        /// <param name="dialogue">要显示的对话数据</param>
        public void StartDialogue(DialogueScriptableObject dialogue)
        {
            if (dialogue == null || dialogue.dialogues.Length == 0)
            {
                Debug.LogWarning("对话数据为空！");
                return;
            }

            currentDialogue = dialogue;
            currentDialogueIndex = 0;

            if (dialoguePanel != null)
                dialoguePanel.SetActive(true);

            ShowCurrentDialogue();
        }

        /// <summary>
        /// 显示当前对话
        /// </summary>
        private void ShowCurrentDialogue()
        {
            if (currentDialogue == null || currentDialogueIndex >= currentDialogue.dialogues.Length)
                return;

            DialogueData currentData = currentDialogue.dialogues[currentDialogueIndex];

           
            // 设置角色名称
            if (characterNameText != null)
                characterNameText.text = currentData.characterName;

            audioSource.clip = currentData.audioClip;
            audioSource.Play();
            // 显示对话内容
            if (useTypewriterEffect)
            {
                StartTypewriter(currentData.dialogueText, currentData.textSpeed);
            }
            else
            {
                if (dialogueText != null)
                    dialogueText.text = currentData.dialogueText;
            }
        }

        /// <summary>
        /// 显示下一段对话
        /// </summary>
        private void ShowNextDialogue()
        {
            currentDialogueIndex++;

            if (currentDialogueIndex >= currentDialogue.dialogues.Length)
            {
                // 对话结束
                EndDialogue();
            }
            else
            {
                // 显示下一段对话
                ShowCurrentDialogue();
            }
        }

        /// <summary>
        /// 打字机效果
        /// </summary>
        private void StartTypewriter(string text, float speed)
        {
            if (typingCoroutine != null)
                StopCoroutine(typingCoroutine);

            typingCoroutine = StartCoroutine(TypewriterEffect(text, speed));
        }

        private IEnumerator TypewriterEffect(string text, float speed)
        {
            isTyping = true;
            dialogueText.text = "";

            foreach (char character in text)
            {
                dialogueText.text += character;
                yield return new WaitForSeconds(speed);
            }

            isTyping = false;
        }

        /// <summary>
        /// 立即完成当前文本显示
        /// </summary>
        private void CompleteCurrentText()
        {
            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
                typingCoroutine = null;
            }

            if (currentDialogue != null && currentDialogueIndex < currentDialogue.dialogues.Length)
            {
                dialogueText.text = currentDialogue.dialogues[currentDialogueIndex].dialogueText;
            }

            isTyping = false;
        }

        /// <summary>
        /// 结束对话
        /// </summary>
        private void EndDialogue()
        {
            if (dialoguePanel != null)
                dialoguePanel.SetActive(false);

            // 触发回调事件
            string callbackName = currentDialogue?.callbackEventName ?? "";
            OnDialogueComplete?.Invoke(callbackName);

            // 重置状态
            currentDialogue = null;
            currentDialogueIndex = 0;
            isTyping = false;
        }

        /// <summary>
        /// 手动结束对话
        /// </summary>
        public void ForceEndDialogue()
        {
            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
                typingCoroutine = null;
            }

            EndDialogue();
        }
    }
}