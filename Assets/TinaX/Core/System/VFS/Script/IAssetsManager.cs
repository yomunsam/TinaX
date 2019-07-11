using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TinaX.VFS
{
    public interface IAssetsManager
    {
        /// <summary>
        /// 加载资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path">资源路径</param>
        /// <returns></returns>
        T LoadAsset<T>(string path) where T : UnityEngine.Object;

        UnityEngine.Object LoadAsset(string path, System.Type type);

        void LoadAssetAsync<T>(string path, Action<T> callback) where T: UnityEngine.Object;

        void LoadAssetAsync(string path, System.Type type, Action<UnityEngine.Object> callback);

        

        /// <summary>
        /// 移除资产引用
        /// </summary>
        /// <param name="file_name">文件路径</param>
        void RemoveUse(string path);

        /// <summary>
        /// 释放无用资源,针对全局已加载资源中无引用的资源
        /// </summary>
        void GC();


        /// <summary>
        /// 获取热更新补丁的VFS根目录
        /// </summary>
        /// <returns></returns>
        string GetVFSPersistentDataPath();


        /// <summary>
        /// 做好销毁准备（释放资源之类的），在热重启方法中使用
        /// </summary>
        void DestroyReady();

#if UNITY_EDITOR

        LoadedAssetBundle[] Debug_GetABLoadedInfo();

#endif

    }
}

