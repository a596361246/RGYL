using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XQ
{
    public class DoorControl : MonoBehaviour
    {

        [Header("门设置")]
        [SerializeField] private float openAngle = 90f;
        [SerializeField] private float duration = 1f;

        private bool isOpen = false;
        private bool isRotating = false;

        [Header("交互设置")]
        [SerializeField] private float maxDistance = 10f;

        void Update()
        {
            if (Input.GetMouseButtonDown(0) && !isRotating)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, maxDistance))
                {
                    if (hit.collider.CompareTag("Door") || hit.collider.gameObject == gameObject)
                    {
                        StartCoroutine(ToggleDoorWithCoroutine());
                    }
                }
            }
        }

        IEnumerator ToggleDoorWithCoroutine()
        {
            isRotating = true;
            isOpen = !isOpen;

            float elapsedTime = 0f;
            Quaternion startRotation = transform.rotation;
            Quaternion endRotation;

            if (isOpen)
            {
                endRotation = Quaternion.Euler(0, openAngle, 0) * startRotation;
            }
            else
            {
                endRotation = Quaternion.Euler(0, -openAngle, 0) * startRotation;
            }

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float t = Mathf.Clamp01(elapsedTime / duration);

                // 使用缓动函数让旋转更自然
                t = Mathf.SmoothStep(0f, 1f, t);

                transform.rotation = Quaternion.Slerp(startRotation, endRotation, t);

                yield return null;
            }

            isRotating = false;
        }

    }
}