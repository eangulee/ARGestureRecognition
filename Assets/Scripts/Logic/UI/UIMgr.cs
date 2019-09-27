using Common.Util;
using Logic.Enums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Logic.UI
{
    public class UIMgr : SingletonMono<UIMgr>
    {
        public Camera uiCamera;
        private Transform _trans;
        private Dictionary<string, BaseView> _uiDic = new Dictionary<string, BaseView>();
        private Dictionary<string, int> _uiDepthDic = new Dictionary<string, int>();
        private Dictionary<System.Type, string> _uiTypeDic = new Dictionary<System.Type, string>();
        private const int _interval = 200;
        private const int MIN_DEPTH = -1000;
        private const int MAX_DEPTH = 1000;
        private int _currentDepth = 1000;
        private const int MIN_LAYER_ORDER = 0;
        private int _currentLayerOrder = 0;
        private int _layerOrderInterval = 10;

        public Dictionary<string, BaseView> uiDic
        {
            get
            {
                return _uiDic;
            }
        }

        public Dictionary<string, int> uiDepthDic
        {
            get
            {
                return _uiDepthDic;
            }
        }

        public Dictionary<System.Type, string> uiTypeDic
        {
            get
            {
                return _uiTypeDic;
            }
        }

        void Awake()
        {
            instance = this;
            _trans = this.transform;
            _currentLayerOrder = 0;
        }

        private void HideOtherView(BaseView topView)
        {
            foreach (var kvp in _uiDic)
            {
                if (kvp.Value != topView)
                {
                    HideView(kvp.Key);
                }
            }
        }

        private void ResortUI(BaseView topView)
        {
            int count = _uiDepthDic.Count;
            _currentDepth = MAX_DEPTH - _interval * count;
            _currentLayerOrder = MIN_LAYER_ORDER + _layerOrderInterval * count;
            List<string> uis = null;
            if (topView != null)
            {
                string path = string.Empty;
                foreach (var kvp in _uiDic)
                {
                    if (kvp.Value == topView)
                    {
                        path = kvp.Key;
                        _uiDepthDic.Remove(path);
                    }
                }
                _uiDepthDic.SortedByValues<string, int>(SortType.Desc);
                uis = _uiDepthDic.GetKeys();
                uis.Add(path);
            }
            else
            {
                _uiDepthDic.SortedByValues<string, int>(SortType.Desc);
                uis = _uiDepthDic.GetKeys();
            }
            _uiDepthDic.Clear();
            for (int i = 0, length = uis.Count; i < length; i++)
            {
                string path = uis[i];
                BaseView view = _uiDic[path];
                int depth = MAX_DEPTH - _interval * i;
                view.canvas.planeDistance = depth;
                view.canvas.sortingOrder = MIN_LAYER_ORDER + _layerOrderInterval * i;
                _uiDepthDic.Add(path, depth);
            }
        }

        /// <summary>
        /// open 一个ui view
        /// </summary>
        /// <param name="path">ui prefab 路径</param>
        /// <returns></returns>
        public GameObject OpenView(string path)
        {
            BaseView result = null;
            if (string.IsNullOrEmpty(path))
                return null;
            if (_uiDic.TryGetValue(path, out result))
            {
                TransformUtil.SwitchLayer(result.transform, (int)LayerType.UI);
                ResortUI(result);
                if (result.uiType == UIType.HideOther)
                    HideOtherView(result);
                Observers.Facade.instance.SendNotification(path, result, "open");
                return result.gameObject;
            }
            GameObject obj = ResMgr.ResMgr.instance.Load<GameObject>(path);
            GameObject go = GameObject.Instantiate(obj, Vector3.zero, Quaternion.identity) as GameObject;
            result = go.GetComponent<BaseView>();
            if (!_uiTypeDic.ContainsKey(result.GetType()))
                _uiTypeDic.Add(result.GetType(), path);
            int layer = (int)LayerType.UI;
            result.transform.SetParent(_trans, false);
            TransformUtil.SwitchLayer(result.transform, layer);
            Canvas canvas = result.transform.GetComponent<Canvas>();
            canvas.worldCamera = uiCamera;
            result.canvas = canvas;
            canvas.sortingOrder = _currentLayerOrder;
            _currentLayerOrder += _layerOrderInterval;
            canvas.planeDistance = _currentDepth;
            _uiDepthDic.Add(path, _currentDepth);
            _currentDepth -= _interval;
            if (_currentDepth < MIN_DEPTH)
                _currentDepth = MIN_DEPTH;
            if (result.uiType == UIType.HideOther)
                HideOtherView(result);
            _uiDic.Add(path, result);
            Observers.Facade.instance.SendNotification(path, result, "open");
            return result.gameObject;
        }

        /// <summary>
        /// open 一个ui view
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path">路径</param>
        /// <returns></returns>
        public T OpenView<T>(string path) where T : BaseView
        {
            T t = default(T);
            GameObject go = OpenView(path);
            if (go)
                t = go.GetComponent<T>();
            return t;
        }

        /// <summary>
        /// 获取一个已经打开的ui view，没有返回null
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public GameObject GetView(string path)
        {
            GameObject result = null;
            BaseView baseView = null;
            if (_uiDic.TryGetValue(path, out baseView))
            {
                result = baseView.gameObject;
            }
            return result;
        }

        /// <summary>
        /// 根据类型获取一个已经打开的ui view，没有返回null
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetView<T>() where T : BaseView
        {
            return GetView(typeof(T)) as T;
        }

        /// <summary>
        /// 根据类型获取一个已经打开的ui view，没有返回null
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public BaseView GetView(System.Type type)
        {
            BaseView baseView = null;
            string path = string.Empty;
            if (_uiTypeDic.TryGetValue(type, out path))
            {
                _uiDic.TryGetValue(path, out baseView);
            }
            return baseView;
        }

        /// <summary>
        /// 隐藏一个已经打开的ui view
        /// </summary>
        /// <param name="path"></param>
        public void HideView(string path)
        {
            BaseView result = null;
            if (_uiDic.TryGetValue(path, out result))
            {
                TransformUtil.SwitchLayer(result.transform, (int)LayerType.None);
                Observers.Facade.instance.SendNotification(path, result, "hide");
            }
        }

        /// <summary>
        /// 根据类型隐藏一个已经打开的ui view
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void HideView<T>() where T : BaseView
        {
            string path = string.Empty;
            if (_uiTypeDic.TryGetValue(typeof(T), out path))
            {
                CloseView(path);
            }
        }

        /// <summary>
        /// 根据类型隐藏一个已经打开的ui view
        /// </summary>
        /// <param name="type"></param>
        public void HideView(System.Type type)
        {
            string path = string.Empty;
            if (_uiTypeDic.TryGetValue(type, out path))
            {
                HideView(path);
            }
        }


        /// <summary>
        /// 关闭一个已经打开的ui view
        /// </summary>
        /// <param name="path"></param>
        public void CloseView(string path)
        {
            BaseView result = null;
            if (_uiDic.TryGetValue(path, out result))
            {
                _currentDepth += _interval;
                _currentLayerOrder -= _layerOrderInterval;
                _uiDic.Remove(path);
                _uiDepthDic.Remove(path);
                ResortUI(null);
                Observers.Facade.instance.SendNotification(path, result, "close");
                path = _uiDepthDic.Last<string, int>().Key;
                OpenView(path);
            }
            GameObject.Destroy(result.gameObject);

        }

        /// <summary>
        /// 根据类型关闭一个已经打开的ui view
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void CloseView<T>() where T : BaseView
        {
            string path = string.Empty;
            if (_uiTypeDic.TryGetValue(typeof(T), out path))
            {
                CloseView(path);
            }
        }

        /// <summary>
        /// 根据类型关闭一个已经打开的ui view
        /// </summary>
        /// <param name="type"></param>
        public void CloseView(System.Type type)
        {
            string path = string.Empty;
            if (_uiTypeDic.TryGetValue(type, out path))
            {
                CloseView(path);
            }
        }

        public void ResetUICanvasScaler()
        {
            foreach (var ui in _uiDic)
            {
                ui.Value.ResetCanvasScaler();
            }
        }

        void OnApplicationPause(bool pause)
        {
            if (!pause)//切换前台
                Invoke("ResetUICanvasScaler", 0.5f);
        }

        public RaycastHit GetRaycastHit(Vector2 position)
        {
            Ray _ray = UIMgr.instance.uiCamera.ScreenPointToRay(position);
            RaycastHit _hit;
            if (Physics.Raycast(_ray, out _hit))
            {
                return _hit;
            }
            return _hit;

        }

        void OnDestroy()
        {
            _uiDic.Clear();
            _uiDic = null;
            _uiDepthDic.Clear();
            _uiDepthDic = null;
            _uiTypeDic.Clear();
            _uiTypeDic = null;
        }
    }
}