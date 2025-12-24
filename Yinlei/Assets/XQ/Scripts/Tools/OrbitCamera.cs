using UnityEngine;

namespace XQ
{
    /// <summary>
    /// 相机环绕观察控制器
    /// 支持鼠标拖拽旋转、滚轮缩放、目标自身旋转等多种功能
    /// </summary>
    public class OrbitCamera : MonoBehaviour
    {
        [Header("目标设置")]
        [Tooltip("相机围绕的目标点")]
        public Transform target;

        [Tooltip("目标点偏移")]
        public Vector3 targetOffset = Vector3.zero;

        [Header("距离设置")]
        [Tooltip("初始距离")]
        public float distance = 10f;

        [Tooltip("最小距离")]
        public float minDistance = 2f;

        [Tooltip("最大距离")]
        public float maxDistance = 20f;

        [Tooltip("滚轮缩放速度")]
        public float zoomSpeed = 2f;

        [Tooltip("距离变化是否使用阻尼")]
        public bool useDistanceDamping = true;

        [Tooltip("距离阻尼系数")]
        [Range(1f, 20f)]
        public float distanceDampingFactor = 10f;

        [Header("相机旋转设置")]
        [Tooltip("鼠标灵敏度")]
        public float mouseSensitivity = 3f;

        [Tooltip("是否反向X轴（水平旋转）")]
        public bool invertX = false;

        [Tooltip("是否反向Y轴（垂直旋转）")]
        public bool invertY = false;

        [Tooltip("旋转是否使用阻尼")]
        public bool useRotationDamping = true;

        [Tooltip("旋转阻尼系数")]
        [Range(1f, 20f)]
        public float rotationDampingFactor = 10f;

        [Header("相机角度限制")]
        [Tooltip("是否限制Y轴旋转角度（俯仰角）")]
        public bool limitYRotation = true;

        [Tooltip("最小俯仰角")]
        [Range(-89f, 10f)]
        public float minYAngle = -80f;

        [Tooltip("最大俯仰角")]
        [Range(10f, 89f)]
        public float maxYAngle = 80f;

        [Tooltip("是否限制X轴旋转角度（水平角）")]
        public bool limitXRotation = false;

        [Tooltip("最小水平角")]
        [Range(-180f, 0f)]
        public float minXAngle = -180f;

        [Tooltip("最大水平角")]
        [Range(0f, 180f)]
        public float maxXAngle = 180f;

        [Header("目标自身旋转设置")]
        [Tooltip("是否启用目标自身旋转")]
        public bool enableTargetRotation = true;

        [Tooltip("用于旋转目标的鼠标按键")]
        public MouseButton targetRotationButton = MouseButton.Left;

        [Tooltip("目标旋转灵敏度")]
        public float targetRotationSensitivity = 3f;

        [Tooltip("目标旋转是否反向X轴")]
        public bool invertTargetX = false;

        [Tooltip("目标旋转是否反向Y轴")]
        public bool invertTargetY = false;

        [Tooltip("目标旋转是否使用阻尼")]
        public bool useTargetRotationDamping = true;

        [Tooltip("目标旋转阻尼系数")]
        [Range(1f, 20f)]
        public float targetRotationDampingFactor = 8f;

        [Header("目标旋转角度限制")]
        [Tooltip("是否限制目标Y轴旋转（俯仰）")]
        public bool limitTargetYRotation = true;

        [Tooltip("目标最小俯仰角")]
        [Range(-89f, 0f)]
        public float minTargetYAngle = -45f;

        [Tooltip("目标最大俯仰角")]
        [Range(0f, 89f)]
        public float maxTargetYAngle = 45f;

        [Tooltip("是否限制目标X轴旋转（水平）")]
        public bool limitTargetXRotation = false;

        [Tooltip("目标最小水平角")]
        [Range(-180f, 0f)]
        public float minTargetXAngle = -180f;

        [Tooltip("目标最大水平角")]
        [Range(0f, 180f)]
        public float maxTargetXAngle = 180f;

        [Header("输入设置")]
        [Tooltip("用于旋转相机的鼠标按键")]
        public MouseButton cameraRotationButton = MouseButton.Right;

        [Tooltip("是否启用触摸控制")]
        public bool enableTouchControl = true;

        [Tooltip("键盘旋转速度")]
        public float keyboardRotationSpeed = 50f;

        [Tooltip("是否启用键盘控制")]
        public bool enableKeyboardControl = false;

        [Header("自动旋转")]
        [Tooltip("是否启用自动旋转")]
        public bool autoRotate = false;

        [Tooltip("自动旋转速度")]
        public float autoRotateSpeed = 10f;

        [Header("碰撞检测")]
        [Tooltip("是否启用碰撞检测")]
        public bool enableCollision = true;

        [Tooltip("碰撞检测层")]
        public LayerMask collisionLayers = -1;

        [Tooltip("相机半径（用于碰撞检测）")]
        public float cameraRadius = 0.2f;

        // 相机旋转私有变量
        public float currentDistance;
        private float targetDistance;
        private float currentX = 0f;
        private float currentY = 0f;
        private float targetX = 0f;
        private float targetY = 0f;
        private Vector3 currentTargetPosition;

        // 目标旋转私有变量
        private float currentTargetRotX = 0f;
        private float currentTargetRotY = 0f;
        private float targetRotX = 0f;
        private float targetRotY = 0f;
        private Quaternion initialTargetRotation;
        private bool hasInitializedTargetRotation = false;

        // 触摸控制
        private Vector2 touchStartPos;
        private bool isTouching = false;

        public enum MouseButton
        {
            Left = 0,
            Right = 1,
            Middle = 2
        }

        private void Start()
        {
            // 初始化
            if (target == null)
            {
                Debug.LogWarning("OrbitCamera: 未设置目标点，部分功能将无法使用");
            }

            currentDistance = distance;
            targetDistance = distance;

            // 从当前旋转初始化相机角度
            Vector3 angles = transform.eulerAngles;
            currentX = angles.y;
            currentY = angles.x;
            targetX = currentX;
            targetY = currentY;

            currentTargetPosition = GetTargetPosition();

            // 初始化目标旋转
            if (target != null)
            {
                initialTargetRotation = target.rotation;
                Vector3 targetAngles = target.eulerAngles;
                currentTargetRotX = targetAngles.y;
                currentTargetRotY = targetAngles.x;
                targetRotX = currentTargetRotX;
                targetRotY = currentTargetRotY;
                hasInitializedTargetRotation = true;
            }
        }

        private void LateUpdate()
        {
            HandleInput();
            UpdateCamera();
            UpdateTargetRotation();
        }

        /// <summary>
        /// 处理输入
        /// </summary>
        private void HandleInput()
        {
            // 相机旋转控制（右键或自定义按键）
            if (Input.GetMouseButton((int)cameraRotationButton))
            {
                float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
                float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

                if (invertX) mouseX = -mouseX;
                if (invertY) mouseY = -mouseY;

                targetX += mouseX;
                targetY -= mouseY;

                ApplyCameraAngleLimits();
            }

            // 目标自身旋转控制（左键或自定义按键）
            if (enableTargetRotation && target != null && Input.GetMouseButton((int)targetRotationButton))
            {
                float mouseX = Input.GetAxis("Mouse X") * targetRotationSensitivity;
                float mouseY = Input.GetAxis("Mouse Y") * targetRotationSensitivity;

                if (invertTargetX) mouseX = -mouseX;
                if (invertTargetY) mouseY = -mouseY;

                targetRotX += mouseX;
                targetRotY -= mouseY;

                ApplyTargetAngleLimits();
            }

            // 触摸控制
            if (enableTouchControl && Input.touchCount == 1)
            {
                Touch touch = Input.GetTouch(0);

                if (touch.phase == TouchPhase.Began)
                {
                    touchStartPos = touch.position;
                    isTouching = true;
                }
                else if (touch.phase == TouchPhase.Moved && isTouching)
                {
                    Vector2 delta = touch.deltaPosition;
                    float touchX = delta.x * mouseSensitivity * 0.1f;
                    float touchY = delta.y * mouseSensitivity * 0.1f;

                    if (invertX) touchX = -touchX;
                    if (invertY) touchY = -touchY;

                    targetX += touchX;
                    targetY -= touchY;

                    ApplyCameraAngleLimits();
                }
                else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                {
                    isTouching = false;
                }
            }

            // 键盘控制
            if (enableKeyboardControl)
            {
                float horizontal = Input.GetAxis("Horizontal");
                float vertical = Input.GetAxis("Vertical");

                targetX += horizontal * keyboardRotationSpeed * Time.deltaTime;
                targetY -= vertical * keyboardRotationSpeed * Time.deltaTime;

                ApplyCameraAngleLimits();
            }

            // 自动旋转
            if (autoRotate && !Input.GetMouseButton((int)cameraRotationButton) && !isTouching)
            {
                targetX += autoRotateSpeed * Time.deltaTime;
                if (limitXRotation)
                {
                    targetX = Mathf.Clamp(targetX, minXAngle, maxXAngle);
                }
            }

            // 滚轮缩放
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll != 0f)
            {
                targetDistance -= scroll * zoomSpeed;
                targetDistance = Mathf.Clamp(targetDistance, minDistance, maxDistance);
            }

            // 触摸缩放（双指）
            if (enableTouchControl && Input.touchCount == 2)
            {
                Touch touch1 = Input.GetTouch(0);
                Touch touch2 = Input.GetTouch(1);

                Vector2 touch1PrevPos = touch1.position - touch1.deltaPosition;
                Vector2 touch2PrevPos = touch2.position - touch2.deltaPosition;

                float prevMagnitude = (touch1PrevPos - touch2PrevPos).magnitude;
                float currentMagnitude = (touch1.position - touch2.position).magnitude;

                float difference = prevMagnitude - currentMagnitude;

                targetDistance += difference * 0.01f;
                targetDistance = Mathf.Clamp(targetDistance, minDistance, maxDistance);
            }
        }

        /// <summary>
        /// 应用相机角度限制
        /// </summary>
        private void ApplyCameraAngleLimits()
        {
            if (limitYRotation)
            {
                targetY = Mathf.Clamp(targetY, minYAngle, maxYAngle);
            }

            if (limitXRotation)
            {
                targetX = Mathf.Clamp(targetX, minXAngle, maxXAngle);
            }
        }

        /// <summary>
        /// 应用目标角度限制
        /// </summary>
        private void ApplyTargetAngleLimits()
        {
            if (limitTargetYRotation)
            {
                targetRotY = Mathf.Clamp(targetRotY, minTargetYAngle, maxTargetYAngle);
            }

            if (limitTargetXRotation)
            {
                targetRotX = Mathf.Clamp(targetRotX, minTargetXAngle, maxTargetXAngle);
            }
        }

        /// <summary>
        /// 更新相机位置和旋转
        /// </summary>
        private void UpdateCamera()
        {
            // 应用阻尼
            if (useRotationDamping)
            {
                currentX = Mathf.Lerp(currentX, targetX, Time.deltaTime * rotationDampingFactor);
                currentY = Mathf.Lerp(currentY, targetY, Time.deltaTime * rotationDampingFactor);
            }
            else
            {
                currentX = targetX;
                currentY = targetY;
            }

            if (useDistanceDamping)
            {
                currentDistance = Mathf.Lerp(currentDistance, targetDistance, Time.deltaTime * distanceDampingFactor);
            }
            else
            {
                currentDistance = targetDistance;
            }

            // 计算旋转
            Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);

            // 更新目标位置（支持平滑移动）
            Vector3 targetPos = GetTargetPosition();
            currentTargetPosition = Vector3.Lerp(currentTargetPosition, targetPos, Time.deltaTime * 10f);

            // 计算理想位置
            Vector3 direction = rotation * Vector3.back;
            Vector3 desiredPosition = currentTargetPosition + direction * currentDistance;

            // 碰撞检测
            if (enableCollision)
            {
                RaycastHit hit;
                if (Physics.Linecast(currentTargetPosition, desiredPosition, out hit, collisionLayers))
                {
                    desiredPosition = hit.point + hit.normal * cameraRadius;
                }
            }

            // 应用位置和旋转
            transform.position = desiredPosition;
            transform.rotation = rotation;
        }

        /// <summary>
        /// 更新目标自身旋转
        /// </summary>
        private void UpdateTargetRotation()
        {
            if (!enableTargetRotation || target == null || !hasInitializedTargetRotation)
                return;

            // 应用阻尼
            if (useTargetRotationDamping)
            {
                currentTargetRotX = Mathf.Lerp(currentTargetRotX, targetRotX, Time.deltaTime * targetRotationDampingFactor);
                currentTargetRotY = Mathf.Lerp(currentTargetRotY, targetRotY, Time.deltaTime * targetRotationDampingFactor);
            }
            else
            {
                currentTargetRotX = targetRotX;
                currentTargetRotY = targetRotY;
            }

            // 计算目标的新旋转（基于初始旋转）
            Quaternion rotationDelta = Quaternion.Euler(currentTargetRotY, currentTargetRotX, 0);
            target.rotation = initialTargetRotation * rotationDelta;
        }

        /// <summary>
        /// 获取目标位置
        /// </summary>
        private Vector3 GetTargetPosition()
        {
            if (target != null)
            {
                return target.position + targetOffset;
            }
            return targetOffset;
        }

        /// <summary>
        /// 设置目标点
        /// </summary>
        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
            currentTargetPosition = GetTargetPosition();

            if (target != null)
            {
                initialTargetRotation = target.rotation;
                Vector3 targetAngles = target.eulerAngles;
                currentTargetRotX = targetAngles.y;
                currentTargetRotY = targetAngles.x;
                targetRotX = currentTargetRotX;
                targetRotY = currentTargetRotY;
                hasInitializedTargetRotation = true;
            }
        }

        /// <summary>
        /// 重置相机到初始角度
        /// </summary>
        public void ResetCamera()
        {
            targetX = 0f;
            targetY = 0f;
            targetDistance = distance;
        }

        /// <summary>
        /// 重置目标旋转到初始状态
        /// </summary>
        public void ResetTargetRotation()
        {
            if (target != null && hasInitializedTargetRotation)
            {
                targetRotX = 0f;
                targetRotY = 0f;
                currentTargetRotX = 0f;
                currentTargetRotY = 0f;
                target.rotation = initialTargetRotation;
            }
        }

        /// <summary>
        /// 重置所有（相机和目标）
        /// </summary>
        public void ResetAll()
        {
            ResetCamera();
            ResetTargetRotation();
        }

        /// <summary>
        /// 聚焦到目标（带动画）
        /// </summary>
        public void FocusOnTarget(Vector3 newTargetOffset, float newDistance = -1f)
        {
            targetOffset = newTargetOffset;
            if (newDistance > 0)
            {
                targetDistance = Mathf.Clamp(newDistance, minDistance, maxDistance);
            }
        }

        // Gizmos绘制
        private void OnDrawGizmosSelected()
        {
            if (target != null || targetOffset != Vector3.zero)
            {
                Vector3 targetPos = GetTargetPosition();
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(targetPos, 0.3f);

                if (Application.isPlaying)
                {
                    Gizmos.color = Color.cyan;
                    Gizmos.DrawLine(targetPos, transform.position);
                    Gizmos.DrawWireSphere(transform.position, cameraRadius);
                }
            }
        }
    }
}
