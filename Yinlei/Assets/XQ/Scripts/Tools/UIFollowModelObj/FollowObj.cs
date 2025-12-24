using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace XQ
{
    /// <summary>
    /// Canvas是Overlay用这个
    /// </summary>
    public class FollowObj : MonoBehaviour
    {
        public Transform target; // 需要跟随的3D物体
        public Vector3 offset;   // UI元素相对于目标物体的位置偏移

        private RectTransform rect; // UI元素的RectTransform
        private Camera mainCamera;       // 主摄像机

        void Start()
        {
            // 获取UI元素的RectTransform
            rect = GetComponent<RectTransform>();
            if (target == null)
            {
                target = GameObject.Find("Obj/" + gameObject.name).transform;
            }

            // 获取主摄像机
            mainCamera = Camera.main;
        }

        public void SetTarget(Transform target)
        {
            this.target = target;
        }


        void Update()
        {
            if (target != null && rect != null && mainCamera != null)
            {
                // 将世界坐标转化为屏幕坐标
                Vector3 screenPosition = mainCamera.WorldToScreenPoint(target.position + offset);

                // 将屏幕坐标设置为UI的定位
                rect.position = screenPosition;
            }
        }
    }
}

