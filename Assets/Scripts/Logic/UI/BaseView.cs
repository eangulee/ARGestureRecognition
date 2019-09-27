using Logic.Enums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace Logic.UI
{
    public class BaseView : MonoBehaviour
    {
        [HideInInspector]
        public BoxCollider boxCollider;
        [HideInInspector]
        public Canvas canvas;
        [HideInInspector]
        public CanvasScaler canvasScaler;
        public UIType uiType = UIType.Normal;

        protected virtual void Awake()
        {
            canvasScaler = gameObject.GetComponent<CanvasScaler>();
            ResetCanvasScaler();//awake中设置CanvasScaler，避免窗口出现闪动缩小效果
        }

        protected virtual void Start()
        {

        }


        public static T Open<T>(string path) where T : BaseView
        {
            T t = UIMgr.instance.OpenView<T>(path);
            return t;
        }

        public virtual void Hide()
        {
            UIMgr.instance.HideView(this.GetType());
        }

        public virtual void Close()
        {
            UIMgr.instance.CloseView(this.GetType());
        }

        //重新更新CanvasScaler
        public virtual void ResetCanvasScaler()
        {
            if (canvasScaler != null)
            {
                canvasScaler.enabled = false;
                canvasScaler.enabled = true;
                canvasScaler.matchWidthOrHeight = 1;
            }
        }
    }
}
