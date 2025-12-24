using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class FullScreenMgr : MonoBehaviour
{
    [DllImport("__Internal")] 
    private static extern void unityFullScreen();
    /// <summary>
    /// 全屏调用 禁止按钮直接调用，请使用 Event Trigger 绑定 
    /// </summary>
    public void FullScreen()
    {
#if UNITY_2020_1_OR_NEWER
        // UnityWeb2021模板
        unityFullScreen();
        
#else
        // UnityWeb2019模板
        if (Screen.fullScreen)
        {
            Screen.fullScreen = false;
            return;
        }
        // 获取设置当前屏幕分辩率
        Resolution[] resolutions = Screen.resolutions;
        // 设置当前分辨率
        Screen.SetResolution(resolutions[resolutions.Length - 1].width, resolutions[resolutions.Length - 1].height, true);
        // 设置成全屏
        Screen.fullScreen = true;

        // 调用全屏选中方法
        WebGLFullScreen.Toggle();
#endif
    }
}
