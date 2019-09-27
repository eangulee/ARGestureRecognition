using UnityEngine;
using System.Collections;
using System.IO;
namespace Common.Localization
{
    public class LocalizationController : SingletonMono<LocalizationController>
    {
        void Awake()
        {
            instance = this;
            Localization.loadFunction = LoadFunction;
            InitLanguagePack();
        }

        private void InitLanguagePack()
        {
            SystemLanguage systemLanguage = Application.systemLanguage;
            //Debug.Log(systemLanguage.ToString());
            //Localization.language = ENGLISH; return;
            switch (systemLanguage)
            {
                case SystemLanguage.Chinese:
#if UNITY_IOS
                    Localization.language = SystemLanguage.ChineseSimplified.ToString();
#elif UNITY_ANDROID
                    Localization.language = SystemLanguage.ChineseSimplified.ToString();
#elif UNITY_EDITOR
                    Localization.language = SystemLanguage.ChineseSimplified.ToString();
#endif
                    break;
                case SystemLanguage.ChineseSimplified:
                    Localization.language = SystemLanguage.ChineseSimplified.ToString();
                    break;
                case SystemLanguage.ChineseTraditional:
                    Localization.language = SystemLanguage.ChineseTraditional.ToString();
                    break;
                case SystemLanguage.English:
                    Localization.language = SystemLanguage.English.ToString();
                    break;
                default:
                    Localization.language = systemLanguage.ToString();
                    break;
            }
        }

        private byte[] LoadFunction(string path)
        {
            if (path == "Localization") return null;
            //Debug.Log("LoadFunction:{0}", path);
            byte[] bytes = Logic.ResMgr.ResMgr.instance.LoadBytes(Path.Combine("config/languages/", path));
            if (bytes == null)
                bytes = Logic.ResMgr.ResMgr.instance.LoadBytes(Path.Combine("config/languages/",  SystemLanguage.English.ToString()));
            return bytes;
        }

        public string Get(string key)
        {
            return Localization.Get(key);
        }

        public void ClearLanguagePack()
        {
            Localization.localizationHasBeenSet = false;
        }
    }
}
