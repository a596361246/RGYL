using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace XQ
{
    /// <summary>
    /// 放置层级最底层
    /// </summary>
    public class UISetting : PanelBase
    {
        #region 变量
        [DllImport("__Internal")]
        private static extern void unityFullScreen();

        [Header("全屏")]
        public Button fullScrennTrigger;

        #endregion

        private void Awake()
        {
            if (fullScrennTrigger != null)
            {
                fullScrennTrigger.onClick.AddListener(OnPointDown);
            }            
        }
        /// <summary>
        /// 切换全屏
        /// </summary>
        /// <param name="downEvent"></param>
        void OnPointDown()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            unityFullScreen();
#endif
        }
    }
}
