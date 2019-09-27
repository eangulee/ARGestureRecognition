/*
 * @Author:lily
 * @Create Time:2017-10-31 10:38:41
 */
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Logic.UI.Editor.Inspector
{
    [CustomEditor(typeof(UIMgr))]
    public class UIMgrInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            UIMgr uiMgr = target as UIMgr;

            if (uiMgr.uiDic != null)
            {
                EditorGUILayout.LabelField("ui dic", uiMgr.uiDic.Count.ToString());
                int i = 0;
                foreach (var kv in uiMgr.uiDic)
                {
                    EditorGUILayout.LabelField((++i).ToString(), kv.Key);
                    EditorGUILayout.LabelField("layer:",((Enums.LayerType)kv.Value.gameObject.layer).ToString());
                    EditorGUILayout.LabelField( "sort:",kv.Value.canvas.planeDistance.ToString());
                }
            }

            if (uiMgr.uiDepthDic != null)
            {
                EditorGUILayout.LabelField("ui depth dic", uiMgr.uiDepthDic.Count.ToString());
                int i = 0;
                foreach (var kv in uiMgr.uiDepthDic)
                {
                    EditorGUILayout.LabelField((++i).ToString(), kv.Key);
                    EditorGUILayout.LabelField("depth:", kv.Value.ToString());
                }
            }

        }
    }
}
