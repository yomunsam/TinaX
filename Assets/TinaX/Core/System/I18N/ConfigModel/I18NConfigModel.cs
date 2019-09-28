using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR && ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace TinaX.I18NKit
{
    public class I18NConfigModel : ScriptableObject
    {
        [Header("启用国际化系统")]
        public bool EnbaleI18N;
        [Header("默认")]
        public string defaultRegion;
        [Header("区域列表")]
        public S_I18N_Region[] Regions;
        [Header("尝试自动匹配语言")]
        public bool autoMatch = false;

        [Header("允许从WebVFS加载语言文件")]
        public bool LoadLanguageFileFromWebVFS = false;
    }

    /// <summary>
    /// I18N 地区
    /// </summary>
    [System.Serializable]
    public struct S_I18N_Region
    {
        /// <summary>
        /// 区域名称
        /// </summary>
        [Header("区域名")]
        public string region_name;

        [Header("语言定义文件")]
        public S_LanguageJsonFiles[] language_json_files;

        [Header("语言定义表")]
        public S_LanguageAssetFiles[] language_asset_files;

        [Header("语言定义文件Base64")]
        public S_LanguageJsonFiles[] language_json_files_base64;

        


        [Header("语言绑定")]
        public XLanguages language_bind;

    }

    [System.Serializable]
    public class S_LanguageJsonFiles
    {
#if UNITY_EDITOR && ODIN_INSPECTOR
        [Sirenix.OdinInspector.FilePath]
#endif
        [Header("Json File Path")]
        public string JsonFilePath;

        [Header("Group Name")]
        public string GroupName = I18NConst.DefaultGroupName;

    }


    [System.Serializable]
    public class S_LanguageAssetFiles
    {
#if UNITY_EDITOR && ODIN_INSPECTOR
        [AssetsOnly]
        [InlineEditor(InlineEditorModes.GUIAndPreview)]
#endif
        [Header("I18N List")]
        public I18NListModel ListFile;

        [Header("Group Name")]
        public string GroupName = I18NConst.DefaultGroupName;
    }

}

