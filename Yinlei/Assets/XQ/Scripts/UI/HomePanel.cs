using UnityEngine;
using UnityEngine.UI;
namespace XQ
{
    public class HomePanel : PanelBase
    {
        #region 变量
        public Button startBtn;
        public GameObject text1;
        public RectTransform text2;
        public float resetPositionY = -1000f; //滚动到某一位置后重置
        private float initialY;
        #endregion

        void Awake()
        {
            initialY = text2.anchoredPosition.y; //保存起始位置
        }

        public override void OnOpen(object[] param)
        {

        }

        private void Start()
        {
            startBtn.onClick.AddListener(OnClickStartBtn);
            Init();
        }

        public void Init()
        {
            text1.SetActive(true);
            text2.gameObject.SetActive(false);
        }
        private void Update()
        {
            //当内容滚动到一定位置后，重置到起始位置，形成无限滚动
            if (text2.anchoredPosition.y <= resetPositionY)
            {
                //向下滚动
                text2.anchoredPosition += new Vector2(0, 50 * Time.deltaTime);

            }
        }

        void OnClickStartBtn()
        {
            if (text1.activeSelf)
            {
                text1.SetActive(false);
                text2.gameObject.SetActive(true);
                text2.anchoredPosition = new Vector2(text2.anchoredPosition.x, initialY);
                UIManager.Instance.GetPanel<UIMenu>().mainBtn.gameObject.SetActive(true);
            }
            else
            {
                gameObject.SetActive(false);
                UIManager.Instance.GetPanel<UIMenu>().Index1Panel.SetActive(true);
            }
        }
    }
}

