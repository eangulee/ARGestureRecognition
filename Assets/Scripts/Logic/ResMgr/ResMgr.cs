/*
 * @Author:lily
 * @Create Time:2017-08-15 10:44:13
 */
using Common.Util;
using Logic.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Logic.ResMgr
{

    public class ResMgr : SingletonMono<ResMgr>
    {
        private Dictionary<string, AssetsObj> _resDic = null;
        void Awake()
        {
            instance = this;
            _resDic = new Dictionary<string, AssetsObj>();
        }

        void Start()
        {

        }

#if UNITY_EDITOR
        public Dictionary<string, AssetsObj> resDic
        {
            get { return _resDic; }
        }
#endif

        #region 文本加载
        public string LoadText(string path)
        {
            string fullPath = string.Empty;
            string content = string.Empty;
            string fileName = path;
            //Debug.Log(fileName);
            TextAsset ta = Resources.Load<TextAsset>(fileName);
            if (ta != null)
            {
                content = ta.text;
                Resources.UnloadAsset(ta);
            }
            return content;
        }

        public byte[] LoadBytes(string path)
        {
            byte[] bytes = default(byte[]);
            string content = LoadText(path);
            if (!string.IsNullOrEmpty(content))
                bytes = System.Text.Encoding.UTF8.GetBytes(content);
            return bytes;
        }
        #endregion

        #region 资源加载
        /// <summary>
        /// 阻塞加载资源
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="path">资源路径</param>
        /// <returns></returns>
        public T Load<T>(string path) where T : Object
        {
            T t = default(T);
            if (string.IsNullOrEmpty(path))
                return t;
            t = LoadLocal<T>(path);
            return t;
        }

        /// <summary>
        /// 异步加载资源
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="path">资源路径</param>
        /// <param name="loadAction">资源加载回调</param>
        /// <param name="prior">资源加载优先级</param>
        public void Load<T>(string path, System.Action<AssetsObj> loadAction, byte prior = 0)
        {
            AssetsObj ao = null;
            if (_resDic.ContainsKey(path))
            {
                ao = _resDic[path];
                if (loadAction != null)
                    loadAction(ao);
            }
            else
                StartCoroutine(LoadAsyncCoroutine(path, loadAction));
        }

        private IEnumerator LoadAsyncCoroutine(string path, System.Action<AssetsObj> loadAction)
        {
            ResourceRequest rr = Resources.LoadAsync<Object>(path);
            while (!rr.isDone)
            {
                yield return null;
            }
            AssetsObj ao = new AssetsObj(new List<Object>() { rr.asset });
            if (!_resDic.ContainsKey(path))
                _resDic.Add(path, ao);
            if (loadAction != null)
                loadAction(ao);
            rr = null;
        }

        private T LoadLocal<T>(string path) where T : Object
        {
            string assetName = path.Substring(path.LastIndexOf("/") + 1);
            T t = GetResFromMemory<T>(path, assetName);
            if (t != null)
                return t;
            //Debug.Log("path:{0}", path);
            if (typeof(T) == typeof(Sprite))
                t = LoadLocalSprite(path) as T;
            else
                t = Resources.Load<T>(path);
            if (t != null)
            {
                AssetsObj ao = new AssetsObj(new List<Object>() { t });
                if (!_resDic.ContainsKey(path))
                    _resDic.Add(path, ao);
            }
            return t;
        }

        private T GetResFromMemory<T>(string path, string assetName) where T : Object
        {
            AssetsObj ao = null;
            T t = default(T);
            if (_resDic.TryGetValue(path, out ao))
            {
                t = ao.GetAssetByName<Object>(assetName) as T;
            }
            return t;
        }

        /// <summary>
        /// 获取资源名
        /// </summary>
        /// <param name="fullPath">资源全路径</param>
        /// <returns></returns>
        private string GetResName(string fullPath)
        {
            int index = fullPath.LastIndexOf("/");
            return fullPath.Substring(index + 1);
        }

        #region load sprite
        private string GetSpriteAssetPath(string fullPath)
        {
            int index = fullPath.LastIndexOf("/");
            if (index > 0)
                return fullPath.Substring(0, index);
            return "invalid:" + fullPath;
        }

        public Sprite LoadSprite(string path)
        {
            return Load<Sprite>(path);
        }

        private Sprite LoadLocalSprite(string path)
        {
            //Debug.Log("path:{0}", "sprite/" + path);
            Sprite sprite = null;
            GameObject go = Resources.Load<GameObject>(path);
            if (go)
                sprite = go.GetComponent<SpriteRenderer>().sprite;
            return sprite;
        }
        #endregion
        #endregion

        public void ClearAllRes()
        {
            _resDic.Clear();
        }

        public void ClearAllRes(System.Action<float> progressAction, System.Action completeAction)
        {
            _resDic.Clear();
            StartCoroutine(UnloadCoroutine(progressAction, completeAction));
        }

        private IEnumerator UnloadCoroutine(System.Action<float> progressAction, System.Action completeAction)
        {
            System.GC.Collect();
            AsyncOperation ao = Resources.UnloadUnusedAssets();
            while (!ao.isDone)
            {
                if (progressAction != null)
                    progressAction(ao.progress);
                yield return null;
            }
            if (completeAction != null)
                completeAction();
        }

        void OnDestroy()
        {
            _resDic.Clear();
        }
    }
}