using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


namespace TinaXEditor
{
    public class ConfigMenu
    {
#if !ODIN_INSPECTOR
        [MenuItem("TinaX/配置/TinaX配置",false,100)]
        public static void Config_main()
        {
            Folder.CreateFolder("Assets/Resources/" + TinaX.Setup.Framework_Config_Path);
            var conf = TinaX.Config.CreateIfNotExist<TinaX.MainConfig>(TinaX.Conf.ConfigPath.main);
            Selection.activeObject = conf;
        }

        [MenuItem("TinaX/配置/虚拟文件系统",false,101)]
        public static void Config_assets()
        {
            Folder.CreateFolder("Assets/Resources/" + TinaX.Setup.Framework_Config_Path);
            var conf = TinaX.Config.CreateIfNotExist<TinaX.VFSKit.VFSConfigModel>(TinaX.Conf.ConfigPath.vfs);
            Selection.activeObject = conf;
        }

        [MenuItem("TinaX/配置/UI系统", false, 102)]
        public static void Config_uikit()
        {
            Folder.CreateFolder("Assets/Resources/" + TinaX.Setup.Framework_Config_Path);
            var conf = TinaX.Config.CreateIfNotExist<TinaX.UIKits.UIKitConfig>(TinaX.Conf.ConfigPath.uikit);
            Selection.activeObject = conf;
        }

        [MenuItem("TinaX/配置/国际化", false, 103)]
        public static void Config_i18n()
        {
            Folder.CreateFolder("Assets/Resources/" + TinaX.Setup.Framework_Config_Path);
            var conf = TinaX.Config.CreateIfNotExist<TinaX.I18NKit.I18NConfigModel>(TinaX.Conf.ConfigPath.i18n);
            Selection.activeObject = conf;
        }

        [MenuItem("TinaX/配置/LuaScript Runtime", false, 104)]
        public static void Config_lua()
        {
            Folder.CreateFolder("Assets/Resources/" + TinaX.Setup.Framework_Config_Path);
            var conf = TinaX.Config.CreateIfNotExist<TinaX.Lua.LuaConfig>(TinaX.Conf.ConfigPath.lua);
            Selection.activeObject = conf;
        }

        [MenuItem("TinaX/配置/热更新", false, 105)]
        public static void Config_upgrade()
        {
            Folder.CreateFolder("Assets/Resources/" + TinaX.Setup.Framework_Config_Path);
            var conf = TinaX.Config.CreateIfNotExist<TinaX.Upgrade.UpgradeConfig>(TinaX.Conf.ConfigPath.upgrade);
            Selection.activeObject = conf;
        }


#endif

    }
}

