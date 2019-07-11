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

        [MenuItem("TinaX/配置/资源系统",false,101)]
        public static void Config_assets()
        {
            Folder.CreateFolder("Assets/Resources/" + TinaX.Setup.Framework_Config_Path);
            var conf = TinaX.Config.CreateIfNotExist<TinaX.VFS.VFSConfigModel>(TinaX.Conf.ConfigPath.vfs);
            Selection.activeObject = conf;
        }

        [MenuItem("TinaX/配置/UI系统", false, 102)]
        public static void Config_uikit()
        {
            Folder.CreateFolder("Assets/Resources/" + TinaX.Setup.Framework_Config_Path);
            var conf = TinaX.Config.CreateIfNotExist<TinaX.UIKit.UIKitConfig>(TinaX.Conf.ConfigPath.uikit);
            Selection.activeObject = conf;
        }

        [MenuItem("TinaX/配置/国际化", false, 102)]
        public static void Config_i18n()
        {
            Folder.CreateFolder("Assets/Resources/" + TinaX.Setup.Framework_Config_Path);
            var conf = TinaX.Config.CreateIfNotExist<TinaX.I18N.I18NConfig>(TinaX.Conf.ConfigPath.i18n);
            Selection.activeObject = conf;
        }



#endif

    }
}

