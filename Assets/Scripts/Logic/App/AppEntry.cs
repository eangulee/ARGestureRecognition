using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Logic.App
{
    public class AppEntry : SingletonMono<AppEntry>
    {

        void Awake()
        {
            instance = this;
        }

        void Start()
        {
            Debug.Log("app is start.");
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            Application.targetFrameRate = 30;
            gameObject.AddComponent<AppMgr>();
        }
    }
}