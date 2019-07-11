using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TinaX.I18N
{
    public class I18NConfig : ScriptableObject
    {
        [Header("启用国际化系统")]
        public bool EnbaleI18N;
        [Header("默认")]
        public string defaultRegion;
        [Header("区域列表")]
        public S_I18N_Region[] Regions;
        [Header("尝试自动匹配语言")]
        public bool autoMatch = false;
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
        [Header("语言文件包")]
#if UNITY_EDITOR && ODIN_INSPECTOR
        [Sirenix.OdinInspector.FilePath]
#endif
        public string[] language_file;
        [Header("语言文件包Base64")]
#if UNITY_EDITOR && ODIN_INSPECTOR
        [Sirenix.OdinInspector.FilePath]
#endif
        public string[] language_file_base64;


        public E_Language language_bind;
    }

    /// <summary>
    /// I18N系统内置语言
    /// </summary>
    [System.Serializable]
    public enum E_Language
    {
        none,
        /// <summary>
        /// 巴斯克
        /// </summary>
        Basque,
        /// <summary>
        /// 简体中文
        /// </summary>
        ChineseSimplified,
        /// <summary>
        /// 繁体中文
        /// </summary>
        ChineseTraditional,
        /// <summary>
        /// 捷克
        /// </summary>
        Czech,
        /// <summary>
        /// 英语
        /// </summary>
        English,
        /// <summary>
        /// 法语
        /// </summary>
        French,
        /// <summary>
        /// 德语
        /// </summary>
        German,
        /// <summary>
        /// 冰岛语
        /// </summary>
        Icelandic,
        /// <summary>
        /// 意大利语
        /// </summary>
        Italian,
        /// <summary>
        /// 日语
        /// </summary>
        Japanese,
        /// <summary>
        /// 韩语
        /// </summary>
        Korean,
        /// <summary>
        /// 挪威语
        /// </summary>
        Norwegian,
        /// <summary>
        /// 波兰语
        /// </summary>
        Polish,
        /// <summary>
        /// 俄语
        /// </summary>
        Russian,
        /// <summary>
        /// 泰语
        /// </summary>
        Thai,
        /// <summary>
        /// 土耳其语
        /// </summary>
        Turkish,
        /// <summary>
        /// 越南语
        /// </summary>
        Vietnamese
    }

}

