using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace XQ
{
	
	public class UIMenu : PanelBase
	{
		public GameObject video;
		public Button mainBtn;
		public GameObject fBtn;
		public HomePanel homePanel;
		public GameObject Index1Panel;
		public GameObject learnPanel;
		public GameObject exam1Panel;
		public GameObject exam2Panel;
		private void Awake()
		{
			mainBtn.onClick.AddListener(OnMainBtn);
			mainBtn.gameObject.SetActive(false);
			OnMainBtn();
		}
		/// <summary>
		/// 首页按钮事件
		/// </summary>
		private void OnMainBtn()
		{
			video.SetActive(true);
			homePanel.gameObject.SetActive(true);
			homePanel.Init();
			mainBtn.gameObject.SetActive(false);
			Index1Panel.SetActive(false);
			learnPanel.SetActive(false);
			exam1Panel.SetActive(false);
			exam2Panel.SetActive(false);
		}


	}
}
