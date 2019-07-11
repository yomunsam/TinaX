using System;
using UnityEngine;

namespace TinaX
{
    public static class XApp
    {

        #region 公开属性
        /// <summary>
        /// 框架版本名
        /// </summary>
        public static string VersionName
        {
            get
            {
                return FrameworkInfo.FrameworkVersionName;
            }
        }

        /// <summary>
        /// 框架版本号
        /// </summary>
        public static int VersionCode
        {
            get
            {
                return FrameworkInfo.FrameworkVersionCode;
            }
        }

        /// <summary>
        /// App版本号
        /// </summary>
        public static int AppVersionCode
        {
            get
            {
                var mainconf = Config.GetTinaXConfig<MainConfig>(Conf.ConfigPath.main);
                if (mainconf == null)
                {
                    return 0;
                }
                else
                {
                    return mainconf.Version_Code;
                }
            }
        }

        /// <summary>
        /// 可读写应用存储路径（沙箱路径）
        /// </summary>
        public static string LocalStorage
        {
            get
            {
                return UnityEngine.Application.persistentDataPath + "/" + Setup.Framework_LocalStorage_App;
            }
        }

        /// <summary>
        /// 屏幕方向
        /// </summary>
        public static ScreenOrientation ScreenOrientation
        {
            get
            {
                return Screen.orientation;
            }
        }

        #endregion
    }
}