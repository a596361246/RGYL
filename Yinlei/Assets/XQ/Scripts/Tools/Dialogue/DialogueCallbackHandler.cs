using UnityEngine;
using UnityEngine.Events;
namespace XQ
{
    public class DialogueCallbackHandler : MonoBehaviour
    {
        [System.Serializable]
        public class DialogueCallback
        {
            public string eventName;
            public UnityEvent callback;
        }

        [Header("对话回调配置")]
        public DialogueCallback[] callbacks;

        private void Start()
        {
            // 注册对话结束事件
            if (DialogueManager.Instance != null)
            {
                //DialogueManager.Instance.OnDialogueComplete.AddListener(OnDialogueComplete);
            }
        }

        private void OnDestroy()
        {
            // 取消注册事件
            if (DialogueManager.Instance != null)
            {
                //DialogueManager.Instance.OnDialogueComplete.RemoveListener(OnDialogueComplete);
            }
        }

        private void OnDialogueComplete(string eventName)
        {
            // 查找并执行对应的回调
            foreach (var callback in callbacks)
            {
                if (callback.eventName == eventName)
                {
                    callback.callback?.Invoke();
                    break;
                }
            }
        }
    }
}
