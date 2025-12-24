using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XQ
{
    public class UltraSimpleUI : MonoBehaviour
    {
        public string key;
        public float range = 3f;     // 显示范围
        private Transform player;            // 玩家
        private bool isShow = false;
        void Start()
        {
            // 自动查找玩家（标签为"Player"）
            GameObject p = GameObject.FindWithTag("Player");
            if (p) player = p.transform;
        }

        void Update()
        {
            if (!player) return;

            // 计算距离
            float dist = Vector3.Distance(transform.position, player.position);

            if (dist <= range)
            {            
                // 显示UI
                UIManager.Instance.GetPanel<UIMenu>().fBtn.SetActive(true);
                isShow = true;
            }
            else
            {
                if(isShow)
                {
                    UIManager.Instance.GetPanel<UIMenu>().fBtn.SetActive(false);
                    isShow = false;
                }
            }
            if (isShow)
            {
                //F键执行事件
                if (Input.GetKeyDown(KeyCode.F))
                {
                    switch (key)
                    {
                        case "大气电场仪":
                            UIManager.Instance.GetPanel<UIMenu>().learnPanel.GetComponent<LearnPanel>().ShowKnowledgePanel(UIManager.Instance.GetPanel<UIMenu>().learnPanel.GetComponent<LearnPanel>().knowledge5Panel);
                            break;
                        case "快慢天线":
                            UIManager.Instance.GetPanel<UIMenu>().learnPanel.GetComponent<LearnPanel>().ShowKnowledgePanel(UIManager.Instance.GetPanel<UIMenu>().learnPanel.GetComponent<LearnPanel>().knowledge9Panel);
                            break;
                        case "磁场传感器":
                            UIManager.Instance.GetPanel<UIMenu>().learnPanel.GetComponent<LearnPanel>().ShowKnowledgePanel(UIManager.Instance.GetPanel<UIMenu>().learnPanel.GetComponent<LearnPanel>().knowledge7Panel);
                            break;
                        case "短基天线":
                            UIManager.Instance.GetPanel<UIMenu>().learnPanel.GetComponent<LearnPanel>().ShowKnowledgePanel(UIManager.Instance.GetPanel<UIMenu>().learnPanel.GetComponent<LearnPanel>().knowledge6Panel);
                            break;
                        case "电流设备舱":
                            UIManager.Instance.GetPanel<UIMenu>().learnPanel.GetComponent<LearnPanel>().ShowKnowledgePanel(UIManager.Instance.GetPanel<UIMenu>().learnPanel.GetComponent<LearnPanel>().knowledge2Panel);
                            break;
                        case "示波器":
                            UIManager.Instance.GetPanel<UIMenu>().learnPanel.GetComponent<LearnPanel>().ShowKnowledgePanel(UIManager.Instance.GetPanel<UIMenu>().learnPanel.GetComponent<LearnPanel>().knowledge1Panel);
                            break;
                        case "人工引雷地面装置":
                            UIManager.Instance.GetPanel<UIMenu>().learnPanel.GetComponent<LearnPanel>().ShowKnowledgePanel(UIManager.Instance.GetPanel<UIMenu>().learnPanel.GetComponent<LearnPanel>().knowledge4Panel);
                            break;
                        default:
                            break;
                    }
                }
            }
        }
    }
}