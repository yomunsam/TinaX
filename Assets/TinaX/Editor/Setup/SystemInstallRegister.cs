using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TinaX;
using TinaX.Conf;

namespace TinaXEditor.Setup
{

    /// <summary>
    /// Systems 安装注册项
    /// </summary>
    public static class SystemInstallRegister
    {
        public static RegModel[] RegItems =
        {
            //框架主配置
            new RegModel()
            {
                Name = "框架主配置",
                Selectable = false,
                DoInstall = () =>
                {
                    Config.CreateIfNotExist<MainConfig>(ConfigPath.main);
                },
                IsInstalled = () =>
                {
                    return Config.GetTinaXConfig<MainConfig>(ConfigPath.main) != null;
                }
            },
            //虚拟文件系统
            new RegModel()
            {
                Name = "虚拟文件系统",
                Selectable = false,
                DoInstall = () =>
                {
                    Config.CreateIfNotExist<TinaX.VFSKit.VFSConfigModel>(ConfigPath.vfs);
                },
                IsInstalled = () =>
                {
                    return Config.GetTinaXConfig<TinaX.VFSKit.VFSConfigModel>(ConfigPath.vfs) != null;
                }
            },
            //UIKit
            new RegModel()
            {
                Name = "UIKit 用户界面系统",
                Selectable = false,
                DoInstall = () =>
                {
                    Config.CreateIfNotExist<TinaX.UIKit.UIKitConfig>(ConfigPath.uikit);
                },
                IsInstalled = () =>
                {
                    return Config.GetTinaXConfig<TinaX.UIKit.UIKitConfig>(ConfigPath.uikit) != null;
                }
            },
            //I18N
            new RegModel()
            {
                Name = "国际化（I18N）",
                Selectable = false,
                DoInstall = () =>
                {
                    Config.CreateIfNotExist<TinaX.I18NKit.I18NConfigModel>(ConfigPath.i18n);
                },
                IsInstalled = () =>
                {
                    return Config.GetTinaXConfig<TinaX.I18NKit.I18NConfigModel>(ConfigPath.i18n) != null;
                }
            },
            //Lua
            new RegModel()
            {
                Name = "LuaScript Runtime",
                Selectable = true,
                DefaultSelect = true,
                DoInstall = () =>
                {
                    Debug.Log("启用TinaX LuaScript Runtime , 请确保在工程中装载 xLua 扩展, 地址：https://github.com/Tencent/xlua");
                    DevOps.SharpDefineReadWriteUtils.AddDefineIfNotExist(UnityEditor.EditorUserBuildSettings.selectedBuildTargetGroup,TinaX.Const.SharpDefineConst.TinaX_LuaRuntime);
                    Config.CreateIfNotExist<TinaX.Lua.LuaConfig>(ConfigPath.lua);

                },
                IsInstalled = () =>
                {
#if TinaX_CA_LuaRuntime_Enable
                    if(Config.GetTinaXConfig<TinaX.Lua.LuaConfig>(ConfigPath.lua) == null)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
#else
                    return false;
#endif
                }
            },
            new RegModel()
            {
                Name = "热更新",
                Selectable = false,
                DoInstall = () =>
                {
                    Config.CreateIfNotExist<TinaX.Upgrade.UpgradeConfig>(ConfigPath.upgrade);
                },
                IsInstalled = () =>
                {
                    return Config.GetTinaXConfig<TinaX.Upgrade.UpgradeConfig>(ConfigPath.upgrade) != null;
                }
            }
        };

        /// <summary>
        /// 注册模板
        /// </summary>
        public struct RegModel
        {
            public string Name;

            /// <summary>
            /// 安装是否可选
            /// </summary>
            public bool Selectable;

            /// <summary>
            /// 如果是可选安装，那么默认是否选择
            /// </summary>
            public bool DefaultSelect;

            ///// <summary>
            ///// 检查是否已安装
            ///// </summary>
            //public System.Func<bool> IsInstalled;

            public System.Action DoInstall;

            public System.Func<bool> IsInstalled;
        }
    }
}

