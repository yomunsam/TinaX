using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TinaX.I18N;
using TinaX;

namespace TinaXEditor.I18N
{
    /// <summary>
    /// I18N编辑器时语言包缓存与预读系统
    /// </summary>
    [InitializeOnLoad]
    public static class XI18NCacheEditor
    {
        /// <summary>
        /// 当前配置
        /// </summary>
        private static I18NConfig mCurConfig;
        /// <summary>
        /// 当前配置中的所有区域
        /// </summary>
        private static List<string> mCurRegions = new List<string>();

        /// <summary>
        /// 当前默认区域中的所有语言包缓存。（只有配置的默认语言会缓存，以节省内存资源）
        /// </summary>
        private static List<S_LanguageFile_Info> mCurRegionLanguageFilesInfo = new List<S_LanguageFile_Info>();
        
        static XI18NCacheEditor()
        {
            //添加配置文件到资源监听
            AssetInputHandle.AddAssetListener("Assets/Resources/" + TinaX.Setup.Framework_Config_Path + "/" + TinaX.Conf.ConfigPath.i18n, OnConfigFileChanged);



            mCurConfig = TinaX.Config.GetTinaXConfig<I18NConfig>(TinaX.Conf.ConfigPath.i18n);
            if (mCurConfig != null)
            {
                RefreshLanguageFiles();
            }
        }
        
        /// <summary>
        /// 获取当前定义的所有Region
        /// </summary>
        /// <returns></returns>
        public static string[] GetRegions()
        {
            return mCurRegions.ToArray();
        }

        public static string GetGameStringByKey(string key, out string fileName)
        {
            if (mCurConfig == null) { fileName = "unknow"; return key; }
            foreach(var item in mCurRegionLanguageFilesInfo)
            {
                if(item.region == mCurConfig.defaultRegion)
                {
                    bool exist;
                    string value = item.GetGameStringByKey(key,out exist);
                    if (exist)
                    {
                        fileName = System.IO.Path.GetFileName(item.path);
                        return value;
                    }
                }
            }
            fileName = "unknow";
            return key;
        }

        public static string GetGameStringByKey(string key)
        {
            if (mCurConfig == null) { return key; }
            foreach (var item in mCurRegionLanguageFilesInfo)
            {
                if (item.region == mCurConfig.defaultRegion)
                {
                    bool exist;
                    string value = item.GetGameStringByKey(key, out exist);
                    if (exist)
                    {
                        
                        return value;
                    }
                }
            }
            return key;
        }




        private static void OnConfigFileChanged(string path,AssetInputHandle.ResChangeType type)
        {
            if (type == AssetInputHandle.ResChangeType.CreateOrModify)
            {
                mCurConfig = TinaX.Config.GetTinaXConfig<I18NConfig>(TinaX.Conf.ConfigPath.i18n);
                if (mCurConfig != null)
                {
                    RefreshLanguageFiles();
                }
            }
        }

        /// <summary>
        /// 语言包缓存是否存在
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private static bool IsLangCacheExist(string path)
        {
            foreach(var item in mCurRegionLanguageFilesInfo)
            {
                if(item.path == path)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 如果语言包缓存存在，则删掉ta
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private static void RemoveIfLangCacheExist(string path)
        {
            foreach (var item in mCurRegionLanguageFilesInfo)
            {
                if (item.path == path)
                {
                    mCurRegionLanguageFilesInfo.Remove(item);
                    return;
                }
            }
            
        }

        private static void RefreshLanguageFiles()
        {
            if(mCurConfig == null)
            {
                return;
            }
            mCurRegionLanguageFilesInfo = new List<S_LanguageFile_Info>();
            //遍历所有区域
            foreach (var item in mCurConfig.Regions)
            {
                mCurRegions.Add(item.region_name);
                if(item.region_name == mCurConfig.defaultRegion)
                {
                    //把语言包统统缓存起来
                    foreach(var lang in item.language_file)
                    {
                        RemoveIfLangCacheExist(lang);
                        var textasset = AssetDatabase.LoadAssetAtPath<TextAsset>(lang);
                        if (textasset != null)
                        {
                            mCurRegionLanguageFilesInfo.Add(new S_LanguageFile_Info()
                            {
                                path = lang,
                                region = item.region_name,
                                isBase64 = false,
                                data = JsonUtility.FromJson<I18NJsonTpl>(textasset.text)
                            });
                        }
                        //把语言添加进变动监听
                        AssetInputHandle.AddAssetListener(lang, OnLangJsonFileChanged);
                    }

                    foreach (var lang in item.language_file_base64)
                    {
                        if (!IsLangCacheExist(lang))
                        {
                            var textasset = AssetDatabase.LoadAssetAtPath<TextAsset>(lang);
                            if (textasset != null)
                            {
                                mCurRegionLanguageFilesInfo.Add(new S_LanguageFile_Info()
                                {
                                    path = lang,
                                    region = item.region_name,
                                    isBase64 = true,
                                    data = JsonUtility.FromJson<I18NJsonTpl>(textasset.text)
                                });
                            }

                        }
                        //把语言添加进变动监听
                        AssetInputHandle.AddAssetListener(lang, OnLangJsonFileChanged);
                    }

                    
                }
            }


        }

        private static void OnLangJsonFileChanged(string path, AssetInputHandle.ResChangeType type)
        {
            if(type == AssetInputHandle.ResChangeType.Remove)
            {
                RemoveIfLangCacheExist(path);
                AssetInputHandle.RemoveAssetListener(path, OnLangJsonFileChanged);
            }
            if(type == AssetInputHandle.ResChangeType.CreateOrModify)
            {
                foreach(var item in mCurRegionLanguageFilesInfo)
                {
                    if (item.path == path)
                    {
                        item.RefreshFile();
                        break;
                    }
                }
            }
        }


        /// <summary>
        /// 语言包文件信息
        /// </summary>
        private struct S_LanguageFile_Info
        {
            /// <summary>
            /// 文件路径
            /// </summary>
            public string path;
            public string region;
            public bool isBase64;
            public I18NJsonTpl data;

            private Dictionary<string, string> mJsonKV;

            public string GetGameStringByKey(string key,out bool exist)
            {
                if (mJsonKV == null)
                {
                    GenJsonKVCache();
                }
                if (mJsonKV.ContainsKey(key))
                {
                    exist = true;
                    return mJsonKV[key];
                }
                else
                {
                    exist = false;
                    return key;
                }
            }

            /// <summary>
            /// 文件有变动时，调用这里刷新解析
            /// </summary>
            /// <returns></returns>
            public void RefreshFile()
            {
                var text = AssetDatabase.LoadAssetAtPath<TextAsset>(path);
                if(text != null)
                {
                    data = JsonUtility.FromJson<I18NJsonTpl>(text.text);
                    GenJsonKVCache();
                }
            }


            private void GenJsonKVCache()
            {
                mJsonKV = new Dictionary<string, string>();
                foreach(var item in data.data)
                {
                    if (mJsonKV.ContainsKey(item.key))
                    {
                        if (isBase64)
                        {
                            mJsonKV[item.key] = item.value.Base64ToStr();
                        }
                        else
                        {
                            mJsonKV[item.key] = item.value;
                        }

                    }
                    else
                    {
                        if (isBase64)
                        {
                            mJsonKV.Add(item.key, item.value.Base64ToStr());
                        }
                        else
                        {
                            mJsonKV.Add(item.key, item.value);
                        }
                            
                    }
                }
            }
        }

        

    }

}

