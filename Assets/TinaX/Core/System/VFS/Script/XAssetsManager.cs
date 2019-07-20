using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TinaX.Conf;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TinaX.VFS
{
    /// <summary>
    /// 资源管理器
    /// </summary>
    public class XAssetsManager : IVFS
    {
        /// <summary>
        /// 资源管理器用户配置，
        /// </summary>
        private VFSConfigModel mAssetsConfig;
        public VFSConfigCache mVFSConfigCache { get; private set; }

        #region 一堆公开的属性

        /// <summary>
        /// VFS IO Path
        /// </summary>
        public string VFSIOPath
        {
            get
            {
                return Application.streamingAssetsPath + "/vfs_io/" + Const.PlatformConst.GetPlatformName(Application.platform).ToLower() + "/";
            }
        }


        #endregion

        private XAssetBundleMgr mXAssetBundleMgr;






        public XAssetsManager()
        {
            //资源配置
            mAssetsConfig = Config.GetTinaXConfig<VFSConfigModel>(ConfigPath.vfs);
            if (mAssetsConfig == null)
            {
                mAssetsConfig = new VFSConfigModel();
            }
            mVFSConfigCache = VFSConfigCache.New(mAssetsConfig);

            //异步加载的子管理器
            mXAssetBundleMgr = GameObjectHelper
                .FindOrCreateGo(Setup.Framework_Base_GameObject)
                .GetComponentOrAdd<XAssetBundleMgr>()
                .Init(this);

        }

        /// <summary>
        /// 加载资源
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="path">路径</param>
        /// <returns></returns>
        public T LoadAsset<T>(string path) where T : UnityEngine.Object
        {
            
#if UNITY_EDITOR
            //编辑器模式下，需要判断，是从哪儿加载资源
            if (Menu.GetChecked(Const.AssetSystemConst.menu_editor_load_from_asset_pack_name))
            {
                //从资源系统加载
                return mXAssetBundleMgr.LoadAsset<T>(path);
            }
            else
            {
                var fileInfo = AssetParse.Parse(path, mVFSConfigCache);
                if (fileInfo.handle_tag == E_FileHandleTag.invalid)
                {
                    EditorUtility.DisplayDialog("资源系统 - 错误", "无法加载资源：" + path + "\n该路径资源在TinaX资源系统的管理范围之外，请尝试调整资源管理策略配置。", "好");
                    return null;
                }
                else
                {
                    //直接使用编辑器加载策略
                    return AssetDatabase.LoadAssetAtPath<T>(path);
                }
                    

            }
#else

            return mXAssetBundleMgr.LoadAsset<T>(path);
#endif
        }

        public UnityEngine.Object LoadAsset(string path, Type type)
        {
            
#if UNITY_EDITOR
            //编辑器模式下，需要判断，是从哪儿加载资源
            if (Menu.GetChecked(Const.AssetSystemConst.menu_editor_load_from_asset_pack_name))
            {
                //从资源系统加载
                return mXAssetBundleMgr.LoadAsset(path, type);
            }
            else
            {
                var fileInfo = AssetParse.Parse(path, mVFSConfigCache);
                if (fileInfo.handle_tag == E_FileHandleTag.invalid)
                {
                    EditorUtility.DisplayDialog("资源系统 - 错误", "无法加载资源：" + path + "\n该路径资源在TinaX资源系统的管理范围之外，请尝试调整资源管理策略配置。", "好");
                    return null;
                }
                else
                {
                    //直接使用编辑器加载策略
                    return AssetDatabase.LoadAssetAtPath(path, type);
                }
            }
#else

            return mXAssetBundleMgr.LoadAsset(path, type);
#endif
        }

        /// <summary>
        /// 加载资源[异步]
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path">路径</param>
        /// <param name="callback">回调</param>
        /// <returns></returns>
        public void LoadAssetAsync<T>(string path, Action<T> callback) where T : UnityEngine.Object
        {
            
#if UNITY_EDITOR
            //编辑器模式下，需要判断，是从哪儿加载资源
            if (Menu.GetChecked(Const.AssetSystemConst.menu_editor_load_from_asset_pack_name))
            {
                //从资源系统加载
                mXAssetBundleMgr.LoadAsset_Async<T>(path, callback);
            }
            else
            {
                var fileInfo = AssetParse.Parse(path, mVFSConfigCache);
                if (fileInfo.handle_tag == E_FileHandleTag.invalid)
                {
                    //编辑器报提示
                    EditorUtility.DisplayDialog("资源系统 - 错误", "无法加载资源：" + path + "\n该路径资源在TinaX资源系统的管理范围之外，请尝试调整资源管理策略配置。", "好");
                    if (callback != null)
                    {
                        callback(null);
                    }
                }
                else
                {
                    //直接使用编辑器加载策略
                    if (callback != null)
                    {
                        callback(AssetDatabase.LoadAssetAtPath<T>(path));
                    }
                }
                

            }
#else

            mXAssetBundleMgr.LoadAsset_Async<T>(path, callback);
#endif

        }


        /// <summary>
        /// 加载资源[异步]
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path">路径</param>
        /// <param name="callback">回调</param>
        /// <returns></returns>
        public void LoadAssetAsync(string path, System.Type type, Action<UnityEngine.Object> callback)
        {
            //尝试解析这个文件可否被加载
            var path_info = AssetParse.Parse(path, mVFSConfigCache);
            if (path_info.handle_tag == E_FileHandleTag.invalid)
            {
#if UNITY_EDITOR
                //编辑器报提示
                EditorUtility.DisplayDialog("资源系统 - 错误", "无法加载资源：" + path + "\n该路径资源在TinaX资源系统的管理范围之外，请尝试调整资源管理策略配置。", "好");
#endif  
                return;
            }
            path_info.ab_name += ".xab";
#if UNITY_EDITOR
            //编辑器模式下，需要判断，是从哪儿加载资源
            if (Menu.GetChecked(Const.AssetSystemConst.menu_editor_load_from_asset_pack_name))
            {
                //从资源系统加载
                mXAssetBundleMgr.LoadAsset_Async(path, type, callback);
            }
            else
            {

                var fileInfo = AssetParse.Parse(path, mVFSConfigCache);
                if (fileInfo.handle_tag == E_FileHandleTag.invalid)
                {
                    //编辑器报提示
                    EditorUtility.DisplayDialog("资源系统 - 错误", "无法加载资源：" + path + "\n该路径资源在TinaX资源系统的管理范围之外，请尝试调整资源管理策略配置。", "好");
                    if (callback != null)
                    {
                        callback(null);
                    }
                }
                else
                {
                    //直接使用编辑器加载策略
                    if (callback != null)
                    {
                        callback(AssetDatabase.LoadAssetAtPath(path, type));
                    }
                }


            }
#else

            mXAssetBundleMgr.LoadAsset_Async(path, type, callback);
#endif

        }

        /// <summary>
        /// 移除资产引用
        /// </summary>
        public void RemoveUse(string path)
        {
            //对外统称为path,不区分fileName和abName
            //尝试解析这个文件可否被加载
            var path_info = AssetParse.Parse(path, mVFSConfigCache);
            if (path_info.handle_tag == E_FileHandleTag.invalid)
            {
                //无效资源，不用管
                return;
            }
            mXAssetBundleMgr.UnRegister_AB_Use(path_info.ab_name);
        }

        /// <summary>
        /// 清理资源
        /// </summary>
        public void GC()
        {
            mXAssetBundleMgr.GC();
        }

        public string GetVFSPersistentDataPath()
        {
            return mXAssetBundleMgr.GetVFSPersistentDataPath();
        }

        /// <summary>
        /// 做好销毁准备（释放资源之类的），在热重启方法中使用
        /// </summary>
        /// <returns></returns>
        public void DestroyReady()
        {

        }

        #region Private




        #endregion





#if UNITY_EDITOR

        public LoadedAssetBundle[] Debug_GetABLoadedInfo()
        {
            List<LoadedAssetBundle> t = new List<LoadedAssetBundle>();
            foreach(var item in mXAssetBundleMgr.GetLoadedAssetBundleInfo())
            {
                t.Add(item.Value);
            }
            return t.ToArray();
        }

#endif

    }

    



#if UNITY_EDITOR
    public class XAssetsMgrEditor
    {
        [MenuItem(Const.AssetSystemConst.menu_editor_load_from_asset_pack_name, false)]
        static void OnMenuOnclicked_LoadFormAssetPack()
        {
            Menu.SetChecked(Const.AssetSystemConst.menu_editor_load_from_asset_pack_name, !Menu.GetChecked(Const.AssetSystemConst.menu_editor_load_from_asset_pack_name));
        }
    }


    

#endif
}

