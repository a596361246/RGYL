using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XQ
{
    /// <summary>
    /// 3D对象层级分类
    /// </summary>
    public enum ObjectLayer
    {
        Background,
        Main,
        Effects
    }

    public class ObjectManager : SingleBase<ObjectManager>
    {
        // 关键数据结构
        private Dictionary<string, GameObject> _objectDict;
        private Dictionary<ObjectLayer, Transform> _layerTransforms;

        private Transform _rootTransform;

        // 公共读取索引
        public Transform RootTransform => _rootTransform;

        public ObjectManager()
        {
            _objectDict = new Dictionary<string, GameObject>();

            // 创建根节点，避免重复创建
            if (_rootTransform == null)
            {
                GameObject go = new GameObject("ObjectRoot");
                _rootTransform = go.transform;

                // 初始化不同层级
                _layerTransforms = new Dictionary<ObjectLayer, Transform>();
                foreach (ObjectLayer layer in System.Enum.GetValues(typeof(ObjectLayer)))
                {
                    GameObject layerGo = new GameObject(layer.ToString());
                    layerGo.transform.SetParent(_rootTransform);
                    _layerTransforms[layer] = layerGo.transform;
                }
            }
        }
        /// <summary>
        /// 添加已存在的物体
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="layer"></param>
        public void AddObject(GameObject obj, ObjectLayer layer = ObjectLayer.Main)
        {
            if (_objectDict.ContainsKey(obj.name))
            {
                Debug.LogWarning($"Object with name {obj.name} already exists.");
                return;
            }
            else
            {
                _objectDict.Add(obj.name, obj);
            }
        }
        /// <summary>
        /// 使用层级创建对象，指定资源路径和名字
        /// </summary>
        public GameObject CreateObject(string prefabPath, string objectName, ObjectLayer layer = ObjectLayer.Main)
        {
            if (_objectDict.ContainsKey(objectName))
            {
                Debug.LogWarning($"Object with name {objectName} already exists.");
                return _objectDict[objectName];
            }

            GameObject prefab = ResMgr.Instance.LoadPrefab(prefabPath);
            if (prefab == null)
            {
                Debug.LogError($"Prefab at {prefabPath} failed to load.");
                return null;
            }

            GameObject obj = Object.Instantiate(prefab, _layerTransforms[layer]);
            obj.name = objectName;

            _objectDict.Add(objectName, obj);
            return obj;
        }

        /// <summary>
        /// 获取已存在的对象
        /// </summary>
        public GameObject GetObject(string objectName)
        {
            if (_objectDict.ContainsKey(objectName))
            {
                return _objectDict[objectName];
            }
            Debug.LogWarning($"Object {objectName} not found!");
            return null;
        }

        /// <summary>
        /// 隐藏对象（设置不可见）
        /// </summary>
        public void HideObject(string objectName)
        {
            if (_objectDict.TryGetValue(objectName, out GameObject obj))
            {
                obj.SetActive(false);
            }
        }

        /// <summary>
        /// 显示对象
        /// </summary>
        public void ShowObject(string objectName)
        {
            if (_objectDict.TryGetValue(objectName, out GameObject obj))
            {
                obj.SetActive(true);
            }
        }

        /// <summary>
        /// 销毁对象并删除
        /// </summary>
        public void DestroyObject(string objectName)
        {
            if (_objectDict.TryGetValue(objectName, out GameObject obj))
            {
                Object.Destroy(obj);
                _objectDict.Remove(objectName);
            }
            else
            {
                Debug.LogWarning($"Object {objectName} not found for destruction.");
            }
        }

        /// <summary>
        /// 销毁所有对象
        /// </summary>
        public void DestroyAllObjects()
        {
            foreach (var kvp in _objectDict)
            {
                Object.Destroy(kvp.Value);
            }
            _objectDict.Clear();
        }
    }
}
