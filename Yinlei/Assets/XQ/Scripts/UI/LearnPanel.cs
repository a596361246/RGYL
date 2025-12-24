using UnityEngine;
using UnityEngine.UI;
namespace XQ
{
    public class LearnPanel : PanelBase
    {
        public override void OnOpen(object[] param)
        {

        }

        // 视频界面相关
        [Header("Video Interface")]
        public GameObject videoPanel;
        public Button videoContinueBtn;
        public Button closeVideoBtn;

        // 主按钮区域
        [Header("Main Buttons")]
        public GameObject knowledgePanel;
        public GameObject mainButtonsPanel;
        public Button controlAreaBtn;
        public Button rocketAreaBtn;
        public Button observationAreaBtn;

        // 实验助手界面
        [Header("Assistant Interface")]
        public GameObject assistantPanel;
        public Text assistantText;
        public Button assistantContinueBtn;
        public Button assistantReturnBtn;

        // 指挥控制区域
        [Header("Control Area")]
        public GameObject controlRoomBtn;

        // 火箭发射区域
        [Header("Rocket Area")]
        public GameObject rocketAreaPanel;
        public Button currentDeviceBtn;
        public Button rocketLauncherBtn;
        public Button rocketWireBtn;
        public Button fieldMeterBtn;

        // 综合观测区域
        [Header("Observation Area")]
        public GameObject observationPanel;
        public Button antennaBtn;
        public Button sensorBtn;
        public Button opticalBtn;
        public Button fieldChangeBtn;

        // 知识点界面
        [Header("Knowledge Panels")]
        public GameObject knowledge1Panel;
        public GameObject knowledge2Panel;
        public GameObject knowledge3Panel;
        public GameObject knowledge4Panel;
        public GameObject knowledge5Panel;
        public GameObject knowledge6Panel;
        public GameObject knowledge7Panel;
        public GameObject knowledge8Panel;
        public GameObject knowledge9Panel;

        // 状态跟踪
        private string originalAssistantText;
        private bool isAssistantSecondClick = false;

        private void OnEnable()
        {
            SetInitialState();
        }
        void Start()
        {
            // 保存原始文本
            originalAssistantText = assistantText.text;

            // 初始化按钮监听
            InitializeButtonListeners();

            // 设置初始状态
            SetInitialState();
        }
        void OnClickCloseBtn()
        {
            UIManager.Instance.GetPanel<UIMenu>().Index1Panel.SetActive(true);
            gameObject.SetActive(false);
        }
        void InitializeButtonListeners()
        {
            closeVideoBtn.onClick.AddListener(OnClickCloseBtn);
            // 视频继续按钮
            videoContinueBtn.onClick.AddListener(OnVideoContinueClick);

            // 主区域按钮
            controlAreaBtn.onClick.AddListener(OnControlAreaClick);
            rocketAreaBtn.onClick.AddListener(OnRocketAreaClick);
            observationAreaBtn.onClick.AddListener(OnObservationAreaClick);

            // 实验助手按钮
            assistantContinueBtn.onClick.AddListener(OnAssistantContinueClick);
            assistantReturnBtn.onClick.AddListener(OnAssistantReturnClick);

            // 控制室按钮
            if (controlRoomBtn != null)
            {
                Button controlRoomBtnComp = controlRoomBtn.GetComponent<Button>();
                if (controlRoomBtnComp != null)
                {
                    controlRoomBtnComp.onClick.AddListener(() => ShowKnowledgePanel(knowledge1Panel));
                }
            }

            // 火箭区域按钮
            if (rocketLauncherBtn != null)
                rocketLauncherBtn.onClick.AddListener(() => ShowKnowledgePanel(knowledge2Panel));

            if (rocketWireBtn != null)
            {
                rocketWireBtn.onClick.AddListener(() => {
                    ShowKnowledgePanel(knowledge3Panel);
                    // 如果需要显示知识点4，可以添加额外逻辑
                });
            }

            if (fieldMeterBtn != null)
                fieldMeterBtn.onClick.AddListener(() => ShowKnowledgePanel(knowledge5Panel));

            // 观测区域按钮
            if (antennaBtn != null)
                antennaBtn.onClick.AddListener(() => ShowKnowledgePanel(knowledge6Panel));

            if (sensorBtn != null)
                sensorBtn.onClick.AddListener(() => ShowKnowledgePanel(knowledge7Panel));

            if (opticalBtn != null)
                opticalBtn.onClick.AddListener(() => ShowKnowledgePanel(knowledge8Panel));

            if (fieldChangeBtn != null)
                fieldChangeBtn.onClick.AddListener(() => ShowKnowledgePanel(knowledge9Panel));

            // 电流测量设备舱按钮（知识点4）
            if (currentDeviceBtn != null)
                currentDeviceBtn.onClick.AddListener(() => ShowKnowledgePanel(knowledge4Panel));
        }

        void SetInitialState()
        {
            // 隐藏所有面板
            HideAllPanels();

            // 显示初始界面（视频界面）
            videoPanel.SetActive(true);
        }

        void HideAllPanels()
        {
            isAssistantSecondClick = false;
            videoPanel.SetActive(false);
            mainButtonsPanel.SetActive(false);
            assistantPanel.SetActive(false);
            controlRoomBtn.SetActive(false);
            rocketAreaPanel.SetActive(false);
            observationPanel.SetActive(false);

            // 隐藏所有知识点面板
            knowledge1Panel.SetActive(false);
            knowledge2Panel.SetActive(false);
            knowledge3Panel.SetActive(false);
            knowledge4Panel.SetActive(false);
            knowledge5Panel.SetActive(false);
            knowledge6Panel.SetActive(false);
            knowledge7Panel.SetActive(false);
            knowledge8Panel.SetActive(false);
            knowledge9Panel.SetActive(false);
        }

        // 点击视频界面的继续按钮
        void OnVideoContinueClick()
        {
            UIManager.Instance.GetPanel<UIMenu>().video.SetActive(false);
            videoPanel.SetActive(false);
            knowledgePanel.SetActive(true);
            mainButtonsPanel.SetActive(true);
            assistantPanel.SetActive(true);
            assistantContinueBtn.gameObject.SetActive(true);
            assistantReturnBtn.gameObject.SetActive(false);

            // 重置助手状态
            isAssistantSecondClick = false;
            assistantText.text = originalAssistantText;

        }

        // 点击指挥控制与气象预警区按钮
        void OnControlAreaClick()
        {
            mainButtonsPanel.SetActive(false);
            assistantContinueBtn.gameObject.SetActive(false);

            assistantReturnBtn.gameObject.SetActive(true);
            controlRoomBtn.SetActive(true);
        }

        // 点击火箭发射与雷击接闪区按钮
        void OnRocketAreaClick()
        {
            mainButtonsPanel.SetActive(false);
            assistantContinueBtn.gameObject.SetActive(false);

            assistantReturnBtn.gameObject.SetActive(true);
            rocketAreaPanel.SetActive(true);
        }

        // 点击综合观测区按钮
        void OnObservationAreaClick()
        {
            mainButtonsPanel.SetActive(false);
            assistantContinueBtn.gameObject.SetActive(false);

            assistantReturnBtn.gameObject.SetActive(true);
            observationPanel.SetActive(true);
        }

        // 点击实验助手界面的返回按钮
        void OnAssistantReturnClick()
        {
            // 隐藏当前显示的子区域面板
            controlRoomBtn.SetActive(false);
            rocketAreaPanel.SetActive(false);
            observationPanel.SetActive(false);

            // 隐藏所有知识点面板
            HideAllKnowledgePanels();

            // 显示主按钮
            mainButtonsPanel.SetActive(true);
            assistantContinueBtn.gameObject.SetActive(true);
            assistantReturnBtn.gameObject.SetActive(false);
        }

        // 点击实验助手界面的继续按钮
        void OnAssistantContinueClick()
        {
            if (!isAssistantSecondClick)
            {
                // 第一次点击：更换文本，隐藏主按钮
                assistantText.text = "熟悉完基本设备及仪器后，让我来次引雷模拟实验吧！";
                mainButtonsPanel.SetActive(false);
                isAssistantSecondClick = true;
            }
            else
            {
                // 第二次点击：返回视频界面
                assistantPanel.SetActive(false);
                videoPanel.SetActive(true);

                // 重置状态
                isAssistantSecondClick = false;
                assistantText.text = originalAssistantText;
                UIManager.Instance.GetPanel<UIMenu>().video.SetActive(true);
                UIManager.Instance.GetPanel<UIMenu>().Index1Panel.SetActive(true);
                gameObject.SetActive(false);

            }
        }

        // 显示知识点面板
        public void ShowKnowledgePanel(GameObject knowledgePanel)
        {
            if (knowledgePanel != null)
            {
                // 隐藏其他知识点面板
                HideAllKnowledgePanels();

                // 显示当前知识点面板
                knowledgePanel.SetActive(true);
            }
        }

        // 隐藏所有知识点面板
        void HideAllKnowledgePanels()
        {
            knowledge1Panel.SetActive(false);
            knowledge2Panel.SetActive(false);
            knowledge3Panel.SetActive(false);
            knowledge4Panel.SetActive(false);
            knowledge5Panel.SetActive(false);
            knowledge6Panel.SetActive(false);
            knowledge7Panel.SetActive(false);
            knowledge8Panel.SetActive(false);
            knowledge9Panel.SetActive(false);
        }

        // 关闭知识点面板（可以在知识点面板上添加关闭按钮调用此方法）
        public void CloseKnowledgePanel(GameObject panel)
        {
            panel.SetActive(false);
        }

    }
}

