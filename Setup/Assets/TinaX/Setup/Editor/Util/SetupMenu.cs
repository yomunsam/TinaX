using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;
using System.Runtime.Remoting.Metadata.W3cXsd2001;

/*
 * 开发Setup包时的部分功能，使用编译器宏：TINAX_SETUP_DEV
 */


namespace TinaXEditor.Setup.Internal
{
    public static class SetupMenu
    {
#if TINAX_SETUP_DEV
        [MenuItem("TinaX Dev/Setup/Gen Empty Package List File")]
        static void GenEmptyPackageListJsonFile()
        {
            var filepath = SetupUtil.CombineUnityPath(SetupUtil.SetupEditorRootFolderPath, "Data/PackageList.json");
            filepath = Path.GetFullPath(filepath);
            if (File.Exists(filepath))
            {
                if (!EditorUtility.DisplayDialog("exist", "file exist , overwrite it ?", "yes", "no"))
                    return;
            }
            var json_empty = new PackageListModel();
            json_empty.packages = new List<PackageListModel.ListItem>() { new PackageListModel.ListItem() };
            var json_str = JsonUtility.ToJson(json_empty);
            File.WriteAllText(filepath, json_str, Encoding.UTF8);
        }

        [MenuItem("TinaX Dev/Setup/Version Test")]
        static void VersionTest()
        {
            var ver_str = Application.unityVersion;
            Debug.Log("application.version:" + ver_str);
            var index = ver_str.LastIndexOfAny(new[] { 'f', 'a', 'b', 'c' });
            if (index != -1)
                ver_str = ver_str.Substring(0, index);
            Debug.Log("pure:" + ver_str);

            if (System.Version.TryParse(ver_str, out var version)) 
            {
                Debug.Log("parse success.");
            }
            else
            {
                Debug.Log("parse failed.");
            }
        }


        [MenuItem("TinaX Dev/Setup/Add Test")]
        static void AddTest()
        {
            var addreq = UnityEditor.PackageManager.Client.Add("https://github.com/neuecc/UniRx.git?path=Assets/Plugins/UniRx/Scripts");
            //EditorCoroutineUtility.StartCoroutine(WaitAdd(addreq));
            EditorCoroutine.Start(WaitAdd(addreq));
        }

        static IEnumerator WaitAdd(UnityEditor.PackageManager.Requests.AddRequest req)
        {
            while (req.Status == UnityEditor.PackageManager.StatusCode.InProgress)
                yield return null;

            Debug.Log(req.Status);
            Debug.Log("喵");
        }

        [MenuItem("TinaX Dev/Setup/Search Test")]
        static void SearchTest()
        {
            var req = UnityEditor.PackageManager.Client.Search("com.neuecc.unirx", true);
            var i = 0;
            while(!req.IsCompleted && i < 10)
            {
                i++;
            }

            Debug.Log(req.Status);
            Debug.Log("i: " + i);
        }

        [MenuItem("TinaX Dev/Setup/Get List")]
        static void GetList()
        {
            var listModel = SetupUtil.GetPackageList();
            SetupUtil.GetPackages_Main(ref listModel, list =>
            {
                foreach(var item in list)
                {
                    Debug.Log($"package : {item.BaseInfo.packageName} - {(item.Installed ? "Installed" : "Not Install")}");
                }
            });
        }

#endif

    }
}


