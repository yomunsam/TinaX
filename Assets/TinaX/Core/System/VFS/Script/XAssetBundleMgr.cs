using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx.Async;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TinaX.VFS
{
    /*
     * 【说明】
     * 资源优先级：
     * PersistentDataPath目录下的资源是热更新时替换进来的资源，所以PersistentDataPath目录的优先级是高于StreamingAssetsPath的
     * 热更新：
     * 如果管理器在加载某个资源时，发现该资源在PersistentDataPath中存在，而StreamingAssetsPath中的对应资源已被加载，
     * 则StreamingAssetsPath中的对应资源会被即时释放掉
     * 
     * 资源加载流程
     * 1. 解析资源可否被加载
     * 2. 加载资源依赖AB
     * 3. 加载资源本体AB
     * 4. 从AB中加载资源
     * 
     * 【重构的管理器中，步骤2和3采用同一个方法递归处理】
     */

    /// <summary>
    /// 资源管理器 - 新模块，替代原来的XAssetsManager和XAssetMgrAsync
    /// </summary>
    public class XAssetBundleMgr : MonoBehaviour
    {

        /// <summary>
        /// 文件后缀名
        /// </summary>
        private string mAssetBundleFileExt
        {
            get
            {
                return TinaX.Setup.Framework_VFS_AssetBundle_Ext_Name;
            }
        }


        /// <summary>
        /// 虚拟文件系统根目录-PersistentData
        /// </summary>
        private string mPersistentDataPath;
        /// <summary>
        /// 虚拟文件系统根目录-Streaming
        /// </summary>
        private string mStreamingAssetsPath;
        /// <summary>
        /// AB资源清单 - Streaming
        /// </summary>
        private AssetBundleManifest mABManifest_Stream;
        /// <summary>
        /// AB资源清单 - Per
        /// </summary>
        private AssetBundleManifest mABManifest_Per;

        /// <summary>
        /// 已加载AB包缓存池,key为ab包的Unity相对路径
        /// </summary>
        private Dictionary<string, LoadedAssetBundle> mLoadedAssetBundlePool = new Dictionary<string, LoadedAssetBundle>();


        /// <summary>
        /// 资源加载计划（仅在资源加载过程中存在）,key是绝对路径
        /// </summary>
        private Dictionary<string, LoadPlane> mLoadPlane = new Dictionary<string, LoadPlane>();

        private WaitForEndOfFrame mWaitForEndOfFrame = new WaitForEndOfFrame();

        private XAssetsManager mBaseMgr;

        private VFSConfigCache mConfigCache
        {
            get
            {
                return mBaseMgr?.mVFSConfigCache;
            }
        }

        private void Awake()
        {
            //定义路径名
            mPersistentDataPath = XCore.I.LocalStorage_TinaX + "/vfs/" + Const.PlatformConst.GetPlatformName(Application.platform).ToLower() + "/";
            mStreamingAssetsPath = Application.streamingAssetsPath + "/vfs/" + Const.PlatformConst.GetPlatformName(Application.platform).ToLower() + "/";

            void init_ab_load()
            {
                var bundle_S = AssetBundle.LoadFromFile(mStreamingAssetsPath + Const.PlatformConst.GetPlatformName(Application.platform).ToLower());
                mABManifest_Stream = bundle_S.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
                bundle_S.Unload(false);
                bundle_S = null;
                var bundle_P_Path = mPersistentDataPath + Const.PlatformConst.GetPlatformName(Application.platform).ToLower();
                if (File.Exists(bundle_P_Path))
                {
                    var bundle_P = AssetBundle.LoadFromFile(bundle_P_Path);
                    mABManifest_Per = bundle_P.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
                    bundle_P.Unload(false);
                    bundle_P = null;
                }
            }

#if UNITY_EDITOR
            //编辑器模式下，需要判断，是从哪儿加载资源
            if (Menu.GetChecked(Const.AssetSystemConst.menu_editor_load_from_asset_pack_name))
            {
                //从资源系统加载
                //AB资源清单
                init_ab_load();
            }
            else
            {
                //直接使用编辑器加载策略


            }
#else
            //AB资源清单
            init_ab_load();
#endif


        }

        public XAssetBundleMgr()
        {
            
        }


        public string GetVFSPersistentDataPath()
        {
            return mPersistentDataPath; //这个目录是用来存放补丁更新文件的
        }


        public XAssetBundleMgr Init(XAssetsManager baseMgr)
        {
            mBaseMgr = baseMgr;
            return this;
        }

        /// <summary>
        /// 加载资源
        /// </summary>
        /// <typeparam name="T">加载资源类型</typeparam>
        /// <param name="asset_path">虚拟文件系统的可寻址资源路径</param>
        /// <returns>加载资源结果</returns>
        public T LoadAsset<T>(string asset_path) where T : UnityEngine.Object
        {
            //检查文件是否在VFS管理范围内
            S_FileInfo path_info;
            if (!AssetPathTest(asset_path,out path_info))
            {
                //不在管理范围内
                return null;
            }
            path_info.ab_name += mAssetBundleFileExt;   //加上后缀名

            //加载它所在的AssetBundle
            var ab = Load_AssetBundle_And_Dependences_Recursion(path_info.ab_name);
            if(ab == null)
            {
                return null;
            }
            //注册引用数
            Register_AB_Use(path_info.ab_name);
            return ab.LoadAsset<T>(path_info.file_name_in_ab);

        }

        public UnityEngine.Object LoadAsset(string asset_path, Type type)
        {
            //检查文件是否在VFS管理范围内
            S_FileInfo path_info;
            if (!AssetPathTest(asset_path, out path_info))
            {
                //不在管理范围内
                return null;
            }
            path_info.ab_name += mAssetBundleFileExt;   //加上后缀名

            //加载它所在的AssetBundle
            var ab = Load_AssetBundle_And_Dependences_Recursion(path_info.ab_name);
            if (ab == null)
            {
                return null;
            }
            //注册引用数
            Register_AB_Use(path_info.ab_name);
            return ab.LoadAsset(path_info.file_name_in_ab,type);
        }


        public void LoadAsset_Async<T>(string asset_path,Action<T> callback) where T : UnityEngine.Object
        {
            //检查文件是否在VFS管理范围内
            S_FileInfo path_info;
            if (!AssetPathTest(asset_path, out path_info))
            {
                //不在管理范围内
                callback?.Invoke(null);
            }
            path_info.ab_name += mAssetBundleFileExt;   //加上后缀名

            //加载它所在的AssetBundle
            StartCoroutine(Load_AssetBundle_And_Dependences_Recursion_Async(path_info.ab_name, (ab) =>
            {
                if (ab == null)
                {
                    callback?.Invoke(null);
                }

                //注册引用数
                Register_AB_Use(path_info.ab_name);
                //加载资源本体
                StartCoroutine(Load_Asset_From_AssetBundle<T>(ab, path_info.file_name_in_ab, callback));

            }));
            
        }

        public void LoadAsset_Async(string asset_path,Type type, Action<UnityEngine.Object> callback)
        {
            //Debug.Log("异步加载请求, 路径：" + asset_path + " \n类型：" +type.ToString());
            //检查文件是否在VFS管理范围内
            S_FileInfo path_info;
            if (!AssetPathTest(asset_path, out path_info))
            {
                //不在管理范围内
                callback?.Invoke(null);
            }
            path_info.ab_name += mAssetBundleFileExt;   //加上后缀名
            //Debug.Log("    加入加载协程");
            //加载它所在的AssetBundle
            StartCoroutine(Load_AssetBundle_And_Dependences_Recursion_Async(path_info.ab_name, (ab) =>
            {
                if (ab == null)
                {
                    callback?.Invoke(null);
                }

                //注册引用数
                Register_AB_Use(path_info.ab_name);
                //加载资源本体
                StartCoroutine(Load_Asset_From_AssetBundle(ab, path_info.file_name_in_ab,type, callback));

            }));

        }

        /// <summary>
        /// 取消资源引用
        /// </summary>
        /// <param name="path"></param>
        public void UnRegister_AB_Use(string path)
        {
            /*
             * 1. 自身的引用数要-1
             * 2. 所有的的引用数量要-1
             * 
             */
            pUnRegister_AB_Use(path + mAssetBundleFileExt);

        }


        public void GC()
        {
            List<string> Keys = new List<string>();
            foreach(var item in mLoadedAssetBundlePool)
            {
                if (item.Value.GetUsedNum() <= 0)
                {
                    //干掉！盘它！
                    item.Value.ab_asset.Unload(true);
                    Keys.Add(item.Key);
                }
            }

            foreach(var item in Keys)
            {
                if (mLoadedAssetBundlePool.ContainsKey(item))
                {
                    mLoadedAssetBundlePool.Remove(item);
                }
            }
            Keys = null;
        }

        public Dictionary<string, LoadedAssetBundle> GetLoadedAssetBundleInfo()
        {
            return mLoadedAssetBundlePool;
        }

        /// <summary>
        /// 检测文件路径是否在VFS管理范围内
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <param name="fileInfo">解析出的文件信息</param>
        /// <returns>如果在管理范围内，返回true</returns>
        private bool AssetPathTest(string path, out S_FileInfo fileInfo)
        {
            fileInfo = AssetParse.Parse(path, mConfigCache);
            if(fileInfo.handle_tag == E_FileHandleTag.invalid)
            {
#if UNITY_EDITOR
                //编辑器报提示
                EditorUtility.DisplayDialog("资源系统 - 错误", "无法加载资源：" + path + "\n该路径资源在TinaX资源系统的管理范围之外，请尝试调整资源管理策略配置。", "好");
#endif  
                return false;
            }
            return true;
        }

        /// <summary>
        /// 递归加载AB包及其所有依赖
        /// </summary>
        /// <param name="ab_path_pure">ab包相对路径</param>
        /// <returns>返回路径对应的AB包</returns>
        private AssetBundle Load_AssetBundle_And_Dependences_Recursion(string ab_path)
        {
            //检查当前AB包的加载位置
            bool loadFormPerPath = IsLoadABFormPerPath(ab_path);
            
            string[] dependences; //依赖ab包
            if (loadFormPerPath && mABManifest_Per != null)
            {
                dependences = mABManifest_Per.GetAllDependencies(ab_path);
            }
            else
            {
                dependences = mABManifest_Stream.GetAllDependencies(ab_path);
            }


            //加载依赖包
            if(dependences.Length >= 0)
            {
                foreach(var item in dependences)
                {
                    if(Load_AssetBundle_And_Dependences_Recursion(item) != null)
                    {
                        Register_AB_Use(item);
                    }
                }
            }
            //加载AB本体
            //求当前ab的实际加载路径（绝对路径）
            string ab_full_path;
            if (loadFormPerPath)
            {
                ab_full_path = mPersistentDataPath + ab_path;
            }
            else
            {
                ab_full_path = mStreamingAssetsPath + ab_path;
            }


            //检查资源是否已加载
            if (mLoadedAssetBundlePool.ContainsKey(ab_path))
            {
                //确认资源来源位置
                if(mLoadedAssetBundlePool[ab_path].IsFromStreamingAssetsPath && loadFormPerPath)
                {
                    //当前要加载的文件优先级高于已加载的，要把已加载的释放掉
                    mLoadedAssetBundlePool[ab_path].ab_asset.Unload(false);
                    mLoadedAssetBundlePool.Remove(ab_path);
                }
                else
                {
                    //已经完美的加载出来了，不用管了
                    return mLoadedAssetBundlePool[ab_path].ab_asset;
                }
            }

#if UNITY_IOS || UNITY_STANDALONE
            //检查文件是否存在
            if (!File.Exists(ab_full_path))
            {
                XLog.PrintE("[TinaX][虚拟文件系统]加载资源错误，寻址路径：" + ab_path + "  ,实际检测路径：" + ab_full_path);
                return null;
            }
#endif


            //如果在上面没有return 掉，就正式加载
            var ab = AssetBundle.LoadFromFile(ab_full_path);
            //登记
            var loaded_ab_data = new LoadedAssetBundle();
            loaded_ab_data.Init(ab, !loadFormPerPath, ab_path, ab_full_path, dependences);
            mLoadedAssetBundlePool.Add(ab_path, loaded_ab_data);

            return ab;
        }

        IEnumerator Load_AssetBundle_Single(string ab_path, Action<AssetBundle> callback,bool loadFormPerPath,string[] dependences)
        {
            //Debug.Log("协程 收到 单独AB包加载子请求：" + ab_path);
            //加载AB本体
            //求当前ab的实际加载路径（绝对路径）
            string ab_full_path;
            if (loadFormPerPath)
            {
                ab_full_path = mPersistentDataPath + ab_path;
            }
            else
            {
                ab_full_path = mStreamingAssetsPath + ab_path;
            }


            //检查资源是否已加载
            if (mLoadedAssetBundlePool.ContainsKey(ab_path))
            {
                //确认资源来源位置
                if (mLoadedAssetBundlePool[ab_path].IsFromStreamingAssetsPath && loadFormPerPath)
                {
                    //当前要加载的文件优先级高于已加载的，要把已加载的释放掉
                    mLoadedAssetBundlePool[ab_path].ab_asset.Unload(false);
                    mLoadedAssetBundlePool.Remove(ab_path);
                }
                else
                {
                    //已经完美的加载出来了，不用管了
                    if (callback != null)
                    {
                        callback(mLoadedAssetBundlePool[ab_path].ab_asset);
                        yield break;
                    }
                }
            }

            //是否已在加载计划中
            if (mLoadPlane.ContainsKey(ab_full_path))
            {
                //已经有个协程在加载它了，把自己的任务登记进计划，然后不管了
                mLoadPlane[ab_full_path].AddCallback(callback);
                yield break;
            }
            else
            {
                //登记自己的加载计划
                mLoadPlane.Add(ab_full_path, new LoadPlane(ab_path, callback));
            }

#if UNITY_IOS || UNITY_STANDALONE
            //检查文件是否存在
            if (!File.Exists(ab_full_path))
            {
                XLog.PrintE("[TinaX][虚拟文件系统]加载资源错误，寻址路径：" + ab_path + "  ,实际检测路径：" + ab_full_path);
                //加载失败决定
                mLoadPlane[ab_full_path].Fire(null);
                mLoadPlane.Remove(ab_full_path);
                yield break;
            }
#endif



            //Debug.Log("    正式加载:" + ab_full_path);
            //如果在上面没有return 掉，就正式加载
            var ab_req = AssetBundle.LoadFromFileAsync(ab_full_path);
            yield return ab_req;

            if (ab_req.assetBundle == null)
            {
                //加载失败
                mLoadPlane[ab_full_path].Fire(null);
                mLoadPlane.Remove(ab_full_path);
                yield break;
            }

            //登记
            var loaded_ab_data = new LoadedAssetBundle();
            loaded_ab_data.Init(ab_req.assetBundle, !loadFormPerPath, ab_path, ab_full_path, dependences);
            mLoadedAssetBundlePool.Add(ab_path, loaded_ab_data);

            //加载计划处理
            mLoadPlane[ab_full_path].Fire(ab_req.assetBundle);
            mLoadPlane.Remove(ab_full_path);
            yield break;
        }

        /// <summary>
        /// [异步]递归加载AB包及其所有依赖
        /// </summary>
        /// <param name="ab_path">ab包Unity相对路径</param>
        /// <param name="callback">加载完成后的回调</param>
        /// <returns></returns>
        IEnumerator Load_AssetBundle_And_Dependences_Recursion_Async(string ab_path,Action<AssetBundle> callback)
        {
            //Debug.Log("协程 收到 AB包加载请求：" + ab_path);
            //检查当前AB包的加载位置
            bool loadFormPerPath = IsLoadABFormPerPath(ab_path);

            string[] dependences; //依赖ab包
            if (loadFormPerPath && mABManifest_Per != null)
            {
                dependences = mABManifest_Per.GetAllDependencies(ab_path);
            }
            else
            {
                dependences = mABManifest_Stream.GetAllDependencies(ab_path);
            }

            

            //加载依赖包
            if (dependences.Length >= 0)
            {
                int loaded_num = 0; //依赖加载完成计数器
                foreach (var item in dependences)
                {
                    if (Load_AssetBundle_And_Dependences_Recursion(item) != null)
                    {
                        StartCoroutine(Load_AssetBundle_And_Dependences_Recursion_Async(item, (ab) => {
                            //回调
                            loaded_num++;
                            if(loaded_num >= dependences.Length)
                            {
                                //所有依赖加载完成
                                StartCoroutine(Load_AssetBundle_Single(ab_path,callback,loadFormPerPath,dependences));
                            }
                        }));
                        yield return mWaitForEndOfFrame;
                    }
                }
            }

            StartCoroutine(Load_AssetBundle_Single(ab_path, callback, loadFormPerPath, dependences));

        }

        /// <summary>
        /// [异步] 从AB包中加载资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ab"></param>
        /// <param name="file_Name"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        IEnumerator Load_Asset_From_AssetBundle<T>(AssetBundle ab,string file_Name,Action<T> callback) where T: UnityEngine.Object
        {
            var req = ab.LoadAssetAsync<T>(file_Name);
            yield return req;
            if (req.asset == null)
            {
                callback?.Invoke(null);
            }
            else
            {
                callback?.Invoke((T)req.asset);
            }
            
        }

        private async UniTask<T> Load_Asset_From_AssetBundle_Async<T>(AssetBundle ab, string file_Name) where T:UnityEngine.Object
        {
            var req = ab.LoadAssetAsync<T>(file_Name);
            await req;
            return req.asset as T;
        }

        /// <summary>
        /// [异步] 从AB包中加载资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ab"></param>
        /// <param name="file_Name"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        IEnumerator Load_Asset_From_AssetBundle(AssetBundle ab, string file_Name,Type type, Action<UnityEngine.Object> callback)
        {
            var req = ab.LoadAssetAsync(file_Name,type);
            yield return req;
            if (req.asset == null)
            {
                callback?.Invoke(null);
            }
            else
            {
                callback?.Invoke(req.asset);
            }
            
        }

        /// <summary>
        /// [await/async] 异步
        /// </summary>
        /// <param name="ab"></param>
        /// <param name="file_Name"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private async UniTask<UnityEngine.Object> Load_Asset_From_AssetBundle_Async(AssetBundle ab, string file_Name, Type type)
        {
            var req = ab.LoadAssetAsync(file_Name, type);
            await req;
            return req.asset;
        }


        /// <summary>
        /// 检查是否从persistentDataPath位置加载文件
        /// </summary>
        /// <param name="ab_name">AssetBundle包名</param>
        /// <returns>如果persistentDataPath路径下有这个文件，则范围true</returns>
        private bool IsLoadABFormPerPath(string ab_name)
        {
            return File.Exists(mPersistentDataPath + ab_name);
        }



        /// <summary>
        /// 注册AssetBundle资源引用（引用数+1）
        /// </summary>
        private void Register_AB_Use(string ab_path)
        {
            if (mLoadedAssetBundlePool.ContainsKey(ab_path))
            {
                mLoadedAssetBundlePool[ab_path].Register_Use();
            }
        }

        /// <summary>
        /// 可递归的注销资源引用
        /// </summary>
        /// <param name="ab_path"></param>
        private void pUnRegister_AB_Use(string ab_path)
        {
            if (mLoadedAssetBundlePool.ContainsKey(ab_path))
            {
                //依赖
                foreach(var item in mLoadedAssetBundlePool[ab_path].GetDependences())
                {
                    pUnRegister_AB_Use(item);
                }
                //自身
                mLoadedAssetBundlePool[ab_path].Remove_Use();
            }

        }

    }


    /// <summary>
    /// 被加载的AB包
    /// </summary>
    public class LoadedAssetBundle
    {
        /// <summary>
        /// 被加载的AssetBundle资源
        /// </summary>
        public AssetBundle ab_asset;

        /// <summary>
        /// 当前加载的资源是否是来自StreamingAssets路径的
        /// </summary>
        public bool IsFromStreamingAssetsPath;

        /// <summary>
        /// 相对unity路径，以"asset/xxx"开头
        /// </summary>
        public string ab_path;

        /// <summary>
        /// 完整的ab路径
        /// </summary>
        public string ab_full_path;

        /// <summary>
        /// 当前包的所有依赖，相对unity路径
        /// </summary>
        private string[] Dependences;

        /// <summary>
        /// 当前AB包的引用数
        /// </summary>
        private int useNum;

        /// <summary>
        /// 登记的时候初始化数据
        /// </summary>
        /// <param name="ab_data">AssetBundle数据本体</param>
        /// <param name="fromStreamingAssetsPath">是否来自StreamingAssets</param>
        /// <param name="path">Unity相对路径</param>
        /// <param name="full_path">系统绝对路径</param>
        /// /// <param name="dependences">当前包的依赖们</param>
        public void Init(AssetBundle ab_data, bool fromStreamingAssetsPath, string path, string full_path,string[] _dependences)
        {
            ab_asset = ab_data;
            IsFromStreamingAssetsPath = fromStreamingAssetsPath;
            ab_path = path;
            ab_full_path = full_path;
            Dependences = _dependences;
        }


        /// <summary>
        /// 注册引用
        /// </summary>
        public void Register_Use()
        {
            useNum++;
        }

        /// <summary>
        /// 取消注册引用
        /// </summary>
        /// <param name="fileName"></param>
        public void Remove_Use()
        {
            useNum--;
        }

        public int GetUsedNum()
        {
            return useNum;
        }

        /// <summary>
        /// 设置资源的依赖关系
        /// </summary>
        /// <param name="_dependences"></param>
        public void SetDependences(string[] _dependences)
        {
            Dependences = _dependences;
        }

        /// <summary>
        /// 获取资源的依赖关系
        /// </summary>
        /// <returns></returns>
        public string[] GetDependences()
        {
            return Dependences;
        }

    }

    
    /// <summary>
    /// 加载计划【单个AB文件】，只有异步加载AB包才会创建加载计划
    /// </summary>
    public class LoadPlane
    {
        /// <summary>
        /// 计划加载的资源路径
        /// </summary>
        public readonly string load_ab_path;

        private Action<AssetBundle> loadfinish_callback;

        public LoadPlane(string ab_path, Action<AssetBundle> callback)
        {
            load_ab_path = ab_path;
            loadfinish_callback += callback;
        }

        public LoadPlane(string ab_path)
        {
            load_ab_path = ab_path;
        }

        public void AddCallback(Action<AssetBundle> callback)
        {
            loadfinish_callback += callback;
        }

        public void Fire(AssetBundle AB)
        {
            if (loadfinish_callback != null)
            {
                loadfinish_callback(AB);
            }
        }


    }
}
