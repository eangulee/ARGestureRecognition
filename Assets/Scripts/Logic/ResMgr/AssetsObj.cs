/*
 * @Author:lily
 * @Create Time:2017-08-15 10:44:13
 */
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace Logic.ResMgr
{
    public sealed class AssetsObj
    {
        private List<Object> _assetsList;

        public AssetsObj(List<Object> assets)
        {
            _assetsList = assets;
        }

        public AssetsObj(Object[] assets)
        {
            if (assets == null)
                Debug.LogError("assets is null !!");
            else
                _assetsList = assets.ToList();
        }


		public T GetAssetByName<T>(string name) where T:Object
		{
			if (_assetsList == null)
				return null;
			T t = null;
			for (int i = 0, count = _assetsList.Count; i < count; i++)
			{
				t = _assetsList[i] as T;
				if (t != null && t.name == name)
					return t;
			}
			return null;
		}

        public void Clear()
        {
            if (_assetsList != null)
            {
                _assetsList.Clear();
                _assetsList = null;
            }
        }

        public override string ToString()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append("AssetsObj:[");
            sb.Append("_assetsList count:");
            if (_assetsList != null)
            {
                sb.Append(_assetsList.Count);
                for (int i = 0, count = _assetsList.Count; i < count; i++)
                {
                    sb.Append(",");
                    sb.Append(_assetsList[i].name);
                }
            }
            else
                sb.Append("0");
            sb.Append("]");
            return sb.ToString();
        }
    }
}
