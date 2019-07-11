using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UniRx;
using UniRx.Async;
using System;
//using System.Linq; //好像听说有的平台不兼容LINQ，但记不清具体怎么回事了，那反正就先不用吧


namespace TinaX
{
    public class XSceneManager : ISceneManager
    {
        #region runtime
        //----Active Scene

        private List<S_Scene_Load_Info> mSceneChangedPool = new List<S_Scene_Load_Info>();

        /// <summary>
        /// 异步加载计划 记录
        /// </summary>
        private List<SceneLoadAsyncPlan> mAsyncLoadPlanPool = new List<SceneLoadAsyncPlan>();


        #endregion
        
        //构造函数
        public XSceneManager()
        {
            //登记委托
            SceneManager.activeSceneChanged += OnActiveSceneChanged;
        }

        //析构函数
        ~XSceneManager()
        {
            //取消委托
            SceneManager.activeSceneChanged -= OnActiveSceneChanged;
        }

        //获取当前活动场景名
        public string GetActiveSceneName()
        {
            return UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        }

        //获取上一个场景名
        public string GetLastSceneName()
        {
            var last_index = mSceneChangedPool.Count - 1;
            if (last_index < 0)
            {
                return string.Empty;
            }
            else
            {
                return mSceneChangedPool[last_index].SceneName;
            }
        }


        //打开场景
        public ISceneManager OpenScene(string scene_name)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(scene_name);
            return this;
        }


        //        public void OpenSceneAsync(string scene_name, Action OnFinishCallback)
        //        {
        //#pragma warning disable 4014
        //            OpenSceneAsyncWithCallback(scene_name, OnFinishCallback);
        //#pragma warning restore 4014

        //        }

        #region public 一堆异步加载的方法

        public void OpenSceneAsync(string scene_name, Action<SceneLoadAsyncPlan> OnFinishCallback = null)
        {
            OpenSceneAsyncWithRxCoroutine(scene_name: scene_name, onFinishCallback: OnFinishCallback);
        }

        public void OpenSceneAsync(string scene_name, bool allowSceneActivation, Action<SceneLoadAsyncPlan> OnFinishCallback = null)
        {
            OpenSceneAsyncWithRxCoroutine(scene_name: scene_name, onFinishCallback: OnFinishCallback,allowSceneActivation:allowSceneActivation);

        }

        public void OpenSceneAsync(string scene_name, Action<float> OnProgressCallback, Action<SceneLoadAsyncPlan> OnFinishCallback = null)
        {
            OpenSceneAsyncWithRxCoroutine(scene_name: scene_name, onFinishCallback: OnFinishCallback, onProgressCallback: OnProgressCallback);
        }

        public void OpenSceneAsync(string scene_name, bool allowSceneActivation, Action<float> OnProgressCallback, Action<SceneLoadAsyncPlan> OnFinishCallback = null)
        {
            OpenSceneAsyncWithRxCoroutine(scene_name: scene_name, onFinishCallback: OnFinishCallback, onProgressCallback: OnProgressCallback, allowSceneActivation: allowSceneActivation);
        }

        public void OpenSceneAsync(string scene_name, LoadSceneMode loadSceneMode, bool allowSceneActivation = true, Action<float> OnProgressCallback = null, Action<SceneLoadAsyncPlan> OnFinishCallback = null)
        {
            OpenSceneAsyncWithRxCoroutine(scene_name: scene_name, onFinishCallback: OnFinishCallback, onProgressCallback: OnProgressCallback, allowSceneActivation: allowSceneActivation,loadSceneMode: loadSceneMode);
        }


        #endregion

        //public async void OpenSceneAsync(string scene_name)
        //{
        //    await SceneManager.LoadSceneAsync(scene_name);
        //}


        //活动场景切换事件
        private void OnActiveSceneChanged(Scene cur_scene,Scene new_scene)
        {
            //登记场景切换
            mSceneChangedPool.Add(new S_Scene_Load_Info() {
                SceneName = new_scene.name
            });
            //发送事件
            CatLib.App.Trigger(EventDefine.X_OnSceneChanged,cur_scene,new_scene);
            
        }

        
        //private async UniTask OpenSceneAsyncWithCallback(string scene_name, Action finishCallback, LoadSceneMode loadSceneMode = LoadSceneMode.Single)
        //{
        //    await SceneManager.LoadSceneAsync(scene_name, loadSceneMode);
        //    if (finishCallback != null)
        //    {
        //        finishCallback();
        //    }
        //}


        /// <summary>
        /// 
        /// </summary>
        /// <param name="scene_name">场景名</param>
        /// <param name="loadSceneMode">加载模式</param>
        /// <param name="allowSceneActivation">允许在场景准备就绪后立即激活场景。</param>
        /// <param name="onFinishCallback">加载结束回调</param>
        /// <param name="onProgressCallback">加载过程进度回调</param>
        private void OpenSceneAsyncWithRxCoroutine(string scene_name, 
            LoadSceneMode loadSceneMode = LoadSceneMode.Single, 
            bool allowSceneActivation = true, 
            Action<SceneLoadAsyncPlan> onFinishCallback= null,
            Action<float> onProgressCallback = null)
        {
            //判断是否已经在加载
            var flag = false;
            
            for (int i= 0; i < mAsyncLoadPlanPool.Count; i++)
            {
                if (mAsyncLoadPlanPool[i].sceneName == scene_name)
                {
                    flag = true;
                    //欸呀呀，已经正在加载它了呀w(ﾟДﾟ)w
                    if (onFinishCallback != null)   { mAsyncLoadPlanPool[i].AddFinishCallback(onFinishCallback); }                      //以callback之名起誓：等它加载好了顺便告诉我一声哦
                    if (onProgressCallback != null) { mAsyncLoadPlanPool[i].AddLoadProgressCallback(onProgressCallback); }              //ね ね ， 顺便把加载进度告诉我呗


                    break;
                }
            }


            if (!flag)
            {
                //创建一个计划
                var loadPlan = new SceneLoadAsyncPlan(
                        _scene_name: scene_name,
                        _loadSceneMode: loadSceneMode,
                        _allowSceneActivation: allowSceneActivation,
                        _onFinishCallback: onFinishCallback,
                        _onProgressCallback: onProgressCallback
                    );
                //把计划加入到pool
                if (!mAsyncLoadPlanPool.Contains(loadPlan))
                {
                    mAsyncLoadPlanPool.Add(loadPlan);
                }
                //启动一个协程去加载它呀
                MainThreadDispatcher.StartUpdateMicroCoroutine(LoadSceneAsyncCoroutine(loadPlan));
            }

            
        }

        private IEnumerator LoadSceneAsyncCoroutine(SceneLoadAsyncPlan loadPlan)
        {
            var asyncOperation = SceneManager.LoadSceneAsync(loadPlan.sceneName, loadPlan.loadSceneMode);
            loadPlan.LoadStart(asyncOperation); //更新状态
            asyncOperation.allowSceneActivation = loadPlan.allowSceneActivation;
            while (!asyncOperation.isDone)
            {
                loadPlan.InvokeProgress(asyncOperation.progress);
                if (!loadPlan.allowSceneActivation && asyncOperation.progress>= 0.9f)
                {
                    //这时候异步加载的工作已经完成了（虽然还没加载完），要把OnFinish回调出去
                    loadPlan.InvokeFinishIfNotActivation();
                }
                yield return null;
            }

            if (asyncOperation.isDone)
            {
                loadPlan.InvokeFinishOnTrueDone();
                //从计划列表中拿掉自己
                if (mAsyncLoadPlanPool.Contains(loadPlan))
                {
                    mAsyncLoadPlanPool.Remove(loadPlan);
                }

                yield break;
            }
        }



        /*
         * 关于allowSceneActivation，叨叨两句：
         * allowSceneActivation这个是Unity里面一个奇妙的东西。
         * 它的设定是这样的，如果allowSceneActivation = true, 那么这个场景异步加载完成之后会自动切到这个场景。
         * 如果allowSceneActivation = false的话，坑来了啊，它不会加载完。对，我也是第一次看到这么奇葩的设定
         *      如果allowSceneActivation = false的话，Unity会在异步加载到90%左右停住，直到你把allowSceneActivation设为true之后，
         *      它继续加载完，然后自动切过去。
         * 
         * 由于这个设定的存在，在TinaX的场景异步加载策略是这个设定的：
         * 如果allowSceneActivation 为 true的话，没什么特别的，加载完成之后触发OnFinishCallback回调。
         * 如果allowSceneActivation 为 false的话，就不一样了，首先在异步加载到90%以上时，加载会停住嘛，这时候会触发OnFinishCallback,
         * 等场景正式加载完之后会触发第二次OnFinishCallback
         *      当开发者在收到OnFinishCallback之后，会接收一个“SceneLoadAsyncPlan”类型的参数，在这个参数里，有这么几个东西可以用：
         *          1. SceneLoadAsyncPlan.isNotActivationCallback       bool
         *              这个属性为true的时候，说明这是加载到90%停住的这时候的第一次Callback,
         *              其他任何情况获取到它都是false
         *              
         *          2. SceneLoadAsyncPlan.isDone,          bool
         *              检查时候加载完的属性，如果是加载90%停住的时候，值是false
         *              
         *      好了，现在我们能在代码里区分两次callback了，那么在第一次callback之后，不是停住了嘛，我要怎么继续让它加载嘞，
         *          方法是：在callback接收的SceneLoadAsyncPlan类型参数中，调用“SceneLoadAsyncPlan.ContinueLoad”方法，它就会继续执行了。
         *          
         * lua那边封装了一层默认值，具体调用方法和C#差不多，具体怎么用翻文档吧（文档写好之前可以看看注释，在scene.lua.txt （XCore.Scene）这个位置
         * 目前SceneLoadAsyncPlan类中暴露了挺多东西的，可能对强迫症用户不太友好（至少本强迫症看着很别扭），以后看看考虑封装个interface给强迫症们调用
         * 顺便：在加载完之前，包括停在90%的时候，负责这个场景的加载任务的协程和加载记录会一直保持着。
         * （虽然这里的协程是用UniRx驱动的，没有Unity原生协程开销那么大就是了
         * 
         */

    }

    public struct S_Scene_Load_Info
    {
        public string SceneName;
    }


    /// <summary>
    /// 场景异步加载计划
    /// </summary>
    public struct SceneLoadAsyncPlan
    {
        /// <summary>
        /// SceneName
        /// </summary>
        public string sceneName { get; private set; }
        public Action<float> onProgressCallback { get; private set; }
        public Action<SceneLoadAsyncPlan> onFinishCallback { get; private set; }
        public bool allowSceneActivation { get; private set; }
        public LoadSceneMode loadSceneMode { get; private set; }

        private AsyncOperation mAO;
        private int tag_invoke_if_not_activation; //0: 默认状态  1：调用出去了一次

        /// <summary>
        /// 是否是“准备就绪后不自动激活”状态下的“准备就绪”这个部分的回调
        /// </summary>
        public bool isNotActivationCallback
        {
            get
            {
                if (allowSceneActivation)
                {
                    return false;
                }
                else
                {
                    return !mAO.isDone;
                }
            }
        }

        public bool isDone
        {
            get
            {
                return mAO.isDone;
            }
        }


        public SceneLoadAsyncPlan(
            string _scene_name,
            LoadSceneMode _loadSceneMode,
            Action<float> _onProgressCallback,
            Action<SceneLoadAsyncPlan> _onFinishCallback,
            bool _allowSceneActivation)
        {
            sceneName = _scene_name;
            onProgressCallback = _onProgressCallback;
            onFinishCallback = _onFinishCallback;
            allowSceneActivation = _allowSceneActivation;
            loadSceneMode = _loadSceneMode;
            mAO = null;
            tag_invoke_if_not_activation = 0;
        }

        public void LoadStart(AsyncOperation _asyncOperation)
        {
            mAO = _asyncOperation;
        }

        /// <summary>
        /// 继续加载, 用于allowSceneActivation = false 的情况
        /// </summary>
        public void ContinueLoad()
        {
            if (!allowSceneActivation)
            {
                mAO.allowSceneActivation = true;
            }
        }


        /// <summary>
        /// 内部使用，不要调用 | private func , don't invok it !!
        /// </summary>
        /// <param name="callback"></param>
        public void AddFinishCallback(Action<SceneLoadAsyncPlan> callback)
        {
            if(callback != null)
            {
                onFinishCallback += callback;
            }
        }

        /// <summary>
        /// 内部使用，不要调用 | private func , don't invok it !!
        /// </summary>
        /// <param name="callback"></param>
        public void AddLoadProgressCallback(Action<float> callback)
        {
            if(callback != null)
            {
                onProgressCallback += callback;
            }
        }

        /// <summary>
        /// 内部使用，不要调用 | private func , don't invok it !!
        /// </summary>
        /// <param name="value"></param>
        public void InvokeProgress(float value)
        {
            onProgressCallback?.Invoke(value);
        }


        /// <summary>
        /// 内部使用，不要调用 | private func , don't invok it !!
        /// </summary>
        public void InvokeFinishIfNotActivation() //在“不要在场景准备完成后立即激活场景”的状态下，Invoke一次
        {
            if(tag_invoke_if_not_activation == 0 && !allowSceneActivation)
            {
                tag_invoke_if_not_activation = 1;
                onFinishCallback?.Invoke(this);
            }
        }

        /// <summary>
        /// 内部使用，不要调用 | private func , don't invok it !!
        /// </summary>
        public void InvokeFinishOnTrueDone() //真的加载结束了
        {
            onFinishCallback?.Invoke(this);
        }

    }

}

