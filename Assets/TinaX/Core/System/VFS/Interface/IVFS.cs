using System;
using System.Threading.Tasks;
using UniRx.Async;
using UnityEngine;

namespace TinaX
{
    /// <summary>
    /// Virtual File System | 虚拟文件系统
    /// </summary>
    public interface IVFS
    {
        /// <summary>
        /// Load Asset 加载资源
        /// </summary>
        /// <typeparam name="T">Asset Type 资源类型</typeparam>
        /// <param name="assetPath">Asset Path 资源路径地址</param>
        /// <returns></returns>
        T LoadAsset<T>(string assetPath) where T : UnityEngine.Object;

        /// <summary>
        /// Load Asset 加载资源
        /// </summary>
        /// <param name="assetPath">Asset Path 资源路径地址</param>
        /// <param name="type">Asset Type 资源类型</param>
        /// <returns></returns>
        UnityEngine.Object LoadAsset(string assetPath, System.Type type);

        /// <summary>
        /// Load Asset Async (async/await) | 异步加载资源（async/await）
        /// </summary>
        /// <typeparam name="T">Asset Type 资源类型</typeparam>
        /// <param name="assetPath">Asset Path 资源路径地址</param>
        /// <returns></returns>
        UniTask<T> LoadAssetAsync<T>(string assetPath) where T : UnityEngine.Object;

        /// <summary>
        /// Load Asset Async (async/await) | 异步加载资源（async/await）
        /// </summary>
        /// <param name="assetPath">Asset Path 资源路径地址</param>
        /// <param name="type">Asset Type 资源类型</param>
        /// <returns></returns>
        UniTask<UnityEngine.Object> LoadAssetAsync(string assetPath, System.Type type);

        /// <summary>
        /// Load Asset Async (callback) | 异步加载资源（通过回调）
        /// </summary>
        /// <typeparam name="T">Asset Type 资源类型</typeparam>
        /// <param name="assetPath">Asset Path 资源路径地址</param>
        /// <param name="callback">Loaded callback 加载结束回调</param>
        void LoadAssetAsync<T>(string assetPath, Action<T> callback) where T : UnityEngine.Object;

        /// <summary>
        /// Load Asset Async (callback) | 异步加载资源（通过回调）
        /// </summary>
        /// <param name="assetPath">Asset Path 资源路径地址</param>
        /// <param name="type">Asset Type 资源类型</param>
        /// <param name="callback">Loaded callback 加载结束回调</param>
        void LoadAssetAsync(string assetPath, System.Type type, Action<UnityEngine.Object> callback);

        /// <summary>
        /// [WebVFS] Load Asset from Web Async (async/await) | 从WebVFS异步加载资源（async/await）
        /// </summary>
        /// <typeparam name="T">Asset Type 资源类型</typeparam>
        /// <param name="assetPath">Asset Path 资源路径地址</param>
        /// <param name="useCache">cache asset | 缓存资源到本地</param>
        /// <returns></returns>
        UniTask<T> LoadWebAssetAsync<T>(string assetPath, bool useCache = true) where T : UnityEngine.Object;

        /// <summary>
        /// [WebVFS] Load Asset from Web Async (callback) | 从WebVFS异步加载资源（通过回调）
        /// </summary>
        /// <typeparam name="T">Asset Type 资源类型</typeparam>
        /// <param name="assetPath">Asset Path 资源路径地址</param>
        /// <param name="callback"></param>
        /// <param name="useCache">cache asset | 缓存资源到本地</param>
        void LoadWebAssetAsync<T>(string assetPath, Action<T> callback, bool useCache = true) where T : UnityEngine.Object;

        /// <summary>
        /// [WebVFS] Load Asset from Web Async (callback) | 从WebVFS异步加载资源（通过回调）
        /// </summary>
        /// <param name="assetPath">Asset Path 资源路径地址</param>
        /// <param name="type">Asset Type 资源类型</param>
        /// <param name="callback"></param>
        /// <param name="useCache">cache asset | 缓存资源到本地</param>
        void LoadWebAssetAsync(string assetPath, System.Type type, Action<UnityEngine.Object> callback, bool useCache = true);

        UniTask<T> LoadAssetLocalOrWebAsync<T>(string assetPath) where T : UnityEngine.Object;

        void RemoveUse(string load_path);



        #region 常用加载类型封装

        GameObject LoadPrefab(string assetPath);

        UniTask<GameObject> LoadPrefabAsync(string assetPath);

        void LoadPrefabAsync(string assetPath, Action<GameObject> callbcak);


        #endregion


        Task CtorAsync();

    }
}

