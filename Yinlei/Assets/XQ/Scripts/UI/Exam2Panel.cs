using UnityEngine;
using UnityEngine.UI;
namespace XQ
{
    public class Exam2Panel : PanelBase
    {
        [Header("Toggle 组件")]
        [SerializeField] private Toggle laserToggle;
        [SerializeField] private Toggle droneToggle;
        [SerializeField] private Toggle highAltitudeToggle;
        public Button endBtn;
        public Button end2Btn;

        [Header("界面 Panel")]
        [SerializeField] private GameObject mainPanel;    // 激光引雷界面
        [SerializeField] private GameObject laserPanel;    // 激光引雷界面
        [SerializeField] private GameObject dronePanel;    // 无人机引雷界面
        [SerializeField] private GameObject highAltitudePanel; // 高空人工引雷界面
        [SerializeField] private GameObject endPanel; // 结束语界面

        [Header("初始状态")]
        [SerializeField] private bool startWithLaserPanel = true;

        private void OnEnable()
        {
            InitPanel();
        }
        private void Start()
        {
            // 绑定 Toggle 事件
            if (laserToggle != null)
                laserToggle.onValueChanged.AddListener(OnLaserToggleChanged);

            if (droneToggle != null)
                droneToggle.onValueChanged.AddListener(OnDroneToggleChanged);

            if (highAltitudeToggle != null)
                highAltitudeToggle.onValueChanged.AddListener(OnHighAltitudeToggleChanged);

            endBtn.onClick.AddListener(OnClickEndBtn);
            end2Btn.onClick.AddListener(OnClickEnd2Btn);
            // 初始化界面
            if (startWithLaserPanel)
            {
                ShowLaserPanel();
                if (laserToggle != null) laserToggle.isOn = true;
            }
        }

        private void InitPanel()
        {
            mainPanel.SetActive(true);
            endPanel.SetActive(false);
            laserToggle.isOn = true;
        }

        private void OnLaserToggleChanged(bool isOn)
        {
            if (isOn)
            {
                ShowLaserPanel();
            }
        }

        private void OnDroneToggleChanged(bool isOn)
        {
            if (isOn)
            {
                ShowDronePanel();
            }
        }

        private void OnHighAltitudeToggleChanged(bool isOn)
        {
            if (isOn)
            {
                ShowHighAltitudePanel();
            }
        }

        private void OnClickEndBtn()
        {
            mainPanel.SetActive(false);
            endPanel.SetActive(true);
        }
        private void OnClickEnd2Btn()
        {
            UIManager.Instance.GetPanel<UIMenu>().Index1Panel.SetActive(true);
            gameObject.SetActive(false);
        }

        // 显示激光引雷界面
        public void ShowLaserPanel()
        {
            SetAllPanelsInactive();

            if (laserPanel != null)
                laserPanel.SetActive(true);

            Debug.Log("显示激光引雷界面");
        }

        // 显示无人机引雷界面
        public void ShowDronePanel()
        {
            SetAllPanelsInactive();

            if (dronePanel != null)
                dronePanel.SetActive(true);

            Debug.Log("显示无人机引雷界面");
        }

        // 显示高空人工引雷界面
        public void ShowHighAltitudePanel()
        {
            SetAllPanelsInactive();

            if (highAltitudePanel != null)
                highAltitudePanel.SetActive(true);

            Debug.Log("显示高空人工引雷界面");
        }

        // 关闭所有界面
        private void SetAllPanelsInactive()
        {
            if (laserPanel != null)
                laserPanel.SetActive(false);

            if (dronePanel != null)
                dronePanel.SetActive(false);

            if (highAltitudePanel != null)
                highAltitudePanel.SetActive(false);
        }

        // 手动切换界面（也可以通过按钮调用）
        public void SwitchToLaserPanel() => ShowLaserPanel();
        public void SwitchToDronePanel() => ShowDronePanel();
        public void SwitchToHighAltitudePanel() => ShowHighAltitudePanel();

        private void OnDestroy()
        {
            // 清理事件绑定
            if (laserToggle != null)
                laserToggle.onValueChanged.RemoveListener(OnLaserToggleChanged);

            if (droneToggle != null)
                droneToggle.onValueChanged.RemoveListener(OnDroneToggleChanged);

            if (highAltitudeToggle != null)
                highAltitudeToggle.onValueChanged.RemoveListener(OnHighAltitudeToggleChanged);
        }
    }
}

