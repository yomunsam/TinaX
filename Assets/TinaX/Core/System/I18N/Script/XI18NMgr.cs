using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TinaX.Conf;

namespace TinaX.I18N
{
    public class XI18NMgr : IXI18N
    {
        private IVFS mVFS;
        private I18NConfig mConfig;
        private E_Language mCurSystemLanguage = E_Language.Vietnamese;
        private bool mEnable = false;
        private string mCurRegion;
        /// <summary>
        /// 当前语言包中的语言字典
        /// </summary>
        private Dictionary<string, string> m_dict_cur_language = new Dictionary<string, string>();


        public XI18NMgr(IVFS _vfs)
        {
            mVFS = _vfs;
            mConfig = Config.GetTinaXConfig<I18NConfig>(ConfigPath.i18n);
            if (mConfig == null)
            {
                mEnable = false;
                return;
            }
            mEnable = mConfig.EnbaleI18N;
            if (!mEnable)
            {
                return;
            }
            mCurSystemLanguage = GetLanguageByUnityLanguage(Application.systemLanguage);
            string _region = "none";
            if (mConfig.defaultRegion != null && mConfig.defaultRegion != "")
            {
                _region = mConfig.defaultRegion;
            }
            if (mConfig.autoMatch)
            {
                foreach (var item in mConfig.Regions)
                {
                    if(item.language_bind == mCurSystemLanguage)
                    {
                        _region = item.region_name;
                    }
                }
            }
            if (_region != "none")
            {
                mCurRegion = _region;
                UseRegion(mCurRegion);
            }
        }

        public void Start()
        {

        }

        public IXI18N UseRegion(string region)
        {
            if (!mEnable) { return this; }
            Debug.Log("[TinaX][I18N]启用地区：" + region);
            //将定义语言的所有语言文件读入
            var region_s = GetRegionByStr(region);
            //读普通Json
            foreach(var item in region_s.language_file)
            {
                var json_file = mVFS.LoadAsset<TextAsset>(item);
                if(json_file != null)
                {
                    var jsonObj = JsonUtility.FromJson<I18NJsonTpl>(json_file.text);
                    foreach(var v in jsonObj.data)
                    {
                        if(v.key != "" && v.value != "" && v.key != null && v.value != null)
                        {
                            if (!m_dict_cur_language.ContainsKey(v.key))
                            {
                                m_dict_cur_language.Add(v.key, v.value);
                            }
                            else
                            {
                                m_dict_cur_language[v.key] = v.value;
                            }
                        }
                    }
                }
            }
            //读Base64
            foreach (var item in region_s.language_file_base64)
            {
                var json_file = mVFS.LoadAsset<TextAsset>(item);
                if (json_file != null)
                {
                    var jsonObj = JsonUtility.FromJson<I18NJsonTpl>(json_file.text);
                    foreach (var v in jsonObj.data)
                    {
                        if (v.key != "" && v.value != "" && v.key != null && v.value != null)
                        {
                            if (!m_dict_cur_language.ContainsKey(v.key))
                            {
                                m_dict_cur_language.Add(v.key, v.value.Base64ToStr());
                            }
                            else
                            {
                                m_dict_cur_language[v.key] = v.value.Base64ToStr();
                            }
                        }
                    }
                }
            }

            return this;
        }


        public string GetString(string key)
        {
            if (m_dict_cur_language.ContainsKey(key))
            {
                return m_dict_cur_language[key];
            }
            else
            {
                return key;
            }
        }


        /// <summary>
        /// Unity的语言枚举转TinaX语言枚举
        /// </summary>
        /// <param name="lang"></param>
        /// <returns></returns>
        private E_Language GetLanguageByUnityLanguage(SystemLanguage lang)
        {
            switch (lang)
            {
                default:
                    return E_Language.none;
                case SystemLanguage.Chinese:
                    return E_Language.ChineseSimplified;
                case SystemLanguage.ChineseSimplified:
                    return E_Language.ChineseSimplified;
                case SystemLanguage.Basque:
                    return E_Language.Basque;
                case SystemLanguage.ChineseTraditional:
                    return E_Language.ChineseTraditional;
                case SystemLanguage.Czech:
                    return E_Language.Czech;
                case SystemLanguage.English:
                    return E_Language.English;
                case SystemLanguage.French:
                    return E_Language.French;
                case SystemLanguage.German:
                    return E_Language.German;
                case SystemLanguage.Icelandic:
                    return E_Language.Icelandic;
                case SystemLanguage.Italian:
                    return E_Language.Italian;
                case SystemLanguage.Japanese:
                    return E_Language.Japanese;
                case SystemLanguage.Korean:
                    return E_Language.Korean;
                case SystemLanguage.Norwegian:
                    return E_Language.Norwegian;
                case SystemLanguage.Polish:
                    return E_Language.Polish;
                case SystemLanguage.Russian:
                    return E_Language.Russian;
                case SystemLanguage.Thai:
                    return E_Language.Thai;
                case SystemLanguage.Turkish:
                    return E_Language.Turkish;
                case SystemLanguage.Vietnamese:
                    return E_Language.Vietnamese;
            }
        }


        private S_I18N_Region GetRegionByStr(string region)
        {
            if (mConfig == null)
            {
                return new S_I18N_Region();
            }
            foreach(var item in mConfig.Regions)
            {
                if (item.region_name == region)
                {
                    return item;
                }
            }
            return new S_I18N_Region();
        }
    }
}

