using UnityEngine;
using UnityEngine.UI;

namespace XQ
{
    /// <summary>
    /// 图片自动缩小或放大到某个尺寸，限制尺寸大小，按等比例缩放
    /// </summary>
    public class ImageAutoSizeMatcher : MonoBehaviour
    {
        [Header("目标尺寸设置")]
        public float targetWidth = 1600f;
        public float targetHeight = 900f;

        [Header("组件引用")]
        [SerializeField] private Image targetImage;

        private RectTransform imageRectTransform;

        private void Awake()
        {
            // 如果没有手动指定Image组件，则尝试获取当前GameObject上的Image组件
            if (targetImage == null)
            {
                targetImage = GetComponent<Image>();
            }

            if (targetImage != null)
            {
                imageRectTransform = targetImage.GetComponent<RectTransform>();
            }
            else
            {
                Debug.LogError("ImageAutoSizeMatcher: 未找到Image组件！");
            }
        }

        /// <summary>
        /// 设置新的Sprite并自动调整尺寸
        /// </summary>
        /// <param name="newSprite">要设置的新Sprite</param>
        public void SetSpriteAndAutoResize(Sprite newSprite)
        {
            if (targetImage == null || imageRectTransform == null)
            {
                Debug.LogError("ImageAutoSizeMatcher: Image组件或RectTransform为空！");
                return;
            }

            if (newSprite == null)
            {
                Debug.LogWarning("ImageAutoSizeMatcher: 传入的Sprite为空！");
                return;
            }

            // 设置新的Sprite
            targetImage.sprite = newSprite;

            // 自动调整尺寸
            AutoResizeImage(newSprite);
        }

        /// <summary>
        /// 根据当前Image的Sprite自动调整尺寸
        /// </summary>
        public void AutoResizeCurrentSprite()
        {
            if (targetImage == null || targetImage.sprite == null)
            {
                Debug.LogWarning("ImageAutoSizeMatcher: Image或Sprite为空，无法自动调整尺寸！");
                return;
            }

            AutoResizeImage(targetImage.sprite);
        }

        /// <summary>
        /// 核心的尺寸调整逻辑
        /// </summary>
        /// <param name="sprite">要调整的Sprite</param>
        private void AutoResizeImage(Sprite sprite)
        {
            if (sprite == null || imageRectTransform == null) return;

            // 获取Sprite的原始尺寸
            float spriteWidth = sprite.rect.width;
            float spriteHeight = sprite.rect.height;

            // 计算缩放比例
            float scXQ = targetWidth / spriteWidth;
            float scaleY = targetHeight / spriteHeight;

            // 选择较小的缩放比例，确保图片完全适配在目标尺寸内
            float finalScale = Mathf.Min(scXQ, scaleY);

            // 计算最终尺寸
            float finalWidth = spriteWidth * finalScale;
            float finalHeight = spriteHeight * finalScale;

            // 应用新的尺寸
            imageRectTransform.sizeDelta = new Vector2(finalWidth, finalHeight);

            // 输出调试信息
            //Debug.Log($"ImageAutoSizeMatcher: 原始尺寸({spriteWidth}x{spriteHeight}) -> 最终尺寸({finalWidth:F1}x{finalHeight:F1}), 缩放比例: {finalScale:F3}");
        }

        /// <summary>
        /// 手动设置目标尺寸
        /// </summary>
        /// <param name="width">目标宽度</param>
        /// <param name="height">目标高度</param>
        public void SetTargetSize(float width, float height)
        {
            targetWidth = width;
            targetHeight = height;
        }

        /// <summary>
        /// 获取当前目标尺寸
        /// </summary>
        /// <returns>目标尺寸的Vector2</returns>
        public Vector2 GetTargetSize()
        {
            return new Vector2(targetWidth, targetHeight);
        }

        /// <summary>
        /// 重置Image尺寸为目标尺寸
        /// </summary>
        public void ResetToTargetSize()
        {
            if (imageRectTransform != null)
            {
                imageRectTransform.sizeDelta = new Vector2(targetWidth, targetHeight);
            }
        }

        // 在Inspector中提供测试按钮
        [ContextMenu("自动调整当前Sprite尺寸")]
        private void TestAutoResize()
        {
            AutoResizeCurrentSprite();
        }

        [ContextMenu("重置为目标尺寸")]
        private void TestResetSize()
        {
            ResetToTargetSize();
        }
    }
}