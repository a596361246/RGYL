using UnityEngine;
using UnityEngine.UI;
namespace XQ
{
    public class Exam1Panel : PanelBase
    {
        // UI组件
        [Header("主界面按钮")]
        public Button lightningFieldBtn;  // 引雷场按钮

        [Header("引雷场界面")]
        public GameObject lightningFieldPanel;  // 引雷场房间图片
        public Button radarMonitoringBtn;      // 雷达回波监测
        public Button dataRecorderBtn;         // 数据记录仪
        public Button powerConsoleBtn;         // 电源控制台
        public Button equipmentStartBtn;       // 开启方舱内相关仪器设备

        [Header("问题面板")]
        public GameObject question;
        public Button questionSubmit;

        [Header("知识点面板")]
        public GameObject[] knowledgePanels = new GameObject[6]; // 知识点1-5，索引0不使用

        [Header("实验助手相关")]
        public GameObject assistantPanel;
        public Button assistantContinueBtn;    // 实验助手的继续按钮
        public Text assistantText;             // 实验助手文本
        public GameObject endPanel;            //完成试验界面
        public Image image1;                   // 图片1
        public Button completeExperimentBtn;   // 完成实验按钮

        // 实验状态
        private enum ExperimentState
        {
            Initial,        // 初始状态
            LightningField, // 引雷场界面
            SuccessResult,  // 实验成功结果
            ExpandExperiment // 拓展实验
        }

        private ExperimentState currentState = ExperimentState.Initial;

        private void OnEnable()
        {
            // 初始化隐藏所有UI
            InitializeUI();
        }
        void Start()
        {
            // 初始化隐藏所有UI
            InitializeUI();

            // 绑定按钮事件
            BindButtonEvents();
        }

        void InitializeUI()
        {
            currentState = ExperimentState.Initial;
            // 隐藏所有面板
            lightningFieldBtn.gameObject.SetActive(true);
            lightningFieldPanel.SetActive(false);
            UIManager.Instance.GetPanel<UIMenu>().video.SetActive(false);
            endPanel.SetActive(false);
            assistantPanel.SetActive(true);

            assistantText.text = "乌云快要来了，预计今天可以完成引雷实验，走!我们首先进行设备检查!";

            // 隐藏所有知识点面板
            foreach (var panel in knowledgePanels)
            {
                if (panel != null)
                    panel.SetActive(false);
            }
        }

        void BindButtonEvents()
        {
            // 引雷场按钮
            lightningFieldBtn.onClick.AddListener(() =>
            {
                EnterLightningFieldState();
            });
            questionSubmit.onClick.AddListener(() =>
            {
                OnClickSubmitBtn();
            });
            // 知识点按钮
            radarMonitoringBtn.onClick.AddListener(() =>
            {
                ShowKnowledge(1);
            });

            dataRecorderBtn.onClick.AddListener(() =>
            {
                ShowKnowledge(2);
            });

            powerConsoleBtn.onClick.AddListener(() =>
            {
                ShowKnowledge(3);
            });

            equipmentStartBtn.onClick.AddListener(() =>
            {
                ShowKnowledge(4);
            });

            // 实验助手继续按钮
            assistantContinueBtn.onClick.AddListener(OnAssistantContinue);

            // 完成实验按钮
            completeExperimentBtn.onClick.AddListener(() =>
            {
                Debug.Log("实验完成！");
                // 这里可以添加实验完成的逻辑
                UIManager.Instance.GetPanel<UIMenu>().Index1Panel.SetActive(true);
                gameObject.SetActive(false);
            });
        }
        private void OnClickSubmitBtn()
        {
            question.SetActive(false);
            lightningFieldBtn.gameObject.SetActive(false);
            lightningFieldPanel.SetActive(true);

            assistantText.text = "设备检查完毕，一切正常，保持警惕，时刻观察电场数据，随时准备发射";
        }

        void EnterLightningFieldState()
        {
            currentState = ExperimentState.LightningField;

            lightningFieldBtn.gameObject.SetActive(false);
            // 显示引雷场界面
            lightningFieldPanel.SetActive(true);

            // 更新实验助手文本
            assistantText.text = "设备检查完毕，一切正常，保持警惕，时刻观察电场数据，随时准备发射";
        }

        void ShowKnowledge(int knowledgeIndex)
        {
            // 隐藏之前的知识点
            foreach (var panel in knowledgePanels)
            {
                if (panel != null)
                    panel.SetActive(false);
            }

            // 显示当前知识点
            if (knowledgeIndex >= 1 && knowledgeIndex <= 4)
            {
                if (knowledgePanels[knowledgeIndex] != null)
                {
                    knowledgePanels[knowledgeIndex].SetActive(true);
                }
            }
        }

        void OnAssistantContinue()
        {
            switch (currentState)
            {
                case ExperimentState.Initial:
                    currentState = ExperimentState.LightningField;
                    question.SetActive(true);
                    break;
                case ExperimentState.LightningField:
                    lightningFieldPanel.SetActive(false);
                    // 第一次点击继续按钮 - 显示实验成功结果
                    currentState = ExperimentState.SuccessResult;

                    // 显示背景1
                    UIManager.Instance.GetPanel<UIMenu>().video.SetActive(true);

                    // 更新实验助手文本
                    assistantText.text = "太好了，实验非常成功，接上监控摄像机，抓紧测量保存各项数据";
                    break;
                case ExperimentState.SuccessResult:
                    // 第二次点击继续按钮 - 显示拓展实验
                    currentState = ExperimentState.ExpandExperiment;

                    // 隐藏背景1
                    UIManager.Instance.GetPanel<UIMenu>().video.SetActive(false);

                    // 更新实验助手文本
                    assistantText.text = "数据非常完整，让我们记录下实验数据。趁着雷暴没结束，我们还能进行【雷击条件下储油罐安全防护的模拟实验】";

                    // 显示知识点5
                    if (knowledgePanels[5] != null)
                    {
                        knowledgePanels[5].SetActive(true);
                    }

                    break;

                case ExperimentState.ExpandExperiment:
                    // 第三次点击继续按钮 - 显示最终界面
                    // 显示背景1
                    UIManager.Instance.GetPanel<UIMenu>().video.SetActive(true);
                    assistantPanel.SetActive(false);
                    // 显示完成实验
                    endPanel.SetActive(true);

                    // 可以继续更新文本或执行其他操作
                    break;
            }
        }
    }
}
