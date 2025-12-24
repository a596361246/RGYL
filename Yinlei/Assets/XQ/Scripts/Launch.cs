using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XQ
{
    public class Launch : MonoBehaviour
    {
        void Start()
		{
			UIManager.Instance.OpenPanel<UIMenu>();
        }
	}
}
