using UnityEngine;
using UniRx;
using UniRx.Async;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.IO;
using System;
using System.Threading.Tasks;
using System.Linq;
using UnityEngine.Networking;
using TinaX.IO;
using TinaX.Exceptions;

namespace TinaX.VFSKit
{
    public class VFSKit : IVFS
    {
        /// <summary>
        /// vfs file root path, eg: {PersistentDataPath}/tinaxCache/vfs_file/windows/ 
        /// </summary>
        private string mVFilePersistentDataPath;
        private string mVFileStreamingAssetsPath;
        private VFSConfigModel mConfig;
        private VFSFileHashModel mStreamingAsset_FileHash;
        //private string mStreamingAssetManifestBbundlePath;
        //private string mStreamingAssets_AssetBundleManifest_Copy_ReadWriteable_Path;   //在游戏启动的时候，把StreamingAssets下的AssetBundleManifest文件复制一份到PersistentData下
        //private string mPersistenManifestBundlePath;
        private AssetBundleManifest mMainAssetBundleManifest;
#pragma warning disable IDE0052 // 删除未读的私有成员
        private string mMainAssetBundleManifestPath;
#pragma warning restore IDE0052 // 删除未读的私有成员
        private MainABManifestType mMainAssetBundleManifestType = MainABManifestType.None;   //MainAssetBundleManifest来自哪儿呢

        //private AssetBundleManifest mStreamingPathAssetBundleManifest;
        //private AssetBundleManifest mPersistentDataAssetBundleManifest;
        private string mVFileVersionInfoPath; //描述当前VFS资源版本的json文件的路径
        private VFSFileVersionInfoModel mVFileVersionInfo;
        private LoadedAssetBundleMgr mLoadedABMgr;
        private AssetBundleLoadPlanMgr mABLoadPlanMgr;  //异步加载计划管理
        private readonly byte[] mEncrySign = System.Text.Encoding.UTF8.GetBytes(VFSEncryConst.EncryFileSign);    //加密头部标记
        private FileDownloadPlanMgr mDownloadPlanMgr;    //异步 Web下载计划管理

        //======  WEB VFS

        private bool mEnableWebVFS = false;
        private bool mWebVFSIniting = false;
        private readonly UniTask mWebVFSInitTask;
        private bool mWebVFSInited = false;
        private string mVFileWebVFSCachePath;
        private WebVFSConfig mCurWebVFSConfig;
        //private AssetBundleManifest mWebVFSAssetBundleManifest;
        private VFSFileHashModel mWebVFS_FileHash;


#if UNITY_EDITOR
        private string mVFileTinaXWorkFolder;
        //private AssetBundleManifest mTinaXWorkFolderAssetBundleManifest;
#endif

        public VFSKit()
        {



        }

        public async Task CtorAsync()
        {
            mConfig = TinaX.Config.GetTinaXConfig<VFSConfigModel>(Conf.ConfigPath.vfs).GetPerfect();
            mDownloadPlanMgr = new FileDownloadPlanMgr();
            mLoadedABMgr = new LoadedAssetBundleMgr();
            mABLoadPlanMgr = new AssetBundleLoadPlanMgr();

            //Web VFS   -- 这部分的初始化是堵在主线程同步执行的
            if (mConfig.EnableWebVFS)
            {
                //检查是否有符合当前平台的配置
                var myConfs = mConfig.ConfigWebVFS.Where(item => item.Platform == TinaX.Platform.GetPlatform(Application.platform));
                if (myConfs.Count() > 0)
                {
                    mVFileWebVFSCachePath = $"{XCore.I.LocalStorage_TinaX}/{VFSPathConst.VFS_Web_Cache}/{Const.PlatformConst.GetPlatformName(Application.platform).ToLower()}/";
                    XDirectory.CreateIfNotExists(mVFileWebVFSCachePath);

                    var myConf = myConfs.First();
                    mCurWebVFSConfig = myConf;
                    mEnableWebVFS = true;

                    #region 变量的默认值和格式调整
                    //变量的默认值和格式调整

                    if (!mCurWebVFSConfig.Url.EndsWith("/"))
                    {
                        mCurWebVFSConfig.Url += "/";
                    }
                    if (!mCurWebVFSConfig.GetMD5Url.EndsWith("/"))
                    {
                        mCurWebVFSConfig.GetMD5Url += "/";
                    }


                    if (mCurWebVFSConfig.GetMD5ListUrl.IsNullOrEmpty())
                    {
                        mCurWebVFSConfig.GetMD5ListUrl = mCurWebVFSConfig.Url + VFSPathConst.VFS_File_AssetBundleHash_FileName;
                    }

                    #endregion

                    if (mConfig.InitWebVFSOnFrameworkStart)
                    {
                        try
                        {
                            mWebVFSIniting = true;
                            await InitWebVFSAsync();
                            mWebVFSIniting = false;
                            mWebVFSInited = true;

                        }
                        catch (VFSException e)
                        {
                            Exceptions.VFSException.CallInitException(new VFSException("Web VFS Init Failed: " + e.Message, VFSException.VFSErrorType.WebVFS_InitFailed));
                            mWebVFSInited = false;
                            mEnableWebVFS = false;
                            mWebVFSIniting = false;

                        }
                    }
                    else
                    {
                        mWebVFSInited = false;
                        mWebVFSIniting = false;
                    }



                }

            }


            if (mConfig.FileMode == VFSMode.AssetBundle)
            {
                //初始化数据
                mVFilePersistentDataPath = $"{XCore.I.LocalStorage_TinaX}/{VFSPathConst.VFS_File}/{Const.PlatformConst.GetPlatformName(Application.platform).ToLower()}/";
                mVFileStreamingAssetsPath = $"{Application.streamingAssetsPath}/{VFSPathConst.VFS_File}/{Const.PlatformConst.GetPlatformName(Application.platform).ToLower()}/";
                XDirectory.CreateIfNotExists(mVFilePersistentDataPath);




                //检索版本信息
                mVFileVersionInfoPath = Path.Combine(mVFilePersistentDataPath, VFSPathConst.VFS_Data, VFSPathConst.VFS_File_VersionInfo_FileName);
                if (File.Exists(mVFileVersionInfoPath))
                {
                    var json_str = File.ReadAllText(mVFileVersionInfoPath, System.Text.Encoding.UTF8);
                    mVFileVersionInfo = JsonUtility.FromJson<VFSFileVersionInfoModel>(json_str);

                }
                if (mVFileVersionInfo == null)
                {
                    mVFileVersionInfo = new VFSFileVersionInfoModel
                    {
                        BaseVersion = XCore.I.AppVersion,
                        PatchVersion = -1
                    };
                }

#if UNITY_EDITOR
                if (IsUseAssetBundleInEdtor())
                {
                    if (IsLoadAssetBundleFromStreamingAssets())
                    {
                        //先试着初始化PersistentData的VFS空间
                        Init_AssetBundle_PersistentData();
                        //然后试着初始化StreamingAssets的
                        Init_AssetBundle_StreamingAssets();
                    }
                    else
                    {
                        //先试着初始化PersistentData的VFS空间
                        Init_AssetBundle_PersistentData();
                        //然后，因为在编辑器下需要用TinaX Work Folder替代StreamingAssets
                        InitAssetBundleDataForTinaXWorkFolder();

                    }
                }

#else
                //先试着初始化PersistentData的VFS空间
                Init_AssetBundle_PersistentData();
                //然后试着初始化StreamingAssets的
                Init_AssetBundle_StreamingAssets();
#endif




                void Init_AssetBundle_StreamingAssets()
                {
                    var streamingAssets_ABManifest_Path = mVFileStreamingAssetsPath + Const.PlatformConst.GetPlatformName(Application.platform).ToLower();

                    //初始化StreamingAssets下面的文件表
                    InitStreamingAssetsFileHash().Wait();

                    if (mMainAssetBundleManifest == null || mMainAssetBundleManifestType == MainABManifestType.None)
                    {
                        //目前既没有来自WebVFS的Manifest， 也没有来自PersistenData的Manifest，
                        //那老朽就勉为其难的加载一下自己的Manifest吧

                        var streamingAsset_manifest_bundle = AssetBundle.LoadFromFile(streamingAssets_ABManifest_Path);
                        mMainAssetBundleManifest = streamingAsset_manifest_bundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
                        mMainAssetBundleManifestPath = streamingAssets_ABManifest_Path;
                        mMainAssetBundleManifestType = MainABManifestType.StreamingAssets;
                        streamingAsset_manifest_bundle?.Unload(false);
                    }
                }


                void Init_AssetBundle_PersistentData()
                {
                    var persistentData_ABManifest_Path = mVFilePersistentDataPath + Const.PlatformConst.GetPlatformName(Application.platform).ToLower();
                    if (File.Exists(persistentData_ABManifest_Path))
                    {
                        //如果没有来自WebVFS的Manifest， 那就加载我的吧
                        if (mMainAssetBundleManifest == null || mMainAssetBundleManifestType == MainABManifestType.None)
                        {
                            var persisten_manifest_bundle = AssetBundle.LoadFromFile(persistentData_ABManifest_Path);
                            mMainAssetBundleManifest = persisten_manifest_bundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
                            mMainAssetBundleManifestPath = persistentData_ABManifest_Path;
                            mMainAssetBundleManifestType = MainABManifestType.Persistentdatapath;
                            persisten_manifest_bundle?.Unload(false);
                            persisten_manifest_bundle = null;
                        }

                    }
                }

            }
        }

        public T LoadAsset<T>(string assetPath) where T : UnityEngine.Object
        {
            //解析加载路径
            var parseInfo = AssetParseHelper.Parse(assetPath, mConfig);
            //是否有效
            if (parseInfo.IsValid)
            {
                //加载文件
#if UNITY_EDITOR
                //编辑器下，优先以编辑器指定的模式运行
                if (IsUseAssetBundleInEdtor())  //AssetBundle加载模式
                {
                    return LoadAssetByAssetBundle<T>(assetPath, parseInfo);
                }
                else if (IsUseAssetDatabaseInEditor())  //AssetDatabase
                {
                    return LoadAssetByAssetDatabase<T>(assetPath);
                }else if (IsUseResourcesInEditor())
                {
                    return LoadAssetByResources<T>(assetPath);
                }
                
#else
                //Runtime,
                if (mConfig.FileMode == VFSMode.AssetBundle)
                {
                    //使用AssetBundle加载
                    return LoadAssetByAssetBundle<T>(assetPath, parseInfo);

                }
                else if(mConfig.FileMode == VFSMode.Resources)
                {
                    //使用Resources加载
                    return LoadAssetByResources<T>(assetPath);
                }
#endif
            }
            else
            {
                string desc = "";
                if (!parseInfo.InWhiteList)
                {
                    desc = " Asset path not in VFS whitelist. ";
                }
                if (parseInfo.Ignore)
                {
                    desc = " Asset path is ignored. ";
                }
                //抛异常
                throw new TinaX.Exceptions.VFSException($"File Load Fail: {assetPath}  --> {desc}",parseInfo, Exceptions.VFSException.VFSErrorType.PathNotValid);
                
            }
            return default;
        }

        public UnityEngine.Object LoadAsset(string assetPath,System.Type type)
        {
            var parse_info = AssetParseHelper.Parse(assetPath,mConfig);
            if (parse_info.IsValid)
            {
#if UNITY_EDITOR
                //Editor
                if (IsUseAssetBundleInEdtor())
                {
                    return LoadAssetByAssetBundle(assetPath,type, parse_info);
                }
                else if (IsUseAssetDatabaseInEditor())
                {
                    return LoadAssetByAssetDatabase(assetPath,type);
                }
                else if (IsUseResourcesInEditor())
                {
                    return LoadAssetByResources(assetPath,type);
                }
                else
                {
                    return default;
                }

#else
                //Runtime
                switch (mConfig.FileMode)
                {
                    case VFSMode.AssetBundle:
                        return LoadAssetByAssetBundle(assetPath, type,parse_info);
                    case VFSMode.Resources:
                        return LoadAssetByResources(assetPath,type);
                    default:
                        return default;
                }
#endif
            }
            else
            {
                string desc = "";
                if (!parse_info.InWhiteList)
                {
                    desc = " Asset path not in VFS whitelist. ";
                }
                if (parse_info.Ignore)
                {
                    desc = " Asset path is ignored. ";
                }
                //抛异常
                throw new TinaX.Exceptions.VFSException($"File Load Fail: {assetPath}  --> {desc}", parse_info, Exceptions.VFSException.VFSErrorType.PathNotValid);
            }
        }

        public async UniTask<T> LoadAssetAsync<T>(string assetPath) where T : UnityEngine.Object
        {
            var parseInfo = AssetParseHelper.Parse(assetPath, mConfig);
            if (parseInfo.IsValid)
            {
#if UNITY_EDITOR
                //Editor
                if (IsUseAssetBundleInEdtor())
                {
                    return await LoadAssetByAssetBundleAsync<T>(assetPath, parseInfo);
                }else if (IsUseAssetDatabaseInEditor())
                {
                    return await LoadAssetByAssetDatabaseAsync<T>(assetPath);
                }else if (IsUseResourcesInEditor())
                {
                    return await LoadAssetByResourcesAsync<T>(assetPath);
                }
                else
                {
                    return default;
                }

#else
                //Runtime
                switch (mConfig.FileMode)
                {
                    case VFSMode.AssetBundle:
                        return await LoadAssetByAssetBundleAsync<T>(assetPath, parseInfo);
                    case VFSMode.Resources:
                        return await LoadAssetByResourcesAsync<T>(assetPath);
                    default:
                        return default;
                }
#endif
            }
            else
            {
                string desc = "";
                if (!parseInfo.InWhiteList)
                {
                    desc = " Asset path not in VFS whitelist. ";
                }
                if (parseInfo.Ignore)
                {
                    desc = " Asset path is ignored. ";
                }
                //抛异常
                throw new TinaX.Exceptions.VFSException($"File Load Fail: {assetPath}  --> {desc}", parseInfo, Exceptions.VFSException.VFSErrorType.PathNotValid);
            }

        }

        public async UniTask<UnityEngine.Object> LoadAssetAsync(string assetPath, System.Type type)
        {
            var parseInfo = AssetParseHelper.Parse(assetPath, mConfig);
            if (parseInfo.IsValid)
            {
#if UNITY_EDITOR
                //Editor
                if (IsUseAssetBundleInEdtor())
                {
                    return await LoadAssetByAssetBundleAsync(assetPath,type, parseInfo);
                }
                else if (IsUseAssetDatabaseInEditor())
                {
                    return await LoadAssetByAssetDatabaseAsync(assetPath,type);
                }
                else if (IsUseResourcesInEditor())
                {
                    return await LoadAssetByResourcesAsync(assetPath,type);
                }
                else
                {
                    return default;
                }

#else
                //Runtime
                switch (mConfig.FileMode)
                {
                    case VFSMode.AssetBundle:
                        return await LoadAssetByAssetBundleAsync(assetPath,type, parseInfo);
                    case VFSMode.Resources:
                        return await LoadAssetByResourcesAsync(assetPath,type);
                    default:
                        return default;
                }
#endif
            }
            else
            {
                string desc = "";
                if (!parseInfo.InWhiteList)
                {
                    desc = " Asset path not in VFS whitelist. ";
                }
                if (parseInfo.Ignore)
                {
                    desc = " Asset path is ignored. ";
                }
                //抛异常
                throw new TinaX.Exceptions.VFSException($"File Load Fail: {assetPath}  --> {desc}", parseInfo, Exceptions.VFSException.VFSErrorType.PathNotValid);
            }
        }

        public void LoadAssetAsync<T>(string assetPath, Action<T> callback) where T : UnityEngine.Object
        {
            var parse_info = AssetParseHelper.Parse(assetPath, mConfig);
            if (parse_info.IsValid)
            {
#if UNITY_EDITOR
                //Editor
                if (IsUseAssetBundleInEdtor())
                {
                    LoadAssetByAssetBundleAsync<T>(assetPath, parse_info,callback);
                    return;
                }
                else if (IsUseAssetDatabaseInEditor())
                {
                    LoadAssetByAssetDatabaseAsync<T>(assetPath,callback);
                    return;
                }
                else if (IsUseResourcesInEditor())
                {
                    LoadAssetByResourcesAsync(assetPath, callback);
                    return;
                }
                else
                {
                    return;
                }

#else
                //Runtime
                switch (mConfig.FileMode)
                {
                    case VFSMode.AssetBundle:
                        LoadAssetByAssetBundleAsync<T>(assetPath,parse_info,callback);
                        return;
                    case VFSMode.Resources:
                        LoadAssetByResourcesAsync<T>(assetPath, callback);
                        return;
                    default:
                        return;
                }
#endif
            }
            else
            {
                string desc = "";
                if (!parse_info.InWhiteList)
                {
                    desc = " Asset path not in VFS whitelist. ";
                }
                if (parse_info.Ignore)
                {
                    desc = " Asset path is ignored. ";
                }
                //抛异常
                throw new TinaX.Exceptions.VFSException($"File Load Fail: {assetPath}  --> {desc}", parse_info, Exceptions.VFSException.VFSErrorType.PathNotValid);
            }
        }

        public void LoadAssetAsync(string assetPath, System.Type type, Action<UnityEngine.Object> callback)
        {
            var parse_info = AssetParseHelper.Parse(assetPath, mConfig);
            if (parse_info.IsValid)
            {
#if UNITY_EDITOR
                //Editor
                if (IsUseAssetBundleInEdtor())
                {
                    LoadAssetByAssetBundleAsync(assetPath, type, parse_info,callback);
                    return;
                }
                else if (IsUseAssetDatabaseInEditor())
                {
                    LoadAssetByAssetDatabaseAsync(assetPath, type,callback);
                    return;
                }
                else if (IsUseResourcesInEditor())
                {
                    LoadAssetByResourcesAsync(assetPath, type,callback);
                    return;
                }
                else
                {
                    return;
                }

#else
                //Runtime
                switch (mConfig.FileMode)
                {
                    case VFSMode.AssetBundle:
                        LoadAssetByAssetBundleAsync(assetPath, type,parse_info,callback);
                        return;
                    case VFSMode.Resources:
                        LoadAssetByResourcesAsync(assetPath,type, callback);
                        return;
                    default:
                        return;
                }
#endif
            }
            else
            {
                string desc = "";
                if (!parse_info.InWhiteList)
                {
                    desc = " Asset path not in VFS whitelist. ";
                }
                if (parse_info.Ignore)
                {
                    desc = " Asset path is ignored. ";
                }
                //抛异常
                throw new TinaX.Exceptions.VFSException($"File Load Fail: {assetPath}  --> {desc}", parse_info, Exceptions.VFSException.VFSErrorType.PathNotValid);
            }
        }

        //WEBVFS----------------------------------------------------------------------------------------------------------

        public async Task InitWebVFS()
        {
            if (mWebVFSInited) return;
            await InitWebVFSAsync();
        }

        public async UniTask<T> LoadWebAssetAsync<T>(string assetPath,bool useCache = true) where T:UnityEngine.Object
        {
            if (!mEnableWebVFS)
            {
                //抛异常
                throw new Exceptions.VFSException($"web vfs not enable, load web asset failed: {assetPath}", Exceptions.VFSException.VFSErrorType.WebVFS_NotEnable);
            }
            await WaitIfWebVFSIniting();

            //一样，先解析
            var parse_info = AssetParseHelper.Parse(assetPath, mConfig);
            if (parse_info.IsValid)
            {
                //有效
                return await LoadAssetFromWebAsync<T>(assetPath, parse_info, useCache);
            }
            else
            {
                //抛异常
                string desc = "";
                if (!parse_info.InWhiteList)
                {
                    desc = " Asset path not in VFS whitelist. ";
                }
                if (parse_info.Ignore)
                {
                    desc = " Asset path is ignored. ";
                }
                //抛异常
                throw new TinaX.Exceptions.VFSException($"File Load Fail: {assetPath}  --> {desc}", parse_info, Exceptions.VFSException.VFSErrorType.PathNotValid);
            }
        }

        public void LoadWebAssetAsync<T>(string assetPath,Action<T> callback , bool useCache = true) where T : UnityEngine.Object
        {
            LoadWebAssetAsync<T>(assetPath, useCache)
                .ToObservable<T>()
                .ObserveOnMainThread()
                .Subscribe(asset =>
                {
                    callback?.Invoke(asset);
                });
        }

        public void LoadWebAssetAsync(string assetPath, System.Type type, Action<UnityEngine.Object> callback , bool useCache = true)
        {
            LoadWebAssetAsyncWithType(assetPath, type, useCache)
                .ToObservable()
                .ObserveOnMainThread()
                .Subscribe(asset =>
                {
                    callback?.Invoke(asset);
                });
        }

        //本地和WEB混合加载--------------------------------------------------------------------------------------------------

        public async UniTask<T> LoadAssetLocalOrWebAsync<T>(string assetPath) where T : UnityEngine.Object
        {
            try
            {
                return await LoadAssetAsync<T>(assetPath);
            }
            catch (Exceptions.VFSException e)
            {
                if(e.ErrorType == VFSException.VFSErrorType.FileNotExist || e.ErrorType == VFSException.VFSErrorType.PathNotValid)
                {

                    //本地没有，去Web端加载
                    try
                    {
                        return await LoadWebAssetAsync<T>(assetPath);
                    }
                    catch(Exceptions.VFSException we)
                    {
                        throw we;
                    }
                    catch(Exception we)
                    {
                        throw we;
                    }
                }
                else
                {
                    throw e;
                }
            }
            catch(Exception e)
            {
                throw e;
            }
        }


        public Task LoadScene<T>(string assetPath)
        {
            return Task.CompletedTask;
        }

        public void RemoveUse(string load_path)
        {
            RemoveAssetBundleUseNum(load_path);
        }

        public void GC()
        {

        }



        #region 常用加载类型封装

        public GameObject LoadPrefab(string assetPath)
        {
            return this.LoadAsset<GameObject>(assetPath);
        }

        public UniTask<GameObject> LoadPrefabAsync(string assetPath)
        {
            return LoadAssetAsync<GameObject>(assetPath);
        }

        public void LoadPrefabAsync(string assetPath, Action<GameObject> callbcak)
        {
            this.LoadAssetAsync<GameObject>(assetPath, callbcak);
        }

        #endregion





        #region AssetDatabase Load
#if UNITY_EDITOR

        /// <summary>
        /// 从AssetDatabase 加载资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        private T LoadAssetByAssetDatabase<T>(string path) where T : UnityEngine.Object
        {
            return AssetDatabase.LoadAssetAtPath<T>(path);
        }

        /// <summary>
        /// 从AssetDatabase加载资源
        /// </summary>
        /// <param name="path"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private UnityEngine.Object LoadAssetByAssetDatabase(string path, System.Type type)
        {
            return AssetDatabase.LoadAssetAtPath(path, type);
        }

        /// <summary>
        /// 从AssetDatabase 模拟异步加载资源 async/await
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private async UniTask<T> LoadAssetByAssetDatabaseAsync<T>(string path) where T : UnityEngine.Object
        {
            await UniTask.DelayFrame(1);
            return AssetDatabase.LoadAssetAtPath<T>(path);
        }

        private async UniTask<UnityEngine.Object> LoadAssetByAssetDatabaseAsync(string path,Type type)
        {
            await UniTask.DelayFrame(1);
            return AssetDatabase.LoadAssetAtPath(path,type);
        }


        /// <summary>
        /// 从AssetDatabase 模拟异步加载资源 async callback
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <param name="callback"></param>
        private void LoadAssetByAssetDatabaseAsync<T>(string path, Action<T> callback) where T : UnityEngine.Object
        {
            Observable.NextFrame()
                .Subscribe(unit =>
                {
                    var asset = AssetDatabase.LoadAssetAtPath<T>(path);
                    callback?.Invoke(asset);
                });
        }

        /// <summary>
        /// 从AssetDatabase 模拟异步加载资源 async callback
        /// </summary>
        /// <param name="path"></param>
        /// <param name="type"></param>
        /// <param name="callback"></param>
        private void LoadAssetByAssetDatabaseAsync(string path,System.Type type, Action<UnityEngine.Object> callback)
        {
            Observable.NextFrame()
                .Subscribe(unit =>
                {
                    var asset = AssetDatabase.LoadAssetAtPath(path, type);
                    callback?.Invoke(asset);
                });
        }

#endif
#endregion



#region AssetBundle Load

        private T LoadAssetByAssetBundle<T>(string path,AssetParseInfo parseInfo) where T: UnityEngine.Object
        {
            //加载对应的AB包，和依赖
            var ab = LoadAssetBundleAndDependencesRecursion(parseInfo.ABFileNameWithExtName);
            return ab.LoadAsset<T>(path);
        }

        private UnityEngine.Object LoadAssetByAssetBundle(string path,System.Type type, AssetParseInfo parseInfo)
        {
            var ab = LoadAssetBundleAndDependencesRecursion(parseInfo.ABFileNameWithExtName);
            return ab.LoadAsset(path,type);
        }

        private async UniTask<T> LoadAssetByAssetBundleAsync<T>(string path,AssetParseInfo parseInfo) where T : UnityEngine.Object
        {
            var ab = await LoadAssetBundleAndDependencesRecursionAsync(parseInfo.ABFileNameWithExtName);
            return await LoadAssetFromLoadedAssetBundle<T>(path, ab);
        }

        private async UniTask<UnityEngine.Object> LoadAssetByAssetBundleAsync(string path, System.Type type, AssetParseInfo parseInfo)
        {
            var ab = await LoadAssetBundleAndDependencesRecursionAsync(parseInfo.ABFileNameWithExtName);
            return await LoadAssetFromLoadedAssetBundle(path, ab, type);
        }


        private void LoadAssetByAssetBundleAsync<T>(string path,AssetParseInfo parseInfo, Action<T> callback)
        {
            LoadAssetBundleAndDependencesRecursionAsync(parseInfo.ABFileNameWithExtName)
                .ToObservable()
                .ObserveOnMainThread()
                .Subscribe(ab =>
                {
                    ab.LoadAssetAsync<T>(path)
                        .AsAsyncOperationObservable()
                        .Select(req => req.asset)
                        .OfType<UnityEngine.Object,T>()
                        .Subscribe((asset) =>
                        {
                            callback?.Invoke(asset);
                        });
                });
        }


        private void LoadAssetByAssetBundleAsync(string path, System.Type type, AssetParseInfo parseInfo, Action<UnityEngine.Object> callback)
        {
            LoadAssetBundleAndDependencesRecursionAsync(parseInfo.ABFileNameWithExtName)
                .ToObservable()
                .ObserveOnMainThread()
                .Subscribe(ab =>
                {
                    ab.LoadAssetAsync(path,type)
                        .AsAsyncOperationObservable()
                        .Select(req => req.asset)
                        .Subscribe((asset) =>
                        {
                            callback?.Invoke(asset);
                        });
                });
        }

        private AssetBundle LoadAssetBundleAndDependencesRecursion(string ab_path)
        {
            bool loadFromPerPath = IsAssetBundleFileExistInPersistenDataPath(ab_path);
            if (!loadFromPerPath)
            {
                //判断文件是否在StreamingAssets
                if (!IsFileExistsInSteamingAssets(ab_path))
                {
                    throw new Exceptions.VFSException($"File not exists in local :{ab_path}", VFSException.VFSErrorType.FileNotExist);
                }
            }
            string[] dependences = GetABDependences(ab_path);
            
            if (dependences.Length > 0)
            {
                foreach(var item in dependences)
                {
                    if(LoadAssetBundleAndDependencesRecursion(item) != null)
                    {
                        //登记资源引用
                        RegisterAssetBundleUseNum(item);
                    }
                }
            }

            //加载ab本体
            int UseNum = 0;
            Hash128 hash_will_load = GetABHashCode(ab_path);
            //检查下这个AssetBundle是不是已经加载了
            if (mLoadedABMgr.ContainsPath(ab_path))
            {
                var myLoadedData = mLoadedABMgr.GetDataByPath(ab_path);
                //有一个资源，看看是哪儿的
                if (myLoadedData.AssetBundleHashCode.ToString() != hash_will_load.ToString())
                {
                    //在PersistentData的位置有资源， 但目前的资源是来自  StreamingAssets 的
                    //释放掉现在的资源
                    myLoadedData.mAssetBundle.Unload(false);
                    mLoadedABMgr.RemoveByKey(ab_path);
                    //TODO 把旧资源上面的引用数量继承过来
                    UseNum = myLoadedData.UseNum;
                }
                else
                {
                    //当前我要的资源已经在内存里的，直接返回
                    return myLoadedData.mAssetBundle;
                }
            }

            //到这儿就是正式的加载了

            string ab_full_path = GetABFullPath(ab_path, loadFromPerPath);


            AssetBundle loaded_ab = null;
            var encryType = AssetParseHelper.ParseEncryTypeByAssetBundlePath(ab_path, mConfig);
            //各加密方式的加载判定
            if(encryType == EncryptionType.None)
            {
                //直接加载
                loaded_ab = AssetBundle.LoadFromFile(ab_full_path);
            }
            else if(encryType == EncryptionType.Offset)
            {
                ulong offsetValue = 0;
                
                
                //求偏移量
                if (mConfig.Encry_OffsetHandleType == EncryOffsetType.Default)
                {
                    //算出偏移量
                    offsetValue = EncryDefaultHander.GetOffsetValue(hash_will_load, ab_path);
                    // Debug.Log("加密偏移:" +  offsetValue);
                }

                //加载吧
                Debug.Log("offset:" + offsetValue + (ulong)mEncrySign.Length);
                loaded_ab = AssetBundle.LoadFromFile(ab_full_path, 0, offsetValue + (ulong)mEncrySign.Length);
            }

            //登记
            var loaded_info = new LoadedAssetBundle(loaded_ab, hash_will_load, ab_path,ab_full_path, dependences);
            if (UseNum > 0)
            {
                loaded_info.Cover_UseNum(UseNum);   //继承自旧AB的引用数量
            }
            mLoadedABMgr.Add(ab_path, loaded_info);
            //登记引用数量
            RegisterAssetBundleUseNum(ab_path);
            return loaded_ab;
        }

        private async UniTask<AssetBundle> LoadAssetBundleAndDependencesRecursionAsync(string ab_path)
        {
            bool loadFromPerPath = IsAssetBundleFileExistInPersistenDataPath(ab_path);
            if (!loadFromPerPath)
            {
                //判断文件是否在StreamingAssets
                if (!IsFileExistsInSteamingAssets(ab_path))
                {
                    throw new Exceptions.VFSException($"File not exists in local :{ab_path}", VFSException.VFSErrorType.FileNotExist);
                }
            }
            string[] dependences = GetABDependences(ab_path);
            if(dependences.Length > 0) //遍历加载
            {
                foreach(var item in dependences)
                {
                    if(await LoadAssetBundleAndDependencesRecursionAsync(item) != null)
                    {
                        //登记资产引用
                        RegisterAssetBundleUseNum(item);
                    }
                }
            }

            //加载本AB
            int UseNum = 0;
            Hash128 hashCode = GetABHashCode(ab_path);

            //检查下这个AssetBundle是不是已经加载了
            if (mLoadedABMgr.ContainsPath(ab_path))
            {
                var myLoadedData = mLoadedABMgr.GetDataByPath(ab_path);
                //有一个资源，看看是哪儿的
                if (myLoadedData.AssetBundleHashCode.ToString() != hashCode.ToString())
                {
                    //在PersistentData的位置有资源， 但目前的资源是来自  StreamingAssets 的
                    //释放掉现在的资源
                    myLoadedData.mAssetBundle.Unload(false);
                    mLoadedABMgr.RemoveByKey(ab_path);
                    //把旧资源上面的引用数量继承过来
                    UseNum = myLoadedData.UseNum;
                }
                else
                {
                    //当前我要的资源已经在内存里的，直接返回
                    return myLoadedData.mAssetBundle;
                }
            }



            //正式加载
            string ab_full_path = GetABFullPath(ab_path, loadFromPerPath);
            return await LoadSingleAssetBundleFromLocalAsync(ab_path, ab_full_path, hashCode, dependences, UseNum);

            #region 暂时不要了的旧代码
            //string ab_full_path = GetABFullPath(ab_path, loadFromPerPath);

            ////检查是否正在被加载
            //if (mABLoadPlanMgr.ContainsPath(ab_full_path))
            //{
            //    //加载计划中有，说明正在被加载
            //    var plan = mABLoadPlanMgr.Get(ab_full_path);
            //    await plan.LoadRequest;
            //    return plan.LoadRequest.assetBundle;    //结束，完毕，走人
            //}

            //AssetBundle loaded_ab = null;
            //var encryType = AssetParseHelper.ParseEncryTypeByAssetBundlePath(ab_path, mConfig);

            ////真要加载了啊

            ////根据加密方式来加载
            //if(encryType == EncryptionType.None)
            //{
            //    var req = AssetBundle.LoadFromFileAsync(ab_full_path);  //很智障，不知道Unirx为什么不能直接await这里
            //    //加入加载计划
            //    mABLoadPlanMgr.Add(ab_full_path,new AssetBundleLoadPlan(ab_full_path,req));
            //    //等待加载结束
            //    await req;
            //    //移除加载计划
            //    mABLoadPlanMgr.Remove(ab_full_path);
            //    loaded_ab = req.assetBundle;
            //}else if(encryType == EncryptionType.Offset)
            //{
            //    ulong offsetValue = 0;

            //    //求偏移量
            //    if(mConfig.Encry_OffsetHandleType == EncryOffsetType.Default)
            //    {
            //        offsetValue = EncryDefaultHander.GetOffsetValue(hashCode, ab_path);
            //    }


            //    //开始加载
            //    var req = AssetBundle.LoadFromFileAsync(ab_full_path, 0, offsetValue);
            //    //加入加载计划
            //    mABLoadPlanMgr.Add(ab_full_path, new AssetBundleLoadPlan(ab_full_path, req));
            //    //等待加载结束
            //    await req;
            //    //移除加载计划
            //    mABLoadPlanMgr.Remove(ab_full_path);
            //    loaded_ab = req.assetBundle;
            //}
            //else
            //{
            //    //TODO 其他加密方式，暂未实现
            //    await UniTask.DelayFrame(1);
            //    loaded_ab = null;
            //}


            ////登记
            //var loaded_info = new LoadedAssetBundle(loaded_ab, hashCode, ab_path, ab_full_path, dependences);
            //if (UseNum > 0)
            //{
            //    loaded_info.Cover_UseNum(UseNum);   //继承自旧AB的引用数量
            //}
            //mLoadedABMgr.Add(ab_path, loaded_info);
            ////登记引用数量
            //RegisterAssetBundleUseNum(ab_path);
            //return loaded_ab;

            #endregion

        }

        private async UniTask<AssetBundle> LoadSingleAssetBundleFromLocalAsync(string ab_path,string ab_full_path, Hash128 hashCode,string[] dependences, int UseNum = 0)
        {
            //正式加载

            //检查是否正在被加载
            if (mABLoadPlanMgr.ContainsPath(ab_full_path))
            {
                //加载计划中有，说明正在被加载
                var plan = mABLoadPlanMgr.Get(ab_full_path);
                await plan.LoadRequest;
                return plan.LoadRequest.assetBundle;    //结束，完毕，走人
            }

            var encryType = AssetParseHelper.ParseEncryTypeByAssetBundlePath(ab_path, mConfig);


            AssetBundle loaded_ab;
            //真要加载了啊

            //根据加密方式来加载
            if (encryType == EncryptionType.None)
            {
                var req = AssetBundle.LoadFromFileAsync(ab_full_path);  //很智障，不知道Unirx为什么不能直接await这里
                //加入加载计划
                mABLoadPlanMgr.Add(ab_full_path, new AssetBundleLoadPlan(ab_full_path, req));
                //等待加载结束
                await req;
                //移除加载计划
                mABLoadPlanMgr.Remove(ab_full_path);
                loaded_ab = req.assetBundle;
            }
            else if (encryType == EncryptionType.Offset)
            {
                ulong offsetValue = 0;

                //求偏移量
                if (mConfig.Encry_OffsetHandleType == EncryOffsetType.Default)
                {
                    offsetValue = EncryDefaultHander.GetOffsetValue(hashCode, ab_path);
                }


                //开始加载
                var req = AssetBundle.LoadFromFileAsync(ab_full_path, 0, offsetValue + (ulong)mEncrySign.Length);
                //加入加载计划
                mABLoadPlanMgr.Add(ab_full_path, new AssetBundleLoadPlan(ab_full_path, req));
                //等待加载结束
                await req;
                //移除加载计划
                mABLoadPlanMgr.Remove(ab_full_path);
                loaded_ab = req.assetBundle;
            }
            else
            {
                //TODO 其他加密方式，暂未实现
                await UniTask.DelayFrame(1);
                loaded_ab = null;
            }


            //登记
            var loaded_info = new LoadedAssetBundle(loaded_ab, hashCode, ab_path, ab_full_path, dependences);
            if (UseNum > 0)
            {
                loaded_info.Cover_UseNum(UseNum);   //继承自旧AB的引用数量
            }
            mLoadedABMgr.Add(ab_path, loaded_info);
            //登记引用数量
            RegisterAssetBundleUseNum(ab_path);
            return loaded_ab;
        }

        private async UniTask<AssetBundle> LoadSingleAssetBundleFromMemoryAsync(string ab_path, byte[] ab_source_data, Hash128 hashCode, string[] dependences, int UseNum = 0)
        {
            var encryType = AssetParseHelper.ParseEncryTypeByAssetBundlePath(ab_path, mConfig);

            //正式加载

            AssetBundle loaded_ab;
            //真要加载了啊

            //根据加密方式来加载
            if (encryType == EncryptionType.None)
            {

                var req = AssetBundle.LoadFromMemoryAsync(ab_source_data);  //很智障，不知道Unirx为什么不能直接await这里

                //等待加载结束
                await req;

                loaded_ab = req.assetBundle;
            }
            else if (encryType == EncryptionType.Offset)
            {
                ulong offsetValue = 0;

                //求偏移量
                if (mConfig.Encry_OffsetHandleType == EncryOffsetType.Default)
                {
                    offsetValue = EncryDefaultHander.GetOffsetValue(hashCode, ab_path);
                }
                //裁剪
                byte[] cur_data = new byte[ab_source_data.Length - (int)offsetValue - (int)mEncrySign.Length];
                ab_source_data.CopyTo(cur_data, (int)offsetValue + mEncrySign.Length);
                //开始加载
                var req = AssetBundle.LoadFromMemoryAsync(cur_data);
                //等待加载结束
                await req;
                loaded_ab = req.assetBundle;
            }
            else
            {
                //TODO 其他加密方式，暂未实现
                await UniTask.DelayFrame(1);
                loaded_ab = null;
            }


            //登记
            var loaded_info = new LoadedAssetBundle(loaded_ab, hashCode, ab_path, "", dependences);
            if (UseNum > 0)
            {
                loaded_info.Cover_UseNum(UseNum);   //继承自旧AB的引用数量
            }
            mLoadedABMgr.Add(ab_path, loaded_info);
            //登记引用数量
            RegisterAssetBundleUseNum(ab_path);
            return loaded_ab;
        }

        /// <summary>
        /// 从已经加载好的AB包中加载资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="ab"></param>
        /// <returns></returns>
        private async UniTask<T> LoadAssetFromLoadedAssetBundle<T>(string name,AssetBundle ab) where T:UnityEngine.Object
        {
            var req = ab.LoadAssetAsync<T>(name);
            await req;
            return req.asset as T;
        }

        private async UniTask<UnityEngine.Object> LoadAssetFromLoadedAssetBundle(string name , AssetBundle ab,System.Type assetType)
        {
            var req = ab.LoadAssetAsync(name, assetType);
            await req;
            return req.asset;
        }

        private bool IsAssetBundleFileExistInPersistenDataPath(string ab_path)
        {
            return File.Exists(Path.Combine(mVFilePersistentDataPath, ab_path));
        }

        /// <summary>
        /// 取AssetBundle依赖
        /// </summary>
        /// <param name="ab_path"></param>
        /// <param name="loadFromPerPath"></param>
        /// <returns></returns>
        private string[] GetABDependences(string ab_path)
        {
            return mMainAssetBundleManifest.GetAllDependencies(ab_path);
//            string[] result;
//            if (loadFromPerPath)
//                result = mPersistentDataAssetBundleManifest.GetAllDependencies(ab_path);
//            else
//            {
//#if !UNITY_EDITOR
//                if (IsLoadAssetBundleFromStreamingAssets())
//                    result = mStreamingPathAssetBundleManifest.GetAllDependencies(ab_path);
//                else
//                    result = mTinaXWorkFolderAssetBundleManifest.GetAllDependencies(ab_path);
//#else
//                    result = mStreamingPathAssetBundleManifest.GetAllDependencies(ab_path);
//#endif
//            }
//            return result;
        }

        /// <summary>
        /// 取AssetBundle的系统完整路径
        /// </summary>
        /// <param name="ab_path"></param>
        /// <param name="loadFromPerPath"></param>
        /// <returns></returns>
        private string GetABFullPath(string ab_path,bool loadFromPerPath)
        {
            string result;
            if (loadFromPerPath)
                result = mVFilePersistentDataPath + ab_path;
            else
            {
#if UNITY_EDITOR
                if (IsLoadAssetBundleFromStreamingAssets())
                    result = mVFileStreamingAssetsPath + ab_path;
                else
                    result = mVFileTinaXWorkFolder + ab_path;
#else
                result = mVFileStreamingAssetsPath + ab_path;
#endif
            }
            return result;
        }

        /// <summary>
        /// 取AssetBundle的HashCode
        /// </summary>
        /// <param name="ab_path"></param>
        /// <param name="loadFromPerPath"></param>
        /// <returns></returns>
        private UnityEngine.Hash128 GetABHashCode(string ab_path)
        {
            return mMainAssetBundleManifest.GetAssetBundleHash(ab_path);
            //            if (loadFromPerPath)
            //            {
            //                result = mPersistentDataAssetBundleManifest.GetAssetBundleHash(ab_path);

            //            }
            //            else
            //            {
            //#if UNITY_EDITOR
            //                if (IsLoadAssetBundleFromStreamingAssets())
            //                {
            //                    result = mStreamingPathAssetBundleManifest.GetAssetBundleHash(ab_path);
            //                }
            //                else
            //                {
            //                    result = mTinaXWorkFolderAssetBundleManifest.GetAssetBundleHash(ab_path);
            //                }
            //#else
            //                result = mStreamingPathAssetBundleManifest.GetAssetBundleHash(ab_path);
            //#endif

            //            }
            //            return result;
        }


        /// <summary>
        /// 登记引用数量
        /// </summary>
        /// <param name="abPath"></param>
        private void RegisterAssetBundleUseNum(string abPath)
        {
            if (mLoadedABMgr.ContainsPath(abPath))
            {
                mLoadedABMgr.GetDataByPath(abPath).Register_Use();
            }
        }


        /// <summary>
        /// 移除引用数量
        /// </summary>
        private void RemoveAssetBundleUseNum(string abPath)
        {
            if (mLoadedABMgr.ContainsPath(abPath))
            {
                mLoadedABMgr.GetDataByPath(abPath).Remove_Use();
            }
        }


        private bool IsFileExistsInSteamingAssets(string abPath)
        {
            if (mStreamingAsset_FileHash == null) return false;
            return mStreamingAsset_FileHash.IsFileExists(abPath);
        }

#endregion


#region Resources Load

        private T LoadAssetByResources<T>(string assetPath) where T: UnityEngine.Object
        {
            return Resources.Load<T>(assetPath);
        }

        private UnityEngine.Object LoadAssetByResources(string assetPath,System.Type type)
        {
            return Resources.Load(assetPath, type);
        } 

        /// <summary>
        /// 通过Resources 方式 异步加载资源 async/await
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        private async UniTask<T> LoadAssetByResourcesAsync<T>(string path) where T: UnityEngine.Object
        {
            var asset = await Resources.LoadAsync<T>(path);
            return (T)asset;
        }

        private async UniTask<UnityEngine.Object> LoadAssetByResourcesAsync(string path,Type type)
        {
            var asset = await Resources.LoadAsync(path,type);
            return asset;
        }

        private void LoadAssetByResourcesAsync<T>(string path,Action<T> callback) where T : UnityEngine.Object
        {
            Resources.LoadAsync<T>(path)
                .AsAsyncOperationObservable()
                .Select(req => req.asset)
                .OfType<UnityEngine.Object, T>()
                .Subscribe(asset =>
                {
                    callback?.Invoke(asset);
                });
        }

        private void LoadAssetByResourcesAsync(string path,System.Type type, Action<UnityEngine.Object> callback)
        {
            Resources.LoadAsync(path,type)
                .AsAsyncOperationObservable()
                .Select(req => req.asset)
                .Subscribe(asset =>
                {
                    callback?.Invoke(asset);
                });
        }

        #endregion

        #region Web Load

        private async UniTask<T> LoadAssetFromWebAsync<T>(string path, AssetParseInfo parse_info,bool useCache = true) where T : UnityEngine.Object
        {
            
            if (useCache)
            {
                //如果使用缓存，先判断是否已有缓存
                var local_path = Path.Combine(mVFilePersistentDataPath, parse_info.ABFileNameWithExtName);
                if (File.Exists(local_path))
                {
                    //直接加载这里
                    var ab = await LoadAssetBundleAndDependencesRecursionAsync(parse_info.ABFileNameWithExtName);
                    var asset_req = ab.LoadAssetAsync<T>(path);
                    await asset_req;
                    return asset_req.asset as T;
                }
                else
                {
                    //没有缓存，直接下载吧

                    var ab = await LoadAssetBundleAndDependencesRecursionFromWebAsync(parse_info.ABFileNameWithExtName);
                    var asset_req = ab.LoadAssetAsync<T>(path);
                    await asset_req;
                    return asset_req.asset as T;
                }

            }
            else
            {
                //直接下载并打开，不对存储进行读写，完全在内存中
                var ab = await LoadAssetBundleAndDependencesRecursionFromWebAsync(parse_info.ABFileNameWithExtName, false);
                var asset_req = ab.LoadAssetAsync<T>(path);
                await asset_req;
                return asset_req.asset as T;
            }
            
        }

        private async UniTask<UnityEngine.Object> LoadAssetFromWebAsync(string path, AssetParseInfo parse_info, System.Type type, bool useCache = true) 
        {

            if (useCache)
            {
                //如果使用缓存，先判断是否已有缓存
                var local_path = Path.Combine(mVFilePersistentDataPath, parse_info.ABFileNameWithExtName);
                if (File.Exists(local_path))
                {
                    //直接加载这里
                    var ab = await LoadAssetBundleAndDependencesRecursionAsync(parse_info.ABFileNameWithExtName);
                    var asset_req = ab.LoadAssetAsync(path,type);
                    await asset_req;
                    return asset_req.asset;
                }
                else
                {
                    //没有缓存，直接下载吧

                    var ab = await LoadAssetBundleAndDependencesRecursionFromWebAsync(parse_info.ABFileNameWithExtName);
                    var asset_req = ab.LoadAssetAsync(path, type);
                    await asset_req;
                    return asset_req.asset;
                }

            }
            else
            {
                //直接下载并打开，不对存储进行读写，完全在内存中
                var ab = await LoadAssetBundleAndDependencesRecursionFromWebAsync(parse_info.ABFileNameWithExtName, true);
                var asset_req = ab.LoadAssetAsync(path,type);
                await asset_req;
                return asset_req.asset;
            }

        }


        private async UniTask<AssetBundle> LoadAssetBundleAndDependencesRecursionFromWebAsync(string ab_path, bool useCache = true)
        {
            var manifest = await InitMainAssetBundleManifestByWebVFS();
            string[] dependences = manifest.GetAllDependencies(ab_path);
            if(dependences.Length > 0)
            {
                foreach(var item in dependences)
                {
                    if(await LoadAssetBundleAndDependencesRecursionFromWebAsync(item, useCache) != null)
                    {
                        RegisterAssetBundleUseNum(item);
                    }
                }
            }

            //加载本体ab
            var hashCode = await GetWebVFSAssetBundleHashCodeAsync(ab_path);
            int UseNum = 0;
            //是否已经加载了
            if (mLoadedABMgr.ContainsPath(ab_path))
            {
                var myLoadedData = mLoadedABMgr.GetDataByPath(ab_path);
                if (myLoadedData.AssetBundleHashCode.ToString() == hashCode.ToString())
                {
                    //好的，这个就是我们要的资源
                    return myLoadedData.mAssetBundle;
                }
                else
                {
                    //这个不是我们要的资源，准备否定三连
                    myLoadedData.mAssetBundle.Unload(false);
                    mLoadedABMgr.RemoveByKey(ab_path);

                    UseNum = myLoadedData.UseNum;
                }
            }

            //真的要下载了，先下载
            var ab_uri = new Uri(mCurWebVFSConfig.Url + ab_path);
            string ab_full_path;
            if (useCache)
            {
                ab_full_path = Path.Combine(mVFileWebVFSCachePath, ab_path);
                //将文件下载到本地
                await DownloadFileToLocal(ab_uri, ab_full_path);

                //加载
                //是否正在被加载
                if (mABLoadPlanMgr.ContainsPath(ab_full_path))
                {
                    //加载计划中有，说明正在被加载
                    var plan = mABLoadPlanMgr.Get(ab_full_path);
                    await plan.LoadRequest;
                    return plan.LoadRequest.assetBundle;    //结束，完毕，走人
                }

                return await LoadSingleAssetBundleFromLocalAsync(ab_path, ab_full_path, hashCode, dependences, UseNum);
            }
            else
            {
                //下载字节，
                var data = await DownloadFileFromWebOnce(ab_uri);

                return await LoadSingleAssetBundleFromMemoryAsync(ab_path, data, hashCode, dependences, UseNum);
            }
            
            
            
        }


        private async UniTask<AssetBundleManifest> InitMainAssetBundleManifestByWebVFS(int timeout = 5) 
        {

            if (mMainAssetBundleManifest != null)
            {
                return mMainAssetBundleManifest;
            }
            else
            {
                var download_flag = false;  //如果需要下载的话， 则置为true
                //检查本地有没有有效的文件
                var local_path = Path.Combine(mVFileWebVFSCachePath, Const.PlatformConst.GetPlatformName(Application.platform).ToLower());
                var vfs_path = Const.PlatformConst.GetPlatformName(Application.platform).ToLower();
                var remote_md5 = await GetWebVFS_FileMD5(vfs_path);

                if (File.Exists(local_path))
                {
                    //本地有
                    if (mCurWebVFSConfig.VerifyFileHash)
                    {
                        //验证文件MD5
                        var local_md5 = XFile.GetMD5(local_path);
                        if (local_md5 != remote_md5 && !remote_md5.IsNullOrEmpty())
                        {
                            //需要下载
                            download_flag = true;
                            File.Delete(local_path);
                        }
                    }
                }
                else
                    download_flag = true;   //本地没有，直接下载

                if (download_flag)
                {
                    //需要下载咯
                    var uri = new Uri(mCurWebVFSConfig.Url + Const.PlatformConst.GetPlatformName(Application.platform).ToLower());
                    await DownloadFileToLocal(uri, local_path,timeout);

                    var isValid = false;
                    //检查下载结果
                    if (File.Exists(local_path))
                    {
                        var local_md5 = XFile.GetMD5(local_path);
                        if (!remote_md5.IsNullOrEmpty())
                        {
                            if (local_md5 == remote_md5)
                                isValid = true;
                            else
                                throw new Exceptions.VFSException("download webvfs AssetBundleManifest failed : hash error", Exceptions.VFSException.VFSErrorType.HashMismatch);

                        }
                        else
                            isValid = true;

                    }

                    if (!isValid)
                    {
                        //下载失败
                        throw new Exceptions.VFSException("download failed: ", Exceptions.VFSException.VFSErrorType.NetworkError);
                    }

                }


                //从本地读
                //加载计划
                AssetBundle ab = AssetBundle.LoadFromFile(local_path);  //反正这里是要堵塞主线程的，就同步加载好了
                if (ab!= null)
                {
                    if (mMainAssetBundleManifest == null)
                    {
                        lock (this)
                        {
                            mMainAssetBundleManifest = ab.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
                            if(mMainAssetBundleManifest == null)
                            {
                                return null;
                            }
                            else
                            {
                                mMainAssetBundleManifestPath = local_path;
                                mMainAssetBundleManifestType = MainABManifestType.WebVFS;
                                return mMainAssetBundleManifest;
                            }
                        }
                    }
                    else
                    {
                        return mMainAssetBundleManifest;
                    }
                }
                else
                {
                    return null;
                }
                

                //if (mABLoadPlanMgr.ContainsPath(local_path))
                //{
                //    var plan = mABLoadPlanMgr.Get(local_path);
                //    await plan.LoadRequest;
                //    ab = plan.LoadRequest.assetBundle;
                //}
                //else
                //{
                //    var ab_req = AssetBundle.LoadFromFileAsync(local_path);
                //    //加入计划
                //    mABLoadPlanMgr.Add(local_path, new AssetBundleLoadPlan(local_path, ab_req));
                //    await ab_req;
                //    mABLoadPlanMgr.Remove(local_path);
                //    ab = ab_req.assetBundle;
                //}

                //var asset_req = ab.LoadAssetAsync<AssetBundleManifest>("AssetBundleManifest");
                //await asset_req;
                //mWebVFSAssetBundleManifest = asset_req.asset as AssetBundleManifest;

                //return mWebVFSAssetBundleManifest;

            }
        }

        private async UniTask<Hash128> GetWebVFSAssetBundleHashCodeAsync(string ab_path)
        {
            return (await InitMainAssetBundleManifestByWebVFS()).GetAssetBundleHash(ab_path);
        }

        private async UniTask<string> GetWebVFS_FileMD5(string vfsPath)
        {
            if(mCurWebVFSConfig.WebLoadVerifyType == WebLoadVerifyHashType.GetFileMD5ListFromServer)
            {
                var fileList = await GetWebVFS_FileHashList();
                
                return fileList?.GetMD5ByPath(vfsPath);
            }else if(mCurWebVFSConfig.WebLoadVerifyType == WebLoadVerifyHashType.GetMD5FromServer)
            {
                var uri = new Uri(mCurWebVFSConfig.GetMD5Url + vfsPath);
                var md5 = await XWeb.GetText(uri);
                return md5;
            }


            return default;
        }

        private async UniTask<byte[]> DownloadFileFromWebOnce(System.Uri uri,int timeout = 5)
        {
            //是否正在下载
            if (mDownloadPlanMgr.ContainsUri(uri))
            {
                //等这边下载好了返回
                var req = await mDownloadPlanMgr.Get(uri).AsyncOperation;
                if (req.isHttpError)
                {
                    //抛异常
                    throw new Exceptions.VFSException("Download fail, Http Error, uri:" + uri.ToString(), Exceptions.VFSException.VFSErrorType.HttpError, (System.Net.HttpStatusCode)req.responseCode);
                }else if (req.isNetworkError)
                {
                    //抛异常
                    throw new Exceptions.VFSException("Download fail, Http Error, uri:" + uri.ToString(), Exceptions.VFSException.VFSErrorType.NetworkError, (System.Net.HttpStatusCode)req.responseCode);
                }

                var rdata = req.downloadHandler.data;
                req.Dispose();
                return rdata;
            }
            else
            {
                //没有正在下载，那我们下载一个吧
                var req = UnityWebRequest.Get(uri);
                req.timeout = timeout;
                var op = req.SendWebRequest();
                mDownloadPlanMgr.Add(uri, new FileDownloadPlan(uri, op));
                var myReq = await op;
                //移除计划
                mDownloadPlanMgr.Remove(uri);

                if (myReq.isHttpError)
                {
                    Debug.LogError("异常：Download fail, Http Error, uri:" + uri.ToString());
                    //抛异常
                    throw new Exceptions.VFSException("Download fail, Http Error, uri:" + uri.ToString(), Exceptions.VFSException.VFSErrorType.HttpError, (System.Net.HttpStatusCode)req.responseCode);
                }
                else if (myReq.isNetworkError)
                {
                    Debug.LogError("异常：Download fail, Http Error, uri:" + uri.ToString());

                    //抛异常
                    throw new Exceptions.VFSException("Download fail, Http Error, uri:" + uri.ToString(), Exceptions.VFSException.VFSErrorType.NetworkError, (System.Net.HttpStatusCode)req.responseCode);
                }
                var rdata = myReq.downloadHandler.data;
                myReq.Dispose();
                return rdata;
            }
        }

        private async UniTask<string> GetTextFromWebOnce(System.Uri uri,int timeout = 5)
        {
            //是否正在下载
            if (mDownloadPlanMgr.ContainsUri(uri))
            {
                //等这边下载好了返回
                var req = await mDownloadPlanMgr.Get(uri).AsyncOperation;
                if (req.isHttpError)
                {
                    //抛异常
                    throw new Exceptions.VFSException("Download fail, Http Error, uri:" + uri.ToString(), Exceptions.VFSException.VFSErrorType.HttpError, (System.Net.HttpStatusCode)req.responseCode);
                }
                else if (req.isNetworkError)
                {
                    //抛异常
                    throw new Exceptions.VFSException("Download fail, uri:" + uri.ToString(), Exceptions.VFSException.VFSErrorType.NetworkError, (System.Net.HttpStatusCode)req.responseCode);
                }
                var rText = req.downloadHandler.text;
                req.Dispose();
                return rText;
            }
            else
            {

                //没有正在下载，那我们下载一个吧
                var req = UnityWebRequest.Get(uri);
                req.timeout = timeout;
                var op = req.SendWebRequest();
                mDownloadPlanMgr.Add(uri, new FileDownloadPlan(uri, op));
                var myReq = await op;
                //移除计划
                mDownloadPlanMgr.Remove(uri);

                if (myReq.isHttpError)
                {
                    //抛异常
                    throw new Exceptions.VFSException("Download fail, Http Error, uri:" + uri.ToString(), Exceptions.VFSException.VFSErrorType.HttpError, (System.Net.HttpStatusCode)req.responseCode);
                }
                else if (myReq.isNetworkError)
                {
                    //抛异常
                    throw new Exceptions.VFSException("Download fail, uri:" + uri.ToString(), Exceptions.VFSException.VFSErrorType.NetworkError, (System.Net.HttpStatusCode)req.responseCode);
                }


                var rtxt = myReq.downloadHandler.text;
                myReq.Dispose();
                return rtxt;
            }
        }


        private async UniTask DownloadFileToLocal(System.Uri uri,string local_path,int timeout = 5)
        {
            if (!File.Exists(local_path))
            {
                try
                {
                    var data = await DownloadFileFromWebOnce(uri, timeout);
                    if (!File.Exists(local_path))
                    {
                        var dir = Path.GetDirectoryName(local_path);
                        XDirectory.CreateIfNotExists(dir);
                        File.WriteAllBytes(local_path, data);
                    }
                }
                catch (Exceptions.VFSException ve)
                {
                    Debug.Log("emm");
                    throw ve;
                }
                catch (Exception e)
                {
                    //Dev Temp
                    Debug.LogError("Download file to local error:" + e.Message);
                }
                
            }
            
        }

        


        

        private async Task<VFSFileHashModel> GetWebVFS_FileHashList()
        {
            if(mWebVFS_FileHash != null)
            {
                return mWebVFS_FileHash;
            }
            else
            {
                //从web获取
                var uri = new Uri(mCurWebVFSConfig.GetMD5ListUrl);
                try
                {
                    var json_str = await GetTextFromWebOnce(uri, 3);
                    mWebVFS_FileHash = JsonUtility.FromJson<VFSFileHashModel>(json_str);
                    mWebVFS_FileHash?.Init();
                    return mWebVFS_FileHash;
                }
                catch(Exceptions.VFSException e)
                {
                    throw e;
                }
                catch(Exception e)
                {
                    throw e;
                }

                
            }
        }

        /// <summary>
        /// 初始化WebVFS
        /// </summary>
        /// <returns></returns>
        private async Task InitWebVFSAsync() 
        {
            //MD5 List
            if (mCurWebVFSConfig.VerifyFileHash)
            {
                if(mCurWebVFSConfig.WebLoadVerifyType == WebLoadVerifyHashType.GetFileMD5ListFromServer)
                {
                    //获取MD5列表
                    try
                    {
                        await GetWebVFS_FileHashList();
                    }
                    catch (VFSException ve)
                    {
                        mEnableWebVFS = false;
                        throw ve;
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                    
                }
            }


            //AssetBundleManifest
            try
            {
                await InitMainAssetBundleManifestByWebVFS(3);
            }
            catch (VFSException e)
            {
                mEnableWebVFS = false;
                throw e;
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        private async UniTask WaitIfWebVFSIniting()
        {
            if (mWebVFSInited) return;
            if (mWebVFSIniting)
            {
                await mWebVFSInitTask;
            }
        }

        //private async Task DoInitAsync()
        //{
        //    if (mWebVFSInited) return;
        //    if (mWebVFSIniting)
        //    {
        //        await mWebVFSInitTask;
        //    }
        //    else
        //    {
        //        mWebVFSIniting = true;
        //        try
        //        {
        //            mWebVFSInitTask = InitWebVFSAsync();
        //            await mWebVFSInitTask;
        //        }
        //        catch (VFSException e)
        //        {
        //            //初始化时候的异常,捕获，然后
        //            Exceptions.VFSException.CallInitException(e);
        //            mWebVFSInited = false;
        //            mWebVFSIniting = false;
        //            mEnableWebVFS = false;
        //            return;
        //        }
        //        mWebVFSIniting = false;
        //    }
        //    mWebVFSInited = true;
        //}

        

        private async UniTask<UnityEngine.Object> LoadWebAssetAsyncWithType(string assetPath, System.Type type, bool useCache = true)
        {
            if (!mEnableWebVFS)
            {
                //抛异常
                throw new Exceptions.VFSException($"web vfs not enable, load web asset failed: {assetPath}", Exceptions.VFSException.VFSErrorType.WebVFS_NotEnable);
            }
            await WaitIfWebVFSIniting();

            //一样，先解析
            var parse_info = AssetParseHelper.Parse(assetPath, mConfig);
            if (parse_info.IsValid)
            {
                //有效
                try
                {
                    return await LoadAssetFromWebAsync(assetPath, parse_info, type, useCache);
                }
                catch (VFSException e)
                {
                    e.SetParseInfo(parse_info);
                    throw e;
                }
                
            }
            else
            {
                //抛异常
                string desc = "";
                if (!parse_info.InWhiteList)
                {
                    desc = " Asset path not in VFS whitelist. ";
                }
                if (parse_info.Ignore)
                {
                    desc = " Asset path is ignored. ";
                }
                //抛异常
                throw new TinaX.Exceptions.VFSException($"File Load Fail: {assetPath}  --> {desc}", parse_info, Exceptions.VFSException.VFSErrorType.PathNotValid);
            }
        }

        #endregion


        private async Task InitStreamingAssetsFileHash() //异步方法，堵在主线程同步执行
        {
            var streamingAssets_fileHash_path = Path.Combine(mVFileStreamingAssetsPath, VFSPathConst.VFS_File_AssetBundleHash_FileName);
            var uri = new System.Uri(streamingAssets_fileHash_path);
            var req = UnityEngine.Networking.UnityWebRequest.Get(uri);
            req.timeout = 3;
            var result = await req.SendWebRequest();
            if (result.error.IsNullOrEmpty() && !result.downloadHandler.text.IsNullOrEmpty())
            {
                var str_json = result.downloadHandler.text;
                mStreamingAsset_FileHash = JsonUtility.FromJson<VFSFileHashModel>(str_json);
            }

        }

        /// <summary>
        /// Main AB Manifest 来源
        /// </summary>
        private enum MainABManifestType
        {
            None = 0,
            StreamingAssets = 1,
            Persistentdatapath = 2,
            WebVFS = 3
        }


#if UNITY_EDITOR
        /// <summary>
        /// 是否在编辑器下使用AssetBundle加载
        /// </summary>
        /// <returns></returns>
        public static bool IsUseAssetBundleInEdtor()
        {
            if(Menu.GetChecked(VFSMenuConst.MenuStr_LoadAssetBundleFromStramingAssets) || Menu.GetChecked(VFSMenuConst.MenuStr_LoadAssetBundleFromTinaXWorkFolder))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool IsUseAssetDatabaseInEditor()
        {
            if (!Menu.GetChecked(VFSMenuConst.MenuStr_LoadAssetBundleFromStramingAssets)
                && !Menu.GetChecked(VFSMenuConst.MenuStr_LoadAssetBundleFromTinaXWorkFolder)
                && !Menu.GetChecked(VFSMenuConst.MenuStr_LoadAssetFromResources)
                && !Menu.GetChecked(VFSMenuConst.MenuStr_LoadAssetByAssetDatabase)
                )
            {
                Menu.SetChecked(VFSMenuConst.MenuStr_LoadAssetByAssetDatabase, true);
            }
            return Menu.GetChecked(VFSMenuConst.MenuStr_LoadAssetByAssetDatabase);
        }

        public static bool IsUseResourcesInEditor()
        {
            return Menu.GetChecked(VFSMenuConst.MenuStr_LoadAssetFromResources);
        }

        public static bool IsLoadAssetBundleFromStreamingAssets()
        {
            return Menu.GetChecked(VFSMenuConst.MenuStr_LoadAssetBundleFromStramingAssets);
        }

        private void InitAssetBundleDataForTinaXWorkFolder()    //用来直接从TinaX Work Folder中加载AssetBundle的初始化项目
        {
            var platform_name = TinaX.Const.PlatformConst.GetPlatformName(Application.platform).ToLower();

            //fileHash
            var fileHashPath = Path.GetFullPath(Path.Combine(TinaX.Setup.Framework_AssetSystem_Pack_Path, platform_name, VFSPathConst.VFS_File_AssetBundleHash_FileName));
            if (File.Exists(fileHashPath))
            {
                string json = File.ReadAllText(fileHashPath);
                mStreamingAsset_FileHash = JsonUtility.FromJson<VFSFileHashModel>(json);
            }

            
            if(mMainAssetBundleManifest == null || mMainAssetBundleManifestType == MainABManifestType.None)
            {
                //当经历过可能的WebVFS初始化和PersistenData的初始化之后，Manifest还是空的。
                //わたし就要挺身而出了

                var bundle_path = Path.GetFullPath(Path.Combine(TinaX.Setup.Framework_AssetSystem_Pack_Path, platform_name, platform_name));
                var manifest_bundle = AssetBundle.LoadFromFile(bundle_path);
                if (manifest_bundle == null)
                {
                    Debug.LogError("VFS 初始化失败：无法从TinaXWorkFolder加载资源。请尝试切换VFS在编辑器下的工作模式");
                }
                mVFileTinaXWorkFolder = Path.GetFullPath(Path.Combine(TinaX.Setup.Framework_AssetSystem_Pack_Path, platform_name)) + "/";
                mMainAssetBundleManifest = manifest_bundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
                mMainAssetBundleManifestPath = bundle_path;
                mMainAssetBundleManifestType = MainABManifestType.StreamingAssets;
            }




            

            
        }

#endif

    }
}

