using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XQ
{
    public class InteractEvent : MonoBehaviour
    {
        public UnityEngine.UI.Button.ButtonClickedEvent interEvent;
        public void EventInvoke()
        {
            interEvent?.Invoke();
        }
    }
}

