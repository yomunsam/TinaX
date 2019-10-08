using System.Collections.Generic;
using System.IO;
using System.Linq;
using TinaX;
using TinaX.VFSKit;
using UnityEditor;
using UnityEngine;

namespace TinaXEditor.VFSKit
{
    public class XVFSPacker
    {

        public List<VFSPackPlan> PackPlanList => mPackPlanList;

        /// <summary>
        /// 遍历检查工程中的所有文件
        /// </summary>
        public bool CheackAllFileInProject = false;

        /// <summary>
        /// 在所有打包任务结束后，移除工程资源中的AssetBundle标记
        /// </summary>
        public bool RemoveABSignAfterAllPlanFinish = false;



        private List<VFSPackPlan> mPackPlanList = new List<VFSPackPlan>();
        private List<TinaX.VFSKit.FileHashInfo> mFileHashInfos = new List<TinaX.VFSKit.FileHashInfo>();
        

        public bool IsPacking { get; private set; } = false;

        /// <summary>
        /// 添加打包计划
        /// </summary>
        /// <returns></returns>
        public XVFSPacker AddPackPlan(VFSPackPlan plan)
        {
            if (IsPacking) return this;
            mPackPlanList.Add(plan);
            return this;
        }

        public void StartPacker(TinaX.VFSKit.VFSConfigModel config)
        {
            //开始构建
            
            Debug.Log("开始处理AssetBundle资源标记");
            SetFilesABSign(config);
            Debug.Log("开始打包队列");
            for(var i = mPackPlanList.Count -1; i >= 0; i--)
            {
                StartBuildAssetBundle(mPackPlanList[i]);
                HandleEncry(config, mPackPlanList[i]);
                

                //最后执行移动
                if (mPackPlanList[i].CopyToStreamingAssets)
                {
                    CopyToStreamingAssets(mPackPlanList[i]);
                }


                mPackPlanList.RemoveAt(i);
            }

            if (RemoveABSignAfterAllPlanFinish)
            {
                RemoveAllABSign();
            }

            AssetDatabase.Refresh();

        }

        /// <summary>
        /// 为工程中的文件添加打包标记
        /// </summary>
        private void SetFilesABSign(TinaX.VFSKit.VFSConfigModel config)
        {
            EditorUtility.DisplayProgressBar("VFS Packer", "handle assetbundle sign", 0.5f);
            var ab_ext_name = TinaX.Setup.Framework_VFS_AssetBundle_Ext_Name;   //后缀名
            if (ab_ext_name.StartsWith("."))
            {
                ab_ext_name = ab_ext_name.Substring(1, ab_ext_name.Length - 1);
            }
            string[] guids;
            if (CheackAllFileInProject)
            {
                //完全遍历
                guids = AssetDatabase.FindAssets("", AssetDatabase.GetSubFolders("Assets"));
            }
            else
            {
                //只根据VFS白名单
                List<string> _whiteLists = new List<string>();
                foreach(var item in config.VFS_WhiteList)
                {
                    if (item.EndsWith("/"))
                    {
                        _whiteLists.Add(item.Substring(0, item.Length - 1));
                    }
                    else
                    {
                        _whiteLists.Add(item);
                    }
                }
                guids = AssetDatabase.FindAssets("", _whiteLists.ToArray());
            }
            AssetDatabase.RemoveUnusedAssetBundleNames();
            EditorUtility.ClearProgressBar();

            int counter = 0;
            int counter_t = 0;
            foreach (var item in guids)
            {
                counter ++;
                counter_t++;
                if(counter_t >= 30)
                {
                    counter_t = 0;
                    EditorUtility.DisplayProgressBar("VFS Packer", $"handle assetbundle sign - {counter} / {guids.Length}", counter/ guids.Length);
                }
                var _cur_path = AssetDatabase.GUIDToAssetPath(item);
                var path_parse_info = TinaX.VFSKit.AssetParseHelper.Parse(_cur_path, config);
                if (path_parse_info.IsValid)
                {
                    //有效
                    var importer = AssetImporter.GetAtPath(_cur_path);
                    if (!IsFolder(path_parse_info.LoadPath) && !path_parse_info.ABFileName.IsNullOrEmpty())
                    {
                        //标记ab名
                        importer.assetBundleName = path_parse_info.ABFileName;
                        importer.assetBundleVariant = ab_ext_name;

                        //原始文件哈希值
                        mFileHashInfos.Add(new TinaX.VFSKit.FileHashInfo()
                        {
                            Path = path_parse_info.LoadPath,
                            Hash = TinaX.IO.XFile.GetMD5(Path.GetFullPath(path_parse_info.LoadPath))
                        });
                        
                    }

                    //if (IsFolder(path_parse_info.LoadPath))
                    //{
                    //    mFileMap.SetInfoByPath(path_parse_info.LoadPath, TinaX.VFSKit.VFileMapItemType.Folder);
                    //}
                    //else
                    //{
                    //    if (!path_parse_info.ABFileName.IsNullOrEmpty())
                    //    {
                    //        //标记ab名
                    //        importer.assetBundleName = path_parse_info.ABFileName;
                    //        importer.assetBundleVariant = ab_ext_name;
                    //        mFileMap.SetInfoByPath(path_parse_info.LoadPath, TinaX.VFSKit.VFileMapItemType.File);

                    //    }
                    //}
                }
                else
                {
                    //无效
                    if(ExtCanHandle(path_parse_info.LoadPath) && !IsFolder(path_parse_info.LoadPath))
                    {
                        var importer = AssetImporter.GetAtPath(_cur_path);
                        importer.assetBundleName = null;
                    }

                }
                //Debug.Log("解析文件：" + path_parse_info.LoadPath + "    是否有效：" + path_parse_info.IsValid);
            }
            AssetDatabase.SaveAssets();
            Debug.Log("AssetBundle标记完毕");
            EditorUtility.ClearProgressBar();

        }

        private void StartBuildAssetBundle(VFSPackPlan plan)
        {
            var output_path = Path.GetFullPath(plan.OutputPath);
            Debug.Log($"生成[{plan.Platform.ToString()}]AssetBundle到：" + output_path);
            if (plan.ClearOutputFolders)
            {
                if (Directory.Exists(output_path))
                {
                    Directory.Delete(output_path, true);
                }
            }
            if (!Directory.Exists(output_path))
            {
                Directory.CreateDirectory(output_path);
            }
            BuildAssetBundleOptions opt = BuildAssetBundleOptions.ChunkBasedCompression;
            switch (plan.AssetCompressType)
            {
                case VFSPackPlan.CompressType.LZ4:
                    opt = BuildAssetBundleOptions.ChunkBasedCompression;
                    break;
                case VFSPackPlan.CompressType.LZMA:
                    opt = BuildAssetBundleOptions.None;
                    break;
                case VFSPackPlan.CompressType.None:
                    opt = BuildAssetBundleOptions.UncompressedAssetBundle;
                    break;
                default:
                    opt = BuildAssetBundleOptions.ChunkBasedCompression;
                    break;
            }
            if (plan.ClearOutputFolders)
            {
                opt = opt | BuildAssetBundleOptions.ForceRebuildAssetBundle;
            }
            if (plan.StrictMode)
            {
                opt = opt | BuildAssetBundleOptions.StrictMode;
            }
            BuildPipeline.BuildAssetBundles(output_path, opt, plan.Platform);
            //记录文件hash
            List<TinaX.VFSKit.FileHashInfo> fileHashInfos = new List<TinaX.VFSKit.FileHashInfo>();
            var ab_files = Directory.GetFiles(output_path, $"*{TinaX.Setup.Framework_VFS_AssetBundle_Ext_Name}", SearchOption.AllDirectories);
            foreach (var path in ab_files)
            {
                fileHashInfos.Add(new FileHashInfo()
                {
                    Path = path.Substring(output_path.Length,path.Length - output_path.Length).Replace("\\","/"),
                    Hash = TinaX.IO.XFile.GetMD5(path)
                });
            }
            var ab_file_hash = new VFSFileHashModel();
            ab_file_hash.Files = fileHashInfos.ToArray();
            var ab_hash_json = JsonUtility.ToJson(ab_file_hash);
            File.WriteAllText(Path.Combine(output_path, TinaX.VFSKit.VFSPathConst.VFS_File_AssetBundleHash_FileName), ab_hash_json);

            ////打包完,
            //if (plan.CopyToStreamingAssets)
            //{
            //    //移动到
            //    var target_path = Path.GetFullPath(Path.Combine("Assets/StreamingAssets/",TinaX.VFSKit.VFSPathConst.VFS_File,plan.XPlatform.ToString().ToLower()));
            //    //Debug.Log("移动到目录:" + target_path);
            //    //移动目标目录
            //    TinaX.IO.XDirectory.CopyDir(output_path, target_path);
            //}
            
        }


        private void CopyToStreamingAssets(VFSPackPlan plan)
        {
            if (plan.CopyToStreamingAssets)
            {
                var output_path = Path.GetFullPath(plan.OutputPath);

                //移动到
                var target_path = Path.GetFullPath(Path.Combine("Assets/StreamingAssets/", TinaX.VFSKit.VFSPathConst.VFS_File, plan.XPlatform.ToString().ToLower()));
                //Debug.Log("移动到目录:" + target_path);
                if (Directory.Exists(target_path))
                {
                    Directory.Delete(target_path,true);
                }
                Directory.CreateDirectory(target_path);
                //移动目标目录
                TinaX.IO.XDirectory.CopyDir(output_path, target_path);
            }
        }

        /// <summary>
        /// 处理加密
        /// </summary>
        /// <param name="config"></param>
        /// <param name="plan"></param>
        private void HandleEncry( TinaX.VFSKit.VFSConfigModel config, VFSPackPlan plan)
        {
            if (config.VFS_EncryFolder.Length > 0)
            {
                EditorUtility.DisplayProgressBar("VFS Packer", "Handle File Encry", 0.5f);

                //处理加密
                var output_path = Path.GetFullPath(plan.OutputPath);
                var ab_manifest = AssetBundle.LoadFromFile(Path.Combine(output_path, plan.XPlatform.ToString().ToLower()));
                var manifest = ab_manifest.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
                var vfs_encry_sign = System.Text.Encoding.UTF8.GetBytes(VFSEncryConst.EncryFileSign); //TinaX在已加密文件的开头放置几个没用但是最终还能认出来的byte，用来标记这个文件已经被加密了。
                if (ab_manifest == null || manifest == null)
                {
                    Debug.LogError("错误：获取AssetBundle资源的manifest失败");
                    return;
                }
                ab_manifest.Unload(false);
                ab_manifest = null;
                //先找出所有要被处理加密的虚拟地址
                List<string> rule_folder = new List<string>();
                foreach(var item in config.VFS_EncryFolder)
                {
                    //Debug.Log(item.FolderPath);
                    var vfs_folder_path = item.FolderPath.Substring(0, item.FolderPath.Length - 1);
                    if (!rule_folder.Contains(vfs_folder_path))
                    {
                        rule_folder.Add(vfs_folder_path);
                    }
                }
                var vfs_path_guids = AssetDatabase.FindAssets("", rule_folder.ToArray());

                int counter = 0;
                int counter_t = 0;
                foreach (var guid in vfs_path_guids)
                {
                    counter++;
                    counter_t++;
                    if (counter_t >= 30)
                    {
                        counter_t = 0;
                        EditorUtility.DisplayProgressBar("VFS Packer", $"Handle File Encry - {counter} / {vfs_path_guids.Length}", counter / vfs_path_guids.Length);
                    }

                    var path = AssetDatabase.GUIDToAssetPath(guid);
                    var parse_info = AssetParseHelper.Parse(path, config);
                    if(parse_info.IsValid && parse_info.IsEncry && !IsFolder(path))
                    {
                        //开始加密文件
                        string ab_path = Path.GetFullPath(Path.Combine(output_path, parse_info.ABFileNameWithExtName));
                        byte[] fileData = File.ReadAllBytes(ab_path);
                        var hashCode = manifest.GetAssetBundleHash(parse_info.ABFileNameWithExtName);
                        //Debug.Log("hashCode: " + hashCode.ToString());

                        //检查加密文件开头
                        if(!IsBytesEquals(fileData.Skip(0).Take(vfs_encry_sign.Length).ToArray(), vfs_encry_sign))
                        {
                            //Debug.Log("处理ab包：" + ab_path);
                            //Debug.Log(System.Text.Encoding.UTF8.GetString(fileData.Skip(0).Take(5).ToArray()));
                            //Debug.Log(System.Text.Encoding.UTF8.GetString(vfs_encry_sign));

                            byte[] new_file_data = new byte[0];

                            if (parse_info.EncryType == EncryptionType.Offset)  //字节偏移运算
                            {
                                ulong offset = default;
                                switch (config.Encry_OffsetHandleType)
                                {
                                    default:
                                        break;
                                    case EncryOffsetType.Default:
                                        offset = EncryDefaultHander.GetOffsetValue(hashCode, parse_info.ABFileNameWithExtName);
                                        break;
                                }
                                if (offset > 0)
                                {
                                    //Debug.Log("path: " + ab_path + "   | offset: " + offset);
                                    new_file_data = EncryDefaultHander.OffsetData(fileData, offset);
                                }
                            }
                            

                            //加上文件开头标记
                            byte[] final_file_data = new byte[vfs_encry_sign.Length + new_file_data.Length];
                            vfs_encry_sign.CopyTo(final_file_data, 0);
                            new_file_data.CopyTo(final_file_data, vfs_encry_sign.Length);

                            //写出文件
                            var fs = File.OpenWrite(ab_path);
                            fs.Write(final_file_data, 0, final_file_data.Length);
                            fs.Close();
                        }
                        //else
                        //{
                        //    Debug.Log("ab包：" + ab_path + "已经被处理过了");

                        //}

                    }
                }
                EditorUtility.ClearProgressBar();

            }
        }

        private void RemoveAllABSign()
        {

            var ab_names = AssetDatabase.GetAllAssetBundleNames();
            int counter = 0;
            int counter_t = 0;
            foreach (var name in ab_names)
            {
                counter++;
                counter_t++;
                if (counter_t >= 30)
                {
                    counter_t = 0;
                    EditorUtility.DisplayProgressBar("VFS Packer", $"Remove AssetBundle Sign - {counter} / {ab_names.Length}", counter / ab_names.Length);
                }
                
                AssetDatabase.RemoveAssetBundleName(name,true);
            }
            EditorUtility.ClearProgressBar();

            AssetDatabase.RemoveUnusedAssetBundleNames();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            //if (CheackAllFileInProject)
            //{
            //    //全局遍历

            //    foreach(var item )
            //}
            //else
            //{
            //    foreach(var item in mFileHashInfos)
            //    {
            //        var importer = AssetImporter.GetAtPath(item.Path);
            //        importer.assetBundleName = null;
            //    }
            //}
        }

        private bool IsFolder(string path)
        {
            var system_path = System.IO.Path.GetFullPath(path);
            return System.IO.Directory.Exists(system_path);
        }

        private bool IsBytesEquals(byte[] a, byte[] b)
        {
            if (a.Length != b.Length) return false;
            if (a == null && b == null) return true;
            if (a == null || b == null) return false;
            for(var i = 0; i < a.Length; i++)
            {
                if (a[i] != b[i])
                    return false;
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
            ".so",
            ".exe",
            ".apk",
            ".ipa"
        };

        private bool ExtCanHandle(string path)//如果后缀名可以被处理，返回true
        {
            var path_ext_name = System.IO.Path.GetExtension(path).ToLower();
            return !DontHandle_Ext.Any(item => item == path_ext_name.ToLower());
        }

    }

    /// <summary>
    /// VFS 打包计划
    /// </summary>
    public class VFSPackPlan
    {
        public BuildTarget Platform;

        public TinaX.Const.PlatformConst.E_Platform XPlatform;

        /// <summary>
        /// 输出路径（绝对路径）
        /// </summary>
        public string OutputPath;

        public bool ClearOutputFolders = false;

        public bool CopyToStreamingAssets = false;
        
        /// <summary>
        /// 压缩方式
        /// </summary>
        public CompressType AssetCompressType = CompressType.LZ4;

        public bool StrictMode = false;

        /// <summary>
        /// 压缩方式枚举
        /// </summary>
        public enum CompressType
        {
            None,
            LZ4,
            LZMA
        }
    }

}

