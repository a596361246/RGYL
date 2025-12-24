using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace XQ
{
	public enum PanelLevel
	{
		Bottom,
		Common,
		Top
	}
	public class UIManager : SingleBase<UIManager>
	{
		private Dictionary<string, PanelBase> _dicPanel;

        private Transform mUIRoot;
        public Transform UIRoot
        {
            get { return mUIRoot; }
        }
        public UIManager()
        {
            _dicPanel = new Dictionary<string, PanelBase>();

            if (mUIRoot == null)
            {
                GameObject go = GameObject.Instantiate(ResMgr.Instance.LoadPrefab("UI/UIRoot"));
                go.name = "UIRoot";
				GameObject.DontDestroyOnLoad(go);
				mUIRoot = go.transform;
            }
        }
        private Transform UiLevel(PanelLevel level)
        {
            Transform trans = null;
            switch (level)
            {
                case PanelLevel.Bottom:
					trans =  mUIRoot.transform.Find("Bottom");
                    break;
                case PanelLevel.Common:
                    trans = mUIRoot.transform.Find("Common");
					break;
				case PanelLevel.Top:
					trans = mUIRoot.transform.Find("Top");
					break;
			}
            return trans;
        }
        public T GetPanel<T>() where T : PanelBase
        {
            T _t = null;
            string panelName = typeof(T).Name;
			if (_dicPanel.ContainsKey(panelName))
            {
                _t = _dicPanel[panelName] as T;
            }
            else
            {
                Debug.LogError($"Get {panelName} Panel is Null!");
            }
            return _t;

        }
		public T OpenPanel<T>(PanelLevel level = PanelLevel.Common, params object[] param) where T : PanelBase
		{
            T panel = null;
            string panelName = typeof(T).Name;
			if (_dicPanel.ContainsKey(panelName))
			{
				panel = _dicPanel[panelName] as T;
				panel.gameObject.SetActive(true);
				panel.transform.SetAsLastSibling();
				panel.OnOpen(param);
			}
            else
            {
                GameObject go = GameObject.Instantiate(ResMgr.Instance.LoadPrefab("UI/" + panelName), UiLevel(level));
                go.transform.localPosition = Vector3.zero;
                go.transform.localRotation = Quaternion.identity;
                go.transform.localScale = Vector3.one;
                go.name = panelName;
				panel = go.GetComponent<PanelBase>() as T;
				panel.OnOpen(param);
                _dicPanel.Add(panelName, panel);
            }
            return panel;

		}

        public void HidePanel<T>() where T : PanelBase
        {
			if (_dicPanel == null)
            {
                return;
			}
			string panelName = typeof(T).Name;
			if (_dicPanel.ContainsKey(panelName))
            {
                PanelBase panelBase = _dicPanel[panelName];
                panelBase.OnClose();
                panelBase.gameObject.SetActive(false);
            }
        }
        public void ClosePanel<T>()
        {
            if (_dicPanel == null)
            {
                return;
            }
			string panelName = typeof(T).Name;
			if (_dicPanel.ContainsKey(panelName))
            {
                PanelBase panelBase = _dicPanel[panelName];
                panelBase.OnClose();
                GameObject.Destroy(panelBase.gameObject);
                _dicPanel.Remove(panelName);
            }
        }

        public void CloseAllPanel()
        {
            if (_dicPanel == null)
            {
                return;
            }
            foreach (var item in _dicPanel.ToList())
            {
				if (_dicPanel.ContainsKey(item.Key))
				{
					PanelBase panelBase = _dicPanel[item.Key];
					panelBase.OnClose();
					GameObject.Destroy(panelBase.gameObject);
					_dicPanel.Remove(item.Key);
				}
            }
        }
        /// <summary>
        /// 保留一个面板，关闭其他所有面板
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="param"></param>
        public void CloseOtherPanel<T>() where T : PanelBase
        {
            if (_dicPanel == null)
            {
                return;
            }
			string cullingPanelName = typeof(T).Name;
			foreach (var item in _dicPanel.ToList())
            {
                if (item.Key != cullingPanelName)
                {
					PanelBase panelBase = _dicPanel[item.Key];
					panelBase.OnClose();
					GameObject.Destroy(panelBase.gameObject);
					_dicPanel.Remove(item.Key);
				}
            }
        }
    }
}
