using UnityEditor;

namespace Logic.ResMgr.Editor.Inspector
{
    [CustomEditor(typeof(Logic.ResMgr.ResMgr))]
    public class ResMgrInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            Logic.ResMgr.ResMgr resMgr = target as Logic.ResMgr.ResMgr;

            if (resMgr.resDic != null)
            {
                EditorGUILayout.LabelField("resDicCount", resMgr.resDic.Count.ToString());
                int i = 0;
                foreach (var kv in resMgr.resDic)
                {
                    EditorGUILayout.LabelField((++i).ToString(), kv.Key);
                }
            }
        }
    }
}
