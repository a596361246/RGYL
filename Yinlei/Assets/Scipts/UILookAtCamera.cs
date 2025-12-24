using UnityEngine;

namespace XQ
{
    public class UILookAtCamera : MonoBehaviour
    {
        // 你可以在这里指定一个相机，如果为空则使用主相机
        [SerializeField] private Camera targetCamera;

        private void Start()
        {
            // 如果没有手动指定相机，就使用主相机
            if (targetCamera == null)
            {
                targetCamera = Camera.main;
            }
        }

        private void LateUpdate()
        {
            // 关键：确保UI朝向相机，并正确设置其上方方向
            transform.LookAt(transform.position + targetCamera.transform.rotation * Vector3.forward,
                             targetCamera.transform.rotation * Vector3.up);
        }
    }
}
