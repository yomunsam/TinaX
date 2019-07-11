using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
//using CI.HttpClient;
using UnityEngine;
using UniRx;
using UniRx.Async;
using UnityEngine.Networking;
using TinaX.IO;

namespace TinaX.Upgrade
{
    /// <summary>
    /// 更新管理器
    /// </summary>
    public class UpgradeMgr
    {
        
        


        /// <summary>
        /// 配置文件
        /// </summary>
        private UpgradeConfig mUpgradeConfig;
        private LocalVersionInfo mLocalVersion;
        private MainConfig mMainConfig;

        private string mPatchDownloadTempDir; //存放下载下来的".xpk"补丁文件的临时目录
        private string mPatchUnzipTemp; //用来解压补丁包的临时根目录

        private StaticUpgradeConfigJsonTpl mStaticUpgradeTpl;

        private E_UpgradeHandleType mHandleType;

        public UpgradeMgr()
        {
            mUpgradeConfig = TinaX.Config.GetTinaXConfig<UpgradeConfig>(TinaX.Conf.ConfigPath.upgrade);
            mMainConfig = Config.GetTinaXConfig<MainConfig>(Conf.ConfigPath.main);

            mPatchDownloadTempDir = Path.Combine(XCore.I.LocalStorage_TinaX, "patch", "download");
            if (!Directory.Exists(mPatchDownloadTempDir))
            {
                Directory.CreateDirectory(mPatchDownloadTempDir);
            }
            mPatchUnzipTemp = Path.Combine(XCore.I.LocalStorage_TinaX, "patch", "temp");
            if (!Directory.Exists(mPatchUnzipTemp))
            {
                Directory.CreateDirectory(mPatchUnzipTemp);
            }

            //检索本地版本信息
            var local_version_file_path = Path.Combine(XCore.I.LocalStorage_TinaX, "version.json");
            if (!File.Exists(local_version_file_path))
            {
                //文件不存在
                mLocalVersion = new LocalVersionInfo();
                //母包版本
                int platform_base_version;
                if (mUpgradeConfig.GetBaseVersion_IfOverride(out platform_base_version))
                {
                    mLocalVersion.base_version = platform_base_version;
                }
                else
                {
                    mLocalVersion.base_version = mMainConfig.Version_Code;
                }
                
                mLocalVersion.patch_version = -1;

                var json_str = JsonUtility.ToJson(mLocalVersion);
                Directory.CreateDirectory(Directory.GetParent(local_version_file_path).ToString());
                File.WriteAllText(local_version_file_path, json_str,Encoding.UTF8);
            }
            else
            {
                var json_str = File.ReadAllText(local_version_file_path);
                mLocalVersion = JsonUtility.FromJson<LocalVersionInfo>(json_str);
                if (mUpgradeConfig.GetBaseVersion_IfOverride(out int platform_base_version))
                {
                    mLocalVersion.base_version = platform_base_version;
                }
                else
                {
                    mLocalVersion.base_version = mMainConfig.Version_Code;
                }

            }
        }

        /// <summary>
        /// 检查更新
        /// </summary>
        /// <param name="callback"></param>
        public void CheckUpgrade(Action<E_UpgradeCheckResults> callback)
        {
            Debug.Log("检查更新");
            if(callback == null)
            {
                return;
            }

            //读配置
            if(mUpgradeConfig == null)
            {
                callback(E_UpgradeCheckResults.error);
                return;
            }
            //开没开更新功能
            if (!mUpgradeConfig.Enable_upgrade)
            {
                callback(E_UpgradeCheckResults.not_enable);
                return;
            }

            if(mUpgradeConfig.Upgrade_Mode == E_UpgradeMode.static_web)
            {
                //静态web更新模式
                pCheckUpgrade_StaticWeb(callback); //交给static web的检查更新方法去处理了
            }
            else
            {
                callback(E_UpgradeCheckResults.expect);
                return;
            }

        }

        public E_UpgradeHandleType GetUpgradeHandleType()
        {
            if (mUpgradeConfig.Upgrade_Mode == E_UpgradeMode.static_web)
            {
                return mStaticUpgradeTpl.GetUpgradeHandleType();
            }

            return E_UpgradeHandleType.error;
        }

        /// <summary>
        /// 执行更新
        /// </summary>
        /// <param name="OnFinish">更新完成的回调</param>
        /// <param name="OnStart">开始下载时的回调</param>
        /// <param name="OpenUrlCallback">如果需要打开网页的回调</param>
        public void DoUpgrade(Action<E_UpgradeResults> OnFinish, Action OnStart, Action<string> OpenUrlCallback = null)
        {
            //Debug.Log("执行更新");
            //有没有检查过更新
            if (!mUpgradeConfig.Enable_upgrade || mUpgradeConfig == null)
            {
                OnFinish( E_UpgradeResults.not_check_upgrade);
                return;
            }
            if(mUpgradeConfig.Upgrade_Mode == E_UpgradeMode.static_web)
            {
                //Debug.Log("静态更新");
                //静态更新
                if (mStaticUpgradeTpl == null)
                {
                    OnFinish(E_UpgradeResults.not_check_upgrade);
                    return;
                }

                //检查更新方法
                var update_handle_type = mStaticUpgradeTpl.GetUpgradeHandleType();
                if(update_handle_type == E_UpgradeHandleType.webpage)
                {
                    if(OpenUrlCallback != null)
                    {
                        OpenUrlCallback(mStaticUpgradeTpl.GetWebPageUrl());
                        return;
                    }
                    else
                    {
                        UnityEngine.Application.OpenURL(mStaticUpgradeTpl.GetWebPageUrl());
                    }
                }else if(update_handle_type == E_UpgradeHandleType.download)
                {
                    //下载补丁包
                    //1. 取出所有要下载的补丁包
                    //Debug.Log("下载补丁包");
                    var patchs = mStaticUpgradeTpl.GetDownloadList(mLocalVersion.base_version, mLocalVersion.patch_version);
                    if(patchs.Length == 0)
                    {
                        Debug.LogWarning("[TinaX][热更新系统] 似乎服务器并没有正确的返回要下载的补丁包的列表");
                        OnFinish(E_UpgradeResults.success);
                        return;
                    }
                    var patch_max_version = patchs[0].version;
                    foreach(var patch in patchs)
                    {
                        if(patch.version > patch_max_version)
                        {
                            patch_max_version = patch.version;
                        }
                    }
                    OnStart();
                    Download_Patchs(patchs,(_res)=>{
                        //下载操作结束
                        if(_res == E_UpgradeResults.success)
                        {

                            //更新版本信息
                            mLocalVersion.patch_version = patch_max_version;
                            //写版本信息到本地
                            var json_str_local_version = JsonUtility.ToJson(mLocalVersion);
                            var local_version_file_path = Path.Combine(XCore.I.LocalStorage_TinaX, "version.json");
                            if (File.Exists(local_version_file_path))
                            {
                                File.Delete(local_version_file_path);
                            }
                            Directory.CreateDirectory(Directory.GetParent(local_version_file_path).ToString());
                            File.WriteAllText(local_version_file_path, json_str_local_version, Encoding.UTF8);

                            //往上回调啊

                        }
                        OnFinish(_res);
                    });
                }
                


            }
        }

        /// <summary>
        /// 从本地安装一个VFS补丁
        /// </summary>
        /// <param name="DeleteAfterInstall">是否在安装后删除补丁文件</param>
        /// <returns>安装成功返回true,失败返回false</returns>
        public bool InstallPatchFromLocal(string patch_path , bool DeleteAfterInstall = false)
        {
            var target_dir = AssetsMgr.I.GetVFSPersistentDataPath(); //VFS补丁根目录

            //解压补丁
            var patch_unzip_dir_path = Path.Combine(mPatchUnzipTemp, Path.GetFileNameWithoutExtension(patch_path));
            if (Directory.Exists(patch_unzip_dir_path))
            {
                Directory.CreateDirectory(patch_unzip_dir_path);
            }
            TinaX.IO.XFile.Unzip(patch_path, patch_unzip_dir_path);

            //VFS更新处理
            var vfs_patch_path = Path.Combine(patch_unzip_dir_path, "patch.vfs");
            if (File.Exists(vfs_patch_path))
            {
                TinaX.IO.XFile.Unzip(vfs_patch_path, target_dir);
            }

            //特殊文件处理：白名单标记
            var vfs_whitelist_path = Path.Combine(patch_unzip_dir_path, "patch","files_whitelist.conf");
            if (File.Exists(vfs_whitelist_path))
            {
                //对比白名单
                var files_str = System.IO.File.ReadAllText(vfs_whitelist_path);
                if (!files_str.IsNullOrEmpty())
                {
                    var files_arr = files_str.Split(';');
                    List<string> filesList = new List<string>(files_arr);

                    var cur_files = System.IO.Directory.GetFiles(target_dir, "*.*", SearchOption.AllDirectories);
                    foreach(var item in cur_files)
                    {
                        if (!filesList.Contains(item))
                        {
                            //不在白名单上
                            System.IO.File.Delete(item);
                        }
                    }

                }
            }



            //删除文件（夹
            Directory.Delete(patch_unzip_dir_path, true);
            if (DeleteAfterInstall)
            {
                if (File.Exists(patch_path))
                {
                    File.Delete(patch_path);
                }
            }

            return true;
        }




        /// <summary>
        /// 私有方法：Static Web模式，检查更新
        /// </summary>
        /// <param name="callback"></param>
        private void pCheckUpgrade_StaticWeb(Action<E_UpgradeCheckResults> callback)
        {
            //Debug.Log("走到这里了:" + mUpgradeConfig.Static_Json_Url);

            var a = GetJson(mUpgradeConfig.GetStaticJsonUrl(), (json) => {
                if (json.IsNullOrEmpty())
                {
                    callback(E_UpgradeCheckResults.connect_error);
                }
                else
                {
                    //抓到了json，准备解析
                    var obj = pParse_StaticWeb_Json(json);
                    mStaticUpgradeTpl = obj;
                    if (obj.IsNeedUpgrade(mLocalVersion.base_version, mLocalVersion.patch_version))
                    {
                        mHandleType = obj.GetUpgradeHandleType();
                        //需要更新
                        callback(E_UpgradeCheckResults.upgrade);
                    }
                    else
                    {
                        mHandleType = obj.GetUpgradeHandleType();
                        //不需要
                        callback(E_UpgradeCheckResults.newest);
                    }
                }
            });

            
            
        }

        async UniTask GetJson(string url, Action<string> callback)
        {
            // get async webrequest
            async UniTask<string> GetTextAsync(UnityWebRequest req)
            {
                var op = await req.SendWebRequest();
                return op.downloadHandler.text;
            }

            var task = GetTextAsync(UnityWebRequest.Get(url));

            var json_data = await UniTask.WhenAll(task);

            if (json_data.Length == 1)
            {
                callback(json_data[0]);
            }
            else
            {
                callback("");
            }
        }

        private StaticUpgradeConfigJsonTpl pParse_StaticWeb_Json(string json)
        {
            return JsonUtility.FromJson<StaticUpgradeConfigJsonTpl>(json);
        }


        /// <summary>
        /// 下载补丁包
        /// </summary>
        /// <param name="patchs">补丁包信息们</param>
        /// <param name="OnFinish">所有补丁包下载完成</param>
        private void Download_Patchs(StaticUpgradeConfigJsonTpl.PatchInfo[] patchs,Action<E_UpgradeResults> OnFinish)
        {
            int index = -1;

            void Download_one() //处理一个下载，递归
            {
                index++;
                if(patchs.Length < (index + 1))
                {
                    //全部下载完成
                    //Debug.Log("全部下载并处理完成");
                    //删掉全部补丁包
                    foreach(var item in patchs)
                    {
                        var cur_path = Path.Combine(mPatchDownloadTempDir, "patch_" + item.version + ".xpk");
                        if (File.Exists(cur_path))
                        {
                            File.Delete(cur_path);
                        }
                    }

                    //Debug.Log("下载流程开始回调");
                    OnFinish(E_UpgradeResults.success);
                    return;
                }


                var patch_path = Path.Combine(mPatchDownloadTempDir, "patch_" + patchs[index].version + ".xpk");
                //Debug.Log("处理：" + patch_path);
                //处理这个patch的下载任务

                var flag_exist = false;
                //看看这个文件会不会已经存在了
                if (File.Exists(patch_path))
                {
                    //哇还真的存在了

                    //文件名一样的话，验证一下md5,要是md5一下那就不用再下载了
                    if (patchs[index].md5.IsNullOrEmpty())
                    {
                        //服务端没有给MD5信息，就当验证通过了
                        flag_exist = true;
                    }
                    else
                    {
                        if(XFile.GetMD5(patch_path).ToLower() == patchs[index].md5)
                        {
                            //相等
                            flag_exist = true;
                        }
                        else
                        {
                            flag_exist = false;
                        }
                    }

                }

                if (flag_exist)
                {
                    Download_one();
                }
                else
                {
                    //处理当前的下载任务
                    //Debug.Log("  准备下载它");
                    var task = Get_A_File(patchs[index].url, data =>
                    {
                        //Debug.Log("  下载好了");
                        if (data == null)
                        {
                            //Debug.Log("   下载结果：失败");
                            //某个文件下载失败;
                            OnFinish(E_UpgradeResults.connect_lost);
                            return;
                        }
                        else
                        {
                            //下载成功，写出这个文件
                            //Debug.Log("写出文件");
                            if (File.Exists(patch_path))
                            {
                                File.Delete(patch_path);
                            }
                            File.WriteAllBytes(patch_path, data);

                            //验证MD5
                            if (patchs[index].md5.IsNullOrEmpty())
                            {
                                //服务端没有给MD5信息，就当验证通过了
                            }
                            else
                            {
                                if (XFile.GetMD5(patch_path).ToLower() == patchs[index].md5)
                                {
                                    //相等,验证通过
                                }
                                else
                                {
                                    //诶呀，md5不一样，是哪儿出问题了呢？
                                    OnFinish(E_UpgradeResults.files_check_fail);
                                    return;
                                }
                            }

                            //解压
                            //Debug.Log("解压目录：" + AssetsMgr.I.GetVFSPersistentDataPath());
                            //if (!Directory.Exists(AssetsMgr.I.GetVFSPersistentDataPath()))
                            //{
                            //    Directory.CreateDirectory(AssetsMgr.I.GetVFSPersistentDataPath());
                            //}
                            //TinaX.IO.XFile.Unzip(patch_path, AssetsMgr.I.GetVFSPersistentDataPath());
                            InstallPatchFromLocal(patch_path, true);


                            Download_one();
                        }
                    });
                }

                

            }

            Download_one();
        }

        async UniTask Get_A_File(string url, Action<byte[]> callback)
        {
            // get async webrequest
            async UniTask<byte[]> GetBytesAsync(UnityWebRequest req)
            {
                var op = await req.SendWebRequest();
                return op.downloadHandler.data;
            }
            var task = GetBytesAsync(UnityWebRequest.Get(url));

            var download_data = await UniTask.WhenAll(task);
            if(download_data.Length == 1)
            {
                callback(download_data[0]);
            }
            else
            {
                callback(null);
            }
        }

    }

    /// <summary>
    /// 检查升级更新的结果反馈
    /// </summary>
    public enum E_UpgradeCheckResults
    {
        /// <summary>
        /// 未知错误
        /// </summary>
        error,
        /// <summary>
        /// 未启用更新
        /// </summary>
        not_enable, 
        /// <summary>
        /// 功能未实现
        /// </summary>
        expect,
        /// <summary>
        /// 连接服务器失败
        /// </summary>
        connect_error,
        /// <summary>
        /// 已经是最新版本，不需要更新
        /// </summary>
        newest,
        /// <summary>
        /// 需要升级
        /// </summary>
        upgrade,
    }


    /// <summary>
    /// 更新计划信息
    /// </summary>
    public struct S_UpgradePlan_Info
    {
        /// <summary>
        /// 一共要下载这么些文件
        /// </summary>
        public int Total_Download_Num;

        /// <summary>
        /// 一共要下载这么多KiB的数据
        /// </summary>
        public float Total_Download_KiB_Size;
    }

    /// <summary>
    /// 加载进度
    /// </summary>
    public struct S_UpgradeStatus
    {

    }

    /// <summary>
    /// 更新结果
    /// </summary>
    public enum E_UpgradeResults
    {
        /// <summary>
        /// 网络故障
        /// </summary>
        connect_lost,

        /// <summary>
        /// 更新成功
        /// </summary>
        success,

        /// <summary>
        /// 部分文件校验失败
        /// </summary>
        files_check_fail,

        /// <summary>
        /// 没有检查更新或检查更新失败
        /// </summary>
        not_check_upgrade,

        /// <summary>
        /// 未知错误
        /// </summary>
        error,
    }

    /// <summary>
    /// 状态枚举
    /// </summary>
    public enum E_Status
    {

    }


    /// <summary>
    /// 更新处理方式
    /// </summary>
    public enum E_UpgradeHandleType
    {
        /// <summary>
        /// 打开网页
        /// </summary>
        webpage,
        /// <summary>
        /// 直接下载补丁包
        /// </summary>
        download,

        /// <summary>
        /// 下载母包（安装包）
        /// </summary>
        download_base_pack,

        error
    }
}


