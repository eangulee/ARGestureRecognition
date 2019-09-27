using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Logic.UI;
using Common.Localization;
using System;
using com.baidu.ai;

namespace Logic.UI.Home.View
{
    public class HomeView : BaseView
    {
        #region prefab path
        public const string PREFAB_PATH = "ui/home/home_view";
        #endregion
        public Text tipsTxt;
        public RawImage rawImage;
        private bool _clicked = false;

        public static HomeView OpenView()
        {
            return BaseView.Open<HomeView>(PREFAB_PATH);
        }

        protected override void Awake()
        {
            base.Awake();
        }

        protected override void Start()
        {
            base.Start();
            Register();
        }

        protected void OnDestroy()
        {
            UnRegister();
        }

#if UNITY_EDITOR
        [ContextMenu("Rotate")]
        public void Rotate()
        {
            rawImage.texture = Common.Util.TextureUtil.AnticlockwiseRotate90((Texture2D)rawImage.mainTexture);
        }
#endif

        private void Register()
        {
            ARFundationController.instance.gestureRecognitionHandler += GestureRecognitionHandler;
            ARFundationController.instance.generateTextureHandler += GenerateTextureHandler;
        }

        private void UnRegister()
        {
            ARFundationController.instance.gestureRecognitionHandler -= GestureRecognitionHandler;
            ARFundationController.instance.generateTextureHandler -= GenerateTextureHandler;
        }

        #region Event Handler
        public void OnClickRecognitionBtnHandler()
        {
            if (_clicked) return;
            _clicked = true;
            tipsTxt.text = LocalizationController.instance.Get("ui.home.tips_recognitioning");
            ARFundationController.instance.GestureRecognition();
        }

        private void GestureRecognitionHandler(bool success, ResultJson result)
        {
            if (!_clicked) return;
            if (success)
            {
                string r = com.baidu.ai.Gesture.GetDescription(result.classname);
                tipsTxt.text = string.Format(LocalizationController.instance.Get("ui.home.tips_recognition_success"), result.probability * 100, r);
            }
            else
            {
                tipsTxt.text = LocalizationController.instance.Get("ui.home.tips_recognition_fail");
            }
            _clicked = false;
        }

        private void GenerateTextureHandler(Texture2D texture2D)
        {
            rawImage.texture = ARFundationController.instance.m_Texture;
        }
        #endregion
    }
}