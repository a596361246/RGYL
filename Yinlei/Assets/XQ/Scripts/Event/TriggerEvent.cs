using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ========================================================
// 描述：需要Trigger交互物体挂载"
// ========================================================
namespace XQ
{
    public class TriggerEvent : MonoBehaviour
    {
        public string checkNameOrTag = "";
        public UnityEngine.UI.Button.ButtonClickedEvent TriggerEnter;
        public UnityEngine.UI.Button.ButtonClickedEvent TriggerStay;
        public UnityEngine.UI.Button.ButtonClickedEvent TriggerExit;

        private void OnTriggerEnter(Collider other)
        {
            switch (checkType)
            {
                case CheckType.Name:
                    if (other.gameObject.name == checkNameOrTag)
                    {
                        TriggerEnter.Invoke();
                    }
                    break;
                case CheckType.Tag:
                    if (other.gameObject.CompareTag(checkNameOrTag))
                    {
                        TriggerEnter.Invoke();
                    }
                    break;
            }
        }

        private void OnTriggerStay(Collider other)
        {
            switch (checkType)
            {
                case CheckType.Name:
                    if (other.gameObject.name == checkNameOrTag)
                    {
                        TriggerStay.Invoke();
                    }
                    break;
                case CheckType.Tag:
                    if (other.gameObject.CompareTag(checkNameOrTag))
                    {
                        TriggerStay.Invoke();
                    }
                    break;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            switch (checkType)
            {
                case CheckType.Name:
                    if (other.gameObject.name == checkNameOrTag)
                    {
                        TriggerExit.Invoke();
                    }
                    break;
                case CheckType.Tag:
                    if (other.gameObject.CompareTag(checkNameOrTag))
                    {
                        TriggerExit.Invoke();
                    }
                    break;
            }
        }

        public CheckType checkType = CheckType.Name;
        public enum CheckType
        {
            Name,
            Tag
        }
    }
}

