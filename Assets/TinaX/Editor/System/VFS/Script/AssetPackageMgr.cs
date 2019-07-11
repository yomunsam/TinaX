using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TinaX;
using TinaX.Conf;
using TinaX.VFS;

namespace TinaXEditor
{
    public class AssetPackageMgr
    {
        

        /// <summary>
        /// 全局打包
        /// </summary>
        /// <returns></returns>
        public bool PackGlobal(string basePath, BuildTarget target, bool full_rebuild = false, bool re_sign = true, bool cancelAfterSign = false, bool log = false,bool IgnoreStreamingAssets = true)
        {
            
            var conf = Config.GetTinaXConfig<VFSConfigModel>(ConfigPath.vfs);
            var conf_cache = VFSConfigCache.New(conf);
            if (conf == null)
            {
                EditorUtility.DisplayDialog("错误","读取资源配置失败","好");
                return false;
            }

            if (full_rebuild)
            {
                EditorUtility.DisplayProgressBar("完全构建", "正在删除原有文件", 0.5f);
                Debug.Log("完全重构模式：删除原有构建内容:" + System.IO.Path.GetFullPath(basePath));
                if (System.IO.Directory.Exists(System.IO.Path.GetFullPath(basePath)))
                {
                    System.IO.Directory.Delete(System.IO.Path.GetFullPath(basePath), true);
                    System.IO.Directory.CreateDirectory(System.IO.Path.GetFullPath(basePath));
                }
                EditorUtility.ClearProgressBar();
            }

            

            var guids = AssetDatabase.FindAssets("", CatLib.Arr.Merge<string>(conf.Assets_system_whiteList, AssetRuleConst.Tinax_Pack_Path));
            if (re_sign)
            {
                Debug.Log("完全遍历工程，处理白名单外可能存在的AssetBundle打包标记");
                var folders = AssetDatabase.GetSubFolders("Assets");
                guids = AssetDatabase.FindAssets("", folders);
            }
            Debug.Log("检索到 " + guids.Length + "个文件");
            AssetDatabase.RemoveUnusedAssetBundleNames();
            for (int i = 0; i<guids.Length; i++)
            {
                
                var v = guids[i];
                EditorUtility.DisplayProgressBar("标记打包资源", "正在处理：" + v, (i + 1) / guids.Length);
                var _path = AssetDatabase.GUIDToAssetPath(v);
                var ignore = false;
                if (IgnoreStreamingAssets)
                {
                    if (_path.Contains("/StreamingAssets/"))
                    {
                        if (log)
                        {
                            Debug.Log("路径属于StreamingAssets，已忽略处理：" + _path);
                        }
                        ignore = true;
                    }
                }
                if (!ignore)
                {
                    var info = AssetParse.Parse(_path, conf_cache);
                    var importer = AssetImporter.GetAtPath(_path);
                    if (info.handle_tag != E_FileHandleTag.invalid && info.ab_name != "" && !IsFolder(info.file_path))
                    {

                        importer.assetBundleName = info.ab_name;
                        importer.assetBundleVariant = "xab";
                        importer.SaveAndReimport();
                        if (log)
                        {
                            Debug.Log("    [" + (i + 1) + "/" + guids.Length + "]处理 " + info.file_path + "  - 标记打包:" + info.ab_name + ".xab");
                        }
                    }
                    else
                    {
                        if (ExtCanHandle(_path) && !IsFolder(info.file_path))
                        {
                            importer.assetBundleName = null;
                        }

                        if (log && !IsFolder(info.file_path))
                        {
                            Debug.Log("    [" + (i + 1) + "/" + guids.Length + "]处理 " + info.file_path + "  - 忽略");
                        }
                    }
                }
                EditorUtility.ClearProgressBar();
            }
            
            

            System.IO.Directory.CreateDirectory(basePath);
            if (full_rebuild)
            {
                BuildPipeline.BuildAssetBundles(basePath, BuildAssetBundleOptions.ForceRebuildAssetBundle | BuildAssetBundleOptions.ChunkBasedCompression, target);
            }
            else
            {
                BuildPipeline.BuildAssetBundles(basePath,  BuildAssetBundleOptions.ChunkBasedCompression, target);
            }
            

            if (cancelAfterSign)//打包后遍历整个工程，移除打包标记
            {
                Debug.Log("移除全局打包标记");
                var folders = AssetDatabase.GetSubFolders("Assets");
                //Debug.Log(folders.Length);
                foreach (var item in folders)
                {
                    var sub_guids = AssetDatabase.FindAssets("", new string[] { item });
                    foreach (var subguid in sub_guids)
                    {
                        var sub_path = AssetDatabase.GUIDToAssetPath(subguid);
                        var importer = AssetImporter.GetAtPath(sub_path);
                        if (importer != null)
                        {
                            
                            if (ExtCanHandle(sub_path) && !IsFolder(sub_path))
                            {
                                var flag = false;
                                if(importer.assetBundleName != null && importer.assetBundleName != "")
                                {
                                    
                                    flag = true;
                                    importer.assetBundleName = null;
                                }
                                //if(importer.assetBundleVariant != null)
                                //{
                                //    importer.assetBundleVariant = null;
                                //    flag = true;
                                //}
                                if(flag && log)
                                {
                                    Debug.Log("遍历取消ab标记:" + sub_path);
                                }
                            }
                        }
                    }
                    
                }
            }

            return true;
        }

        /// <summary>
        /// 不要处理的后缀
        /// </summary>
        private readonly string[] DontHandle_Ext =
        {
            ".cs",
            ".dll",
            ".so"
        };

        private bool ExtCanHandle(string path)
        {
            foreach(var item in DontHandle_Ext)
            {
                if (System.IO.Path.GetExtension(path).ToLower() == item.ToLower())
                {
                    return false;
                }
            }
            return true;
        }
        

        private bool IsFolder(string path)
        {
            var system_path = System.IO.Path.GetFullPath(path);
            //Debug.Log("判断路径是否为文件夹：" + system_path);
            return System.IO.Directory.Exists(system_path);

        }

    }

    

}
