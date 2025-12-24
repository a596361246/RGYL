using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XQ
{
    public class SingleBase<T> where T : new()
    {
        private static T _instance;
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new T();
                }
                return _instance;
            }
        }
    }
}
