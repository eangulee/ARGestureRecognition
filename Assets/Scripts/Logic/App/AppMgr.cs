using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Logic.App
{
    public class AppMgr : SingletonMono<AppMgr>
    {
        void Awake()
        {
            instance = this;
        }
        
        void Start()
        {
            gameObject.AddComponent<Logic.UI.Home.Controller.HomeController>();
            Logic.UI.Home.View.HomeView.OpenView();
        }
    }
}