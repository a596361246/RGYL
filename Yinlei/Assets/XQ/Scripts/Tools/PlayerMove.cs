using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XQ
{
    public class PlayerMove : MonoBehaviour
    {
        public float moveSpeed = 5.0f; // 移动速度
        public float rotationSpeed = 100.0f; // 旋转速度
        public float verticalSpeed = 3.0f; // 上升和下降的速度
        public float maxHeight = 2.0f; // 最大高度
        public float minHeight = 1.0f;  // 最小高度

        [Header("缩放设置")]
        [SerializeField] private float minFOV = 15f;        // 最小视角（最大缩放）
        [SerializeField] private float maxFOV = 90f;        // 最大视角（最小缩放）
        [SerializeField] private float zoomSpeed = 10f;     // 缩放速度
        [SerializeField] private float smoothTime = 0.2f;   // 平滑时间

        [Header("角度限制设置")]
        [SerializeField] private float maxLookUpAngle = 60f;    // 最大向上看角度
        [SerializeField] private float maxLookDownAngle = -60f; // 最大向下看角度

        [Header("地面检测设置")]
        public bool isCheckGround = false;
        [SerializeField] float groundCheckRadius = 0.2f;
        [SerializeField] Vector3 groundCheckOffset;
        [SerializeField] LayerMask groundLayer;
        float ySpeed;

        bool isGrunded;
        private float targetFOV;
        private float currentVelocity;

        // 用于跟踪当前的垂直旋转角度
        private float currentVerticalRotation = 0f;

        CharacterController characterController;

        Transform camTrf;
        Camera cam;

        Vector3 originPos;
        Quaternion originQuaternion;

        private void Awake()
        {
            characterController = GetComponent<CharacterController>();
            camTrf = transform.GetChild(0);
            cam = camTrf.GetComponent<Camera>();
        }

        private void Start()
        {
            originPos = transform.position;
            originQuaternion = transform.rotation;
            targetFOV = cam.fieldOfView;

            // 初始化当前垂直旋转角度
            currentVerticalRotation = camTrf.localEulerAngles.x;
            if (currentVerticalRotation > 180f)
                currentVerticalRotation -= 360f;
        }

        public void ResetPos()
        {
            transform.position = originPos;
            transform.rotation = originQuaternion;
            currentVerticalRotation = 0f;
        }

        private void FixedUpdate()
        {
            if (!UtilityScript.isCanMove)
            {
                return;
            }
            HandleMovement();
            HandleRotation();
            HandleZoomInput();
            SmoothZoom();
        }

        /// <summary>
        /// 处理鼠标滚轮输入
        /// </summary>
        private void HandleZoomInput()
        {
            float scrollInput = Input.GetAxis("Mouse ScrollWheel");

            if (Mathf.Abs(scrollInput) > 0.01f)
            {
                // 计算新的目标FOV
                targetFOV -= scrollInput * zoomSpeed;

                // 限制在最小和最大视角范围内
                targetFOV = Mathf.Clamp(targetFOV, minFOV, maxFOV);
            }
        }

        void GroundCheck()
        {
            isGrunded = Physics.CheckSphere(transform.TransformPoint(groundCheckOffset), groundCheckRadius, groundLayer);
        }

        /// <summary>
        /// 平滑缩放到目标FOV
        /// </summary>
        private void SmoothZoom()
        {
            if (Mathf.Abs(cam.fieldOfView - targetFOV) > 0.01f)
            {
                cam.fieldOfView = Mathf.SmoothDamp(
                    cam.fieldOfView,
                    targetFOV,
                    ref currentVelocity,
                    smoothTime
                );
            }
        }

        private void HandleMovement()
        {
            // 获取水平和垂直轴上的输入
            float moveHorizontal = Input.GetAxisRaw("Horizontal");
            float moveVertical = Input.GetAxisRaw("Vertical");

            // 获取摄像机的前方向和右方向
            Vector3 forward = transform.forward;
            Vector3 right = transform.right;

            // 忽略Y轴的影响
            forward.y = 0f;
            right.y = 0f;

            // 将方向向量归一化
            forward.Normalize();
            right.Normalize();

            // 计算水平移动向量
            Vector3 movement = (forward * moveVertical + right * moveHorizontal).normalized;

            // 当前的Y轴位置
            float currentY = transform.position.y;

            if (isCheckGround)
            {
                GroundCheck();

                if (isGrunded)
                {
                    ySpeed = -0.5f;
                }
                else
                {
                    ySpeed += Physics.gravity.y * Time.deltaTime;
                }
                movement.y += ySpeed;
            }

            characterController.Move(movement * moveSpeed * 0.5f * Time.deltaTime);
        }

        private void HandleRotation()
        {
            // 检查鼠标右键是否被按下
            if (Input.GetMouseButton(1))
            {
                // 获取鼠标的移动量
                float mouseX = Input.GetAxisRaw("Mouse X");
                float mouseY = -Input.GetAxisRaw("Mouse Y");

#if UNITY_WEBGL && !UNITY_EDITOR
                // 计算旋转角度
                float rotationX = mouseY * rotationSpeed * Time.deltaTime;
                float rotationY = mouseX * rotationSpeed * Time.deltaTime;
#else
                // 计算旋转角度
                float rotationX = mouseY * 2f * rotationSpeed * Time.deltaTime;
                float rotationY = mouseX * 2f * rotationSpeed * Time.deltaTime;
#endif

                // 更新垂直旋转角度并应用限制
                currentVerticalRotation += rotationX;
                currentVerticalRotation = Mathf.Clamp(currentVerticalRotation, maxLookDownAngle, maxLookUpAngle);

                // 应用垂直旋转到摄像机（仅使用限制后的角度）
                camTrf.localRotation = Quaternion.Euler(currentVerticalRotation, 0f, 0f);

                // 应用水平旋转到玩家对象
                Quaternion rotationYQuat = Quaternion.AngleAxis(rotationY, Vector3.up);
                transform.rotation = rotationYQuat * transform.rotation;
            }
        }
    }
}
