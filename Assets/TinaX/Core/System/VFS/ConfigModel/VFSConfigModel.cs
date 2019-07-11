using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR && ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace TinaX.VFS
{
    
    public class VFSConfigModel : ScriptableObject
    {

        #region Conifg_目录设定

#if UNITY_EDITOR && ODIN_INSPECTOR
        [FoldoutGroup("资源目录设定")]
        [Header("文件管理白名单")]
        [FolderPath]
#else
        [Header("[资源目录] 文件管理白名单")]
#endif
        public string[] Assets_system_whiteList =
        {

        };

#if UNITY_EDITOR && ODIN_INSPECTOR
        [FoldoutGroup("资源目录设定")]
        [Header("文件夹特殊打包规则")]
#else
        [Header("[资源目录] 文件夹特殊打包规则名单")]
#endif
        
        public S_Dir_PackInfo[] Special_Package_Folder =
        {

        };

        /// <summary>
        /// 忽略配置的路径及其子路径的文件
        /// </summary>
#if UNITY_EDITOR && ODIN_INSPECTOR
        [Header("打包忽略路径")]
        [FoldoutGroup("资源目录设定")]
        [FolderPath]
#else
        [Header("[资源目录] 打包忽略路径")]
#endif
        public string[] Ignore_Path =
        {

        };

        /// <summary>
        /// 忽略配置中的后缀名（不加"."点）
        /// </summary>
#if UNITY_EDITOR && ODIN_INSPECTOR
        [Header("打包忽略后缀名")]
        [FoldoutGroup("资源目录设定")]
#endif
        public string[] Ignore_Ext =
        {

        };

        /// <summary>
        /// 如果文件路径中包含相关关键字，则忽略
        /// </summary>
#if UNITY_EDITOR && ODIN_INSPECTOR
        [Header("打包忽略路径关键字")]
        [FoldoutGroup("资源目录设定")]
#endif
        public string[] Ignore_Path_keyword =
        {
            
        };

#if UNITY_EDITOR && ODIN_INSPECTOR
        [Header("资源[文件]加密名单")]
        [FoldoutGroup("资源目录设定")]
        [FilePath]
#endif
        public string[] EncryFileList =
        {

        };

#if UNITY_EDITOR && ODIN_INSPECTOR
        [Header("资源[路径]加密名单")]
        [FoldoutGroup("资源目录设定")]
        [FolderPath]
#endif
        public string[] EncryPathList =
        {

        };




        #endregion

        #region Config 特殊资源处理

#if UNITY_EDITOR && ODIN_INSPECTOR
        [FoldoutGroup("资源处理规则")]
        [Header("处理导入文件中的中文")]
#endif
#if UNITY_EDITOR
        public bool EnableChineseFileInput = false;
#endif

#if UNITY_EDITOR && ODIN_INSPECTOR
        [ShowIf("EnableChineseFileInput")]
        [FoldoutGroup("资源处理规则")]
        [Header("导入文件名包含中文时的处理方式")]
#endif
#if UNITY_EDITOR
        public InputFileNameIncludeChineseHandleType ChineseHandleType = InputFileNameIncludeChineseHandleType.Ignore;
#endif

#if UNITY_EDITOR && ODIN_INSPECTOR
        [FoldoutGroup("资源处理规则")]
        [Header("导入图片到UI目录时自动设置为Sprite")]
#endif
#if UNITY_EDITOR
        public bool EnableTextureToSpriteInUIAssetsFolder = true;
#endif

        #endregion




    }

    /// <summary>
    /// 目录打包类型
    /// </summary>
    [System.Serializable]
    public enum E_Dir_PackType
    {
        [Header("普通打包")]
        normal,
        /// <summary>
        /// 指定目录整合打包
        /// </summary>
        [Header("整体打包")]
        whole,
        /// <summary>
        /// 指定目录的每个子目录整合打包
        /// </summary>
        [Header("子目录整体打包")]
        sub_dir,
    }

    /// <summary>
    /// 目录打包设定
    /// </summary>
    [System.Serializable]
    public struct S_Dir_PackInfo
    {
#if UNITY_EDITOR && ODIN_INSPECTOR
        [FolderPath]
#endif
        public string FolderName;
        //[EnumLabel("打包方式")]
        public E_Dir_PackType PackType;
    }

    /// <summary>
    /// 文件信息
    /// </summary>
    public struct S_FileInfo
    {
        /// <summary>
        /// 文件路径
        /// </summary>
        public string file_path;
        /// <summary>
        /// 对应assetbundle名
        /// </summary>
        public string ab_name;
        /// <summary>
        /// assetbundle包中的文件名
        /// </summary>
        public string file_name_in_ab;
        /// <summary>
        /// 文件处理方式标记
        /// </summary>
        public E_FileHandleTag handle_tag;

        public string invalidInfo;

    }

    /// <summary>
    /// 文件处理标记
    /// </summary>
    public enum E_FileHandleTag
    {
        /// <summary>
        /// 无效（不被TinaX资源系统处理）
        /// </summary>
        invalid,
        /// <summary>
        /// 单文件打包
        /// </summary>
        single,
        /// <summary>
        /// 被命中特殊打包规则
        /// </summary>
        special
    }


    /// <summary>
    /// 导入文件中包含中文的处理方式
    /// </summary>
    public enum InputFileNameIncludeChineseHandleType
    {
        /// <summary>
        /// 忽略（不要管）
        /// </summary>
        Ignore,
        /// <summary>
        /// 删除
        /// </summary>
        Delete,
        /// <summary>
        /// 改名
        /// </summary>
        ReName,

    }

}

