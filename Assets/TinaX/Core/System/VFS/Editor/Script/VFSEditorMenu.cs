using System.Collections;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TinaX.VFSKit;
using System.IO;

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




        //缓存、们----------------------------------------------------------------------------------------------------------------------


        [MenuItem("TinaX/Editor/VFS/Explorer VFS Cache Folder",false, 1)]
        static void ShowVFSCache()
        {
            var path = System.IO.Path.Combine(UnityEngine.Application.persistentDataPath, TinaX.Setup.Framework_LocalStorage_TinaX, VFSPathConst.VFS_File, TinaX.Const.PlatformConst.GetPlatformName(Application.platform).ToLower());
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            var uri = new System.Uri(path);
            Debug.Log(uri.AbsoluteUri);
            Application.OpenURL(uri.AbsoluteUri);
        }

        [MenuItem("TinaX/Editor/VFS/Clear VFS Cache Folder", false, 21)]
        static void ClearVFSCache()
        {
            var path = System.IO.Path.Combine(UnityEngine.Application.persistentDataPath, TinaX.Setup.Framework_LocalStorage_TinaX, VFSPathConst.VFS_File, TinaX.Const.PlatformConst.GetPlatformName(Application.platform).ToLower());
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
                Directory.CreateDirectory(path);
            }
            Debug.Log("clear success");
        }

        [MenuItem("TinaX/Editor/VFS/Explorer WebVFS Cache Folder", false, 2)]
        static void ShowWebVFSCache()
        {
            var path = System.IO.Path.Combine(UnityEngine.Application.persistentDataPath, TinaX.Setup.Framework_LocalStorage_TinaX, VFSPathConst.VFS_Web_Cache, TinaX.Const.PlatformConst.GetPlatformName(Application.platform).ToLower());
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            var uri = new System.Uri(path);
            Debug.Log(uri.AbsoluteUri);
            Application.OpenURL(uri.AbsoluteUri);
        }

        [MenuItem("TinaX/Editor/VFS/Clear WebVFS Cache Folder", false, 22)]
        static void ClearWebVFSCache()
        {
            var path = System.IO.Path.Combine(UnityEngine.Application.persistentDataPath, TinaX.Setup.Framework_LocalStorage_TinaX, VFSPathConst.VFS_Web_Cache, TinaX.Const.PlatformConst.GetPlatformName(Application.platform).ToLower());
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
                Directory.CreateDirectory(path);
            }
            Debug.Log("clear success");
        }


        [MenuItem("TinaX/Editor/VFS/Refresh StreamingAssets fileHashList",false,41)]
        [MenuItem("TinaX/发布流程/刷新 StreamingAssets 文件哈希表",false,5)]
        public static void Refresh_StreamingAssets_FileHashList()
        {
            var rootPath = System.IO.Path.Combine(Application.streamingAssetsPath, TinaX.VFSKit.VFSPathConst.VFS_File);
            var files = Directory.GetFiles(rootPath, VFSPathConst.VFS_File_AssetBundleHash_FileName, SearchOption.AllDirectories);

            foreach(var item in files)
            {
                RefreshByFileHashFile(item);
            }


        }

        private static void RefreshByFileHashFile(string filePath)
        {
            var model = new TinaX.VFSKit.VFSFileHashModel();
            List<FileHashInfo> list = new List<FileHashInfo>();

            var parent_path = Directory.GetParent(filePath).ToString();
            var asset_root_path = Path.Combine(Directory.GetParent(filePath).ToString(), "assets");
            var files = Directory.GetFiles(asset_root_path, "*.xab", SearchOption.AllDirectories);
            Debug.Log("total:" + files.Length);
            foreach(var file in files)
            {
                var md5 = TinaX.IO.XFile.GetMD5(file);
                var path = file.Substring(parent_path.Length , file.Length - parent_path.Length);
                if(path.StartsWith("/") || path.StartsWith("\\"))
                {
                    path = path.Substring(1, path.Length - 1);
                }
                path = path.Replace("\\", "/");
                list.Add(new FileHashInfo()
                {
                    Path = path,
                    Hash = md5
                }) ;

            }

            model.Files = list.ToArray();

            var json_str = JsonUtility.ToJson(model);
            File.Delete(filePath);
            File.WriteAllText(filePath, json_str);

            Debug.Log("Refresh success: " + filePath);
        }


    }
}

