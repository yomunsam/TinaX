using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TinaX.Conf;
using System.Linq;
using UniRx.Async;
using System.Threading.Tasks;

namespace TinaX.I18NKit
{
    public class XI18NMgr : IXI18N
    {
        private IVFS mVFS;
        private I18NConfigModel mConfig;
        private XLanguages mCurSystemLanguage = XLanguages.English;
        private bool mEnable = false;
        private string mCurRegionName;
        private S_I18N_Region? mCurRegion;

        private List<string> mLoadedAssetPath = new List<string>();

        public System.Action<string, string> OnRegionSwitched { get; set; }   
        

        #region I18N Language Pack Cache

        private Dictionary<string, Dictionary<string, string>> mDict_I18NLists = new Dictionary<string, Dictionary<string, string>>();
        /*
         * {
         *      [group_name] = 
         *      {
         *          [key] = value,
         *          [key2] = value2
         *      }
         * }
         * 
         */

        #endregion


        public XI18NMgr(IVFS _vfs)
        {
            mVFS = _vfs;
            mConfig = Config.GetTinaXConfig<I18NConfigModel>(ConfigPath.i18n);
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

            //检查当前语言是否有对应的区域绑定，如果有，使用当前语言对应的区域，如果没有，使用默认区域
            mCurSystemLanguage = GetLanguageByUnityLanguage(Application.systemLanguage);
            if (mConfig.autoMatch)
            {
                if (mCurSystemLanguage != XLanguages.none)
                {
                    //自动适配对应的区域
                    //检查是否有对应的区域
                    if (TryMatchRegionByLanguage(mCurSystemLanguage, out var region))
                    {
                        //有效，使用改语言作为当前语言
                        if (mConfig.LoadLanguageFileFromWebVFS)
                        {
#pragma warning disable CS4014 // 由于此调用不会等待，因此在调用完成前将继续执行当前方法
                            SwitchRegionAsync(region);
#pragma warning restore CS4014 // 由于此调用不会等待，因此在调用完成前将继续执行当前方法
                        }
                        else
                        {
                            SwitchRegion(region);
                        }
                    }
                }
                else
                {
                    UseDefaultRegion();
                }
            }
            else
            {
                UseDefaultRegion();
            }
            

            void UseDefaultRegion()
            {
                if (!mConfig.defaultRegion.IsNullOrEmpty())
                {
                    //使用默认区域
                    //检查默认区域是否有效
                    if (TryGetRegionByName(mConfig.defaultRegion, out var region))
                    {
                        //有效，使用改语言作为当前语言
                        if (mConfig.LoadLanguageFileFromWebVFS)
                        {
#pragma warning disable CS4014 // 由于此调用不会等待，因此在调用完成前将继续执行当前方法
                            SwitchRegionAsync(region);
#pragma warning restore CS4014 // 由于此调用不会等待，因此在调用完成前将继续执行当前方法
                        }
                        else
                        {
                            SwitchRegion(region);
                        }

                    }
                    else
                    {
                        //默认区域无效
                        XLog.PrintE($"[TinaX.I18N Kit] default region name \"{mConfig.defaultRegion}\" is not vaild");
                    }
                }
            }
            
        }

        

        public void Start()
        {

        }

        

        public IXI18N UseRegion(string regionName)
        {
            if (!mEnable) { return this; }
            if (regionName == mCurRegionName) return this;

            if (TryGetRegionByName(regionName, out var region))
            {
                
                SwitchRegion(region);
            }

            return this;
        }

        public async Task UseRegionAsync(string regionName)
        {
            if (!mEnable) return;
            if (regionName == mCurRegionName) return;
            if(TryGetRegionByName(regionName,out var region))
            {
                await SwitchRegionAsync(region);
            }
        }

        public string GetString(string key , string group = I18NConst.DefaultGroupName)
        {
            if (mDict_I18NLists.ContainsKey(group))
            {
                if(mDict_I18NLists[group].TryGetValue(key,out var value))
                {
                    return value;
                }
            }

            return key;
        }

        public bool TryGetString(string key, out string value, string groupName = I18NConst.DefaultGroupName)
        {
            if (mDict_I18NLists.ContainsKey(groupName))
            {
                if (mDict_I18NLists[groupName].TryGetValue(key, out var _value))
                {
                    value = _value;
                    return true;
                }
            }

            value = key;
            return false;
        }

        /// <summary>
        /// Unity的语言枚举转TinaX语言枚举
        /// </summary>
        /// <param name="lang"></param>
        /// <returns></returns>
        private XLanguages GetLanguageByUnityLanguage(SystemLanguage lang)
        {
            switch (lang)
            {
                default:
                    return XLanguages.none;
                case SystemLanguage.Chinese:
                    return XLanguages.ChineseSimplified;
                case SystemLanguage.ChineseSimplified:
                    return XLanguages.ChineseSimplified;
                case SystemLanguage.Basque:
                    return XLanguages.Basque;
                case SystemLanguage.ChineseTraditional:
                    return XLanguages.ChineseTraditional;
                case SystemLanguage.Czech:
                    return XLanguages.Czech;
                case SystemLanguage.English:
                    return XLanguages.English;
                case SystemLanguage.French:
                    return XLanguages.French;
                case SystemLanguage.German:
                    return XLanguages.German;
                case SystemLanguage.Icelandic:
                    return XLanguages.Icelandic;
                case SystemLanguage.Italian:
                    return XLanguages.Italian;
                case SystemLanguage.Japanese:
                    return XLanguages.Japanese;
                case SystemLanguage.Korean:
                    return XLanguages.Korean;
                case SystemLanguage.Norwegian:
                    return XLanguages.Norwegian;
                case SystemLanguage.Polish:
                    return XLanguages.Polish;
                case SystemLanguage.Russian:
                    return XLanguages.Russian;
                case SystemLanguage.Thai:
                    return XLanguages.Thai;
                case SystemLanguage.Turkish:
                    return XLanguages.Turkish;
                case SystemLanguage.Vietnamese:
                    return XLanguages.Vietnamese;
            }
        }


        

        private bool TryGetRegionByName(string regionName,out S_I18N_Region region)
        {
            if(mConfig == null)
            {
                region = default;
                return false;
            }

            var regions = mConfig.Regions.Where(r => r.region_name == regionName);
            if (regions.Count() <= 0)
            {
                region = default;
                return false;
            }
            else
            {
                region = regions.First();
                return true;
            }

        }

        private bool TryMatchRegionByLanguage(XLanguages language, out S_I18N_Region region)
        {
            if (mConfig == null)
            {
                region = default;
                return false;
            }

            var regions = mConfig.Regions.Where(r => r.language_bind == language);
            if (regions.Count() <= 0)
            {
                region = default;
                return false;
            }
            else
            {
                region = regions.First();
                return true;
            }
        }

        /// <summary>
        /// 切换到当前区域
        /// </summary>
        /// <param name="region"></param>
        private async Task SwitchRegionAsync(S_I18N_Region region)
        {
            mDict_I18NLists = new Dictionary<string, Dictionary<string, string>>();
            if(mLoadedAssetPath.Count > 0)
            {
                foreach(var path in mLoadedAssetPath)
                {
                    mVFS.RemoveUse(path);
                }
            }
            mLoadedAssetPath.Clear();

            var origin_region_name = mCurRegionName;

            //读取Json
            if(region.language_json_files != null)
            {
                foreach (var item in region.language_json_files)
                {
                    try
                    {
                        var json_file = await mVFS.LoadAssetLocalOrWebAsync<TextAsset>(item.JsonFilePath);
                        mLoadedAssetPath.Add(item.JsonFilePath);

                        var json_obj = UnityEngine.JsonUtility.FromJson<I18NJsonTpl>(json_file.text);
                        if (json_obj != null && json_obj.data != null)
                        {
                            lock (this)
                            {
                                if (!mDict_I18NLists.ContainsKey(item.GroupName))
                                {
                                    mDict_I18NLists.Add(item.GroupName, new Dictionary<string, string>());
                                }
                                foreach (var kv_item in json_obj.data)
                                {
                                    if (!kv_item.key.IsNullOrEmpty() && !kv_item.value.IsNullOrEmpty())
                                    {
                                        if (!mDict_I18NLists[item.GroupName].ContainsKey(kv_item.key))
                                        {
                                            mDict_I18NLists[item.GroupName].Add(kv_item.key, kv_item.value);
                                        }
                                        else
                                        {
                                            XLog.PrintE($"[TinaX.I18N Kit] Group \"{item.GroupName}\" had the same key \"{kv_item.key}\" , value:\"{mDict_I18NLists[item.GroupName][kv_item.key]}\" and \"{kv_item.value}\"");
                                        }
                                    }
                                }
                            }

                        }

                    }
                    catch (Exceptions.VFSException e)
                    {
                        if (e.ErrorType == Exceptions.VFSException.VFSErrorType.FileNotExist || e.ErrorType == Exceptions.VFSException.VFSErrorType.PathNotValid)
                        {
                            XLog.PrintW("Can't load language file from VFS:" + item.JsonFilePath);
                        }
                    }



                }

            }

            //读取.asset文件
            if(region.language_asset_files != null)
            {
                foreach (var item in region.language_asset_files)
                {
                    if (item.ListFile != null)
                    {
                        if (item.ListFile.Items != null)
                        {
                            lock (this)
                            {
                                if (!mDict_I18NLists.ContainsKey(item.GroupName))
                                {
                                    mDict_I18NLists.Add(item.GroupName, new Dictionary<string, string>());
                                }
                                foreach (var kv_item in item.ListFile.Items)
                                {
                                    if (!kv_item.key.IsNullOrEmpty() && !kv_item.value.IsNullOrEmpty())
                                    {
                                        if (!mDict_I18NLists[item.GroupName].ContainsKey(kv_item.key))
                                        {
                                            mDict_I18NLists[item.GroupName].Add(kv_item.key, kv_item.value);
                                        }
                                        else
                                        {
                                            XLog.PrintE($"[TinaX.I18N Kit] Group \"{item.GroupName}\" had the same key \"{kv_item.key}\" , value:\"{mDict_I18NLists[item.GroupName][kv_item.key]}\" and \"{kv_item.value}\"");
                                        }
                                    }
                                }
                            }

                        }
                    }
                }
            }
            

            //读取Json base64文件，
            if(region.language_json_files_base64 != null)
            {
                foreach (var item in region.language_json_files_base64)
                {
                    try
                    {
                        var json_file = await mVFS.LoadAssetLocalOrWebAsync<TextAsset>(item.JsonFilePath);
                        mLoadedAssetPath.Add(item.JsonFilePath);

                        var json_obj = UnityEngine.JsonUtility.FromJson<I18NJsonTpl>(json_file.text);
                        if (json_obj != null && json_obj.data != null)
                        {
                            lock (this)
                            {
                                if (!mDict_I18NLists.ContainsKey(item.GroupName))
                                {
                                    mDict_I18NLists.Add(item.GroupName, new Dictionary<string, string>());
                                }
                                foreach (var kv_item in json_obj.data)
                                {
                                    if (!kv_item.key.IsNullOrEmpty() && !kv_item.value.IsNullOrEmpty())
                                    {
                                        if (!mDict_I18NLists[item.GroupName].ContainsKey(kv_item.key))
                                        {
                                            mDict_I18NLists[item.GroupName].Add(kv_item.key, kv_item.value.Base64ToStr());
                                        }
                                        else
                                        {
                                            XLog.PrintE($"[TinaX.I18N Kit] Group \"{item.GroupName}\" had the same key \"{kv_item.key}\" , value:\"{mDict_I18NLists[item.GroupName][kv_item.key]}\" and \"{kv_item.value}\"");
                                        }
                                    }
                                }
                            }

                        }

                    }
                    catch (Exceptions.VFSException e)
                    {
                        if (e.ErrorType == Exceptions.VFSException.VFSErrorType.FileNotExist || e.ErrorType == Exceptions.VFSException.VFSErrorType.PathNotValid)
                        {
                            XLog.PrintW("Can't load language file from VFS:" + item.JsonFilePath);
                        }
                    }
                }
            }
            

            mCurRegionName = region.region_name;
            mCurRegion = region;

            OnRegionSwitched?.Invoke(origin_region_name, mCurRegionName);
            XEvent.Call(EventDefine.X_OnI18NRegionChanged, mCurRegionName);

            XLog.Print($"[TinaX.I18N Kit] I18N use region \"<color=#{UnityEngine.ColorUtility.ToHtmlStringRGB(TinaX.Core.XEditorStyleDefine.Color_Blue)}>{mCurRegionName}</color>\" ");
        }


        /// <summary>
        /// 切换到当前区域
        /// </summary>
        /// <param name="region"></param>
        private void SwitchRegion(S_I18N_Region region)
        {
            mDict_I18NLists = new Dictionary<string, Dictionary<string, string>>();
            if (mLoadedAssetPath.Count > 0)
            {
                foreach (var path in mLoadedAssetPath)
                {
                    mVFS.RemoveUse(path);
                }
            }
            mLoadedAssetPath.Clear();


            var origin_region_name = mCurRegionName;
            //读取Json
            if(region.language_json_files != null)
            {
                foreach (var item in region.language_json_files)
                {
                    try
                    {
                        var json_file = mVFS.LoadAsset<TextAsset>(item.JsonFilePath);
                        mLoadedAssetPath.Add(item.JsonFilePath);
                        var json_obj = UnityEngine.JsonUtility.FromJson<I18NJsonTpl>(json_file.text);
                        if (json_obj != null && json_obj.data != null)
                        {
                            if (!mDict_I18NLists.ContainsKey(item.GroupName))
                            {
                                mDict_I18NLists.Add(item.GroupName, new Dictionary<string, string>());
                            }
                            foreach (var kv_item in json_obj.data)
                            {
                                if (!kv_item.key.IsNullOrEmpty() && !kv_item.value.IsNullOrEmpty())
                                {
                                    if (!mDict_I18NLists[item.GroupName].ContainsKey(kv_item.key))
                                    {
                                        mDict_I18NLists[item.GroupName].Add(kv_item.key, kv_item.value);
                                    }
                                    else
                                    {
                                        XLog.PrintE($"[TinaX.I18N Kit] Group \"{item.GroupName}\" had the same key \"{kv_item.key}\" , value:\"{mDict_I18NLists[item.GroupName][kv_item.key]}\" and \"{kv_item.value}\"");
                                    }
                                }
                            }
                        }
                    }
                    catch (Exceptions.VFSException e)
                    {
                        if (e.ErrorType == Exceptions.VFSException.VFSErrorType.FileNotExist || e.ErrorType == Exceptions.VFSException.VFSErrorType.PathNotValid)
                            XLog.PrintW("Can't load language file from VFS:" + item.JsonFilePath);
                    }

                }
            }
            

            //读取.asset文件
            if(region.language_asset_files != null)
            {
                foreach (var item in region.language_asset_files)
                {
                    if (item.ListFile != null)
                    {
                        if (item.ListFile.Items != null)
                        {
                            if (!mDict_I18NLists.ContainsKey(item.GroupName))
                            {
                                mDict_I18NLists.Add(item.GroupName, new Dictionary<string, string>());
                            }
                            foreach (var kv_item in item.ListFile.Items)
                            {
                                if (!kv_item.key.IsNullOrEmpty() && !kv_item.value.IsNullOrEmpty())
                                {
                                    if (!mDict_I18NLists[item.GroupName].ContainsKey(kv_item.key))
                                    {
                                        mDict_I18NLists[item.GroupName].Add(kv_item.key, kv_item.value);
                                    }
                                    else
                                    {
                                        XLog.PrintE($"[TinaX.I18N Kit] Group \"{item.GroupName}\" had the same key \"{kv_item.key}\" , value:\"{mDict_I18NLists[item.GroupName][kv_item.key]}\" and \"{kv_item.value}\"");
                                    }
                                }

                            }
                        }
                    }
                }

            }

            //读取Json base64文件，
            if(region.language_json_files_base64 != null)
            {
                foreach (var item in region.language_json_files_base64)
                {
                    try
                    {
                        var json_file = mVFS.LoadAsset<TextAsset>(item.JsonFilePath);
                        mLoadedAssetPath.Add(item.JsonFilePath);
                        var json_obj = UnityEngine.JsonUtility.FromJson<I18NJsonTpl>(json_file.text);
                        if (json_obj != null && json_obj.data != null)
                        {
                            if (!mDict_I18NLists.ContainsKey(item.GroupName))
                            {
                                mDict_I18NLists.Add(item.GroupName, new Dictionary<string, string>());
                            }
                            foreach (var kv_item in json_obj.data)
                            {
                                if (!kv_item.key.IsNullOrEmpty() && !kv_item.value.IsNullOrEmpty())
                                {
                                    if (!mDict_I18NLists[item.GroupName].ContainsKey(kv_item.key))
                                    {
                                        mDict_I18NLists[item.GroupName].Add(kv_item.key, kv_item.value.Base64ToStr());
                                    }
                                    else
                                    {
                                        XLog.PrintE($"[TinaX.I18N Kit] Group \"{item.GroupName}\" had the same key \"{kv_item.key}\" , value:\"{mDict_I18NLists[item.GroupName][kv_item.key]}\" and \"{kv_item.value}\"");
                                    }
                                }

                            }
                        }

                    }
                    catch (Exceptions.VFSException e)
                    {
                        if (e.ErrorType == Exceptions.VFSException.VFSErrorType.FileNotExist || e.ErrorType == Exceptions.VFSException.VFSErrorType.PathNotValid)
                        {
                            XLog.PrintW("Can't load language file from VFS:" + item.JsonFilePath);
                        }
                    }
                }

            }

            mCurRegionName = region.region_name;
            mCurRegion = region;

            OnRegionSwitched?.Invoke(origin_region_name, mCurRegionName);
            XEvent.Call(EventDefine.X_OnI18NRegionChanged, mCurRegionName);

            XLog.Print($"[TinaX.I18N Kit] I18N use region \"<color=#{UnityEngine.ColorUtility.ToHtmlStringRGB(TinaX.Core.XEditorStyleDefine.Color_Blue)}>{mCurRegionName}</color>\" ");

        }

    }
}

