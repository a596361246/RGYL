using System;
using UnityEngine;
namespace XQ
{
    /// <summary>
    /// 鼠标抓取和释放物体
    /// </summary>
    public class MouseGrabSystem : MonoBehaviour
    {
        [Header("射线检测设置")]
        [Tooltip("用于检测可抓取物体的层级")]
        public LayerMask grabLayer;
        //int grabLayerIndex= LayerMask.NameToLayer(grabbableLayer);
        int grabLayerMask;

        [Tooltip("用于检测可抓取物体的标签")]
        public string grabTag = "Grab";

        [Tooltip("用于检测放置目标的层级")]
        public LayerMask dropLayer;
        int dropLayerMask;

        [Tooltip("用于检测放置目标的标签")]
        public string dropTag = "Drop";

        [Header("抓取设置")]
        [Tooltip("物体跟随鼠标的距离")]
        public float grabDistance = 5f;

        [Tooltip("物体跟随的平滑度")]
        public float followSpeed = 10f;

        [Tooltip("是否使用标签检测而不是层级检测")]
        public bool useTagInsteadOfLayer = false;

        [Header("事件")]
        [Tooltip("成功放置物体时触发")]
        public Action<GameObject, GameObject> OnSuccessfulDrop;

        [Tooltip("物体回到原位置时触发")]
        public Action<GameObject> OnObjectReturned;

        // 私有变量
        private Camera mainCamera;
        Ray ray;
        RaycastHit hit;
        private GameObject grabbedObject;
        private Vector3 originalPosition;
        private Quaternion originalRotation;
        private bool isGrabbing = false;
        private Rigidbody grabbedRigidbody;
        private bool wasKinematic;

        void Start()
        {
            mainCamera = Camera.main;
            if (mainCamera == null)
            {
                mainCamera = FindObjectOfType<Camera>();
            }

            if (mainCamera == null)
            {
                Debug.LogError("未找到相机！请确保场景中有Camera组件。");
            }

            //grabLayerMask = 1 << LayerMask.NameToLayer(grableLayer);
            //dropLayerMask = 1 << LayerMask.NameToLayer(dropLayer);
        }

        void Update()
        {
            float v = Input.GetAxisRaw("Vertical");
            grabDistance += v * Time.deltaTime * 0.1f;

            if (Input.GetMouseButtonDown(0))
            {
                HandleMouseClick();
            }

            if (isGrabbing && grabbedObject != null)
            {
                MoveGrabbedObject();
            }
        }

        void HandleMouseClick()
        {
            ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            if (!isGrabbing)
            {
                // 第一次点击：尝试抓取物体
                if (Physics.Raycast(ray, out hit, 10f, grabLayer))
                {
                    GameObject hitObject = hit.collider.gameObject;
                    if (IsGrabbableObject(hitObject))
                    {
                        if (hitObject.CompareTag(grabTag))
                        {
                            GrabObject(hitObject);
                        }

                    }
                    //Debug.Log("抓：" + hitObject.name);
                }
            }
            else
            {
                // 第二次点击：尝试放置物体
                if (Physics.Raycast(ray, out hit, 10f, dropLayer))
                {
                    GameObject hitObject = hit.collider.gameObject;

                    if (IsDropTarget(hitObject))
                    {
                        if (hitObject.CompareTag(dropTag))
                        {
                            DropObject(hitObject);
                        }

                    }
                    else
                    {
                        ReturnObjectToOriginalPosition();
                    }
                    //Debug.Log($"放：{hitObject.name} {hitObject.layer} {dropLayer}");
                }
                else
                {
                    ReturnObjectToOriginalPosition();
                }
            }
        }

        bool IsGrabbableObject(GameObject obj)
        {
            return obj.CompareTag(grabTag);
            //if (useTagInsteadOfLayer)
            //{

            //}
            //else
            //{
            //    return ((1 << obj.layer) & grabbableLayer) != 0;
            //}

            //int objLayer = 1 << obj.layer;
            //Debug.Log((objLayer & grabbableLayer.value) > 0);
            //if (obj.CompareTag(grabbableTag) && (objLayer & grabbableLayer) > 0)
            //{
            //    Debug.Log("grabbe true");
            //    return true;
            //}
            //else
            //{
            //    Debug.Log("grabbe false");
            //    return false;
            //}
        }

        bool IsDropTarget(GameObject obj)
        {
            return obj.CompareTag(dropTag);
            //if (useTagInsteadOfLayer)
            //{

            //}
            //else
            //{
            //    return ((1 << obj.layer) & dropLayer) != 0;
            //}
            //int objLayer = 1 << obj.layer;
            //if (obj.CompareTag(dropTag) && (objLayer & dropLayer) > 0)
            //{
            //    Debug.Log("drop true");
            //    return true;
            //}
            //else
            //{
            //    Debug.Log("drop false");
            //    return false;
            //}
        }

        void GrabObject(GameObject obj)
        {
            grabbedObject = obj;
            originalPosition = obj.transform.position;
            originalRotation = obj.transform.rotation;
            isGrabbing = true;

            // 如果物体有Rigidbody，设置为kinematic避免物理干扰
            grabbedRigidbody = obj.GetComponent<Rigidbody>();
            if (grabbedRigidbody != null)
            {
                wasKinematic = grabbedRigidbody.isKinematic;
                grabbedRigidbody.isKinematic = true;
            }

            //Debug.Log($"抓取了物体: {obj.name}");
        }

        void MoveGrabbedObject()
        {
            if (grabbedObject == null) return;

            // 计算鼠标在世界坐标中的位置
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = grabDistance;
            Vector3 worldPos = mainCamera.ScreenToWorldPoint(mousePos);

            // 平滑移动物体
            grabbedObject.transform.position = Vector3.Lerp(
                grabbedObject.transform.position,
                worldPos,
                followSpeed * Time.deltaTime
            );
        }

        void DropObject(GameObject dropTarget)
        {
            Debug.Log($"成功将 {grabbedObject.name} 放置到 {dropTarget.name}");

            // 恢复Rigidbody设置
            if (grabbedRigidbody != null)
            {
                grabbedRigidbody.isKinematic = wasKinematic;
            }

            // 触发成功放置事件
            OnSuccessfulDrop?.Invoke(grabbedObject, dropTarget);

            // 重置状态
            ResetGrabState();
        }

        void ReturnObjectToOriginalPosition()
        {
            if (grabbedObject != null)
            {
                Debug.Log($"物体 {grabbedObject.name} 回到原位置");

                // 将物体移回原位置
                grabbedObject.transform.position = originalPosition;
                grabbedObject.transform.rotation = originalRotation;

                // 恢复Rigidbody设置
                if (grabbedRigidbody != null)
                {
                    grabbedRigidbody.isKinematic = wasKinematic;
                }

                // 触发回到原位事件
                OnObjectReturned?.Invoke(grabbedObject);
            }

            // 重置状态
            ResetGrabState();
        }

        void ResetGrabState()
        {
            grabbedObject = null;
            grabbedRigidbody = null;
            isGrabbing = false;
            wasKinematic = false;
        }

        // 在编辑器中显示调试信息
        void OnDrawGizmosSelected()
        {
            if (isGrabbing && grabbedObject != null)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(originalPosition, 0.5f);
                Gizmos.color = Color.red;
                Gizmos.DrawLine(originalPosition, grabbedObject.transform.position);
            }
        }
    }
}