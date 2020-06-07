using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using TinaXEditor.Setup.Internal;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;
using PackageInfo = UnityEditor.PackageManager.PackageInfo;


namespace TinaXEditor.Setup.Internal
{
    public class SetupUtil
    {
        private const string SetupEditorRootSignFileName = "TinaX.Setup.Editor.Root.txt";
        private static string SetupEditorRootSignFileNameWithoutExtension = Path.GetFileNameWithoutExtension(SetupEditorRootSignFileName);
        private const string DefaultSetupEditorRootFolderPath = "Assets/TinaX/Setup/Editor";

        private static string _setup_editor_root_path;
        public static string SetupEditorRootFolderPath
        {
            get
            {
                if (_setup_editor_root_path == null)
                {
                    if (File.Exists(Path.Combine(Directory.GetCurrentDirectory(), DefaultSetupEditorRootFolderPath, SetupEditorRootSignFileName)))
                        _setup_editor_root_path = DefaultSetupEditorRootFolderPath;
                    else
                    {
                        string[] guids = AssetDatabase.FindAssets($"{SetupEditorRootSignFileNameWithoutExtension} t:TextAsset", new string[] { "Assets" });
                        if (guids != null && guids.Length > 0)
                        {
                            _setup_editor_root_path = Path.GetDirectoryName(AssetDatabase.GUIDToAssetPath(guids[0]));
                            if (_setup_editor_root_path.IndexOf("\\") != -1)
                                _setup_editor_root_path = _setup_editor_root_path.Replace("\\", "/");
                        }
                        else
                        {
                            _setup_editor_root_path = "";
                            Debug.LogWarning("[TinaX.Setup]Not found root folder.");
                        }
                    }

                }
                return _setup_editor_root_path;
            }
        }


        public static string CombineUnityPath(params string[] paths)
        {
            var result = Path.Combine(paths);
            if (result.IndexOf("\\") != -1)
                result = result.Replace("\\", "/");
            return result;
        }


        public static PackageListModel GetPackageList()
        {
            var jsonPath = Path.Combine(SetupEditorRootFolderPath, "Data/PackageList.json");
            if (!File.Exists(jsonPath))
                throw new System.Exception("Packages list file not found");
            var jsonText = File.ReadAllText(jsonPath);
            var jsonObj = JsonUtility.FromJson<PackageListModel>(jsonText);
            return jsonObj;
        }


        public static void GetPackages_Main(ref PackageListModel model, Action<List<PackageListInfo>> callback)
        {
            var result = new List<PackageListInfo>();
            foreach (var item in model.packages)
            {
                result.Add(new PackageListInfo() { BaseInfo = item });
            }

            SetPackagesInfo(result, () =>
            {
                callback?.Invoke(new List<PackageListInfo>(result.Where(info => !info.BaseInfo.thirdparty)));
            });
        }

        public static void GetPackages_Installed(ref PackageListModel model, Action<List<PackageListInfo>> callback)
        {
            var result = new List<PackageListInfo>();
            foreach (var item in model.packages)
            {
                result.Add(new PackageListInfo() { BaseInfo = item });
            }

            SetPackagesInfo(result, () =>
            {
                var filter = result.Where(info => info.Installed);
                callback?.Invoke(new List<PackageListInfo>(filter));
            });
        }

        public static void GetPackages_Thirdparty(ref PackageListModel model, Action<List<PackageListInfo>> callback)
        {
            var result = new List<PackageListInfo>();
            foreach (var item in model.packages)
            {
                result.Add(new PackageListInfo() { BaseInfo = item });
            }

            SetPackagesInfo(result, () =>
            {
                callback?.Invoke(new List<PackageListInfo>(result.Where(info => info.BaseInfo.thirdparty)));
            });
        }

        public static void GetPackages_All(ref PackageListModel model, Action<List<PackageListInfo>> callback)
        {
            var result = new List<PackageListInfo>();
            foreach (var item in model.packages)
            {
                result.Add(new PackageListInfo() { BaseInfo = item });
            }

            SetPackagesInfo(result, () =>
            {
                callback?.Invoke(result);
            });
        }

        public static void SetPackagesInfo(List<PackageListInfo> infos, Action onFinish)
        {
            if (infos == null || infos.Count == 0)
                return;
            GetInstalledList(myList =>
            {
                for (int i = 0; i < infos.Count; i++)
                {
                    var _pinfos = myList.Where(pinfo => pinfo.name.Equals(infos[i].BaseInfo.packageName));
                    if (_pinfos.Count() > 0)
                    {
                        var _p_info = _pinfos.First();
                        infos[i].Installed = true;
                        infos[i].Installed_VersionName = _p_info.version;
                    }
                    else
                    {
                        infos[i].Installed = false;
                        infos[i].Installed_VersionName = string.Empty;
                    }
                }
                onFinish?.Invoke();
            });
        }

        public static void GetInstalledList(Action<List<PackageInfo>> callback)
        {
            var req = Client.List(true, true);
            EditorCoroutine.Start(waitGetList(req, () =>
            {
                var result = req.Result;
                List<PackageInfo> pack_list = new List<PackageInfo>();
                foreach (var item in result)
                {
                    if (item.status != PackageStatus.Available)
                        continue;
                    pack_list.Add(item);
                }

                callback?.Invoke(pack_list);
            }));
        }

        private static IEnumerator waitGetList(UnityEditor.PackageManager.Requests.ListRequest req, Action onFinish) //这种闭包写法会导致内存浪费，但是懒得折腾了，总量不多，所以性能上没问题哒
        {
            while (req.Status == StatusCode.InProgress)
                yield return null;
            onFinish?.Invoke();
            yield break;
        }

        /// <summary>
        /// 总入口
        /// </summary>
        /// <param name="packageName"></param>
        public static void AddPackageAndDependencies(string packageName)
        {
            //搜索
            
        }

        public static PackageListModel.ListItem GetPackageInfoByName(string packageName)
        {
            var list = GetPackageList();
            var infos = list.packages.Where(item => item.packageName.Equals(packageName));
            if (infos.Count() == 0)
                return null;
            else
                return infos.First();
        }

        public static void DirectAddPackage(string uri, Action<bool> callback)
        {
            var req = UnityEditor.PackageManager.Client.Add(uri);
            EditorCoroutine.Start(waitAdd(req, () =>
            {
                callback?.Invoke(true);
            }));
        }

        private static IEnumerator waitAdd(UnityEditor.PackageManager.Requests.AddRequest req, Action onFinish)
        {
            while (req.Status == StatusCode.InProgress)
                yield return null;
            onFinish?.Invoke();
            yield break;
        }


    }

}
