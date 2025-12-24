using System;
using System.Collections;
using System.Collections.Generic;
using HighlightingSystem;


using UnityEngine;
using UnityEngine.Events;
namespace XQ
{
    [Serializable]
    public class RaycastEvent : UnityEvent<RaycastHit> { }

    [RequireComponent(typeof(Highlighter))]
    //[RequireComponent(typeof(BoxCollider))]
    public class RayEvent : MonoBehaviour
    {
        public RaycastEvent OnRayPickEvent;
        private Highlighter highlighter;
        //BoxCollider boxCollider;
        private void Awake()
        {
            highlighter = GetComponent<Highlighter>();
            //highlighter.tween = true;
            //boxCollider = GetComponent<BoxCollider>();
        }

        /// <summary>
        /// 激活高亮
        /// </summary>
        /// <param name="isActive"></param>
        public void ActiveHighlight(bool isActive)
        {
            highlighter.tween = isActive;
            //boxCollider.enabled = isActive;
        }
        /// <summary>
        /// 射线选中后事件开启
        /// </summary>
        /// <param name="info"></param>
		public void ActivePickEvent(RaycastHit info)
        {
            OnRayPickEvent?.Invoke(info);
            ActiveHighlight(false);
        }
    }
}
