using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace TinaX
{
    /// <summary>
    /// 场景管理器接口 | TinaX scene manager interface
    /// </summary>
    public interface ISceneManager
    {
        /// <summary>
        /// 打开场景 | Open scene
        /// </summary>
        /// <param name="scene_name">场景名 | Scene Name</param>
        /// <returns></returns>
        ISceneManager OpenScene(string scene_name);

        /// <summary>
        /// 获取当前场景名称 | Get current active scene name
        /// </summary>
        /// <returns>场景名 | scene name</returns>
        string GetActiveSceneName();

        /// <summary>
        /// 获取上一个场景名，如果不存在，返回"string.Empty" | Get the last scene's name (load with TinaX scene mgr), if not exist, will return "string.Empty"
        /// </summary>
        /// <returns>上一个场景名，如果不存在，返回string.Empty | scene name or "string.Empty"</returns>
        string GetLastSceneName();

        /// <summary>
        /// 打开场景[异步 回调] | Open Scene [async with callback]
        /// </summary>
        /// <param name="scene_name"></param>
        /// <param name="OnFinishCallback"></param>
        void OpenSceneAsync(string scene_name, Action<SceneLoadAsyncPlan> OnFinishCallback);

        /// <summary>
        /// 打开场景[异步] | Open Scene [async with callback]
        /// </summary>
        /// <param name="scene_name"></param>
        /// <param name="allowSceneActivation">允许在场景准备就绪后立即激活场景。|Allow Scenes to be activated as soon as it is ready.</param>
        /// <param name="OnFinishCallback"></param>
        void OpenSceneAsync(string scene_name, bool allowSceneActivation, Action<SceneLoadAsyncPlan> OnFinishCallback = null);


        /// <summary>
        /// 打开场景[异步] | Open Scene [async with callback]
        /// </summary>
        /// <param name="scene_name">场景名 | scene name</param>
        /// <param name="OnProgressCallback"> 加载进度回调 |  callback for loading progress</param>
        /// <param name="OnFinishCallback">加载完成回调 | callbcak on loading finish</param>
        void OpenSceneAsync(string scene_name, Action<float> OnProgressCallback, Action<SceneLoadAsyncPlan> OnFinishCallback = null);

        /// <summary>
        /// 打开场景[异步] | Open Scene [async with callback]
        /// </summary>
        /// <param name="scene_name">场景名 | scene name</param>
        /// <param name="allowSceneActivation">允许在场景准备就绪后立即激活场景。|Allow Scenes to be activated as soon as it is ready.</param>
        /// <param name="OnProgressCallback">加载进度回调 |  callback for loading progress</param>
        /// <param name="OnFinishCallback">加载完成回调 | callbcak on loading finish</param>
        void OpenSceneAsync(string scene_name, bool allowSceneActivation, Action<float> OnProgressCallback, Action<SceneLoadAsyncPlan> OnFinishCallback = null);

        /// <summary>
        /// 打开场景[异步] | Open Scene [async with callback]
        /// </summary>
        /// <param name="scene_name">场景名 | scene name</param>
        /// <param name="loadSceneMode">加载模式 | laodmode</param>
        /// <param name="allowSceneActivation">允许在场景准备就绪后立即激活场景。|Allow Scenes to be activated as soon as it is ready.</param>
        /// <param name="OnProgressCallback">加载进度回调 |  callback for loading progress</param>
        /// <param name="OnFinishCallback">加载完成回调 | callbcak on loading finish</param>
        void OpenSceneAsync(string scene_name, UnityEngine.SceneManagement.LoadSceneMode loadSceneMode, bool allowSceneActivation = true, Action<float> OnProgressCallback = null, Action<SceneLoadAsyncPlan> OnFinishCallback = null);


        /// <summary>
        /// 打开场景[异步 async/await] Open scene [async with "async/await"]
        /// </summary>
        /// <param name="scene_name"></param>
        //void OpenSceneAsync(string scene_name);

    }
}

