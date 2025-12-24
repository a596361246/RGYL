using UnityEngine;

public class GengSui : MonoBehaviour
{
    [Header("跟随设置")]
    [Tooltip("要跟随的目标")]
    public Transform target;

    [Tooltip("跟随平滑度 (0 = 无平滑)")]
    [Range(0f, 1f)]
    public float smoothness = 0.1f;

    [Tooltip("位置偏移")]
    public Vector3 offset = Vector3.zero;

    [Header("轴限制")]
    public bool followX = true;
    public bool followY = true;
    public bool followZ = true;

    private Vector3 currentVelocity;

    void Start()
    {
        // 如果没有设置目标，尝试自动查找
        if (target == null)
        {
            // 先尝试按标签查找
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                target = player.transform;
            }
            else
            {
                // 尝试按名称查找
                GameObject found = GameObject.Find("Target");
                if (found != null) target = found.transform;
            }

            if (target != null)
            {
                Debug.Log("自动找到目标: " + target.name);
                // 自动计算初始偏移
                if (offset == Vector3.zero)
                {
                    offset = transform.position - target.position;
                }
            }
        }
    }

    void Update()
    {
        if (target == null) return;

        FollowTarget();
    }

    void FollowTarget()
    {
        // 计算目标位置
        Vector3 targetPosition = target.position + offset;

        // 应用轴限制
        Vector3 currentPos = transform.position;
        if (!followX) targetPosition.x = currentPos.x;
        if (!followY) targetPosition.y = currentPos.y;
        if (!followZ) targetPosition.z = currentPos.z;

        // 平滑移动
        if (smoothness > 0)
        {
            transform.position = Vector3.SmoothDamp(
                currentPos,
                targetPosition,
                ref currentVelocity,
                smoothness
            );
        }
        else
        {
            transform.position = targetPosition;
        }
    }

    // 公共方法
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    public void SnapToTarget()
    {
        if (target == null) return;
        transform.position = target.position + offset;
    }
}