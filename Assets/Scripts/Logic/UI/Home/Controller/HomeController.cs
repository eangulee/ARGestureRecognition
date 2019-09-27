using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Logic.UI.Home.Controller
{
    public class HomeController : SingletonMono<HomeController>
    {
        void Awake()
        {
            instance = this;
        }
    }
}