using System;
using UnityEngine;

namespace XQ
{
    /// <summary>
    /// 点击模式枚举
    /// </summary>
    public enum ClickMode
    {
        SingleClick,  // 单击模式
        DoubleClick   // 双击模式
    }

    /// <summary>
    /// 可点击物体接口
    /// </summary>
    public interface IClickable
    {
        void OnSingleClicked(RaycastHit hitInfo);
        void OnDoubleClicked(RaycastHit hitInfo);
    }

    /// <summary>
    /// 鼠标点击检测器 - 支持单击和双击检测
    /// </summary>
    public class ClickDetector : MonoBehaviour
    {
        [Header("点击模式设置")]
        [SerializeField] private ClickMode clickMode = ClickMode.DoubleClick;
        [SerializeField] private float doubleClickTime = 0.3f; // 双击间隔时间

        [Header("射线检测设置")]
        [SerializeField] private Camera mainCamera;
        [SerializeField] private float maxDistance = 100f; // 射线最大距离
        [SerializeField] private LayerMask targetLayer = -1; // 目标层级（-1表示所有层）
        [SerializeField] private string targetTag = ""; // 目标标签（空表示不限制）

        [Header("调试设置")]
        [SerializeField] private bool showDebugRay = false; // 是否显示射线
        [SerializeField] private Color debugRayColor = Color.red;

        // 回调事件
        public event Action<GameObject, RaycastHit> OnSingleClickCallback;
        public event Action<GameObject, RaycastHit> OnDoubleClickCallback;

        private float lastClickTime = 0f;
        private GameObject lastClickedObject = null;
        private bool waitingForDoubleClick = false;

        private void Start()
        {
            // 如果没有指定相机，使用主相机
            if (mainCamera == null)
            {
                mainCamera = Camera.main;
            }

            // 如果没有设置层级，默认所有层
            if (targetLayer.value == 0)
            {
                targetLayer = -1;
            }
        }

        private void Update()
        {
            // 检测鼠标左键点击
            if (Input.GetMouseButtonDown(0))
            {
                DetectClick();
            }

            // 双击超时检测
            if (waitingForDoubleClick && Time.time - lastClickTime > doubleClickTime)
            {
                // 超时，触发单击
                if (clickMode == ClickMode.DoubleClick && lastClickedObject != null)
                {
                    PerformRaycast(true); // 延迟触发的单击
                }
                waitingForDoubleClick = false;
            }
        }

        /// <summary>
        /// 检测点击
        /// </summary>
        private void DetectClick()
        {
            if (clickMode == ClickMode.SingleClick)
            {
                // 单击模式 - 直接触发
                PerformRaycast(false);
            }
            else if (clickMode == ClickMode.DoubleClick)
            {
                // 双击模式 - 需要判断
                DetectDoubleClick();
            }
        }

        /// <summary>
        /// 执行射线检测
        /// </summary>
        private void PerformRaycast(bool isDelayedSingleClick)
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // 显示调试射线
            if (showDebugRay)
            {
                Debug.DrawRay(ray.origin, ray.direction * maxDistance, debugRayColor, 1f);
            }

            // 射线检测
            if (Physics.Raycast(ray, out hit, maxDistance, targetLayer))
            {
                GameObject clickedObject = hit.collider.gameObject;

                // 检查标签过滤
                if (!string.IsNullOrEmpty(targetTag) && !clickedObject.CompareTag(targetTag))
                {
                    return;
                }

                // 触发单击回调
                if (isDelayedSingleClick)
                {
                    OnSingleClick(lastClickedObject, hit);
                }
                else
                {
                    OnSingleClick(clickedObject, hit);
                }
            }
        }

        /// <summary>
        /// 检测双击
        /// </summary>
        private void DetectDoubleClick()
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // 显示调试射线
            if (showDebugRay)
            {
                Debug.DrawRay(ray.origin, ray.direction * maxDistance, debugRayColor, 1f);
            }

            // 射线检测
            if (Physics.Raycast(ray, out hit, maxDistance, targetLayer))
            {
                GameObject clickedObject = hit.collider.gameObject;

                // 检查标签过滤
                if (!string.IsNullOrEmpty(targetTag) && !clickedObject.CompareTag(targetTag))
                {
                    return;
                }

                float timeSinceLastClick = Time.time - lastClickTime;

                // 判断是否为双击
                if (waitingForDoubleClick && clickedObject == lastClickedObject && timeSinceLastClick <= doubleClickTime)
                {
                    // 触发双击事件
                    OnDoubleClick(clickedObject, hit);

                    // 重置数据
                    lastClickedObject = null;
                    lastClickTime = 0f;
                    waitingForDoubleClick = false;
                }
                else
                {
                    // 记录第一次点击信息
                    lastClickedObject = clickedObject;
                    lastClickTime = Time.time;
                    waitingForDoubleClick = true;
                }
            }
        }

        /// <summary>
        /// 单击事件处理
        /// </summary>
        private void OnSingleClick(GameObject clickedObject, RaycastHit hitInfo)
        {
            Debug.Log($"[单击] 物体: {clickedObject.name}, 位置: {hitInfo.point}");

            // 触发回调
            OnSingleClickCallback?.Invoke(clickedObject, hitInfo);

            // 尝试调用物体上的接口
            IClickable clickable = clickedObject.GetComponent<IClickable>();
            clickable?.OnSingleClicked(hitInfo);
        }

        /// <summary>
        /// 双击事件处理
        /// </summary>
        private void OnDoubleClick(GameObject clickedObject, RaycastHit hitInfo)
        {
            Debug.Log($"[双击] 物体: {clickedObject.name}, 位置: {hitInfo.point}");

            // 触发回调
            OnDoubleClickCallback?.Invoke(clickedObject, hitInfo);

            // 尝试调用物体上的接口
            IClickable clickable = clickedObject.GetComponent<IClickable>();
            clickable?.OnDoubleClicked(hitInfo);
        }

        #region 公共接口

        /// <summary>
        /// 设置点击模式
        /// </summary>
        public void SetClickMode(ClickMode mode)
        {
            clickMode = mode;
            ResetClickState();
            Debug.Log($"点击模式已切换为: {mode}");
        }

        /// <summary>
        /// 设置射线检测距离
        /// </summary>
        public void SetMaxDistance(float distance)
        {
            maxDistance = Mathf.Max(0, distance);
        }

        /// <summary>
        /// 设置目标层级
        /// </summary>
        public void SetTargetLayer(LayerMask layer)
        {
            targetLayer = layer;
        }

        /// <summary>
        /// 设置目标标签
        /// </summary>
        public void SetTargetTag(string tag)
        {
            targetTag = tag;
        }

        /// <summary>
        /// 设置双击间隔时间
        /// </summary>
        public void SetDoubleClickTime(float time)
        {
            doubleClickTime = Mathf.Max(0.1f, time);
        }

        /// <summary>
        /// 重置点击状态
        /// </summary>
        public void ResetClickState()
        {
            lastClickTime = 0f;
            lastClickedObject = null;
            waitingForDoubleClick = false;
        }

        /// <summary>
        /// 获取当前点击模式
        /// </summary>
        public ClickMode GetCurrentClickMode()
        {
            return clickMode;
        }

        #endregion
    }

 
}
