using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ========================================================
// 描述：需要Collision交互物体挂载"
// ========================================================
namespace XQ
{
    public class CollisionEvent : MonoBehaviour
    {
        public string checkNameOrTag = "";
        public UnityEngine.UI.Button.ButtonClickedEvent CollisionEnter;
        public UnityEngine.UI.Button.ButtonClickedEvent CollisionStay;
        public UnityEngine.UI.Button.ButtonClickedEvent CollisionExit;


        private void OnCollisionEnter(Collision other)
        {
            switch (checkType)
            {
                case CheckType.Name:
                    if (other.gameObject.name == checkNameOrTag)
                    {
                        CollisionEnter.Invoke();
                    }
                    break;
                case CheckType.Tag:
                    if (other.gameObject.CompareTag(checkNameOrTag))
                    {
                        CollisionEnter.Invoke();
                    }
                    break;
            }
        }

        private void OnCollisionStay(Collision other)
        {
            switch (checkType)
            {
                case CheckType.Name:
                    if (other.gameObject.name == checkNameOrTag)
                    {
                        CollisionStay.Invoke();
                    }
                    break;
                case CheckType.Tag:
                    if (other.gameObject.CompareTag(checkNameOrTag))
                    {
                        CollisionStay.Invoke();
                    }
                    break;
            }
        }

        private void OnCollisionExit(Collision other)
        {
            switch (checkType)
            {
                case CheckType.Name:
                    if (other.gameObject.name == checkNameOrTag)
                    {
                        CollisionExit.Invoke();
                    }
                    break;
                case CheckType.Tag:
                    if (other.gameObject.CompareTag(checkNameOrTag))
                    {
                        CollisionExit.Invoke();
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

