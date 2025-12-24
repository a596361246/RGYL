using UnityEngine;
using UnityEngine.UI;
namespace XQ
{
    public class Index1Panel : PanelBase
    {
        #region 变量
        public Button renZhiBtn;
        public Button chuanTongBtn;
        public Button qiTaBtn;

        #endregion

        public override void OnOpen(object[] param)
        {

        }

        private void Start()
        {
            renZhiBtn.onClick.AddListener(OnClickRenZhiBtn);
            //chuanTongBtn.onClick.AddListener(OnClickChuanTongBtn);
            //qiTaBtn.onClick.AddListener(OnClickQiTaBtn);
        }

        void OnClickRenZhiBtn()
        {
            UIManager.Instance.GetPanel<UIMenu>().learnPanel.SetActive(true);
            gameObject.SetActive(false);
        }
        void OnClickChuanTongBtn()
        {
            UIManager.Instance.GetPanel<UIMenu>().video.SetActive(false);
            UIManager.Instance.GetPanel<UIMenu>().exam1Panel.SetActive(true);
            gameObject.SetActive(false);
        }
        void OnClickQiTaBtn()
        {
            UIManager.Instance.GetPanel<UIMenu>().exam2Panel.SetActive(true);
            gameObject.SetActive(false);
        }

    }
}

