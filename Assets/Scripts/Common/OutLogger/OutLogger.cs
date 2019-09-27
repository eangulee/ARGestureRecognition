using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
namespace Common.OutLogger
{
    public class OutLogger : MonoBehaviour
    {
        static List<string> mLines = new List<string>();
        static List<string> mWriteTxt = new List<string>();
        private string outpath;
        public bool logScreen;
        void Start()
        {
            //Application.persistentDataPath Unity��ֻ�����·���Ǽȿ��Զ�Ҳ����д�ġ�
            outpath = Application.persistentDataPath + "/outLog.txt";
            //ÿ�������ͻ���ɾ��֮ǰ�����Log
            if (System.IO.File.Exists(outpath))
            {
                File.Delete(outpath);
            }
            //��������һ��Log�ļ���
            Application.logMessageReceived += HandleLog;
#if !UNITY_EDITOR
            Debug.Log("path="+outpath);
            //Debugger.Log("------------------------------------------------------------------start log--------------------------------------------------------------------");           
#endif
        }

        void Update()
        {
            //��Ϊд���ļ��Ĳ������������߳�����ɣ�������Update��Ŷ����д���ļ���
            if (mWriteTxt.Count > 0)
            {
                string[] temp = mWriteTxt.ToArray();
                foreach (string t in temp)
                {
                    using (StreamWriter writer = new StreamWriter(outpath, true, Encoding.UTF8))
                    {
                        writer.WriteLine(t);
                    }
                    mWriteTxt.Remove(t);
                }
            }
        }

        void OnDestroy()
        {
#if !UNITY_EDITOR
            //Debugger.Log("------------------------------------------------------------------end log--------------------------------------------------------------------");   
#endif
            Application.logMessageReceived -= HandleLog;
        }

        void HandleLog(string logString, string stackTrace, LogType type)
        {
            mWriteTxt.Add(logString);
#if !UNITY_EDITOR
            mWriteTxt.Add(stackTrace);
#endif
            Update();

            if (type == LogType.Error || type == LogType.Exception || type == LogType.Log)
            {
                Log(logString);
                //Log(stackTrace);
            }
        }

        //�����ҰѴ������Ϣ��������������������ֻ���Ļ��
        static public void Log(params object[] objs)
        {
            string text = "";
            for (int i = 0; i < objs.Length; ++i)
            {
                if (i == 0)
                {
                    text += objs[i].ToString();
                }
                else
                {
                    text += ", " + objs[i].ToString();
                }
            }
            if (Application.isPlaying)
            {
                if (mLines.Count > 20)
                {
                    mLines.RemoveAt(0);
                }
                mLines.Add(text);
            }
        }

        void OnGUI()
        {
            if (logScreen)
            {
                GUI.color = Color.red;
                if (mLines.Count > 0)
                {
                    if (GUI.Button(new Rect(800, 0, 80, 30), "clear"))
                    {
                        mLines.Clear();
                    }
                    for (int i = mLines.Count - 1; i >= 0; i--)
                    {
                        GUILayout.Label(mLines[i]);
                    }
                }
            }
        }
    }
}