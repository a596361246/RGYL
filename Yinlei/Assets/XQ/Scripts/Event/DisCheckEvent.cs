using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XQ
{
    public class DisCheckEvent : MonoBehaviour
    {
        [Header("检测范围")]
        public float range = 1.0f;
        [Header("到达检测范围内触发的事件")]
        public UnityEngine.UI.Button.ButtonClickedEvent arriveRangeEvent;
        [Header("离开检测范围内触发的事件")]
        public UnityEngine.UI.Button.ButtonClickedEvent exitRangeEvent;
        [Header("是否显示范围线条")]
        public bool showRangeLine = true;
        [Header("检测对象Tag")]
        public string checkTag = "Player";


        void Update()
        {
            if (GameObject.FindGameObjectsWithTag(checkTag).Length <= 0)
            {
                return;
            }

            foreach (GameObject item in GameObject.FindGameObjectsWithTag(checkTag))
            {
                if (Vector3.Distance(transform.position, item.transform.position) <= range)
                {
                    if (checkStatus != CheckStatus.CS_IsIn)
                    {
                        checkStatus = CheckStatus.CS_IsIn;
                        arriveRangeEvent.Invoke();
                    }
                }
                else
                {
                    if (checkStatus != CheckStatus.CS_IsExit)
                    {
                        checkStatus = CheckStatus.CS_IsExit;
                        exitRangeEvent.Invoke();
                    }
                }
            }
        }

        private void OnDrawGizmos()
        {
            if (showRangeLine)
            {
                Gizmos.DrawWireSphere(transform.position, range);
            }
        }

        private CheckStatus checkStatus = CheckStatus.CS_None;
        public enum CheckStatus
        {
            CS_None,
            CS_IsIn,
            CS_IsExit
        }
    }
}

