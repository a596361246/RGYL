using TMPro;

using UnityEngine;
using UnityEngine.UI;

namespace XQ
{
    /// <summary>
    /// UI跟随3D物体脚本 - 针对Canvas是Camera渲染模式
    /// </summary>
    public class UIFollowTarget : MonoBehaviour
    {
        [Header("跟随设置")]
        [SerializeField] private Transform target;           // 要跟随的3D物体
        [SerializeField] private Vector3 worldOffset = Vector3.zero;  // 世界空间偏移
        [SerializeField] private Vector2 screenOffset = Vector2.zero;    // 屏幕空间偏移

        [Header("显示控制")]
        [SerializeField] private float maxDistance = 50f;    // 最大显示距离
        [SerializeField] private bool hideWhenBehind = true; // 在物体后面时是否隐藏
        [SerializeField] private bool hideWhenTooFar = true; // 距离过远时是否隐藏

        [Header("平滑设置")]
        [SerializeField] private bool useSmooth = true;      // 是否使用平滑移动
        [SerializeField] private float smoothSpeed = 10f;    // 平滑速度

        private Camera mainCamera;
        private Camera canvasCamera;  // Canvas使用的摄像机
        private RectTransform rectTransform;
        private CanvasGroup canvasGroup;
        private Canvas parentCanvas;
        private Vector3 targetScreenPos;

        TMP_Text nameText;

        private void Awake()
        {
            FollowObj followObj = GetComponent<FollowObj>();
            target = followObj != null ? followObj.target : target;
            worldOffset = followObj != null ? followObj.offset : worldOffset;
            rectTransform = GetComponent<RectTransform>();
            canvasGroup = GetComponent<CanvasGroup>();
            nameText = GetComponentInChildren<TMP_Text>();

            // 如果没有CanvasGroup组件，自动添加一个
            if (canvasGroup == null)
            {
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }

            // 获取父Canvas
            parentCanvas = GetComponentInParent<Canvas>();
        }

        private void Start()
        {
            // 获取主摄像机
            if (mainCamera == null)
            {
                mainCamera = Camera.main;
                if (mainCamera == null)
                {
                    mainCamera = FindObjectOfType<Camera>();
                }
            }

            // 获取Canvas使用的摄像机
            if (parentCanvas != null && parentCanvas.renderMode == RenderMode.ScreenSpaceCamera)
            {
                canvasCamera = parentCanvas.worldCamera;
            }
        }

        private void LateUpdate()
        {
            if (target == null || mainCamera == null || parentCanvas == null) return;

            UpdateUIPosition();
            UpdateVisibility();
        }

        /// <summary>
        /// 更新UI位置
        /// </summary>
        private void UpdateUIPosition()
        {
            // 计算目标世界位置（加上偏移）
            Vector3 targetWorldPos = target.position + worldOffset;

            // 转换到屏幕坐标
            Vector3 screenPos = mainCamera.WorldToScreenPoint(targetWorldPos);

            Vector2 finalPos;

            // 针对Camera渲染模式的特殊处理
            if (parentCanvas.renderMode == RenderMode.ScreenSpaceCamera && canvasCamera != null)
            {
                // 将屏幕坐标转换为Canvas坐标
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    parentCanvas.transform as RectTransform,
                    screenPos,
                    canvasCamera,
                    out finalPos
                );

                // 添加屏幕空间偏移
                finalPos += screenOffset;
            }
            else
            {
                // 直接使用屏幕坐标（适用于Overlay模式）
                finalPos = screenPos;
                finalPos += screenOffset;
            }

            targetScreenPos = new Vector3(finalPos.x, finalPos.y, screenPos.z);

            // 平滑移动或直接设置位置
            if (useSmooth)
            {
                Vector3 currentPos = rectTransform.anchoredPosition;
                Vector2 targetPos = new Vector2(targetScreenPos.x, targetScreenPos.y);
                rectTransform.anchoredPosition = Vector2.Lerp(currentPos, targetPos,
                    smoothSpeed * Time.deltaTime);
            }
            else
            {
                rectTransform.anchoredPosition = new Vector2(targetScreenPos.x, targetScreenPos.y);
            }
        }

        /// <summary>
        /// 更新可见性
        /// </summary>
        private void UpdateVisibility()
        {
            bool shouldShow = true;

            // 检查是否在摄像机后面
            if (hideWhenBehind && targetScreenPos.z < 0)
            {
                shouldShow = false;
            }

            // 检查距离
            if (hideWhenTooFar && shouldShow)
            {
                float distance = Vector3.Distance(mainCamera.transform.position, target.position);
                if (distance > maxDistance)
                {
                    shouldShow = false;
                }
            }

            // 检查是否在屏幕范围内
            if (shouldShow && parentCanvas != null)
            {
                RectTransform canvasRect = parentCanvas.transform as RectTransform;
                Vector2 canvasSize = canvasRect.sizeDelta;

                if (Mathf.Abs(targetScreenPos.x) > canvasSize.x * 0.6f ||
                    Mathf.Abs(targetScreenPos.y) > canvasSize.y * 0.6f)
                {
                    // 可选：在屏幕边缘外时隐藏或保持显示
                    // shouldShow = false;
                }
            }

            // 设置透明度
            float targetAlpha = shouldShow ? 1f : 0f;
            if (useSmooth)
            {
                canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, targetAlpha,
                    smoothSpeed * Time.deltaTime);
            }
            else
            {
                canvasGroup.alpha = targetAlpha;
            }
        }

        /// <summary>
        /// 强制更新位置（不使用平滑）
        /// </summary>
        public void ForceUpdatePosition()
        {
            if (target == null || mainCamera == null || parentCanvas == null) return;

            Vector3 targetWorldPos = target.position + worldOffset;
            Vector3 screenPos = mainCamera.WorldToScreenPoint(targetWorldPos);

            Vector2 finalPos;

            if (parentCanvas.renderMode == RenderMode.ScreenSpaceCamera && canvasCamera != null)
            {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    parentCanvas.transform as RectTransform,
                    screenPos,
                    canvasCamera,
                    out finalPos
                );
            }
            else
            {
                finalPos = screenPos;
            }

            finalPos += screenOffset;
            rectTransform.anchoredPosition = finalPos;
        }

        // 其他方法保持不变...
        public void SetTarget(Transform newTarget) { target = newTarget; nameText.text = newTarget == null ? null : newTarget.name; canvasGroup.alpha = newTarget == null ? 0f : 1f; }
        public void SetWorldOffset(Vector3 offset) { worldOffset = offset; }
        public void SetScreenOffset(Vector2 offset) { screenOffset = offset; }
        public void SetMaxDistance(float distance) { maxDistance = distance; }
    }
}
