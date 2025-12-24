using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XQ
{

/// <summary>
/// 继承MonoBehaviou单例基类，自动生成挂载
/// </summary>
/// <typeparam name="T"></typeparam>
public class SingleAutoBase<T> : MonoBehaviour where T:MonoBehaviour
{
    private static T _instance;
    public  static T Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject obj = new GameObject(typeof(T).ToString());
                DontDestroyOnLoad(obj);
                _instance = obj.AddComponent<T>();
            }
            return _instance;
        }
       
    }

    }
}
