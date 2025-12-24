using UnityEngine;
namespace XQ
{
    public class DialogueTrigger : MonoBehaviour
    {
        [Header("对话配置")]
        public DialogueScriptableObject dialogueData;

        [Header("触发设置")]
        public bool triggerOnStart = false;
        public bool triggerOnCollision = false;
        public bool triggerOnClick = false;

        private void Start()
        {
            if (triggerOnStart)
            {
                TriggerDialogue();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (triggerOnCollision && other.CompareTag("Player"))
            {
                TriggerDialogue();
            }
        }

        private void OnMouseDown()
        {
            if (triggerOnClick)
            {
                TriggerDialogue();
            }
        }

        public void TriggerDialogue()
        {
            if (DialogueManager.Instance != null && dialogueData != null)
            {
                DialogueManager.Instance.StartDialogue(dialogueData);
            }
            else
            {
                Debug.LogWarning("DialogueManager未找到或对话数据为空！");
            }
        }

        public void DebugTest()
        {
            Debug.Log("对话结束");
        }
    }
}