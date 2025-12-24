using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XQ
{
    /// <summary>
    /// 继承MonoBehav的单例,手动挂载注意唯一性
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SingleMonoBase<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;
        public static T Instance
        {
            get
            {
                return _instance;
            }
        }
        protected virtual void Awake()
        {
            _instance = this as T;
        }
    }
}
