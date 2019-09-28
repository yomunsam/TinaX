using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TinaX.VFSKit;

namespace TinaX.Conf
{
    public class ConfigPath
    {

        /// <summary>
        /// 一个特殊的基础配置文件
        /// framework内部会用它来记录一些基础信息，
        /// 这个framework不会完全暴露给用户进行配置
        /// </summary>
        public static string base_config = "tinax_base_config.asset";


        /// <summary>
        /// 主配置文件
        /// </summary>
        public const string main = "tinax_config.asset";

        /// <summary>
        /// 资源系统（虚拟文件系统
        /// </summary>
        public const string vfs = "vfs.asset";
        /// <summary>
        /// UI系统
        /// </summary>
        public const string uikit = "uikit.asset";
        /// <summary>
        /// 国际化系统
        /// </summary>
        public const string i18n = "i18n.asset";

        /// <summary>
        /// Lua Script
        /// </summary>
        public const string lua = "lua_script.asset";

        //public const string vfs2 = "vfs2.asset";

        public const string upgrade = "upgrade.asset";


    }

#if UNITY_EDITOR
    public class ConfigRegister
    {
        public static S_ConfigInfo[] ConfigRegisters = new S_ConfigInfo[]
        {
            new S_ConfigInfo()
            {
                Title = "主框架配置",
                Action_Create = () =>
                {

                    return Config.CreateIfNotExist<MainConfig>(ConfigPath.main);

                },
                Action_GetInstance = () =>
                {
                    return Config.GetTinaXConfig<MainConfig>(ConfigPath.main);
                }
            },
            new S_ConfigInfo()
            {
                Title = "虚拟文件系统",
                Action_Create = () =>
                {
                    return Config.CreateIfNotExist<VFSConfigModel>(ConfigPath.vfs);
                },
                Action_GetInstance = () =>
                {
                    return Config.GetTinaXConfig<VFSConfigModel>(ConfigPath.vfs);
                }
            },
            new S_ConfigInfo()
            {
                Title = "UIKit 用户界面系统",
                Action_Create = () =>
                {
                    return Config.CreateIfNotExist<TinaX.UIKit.UIKitConfig>(ConfigPath.uikit);
                },
                Action_GetInstance = () =>
                {
                    return Config.GetTinaXConfig<TinaX.UIKit.UIKitConfig>(ConfigPath.uikit);
                }
            },
            new S_ConfigInfo()
            {
                Title = "国际化系统",
                Action_Create = () =>
                {
                    return Config.CreateIfNotExist<TinaX.I18NKit.I18NConfigModel>(ConfigPath.i18n);
                },
                Action_GetInstance = () =>
                {
                    return Config.GetTinaXConfig<TinaX.I18NKit.I18NConfigModel>(ConfigPath.i18n);
                }
            },
#if TinaX_CA_LuaRuntime_Enable
            new S_ConfigInfo()
            {
                Title = "Lua Script",
                Action_Create = () =>
                {
                    return Config.CreateIfNotExist<TinaX.Lua.LuaConfig>(ConfigPath.lua);
                },
                Action_GetInstance = () =>
                {
                    return Config.GetTinaXConfig<TinaX.Lua.LuaConfig>(ConfigPath.lua);
                }
            },
#endif
            new S_ConfigInfo()
            {
                Title = "热更新",
                Action_Create = () =>
                {
                    return Config.CreateIfNotExist<TinaX.Upgrade.UpgradeConfig>(ConfigPath.upgrade);
                },
                Action_GetInstance = () =>
                {
                    return Config.GetTinaXConfig<TinaX.Upgrade.UpgradeConfig>(ConfigPath.upgrade);
                }
            },
            //new S_ConfigInfo()
            //{
            //    Title = "虚拟文件系统 (preview)",
            //    Action_Create = () =>
            //    {
            //        return Config.CreateIfNotExist<TinaX.VFS.VFS2Config>(ConfigPath.vfs2);
            //    },
            //    Action_GetInstance = () =>
            //    {
            //        return Config.GetTinaXConfig<TinaX.VFS.VFS2Config>(ConfigPath.vfs2);
            //    }
            //},

        };

        public struct S_ConfigInfo
        {
            public string Title;

            public string FileName;


            public System.Func<ScriptableObject> Action_Create;

            public System.Func<ScriptableObject> Action_GetInstance;

        }
    }

#endif
        }

