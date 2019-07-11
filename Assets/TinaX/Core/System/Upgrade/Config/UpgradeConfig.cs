using UnityEngine;
#if UNITY_EDITOR && ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace TinaX.Upgrade
{

    public class UpgradeConfig : ScriptableObject
    {
        #region 热更新
        /// <summary>
        /// 是否启用热更新功能
        /// </summary>
#if UNITY_EDITOR && ODIN_INSPECTOR
        [Header("启用热更新")]
        [FoldoutGroup("热更新")]
#endif
        public bool Enable_upgrade;

#if UNITY_EDITOR && ODIN_INSPECTOR
        [Header("更新模式")]
        [FoldoutGroup("热更新")]
#endif
        public E_UpgradeMode Upgrade_Mode;

#if UNITY_EDITOR && ODIN_INSPECTOR
        [Header("Framework自动处理更新")]
        [FoldoutGroup("热更新")]
#endif
        public bool Auto_Upgrade;

        #endregion


        #region 热更新_Static_Web_Mode
        /// <summary>
        /// 热更新访问Url
        /// </summary>
#if UNITY_EDITOR && ODIN_INSPECTOR
        [Header("Json索引[默认]URL")]
        [FoldoutGroup("更新配置")]
        [ShowIf("Is_Select_Static_Web_Mode")]
        [InfoBox("未读取到下方的平台配置时，会使用此处的配置")]
#endif
        public string Static_Json_Url_Default;

#if UNITY_EDITOR && ODIN_INSPECTOR
        [Header("Json索引URL与平台配置")]
        [FoldoutGroup("更新配置")]
        [ShowIf("Is_Select_Static_Web_Mode")]
#endif
        public S_StaticWeb_Json_Url_Platform[] Static_Json_Platform;


        #endregion

        /// <summary>
        /// 根据平台信息什么的，获取静态Web更新的Json地址
        /// </summary>
        /// <returns></returns>
        public string GetStaticJsonUrl()
        {
            var myplatform = Platform.GetPlatform(Application.platform);
            foreach(var item in Static_Json_Platform)
            {
                if(item.Platform == myplatform)
                {
                    return item.JsonURL;
                }
            }

            return Static_Json_Url_Default;
        }

        /// <summary>
        /// 如果平台配置覆写了母包版本信息，则获取它
        /// </summary>
        /// <param name="base_version"></param>
        /// <returns></returns>
        public bool GetBaseVersion_IfOverride(out int base_version)
        {
            var myplatform = Platform.GetPlatform(Application.platform);
            foreach (var item in Static_Json_Platform)
            {
                if (item.Platform == myplatform)
                {
                    if (item.Overrides_BaseVersion)
                    {
                        base_version = item.BaseVersion;
                        return true;
                    }
                    break;
                }
            }

            base_version = 0;
            return false;
        }


#if UNITY_EDITOR

        private bool Is_Select_Static_Web_Mode()
        {
            return Upgrade_Mode == E_UpgradeMode.static_web;
        }


#endif

    }




    /// <summary>
    /// 枚举：更新方式
    /// </summary>
    [System.Serializable]
    public enum E_UpgradeMode
    {
        static_web,     //纯静态服务器的更新方式
        cmsn,
    }

    /// <summary>
    /// 静态web更新模式下，特殊平台设定的特殊json索引url
    /// </summary>
    [System.Serializable]
    public struct S_StaticWeb_Json_Url_Platform
    {
        /// <summary>
        /// 平台
        /// </summary>
        public Const.PlatformConst.E_Platform Platform;

        /// <summary>
        /// 
        /// </summary>
        public string JsonURL;

        /// <summary>
        /// 是否重写母包版本
        /// </summary>
        public bool Overrides_BaseVersion;

        /// <summary>
        /// 重写的母包版本号
        /// </summary>
        public int BaseVersion;
    }
}
