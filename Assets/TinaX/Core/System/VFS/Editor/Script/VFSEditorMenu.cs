using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TinaX.VFSKit;

namespace TinaXEditor.VFSKit
{
    /// <summary>
    /// 编辑器菜单
    /// </summary>
    public class VFSEditorMenu
    {

        

        [MenuItem(VFSMenuConst.MenuStr_LoadAssetByAssetDatabase,true)]
        static bool CheckMenu()
        {
            if(!Menu.GetChecked(VFSMenuConst.MenuStr_LoadAssetBundleFromStramingAssets) 
                && !Menu.GetChecked(VFSMenuConst.MenuStr_LoadAssetBundleFromTinaXWorkFolder)
                && !Menu.GetChecked(VFSMenuConst.MenuStr_LoadAssetFromResources)
                && !Menu.GetChecked(VFSMenuConst.MenuStr_LoadAssetByAssetDatabase)
                )
            {
                Menu.SetChecked(VFSMenuConst.MenuStr_LoadAssetByAssetDatabase,true);
            }
            return true;
        }

        //[UnityEditor.InitializeOnLoadMethod]
        //static void InitStatus()
        //{
        //    CheckMenu();
        //}

        [MenuItem(VFSMenuConst.MenuStr_LoadAssetByAssetDatabase,false,0)]
        static void SwitchToUseAssetDatabase()
        {
            Menu.SetChecked(VFSMenuConst.MenuStr_LoadAssetByAssetDatabase, true);
            Menu.SetChecked(VFSMenuConst.MenuStr_LoadAssetBundleFromStramingAssets, false);
            Menu.SetChecked(VFSMenuConst.MenuStr_LoadAssetBundleFromTinaXWorkFolder, false);
            Menu.SetChecked(VFSMenuConst.MenuStr_LoadAssetFromResources, false);
            Debug.Log($"[TinaX] VFS 在编辑器将使用 <color=#{ColorUtility.ToHtmlStringRGB(TinaX.Core.XEditorStyleDefine.Color_Blue)}>AssetDatabase</color> 方式处理资源");
        }

        [MenuItem(VFSMenuConst.MenuStr_LoadAssetBundleFromStramingAssets, false, 1)]
        static void SwitchToLoadStreamingAssets()
        {
            Menu.SetChecked(VFSMenuConst.MenuStr_LoadAssetByAssetDatabase, false);
            Menu.SetChecked(VFSMenuConst.MenuStr_LoadAssetBundleFromStramingAssets, true);
            Menu.SetChecked(VFSMenuConst.MenuStr_LoadAssetBundleFromTinaXWorkFolder, false);
            Menu.SetChecked(VFSMenuConst.MenuStr_LoadAssetFromResources, false);
            Debug.Log($"[TinaX] VFS 在编辑器将使用 <color=#{ColorUtility.ToHtmlStringRGB(TinaX.Core.XEditorStyleDefine.Color_Blue)}>AssetBundle (StramingAssets)</color> 方式处理资源");

        }

        [MenuItem(VFSMenuConst.MenuStr_LoadAssetBundleFromTinaXWorkFolder,false,2)]
        static void SwitchToLoadTinaXWorkFolder()
        {
            Menu.SetChecked(VFSMenuConst.MenuStr_LoadAssetByAssetDatabase, false);
            Menu.SetChecked(VFSMenuConst.MenuStr_LoadAssetBundleFromStramingAssets, false);
            Menu.SetChecked(VFSMenuConst.MenuStr_LoadAssetBundleFromTinaXWorkFolder, true);
            Menu.SetChecked(VFSMenuConst.MenuStr_LoadAssetFromResources, false);
            Debug.Log($"[TinaX] VFS 在编辑器将使用 <color=#{ColorUtility.ToHtmlStringRGB(TinaX.Core.XEditorStyleDefine.Color_Blue)}>AssetBundle (TinaX work folder)</color> 方式处理资源");

        }

        [MenuItem(VFSMenuConst.MenuStr_LoadAssetFromResources, false, 3)]
        static void SwitchToLoadResources()
        {
            Menu.SetChecked(VFSMenuConst.MenuStr_LoadAssetByAssetDatabase, false);
            Menu.SetChecked(VFSMenuConst.MenuStr_LoadAssetBundleFromStramingAssets, false);
            Menu.SetChecked(VFSMenuConst.MenuStr_LoadAssetBundleFromTinaXWorkFolder, false);
            Menu.SetChecked(VFSMenuConst.MenuStr_LoadAssetFromResources, true);
            Debug.Log($"[TinaX] VFS 在编辑器将使用 <color=#{ColorUtility.ToHtmlStringRGB(TinaX.Core.XEditorStyleDefine.Color_Blue)}>Resources</color> 方式处理资源");

        }

    }
}

