//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using TinaX;
//using System.IO;
//using TinaX.VFS;
//using UnityEditor;

//namespace TinaXEditor.VFS
//{

//    /// <summary>
//    /// VFS 资源打包与编辑
//    /// </summary>
//    public class VFSPatchPackage
//    {

//        /// <summary>
//        /// 补丁母包配置目录
//        /// </summary>
//        private string mPatchBaseRootPatch; //如：TinaX6\TinaXWorkFolder\TinaX_VFS\Patch\xxx
//        private VFSPatchConfigModel mPatchBaseInfo; //配置根目录的配置信息

//        /// <summary>
//        /// package 存放补丁包的目录
//        /// </summary>
//        private string mPackageDirPath
//        {
//            get
//            {
//                return Path.Combine(mPatchBaseRootPatch, "package");
//            }
//        }

//        private string mPackageDataDirPath
//        {
//            get
//            {
//                return Path.Combine(mPatchBaseRootPatch, "package_data");
//            }
//        }

//        private string mTempVFSPatchPath
//        {
//            get
//            {
//                return Path.Combine(mPatchBaseRootPatch, "temp_vfs");
//            }
//        }

//        private string mTempPath
//        {
//            get
//            {
//                return Path.Combine(mPatchBaseRootPatch, "temp");
//            }
//        }

//        private string mPatchVFSFilePath
//        {
//            get
//            {
//                return Path.Combine(mTempPath, "patch.vfs");
//            }
//        }

//        private string mPatchFolderPatch // 补丁里的 /patch ， 在temp的位置
//        {
//            get
//            {
//                return Path.Combine(mTempPath, "patch");
//            }
//        }


//        public VFSPatchPackage SetBaseInfo(string _PatchDataRootFolder)
//        {
//            mPatchBaseRootPatch = _PatchDataRootFolder;
//            var basePackInfoJsonPath = Path.Combine(mPatchBaseRootPatch, "config.json");
//            if (File.Exists(basePackInfoJsonPath))
//            {
//                var jsonStr = File.ReadAllText(basePackInfoJsonPath);
//                mPatchBaseInfo = JsonUtility.FromJson<VFSPatchConfigModel>(jsonStr);
//            }
//            else
//            {
//                Debug.LogWarning("打补丁失败，没找到母包配置JSON文件:" + basePackInfoJsonPath);
//                return this;
//            }


//            return this;
//        }

//        /// <summary>
//        /// 制作补丁包,如果包重复，不覆盖
//        /// </summary>
//        /// <param name="PatchConfDir">补丁配置的目录</param>
//        /// <param name="PatchName">要打的补丁包叫什么名字</param>
//        /// <param name="Deletable">包含可删除信息（补丁中会有一个文件记录可被删除的内容）</param>
//        public void MakePatch(string PatchName,bool Deletable = true)
//        {

//            if(mPatchBaseInfo.PatchType == VFSPatchConfigModel.EPatchType.Newest)
//            {
//                MakePatch_Newest(PatchName, Deletable);
//            }
//            else
//            {
//                Debug.Log("暂未实现 依次补丁的打包");
//            }
//        }



//        /// <summary>
//        /// 打包，基于母包的最新包
//        /// </summary>
//        /// <param name="PatchName">要打的补丁包叫什么名字</param>
//        private void MakePatch_Newest(string PatchName,bool Deletable = true)
//        {
//            List<string> FileWhiteList = new List<string>(); //到最新版本时候的文件白名单
//            var patch_path = Path.Combine(mPackageDirPath, PatchName + TinaX.Setup.Framework_Patch_Ext_Name); //打包目标 补丁文件路径
//            if (File.Exists(patch_path))
//            {
//                Debug.LogWarning("打补丁包失败，文件已存在：" + patch_path);
//                return;
//            }

//            if (!Directory.Exists(mPackageDirPath))
//            {
//                Directory.CreateDirectory(mPackageDirPath);
//            }

//            var patch_log_info = new VFSPatchInfoModel(); //记录这个补丁的信息
//            patch_log_info.AddFile = new List<VFSPatchInfoModel.SFileHash>();
//            patch_log_info.DeleteFile = new List<VFSPatchInfoModel.SFileHash>();
//            patch_log_info.ModfiyFile = new List<VFSPatchInfoModel.SFileHash>();

//            //将母包中的文件记录存成一个字典，方便接下来使用
//            Dictionary<string, VFSPatchConfigModel.FileHash> dict_basePkg_files = new Dictionary<string, VFSPatchConfigModel.FileHash>();
//            Debug.Log("预处理母包记录");
//            for (int i = 0; i < mPatchBaseInfo.files.Length; i++)
//            {
//                var path = Path.Combine(Directory.GetCurrentDirectory(), TinaX.Setup.Framework_AssetSystem_Pack_Path, mPatchBaseInfo.PlatformName, mPatchBaseInfo.files[i].FileName);
//                dict_basePkg_files.Add(path, mPatchBaseInfo.files[i]);

//            }

//            //对比母包，看看有没有删除的文件
//            foreach (var item in dict_basePkg_files)
//            {
//                if (!File.Exists(item.Key))
//                {
//                    patch_log_info.DeleteFile.Add(new VFSPatchInfoModel.SFileHash() {
//                        VFSFileName = item.Value.FileName,
//                        MD5 = item.Value.MD5
//                    });
//                }
//            }

//            //遍历当前的VFS包文件
//            Debug.Log("遍历当前资源包");
//            var cur_vfs_root_path = Path.Combine(Directory.GetCurrentDirectory(), TinaX.Setup.Framework_AssetSystem_Pack_Path, mPatchBaseInfo.PlatformName); //这里是存放在本地的VFS 平台资源根路径
//            var files = Directory.GetFiles(cur_vfs_root_path, "*.*", SearchOption.AllDirectories); //抓到现在VFS 平台资源中的所有文件，准备进行平台对比

//            List<string> modify_path = new List<string>();

//            foreach (var file_path in files)
//            {
//                var cur_md5 = TinaX.IO.XFile.GetMD5(file_path).ToLower();
//                //处理
//                if (dict_basePkg_files.ContainsKey(file_path))
//                {
                    

//                    //已存在，对比md5
//                    if (dict_basePkg_files[file_path].MD5 != cur_md5)
//                    {

//                        //md5与母包记录中不一致
//                        modify_path.Add(file_path);
//                        patch_log_info.ModfiyFile.Add(new VFSPatchInfoModel.SFileHash()
//                        {
//                            VFSFileName = dict_basePkg_files[file_path].FileName,
//                            MD5 = cur_md5
//                        });
//                    }

//                }
//                else
//                {
//                    //不存在,这是一个新增文件
//                    //Debug.Log("<color=#FF83FA>文件增加：" + file_path + "</color>");
//                    var vfs_path = file_path.Replace(cur_vfs_root_path, "").ToLower();
//                    vfs_path = vfs_path.Replace("\\","/");
//                    patch_log_info.AddFile.Add(new VFSPatchInfoModel.SFileHash()
//                    {
//                        VFSFileName = vfs_path,
//                        MD5 = cur_md5
//                    });

//                    modify_path.Add(file_path);
//                }
//            }

//            //打包操作开始咯
//            Debug.Log("开始打包");
//            if (Directory.Exists(mTempVFSPatchPath))
//            {
//                Directory.Delete(mTempVFSPatchPath, true);
//            }
//            Directory.CreateDirectory(mTempVFSPatchPath);
//            if (Directory.Exists(mTempPath))
//            {
//                Directory.Delete(mTempPath, true);
//            }
//            Directory.CreateDirectory(mTempPath);

//            Debug.Log("开始打包VFS文件");
//            foreach(var file in modify_path)
//            {
//                var file_path_target = file.Replace(cur_vfs_root_path, "");
//                file_path_target = file_path_target.Substring(1, file_path_target.Length - 1);

//                file_path_target = Path.Combine(mTempVFSPatchPath, file_path_target);
//                Directory.CreateDirectory(Path.GetDirectoryName(file_path_target));
//                File.Copy(file, file_path_target);
//            }

//            if (File.Exists(mPatchVFSFilePath))
//            {
//                File.Delete(mPatchVFSFilePath);
//            }

//            TinaX.IO.XFolder.ZipFolder(mTempVFSPatchPath, mPatchVFSFilePath);

//            //处理补丁描述
//            Debug.Log("处理补丁描述");
//            if (!Directory.Exists(mPatchFolderPatch))
//            {
//                Directory.CreateDirectory(mPatchFolderPatch);
//            }

//            if (Deletable)
//            {
//                Debug.Log("生成文件白名单");
//                //文件白名单= 母包文件 + 新增文件 - 删除文件
//                foreach(var item in mPatchBaseInfo.files)
//                {
//                    FileWhiteList.Add(item.FileName);
//                }
//                foreach(var item in patch_log_info.AddFile)
//                {
//                    if (!FileWhiteList.Contains(item.VFSFileName))
//                    {
//                        FileWhiteList.Add(item.VFSFileName);
//                    }
//                }
//                foreach(var item in patch_log_info.DeleteFile)
//                {
//                    if (FileWhiteList.Contains(item.VFSFileName))
//                    {
//                        FileWhiteList.Remove(item.VFSFileName);
//                    }
//                }

//                //输出
//                var whitelistStr = "";
//                for(int i = 0; i < FileWhiteList.Count; i++)
//                {
//                    whitelistStr += FileWhiteList[i];
//                    if(i < FileWhiteList.Count - 1)
//                    {
//                        whitelistStr += ";";
//                    }
//                }
//                var whitelistPath = Path.Combine(mPatchFolderPatch, "file_whitelist.conf");
//                if (File.Exists(whitelistPath))
//                {
//                    File.Delete(whitelistPath);
//                }
//                File.WriteAllText(whitelistPath, whitelistStr);
//            }

//            //处理补丁总包
//            TinaX.IO.XFolder.ZipFolder(mTempPath, patch_path);
//            var md5 = TinaX.IO.XFile.GetMD5(patch_path);
//            var md5_path = patch_path + ".md5.txt";
//            if (File.Exists(md5_path))
//            {
//                File.Delete(md5_path);
//            }
//            File.WriteAllText(md5_path, md5);

//            var patch_log_json = JsonUtility.ToJson(patch_log_info);
//            var patch_log_path = Path.Combine(mPackageDataDirPath, PatchName + ".log");
//            if (File.Exists(patch_log_path))
//            {
//                File.Delete(patch_log_path);
//            }
//            if (!Directory.Exists(mPackageDataDirPath))
//            {
//                Directory.CreateDirectory(mPackageDataDirPath);
//            }
//            File.WriteAllText(patch_log_path, patch_log_json);

//            Directory.Delete(mTempVFSPatchPath, true);
//            Directory.Delete(mTempPath, true);

//            Debug.Log("补丁打包完成");
//            Debug.Log("补丁包："+patch_path);
//            Debug.Log("日志：" + patch_log_path);
            
//        }


//        /*
//         * 补丁包打包流程：
//         * 
//         * 1. 区分补丁包类型
//         *  - 最新包（打包时只需对比母包数据）
//         *      - 补丁包中包含一份文件白名单，安装补丁包时，白名单之外的应该被删除
//         *      
//         *  - 依次安装（打包时需要遍历对比所有版本比自己小的包）
//         *      - 由于每个补丁安装的时候都有删除操作，所以不用关心漏删文件的问题
//         * 
//         * 
//         */


//    }
//}

