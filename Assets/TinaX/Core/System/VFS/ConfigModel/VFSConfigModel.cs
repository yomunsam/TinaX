using UnityEngine;
#if UNITY_EDITOR && ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif
using System;

namespace TinaX.VFSKit
{
    /// <summary>
    /// VFS 配置定义模板
    /// </summary>
    public class VFSConfigModel : ScriptableObject,ICloneable
    {
        /// <summary>
        /// VFS 实现模式
        /// </summary>
        [Header("VFS Mode")]
        public VFSMode FileMode = VFSMode.AssetBundle;

        /// <summary>
        /// 文件白名单
        /// </summary>
#if UNITY_EDITOR && ODIN_INSPECTOR
        [FoldoutGroup("Assets Manager")]
        [FolderPath(AbsolutePath =false,RequireExistingPath =true)]
#endif
        [Header("VFS 文件管理白名单")]
        public string[] VFS_WhiteList = {};


        #region Assets Manager

        /// <summary>
        /// 文件特殊打包规则
        /// </summary>
#if UNITY_EDITOR && ODIN_INSPECTOR
        [FoldoutGroup("Assets Manager")]
#endif
        [Header("VFS 特殊打包规则")]
        public FolderPackageRule[] FolderPackRule = { };


#if UNITY_EDITOR && ODIN_INSPECTOR
        [FoldoutGroup("Assets Manager")]
#endif
        [Header("全局忽略路径")]
        public string[] Ignore_Path = { };

#if UNITY_EDITOR && ODIN_INSPECTOR
        [FoldoutGroup("Assets Manager")]
        [InfoBox("如果希望忽略路径下的某个子路径参与打包，则同时添加到白名单和该名单列表中。")]
#endif
        [Header("忽略路径中的白名单")]
        public string[] Ignore_Path_WhiteList = { };

#if UNITY_EDITOR && ODIN_INSPECTOR
        [FoldoutGroup("Assets Manager")]
#endif
        [Header("全局忽略后缀名")]
        public string[] Ignore_ExtName = { };

#if UNITY_EDITOR && ODIN_INSPECTOR
        [FoldoutGroup("Assets Manager")]
#endif
        [Header("全局忽略路径项")]
        public string[] Ignore_Path_Item_Keyword = { };

#if UNITY_EDITOR && ODIN_INSPECTOR
        [FoldoutGroup("Assets Manager")]
#endif
        [Header("全局忽略路径关键字")]
        public string[] Ignore_Path_Keyword = { };

        /*
         * Ignore_Path_Item_Keyword 与 Ignore_Path_Keyword 的区别
         * 比如有路径：
         *      路径一： Assets/ABC/DEF/aaa.txt
         *      路径二： Assets/ABCDE/DEFG/aaa.txt
         *      路径三： Assets/abc/DEF/aaa.txt
         * 
         * 如果在Ignore_Path_Item_Keyword中定义“abc”，则路径一和路径三被忽略，
         * 如果在Ignore_Path_Keyword中定义"abc"，则路径一、二、三都被忽略
         * 
         */

        #endregion

        #region Web加载资源

#if UNITY_EDITOR && ODIN_INSPECTOR
        [FoldoutGroup("Web VFS")]
        [ShowIf("IsAssetBundleMode")]
#endif
        [Header("启用 以Web方式加载资源")]
        public bool EnableWebVFS = false;

#if UNITY_EDITOR && ODIN_INSPECTOR
        [FoldoutGroup("Web VFS")]
        [ShowIf("IsAssetBundleMode")]
        [ShowIf("EnableWebVFS")]
#endif
        [Header("以Web方式加载资源的基础Url路径")]
        public WebVFSConfig[] ConfigWebVFS;




        #endregion

        #region 资源加密

#if UNITY_EDITOR && ODIN_INSPECTOR
        [FoldoutGroup("Assets Encry")]
        [InlineButton("DoCheckEncryConfig", "Check Encry Config")]
#endif
        [Header("资源加密")]
        public FolderEncryDefine[] VFS_EncryFolder = { };

#if UNITY_EDITOR && ODIN_INSPECTOR
        [FoldoutGroup("Assets Encry")]
#else
        [EnumLabel("Offset 加密处理方式")]
#endif
        [Header("Offset 加密处理方式")]
        public EncryOffsetType Encry_OffsetHandleType = EncryOffsetType.Default;

        //[Header("RSA 私钥（解密数据）")]
        //public string RSAPrivateKey;


        #endregion

#if UNITY_EDITOR

#if UNITY_EDITOR && ODIN_INSPECTOR
        [FoldoutGroup("Assets Input Rule")]
#endif
        [Header("处理导入的非字母或数字文件")]
        public bool HandlerInputNotLettersOrNums = false;

#if UNITY_EDITOR && ODIN_INSPECTOR
        [FoldoutGroup("Assets Input Rule")]
        [ShowIf("HandlerInputNotLettersOrNums")]
#endif
        [Header("当导入了非字母、数字或下划线命名的文件的处理方式")]
        public InputNotLettersOrNumsHandleType NotLettersOrNumsHandleType = InputNotLettersOrNumsHandleType.Warning;
#endif

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        /// <summary>
        /// 检查加密配置，如果配置正常返回true,有错误配置返回false
        /// </summary>
        /// <returns></returns>

        public bool CheckEncryConfig(bool Log = false, bool MsgBoxInEditor = false)
        {
            var flag = true;
            //加密Folder的设置粒度不可以大于打包粒度，所以这里要检查特殊打包设置中的
            foreach(var item in VFS_EncryFolder)
            {
                var _folder = item.FolderPath;
                if (!string.IsNullOrEmpty(_folder))
                {
                    if (!_folder.EndsWith("/")) _folder += "/";

                    foreach (var pack in FolderPackRule)
                    {
                        var p_folder = pack.FolderPath;
                        if (!string.IsNullOrEmpty(p_folder))
                        {
                            if (!p_folder.EndsWith("/")) p_folder += "/";

                            if (pack.PackType == FolderPackageType.whole)
                            {
                                if (_folder.StartsWith(p_folder) && _folder.Length > p_folder.Length)
                                {
                                    if (Log)
                                    {
                                        Debug.LogError("配置冲突：加密设置中的目录不可小于打包粒度, 加密Folder: " + item.FolderPath + "\n与之冲突的打包规则：" + pack.FolderPath);
                                    }
                                    flag = false;
#if UNITY_EDITOR
                                    if (MsgBoxInEditor)
                                    {
                                        if (Application.systemLanguage == SystemLanguage.Chinese || Application.systemLanguage == SystemLanguage.ChineseSimplified)
                                        {
                                            UnityEditor.EditorUtility.DisplayDialog("配置冲突", $"配置冲突: \n设置中的加密Folder路径不可小于打包粒度。\n加密Folder:{item.FolderPath}\n与之冲突的打包规则:{pack.FolderPath}\n打包方式：{pack.PackType.ToString()}", "好");
                                        }
                                        else
                                        {
                                            UnityEditor.EditorUtility.DisplayDialog("Error Config", $"Configuration conflict: \nEncryption folder settings should not be less than packing granularity\nPath of encry rule:{item.FolderPath}\nConflicting packing rules:{pack.FolderPath}\nPacking method:{pack.PackType.ToString()}", "OK");
                                        }
                                    }

#endif
                                }
                            }
                            if(pack.PackType == FolderPackageType.sub_dir)
                            {
                                if (_folder.StartsWith(p_folder) && _folder.Length > p_folder.Length && _folder.Split('/').Length > (p_folder.Split('/').Length + 1) )
                                {
                                    if (Log)
                                    {
                                        Debug.LogError("配置冲突：加密设置中的目录不可小于打包粒度，加密Folder: " + item.FolderPath + "\n与之冲突的打包规则：" + pack.FolderPath);
                                    }
                                    flag = false;
#if UNITY_EDITOR
                                    if (MsgBoxInEditor)
                                    {
                                        if (Application.systemLanguage == SystemLanguage.Chinese || Application.systemLanguage == SystemLanguage.ChineseSimplified)
                                        {
                                            UnityEditor.EditorUtility.DisplayDialog("配置冲突", $"配置冲突: \n设置中的加密Folder路径不可小于打包粒度。\n加密Folder:{item.FolderPath}\n与之冲突的打包规则:{pack.FolderPath}\n打包方式：{pack.PackType.ToString()}", "好");
                                        }
                                        else
                                        {
                                            UnityEditor.EditorUtility.DisplayDialog("Error Config", $"Configuration conflict: \nEncryption folder settings should not be less than packing granularity\nPath of encry rule:{item.FolderPath}\nConflicting packing rules:{pack.FolderPath}\nPacking method:{pack.PackType.ToString()}", "OK");
                                        }
                                    }
                                    
#endif
                                }
                            }
                        }
                        


                    }
                }
                

            }

            return flag;
        }


#if UNITY_EDITOR
        public void DoCheckEncryConfig()
        {
            if (CheckEncryConfig(true,true))
            {
                UnityEditor.EditorUtility.DisplayDialog("Success", "All configurations are correct\n配置看起来没什么不对的", "OK");
            }
        }


        private bool IsAssetBundleMode()
        {
            return FileMode == VFSMode.AssetBundle;
        }

#endif

    }

    

}

