using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR && ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace TinaX.UIKit
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(UIEntity))]
    public class UIBase : UIBaseSafer
    {
#if UNITY_EDITOR && ODIN_INSPECTOR
        [FoldoutGroup("ConfIg")]
#endif
        [Header("启用Update事件")]
        public bool EnableUpdate;
#if UNITY_EDITOR && ODIN_INSPECTOR
        [FoldoutGroup("ConfIg")]
#endif
        [Header("次序")]
        public int UpdateOrder = 0;
#if UNITY_EDITOR && ODIN_INSPECTOR
        [FoldoutGroup("ConfIg")]
#endif
        [Header("启用FixedUpdate事件")]
        public bool EnableLateUpdate;
#if UNITY_EDITOR && ODIN_INSPECTOR
        [FoldoutGroup("ConfIg")]
#endif
        [Header("次序")]
        public int LateUpdateOrder = 0;

        private bool mEnableUpdate;
        private int mUpdateOrder;
        private bool mEnableLateUpdate;
        private int mLateUpdateOrder;

        private UIEntity mUIEntity;


        #region 生命周期

        protected override sealed void Awake()
        {
            //编辑器里需要直接使用Public的变量作为配置，所有不能做限制。但是要防止它在运行时被改变，所有在初始化时把它转成私有的，接下来都用私有里面的数值
            mEnableUpdate = EnableUpdate;
            mEnableLateUpdate = EnableLateUpdate;
            mUpdateOrder = UpdateOrder;
            mLateUpdateOrder = LateUpdateOrder;
            if (mEnableUpdate)
            {
                TimeMachine.I.AddUpdate(Self_Update, mUpdateOrder);
            }
            if (mEnableLateUpdate)
            {
                TimeMachine.I.AddLateUpdate(Self_LateUpdate, mLateUpdateOrder);
            }

            mUIEntity = gameObject.GetComponent<UIEntity>();

            XAwake();
        }

        protected virtual void XAwake()
        {

        }

        protected override sealed void Start()
        {
            XStart();
        }

        protected virtual void XStart()
        {

        }

        protected override sealed void OnDestroy()
        {
            //Debug.Log("UI被关闭");
            if (mEnableUpdate)
            {
                TimeMachine.I.RemoveUpdate(Self_Update, mUpdateOrder);
            }
            if (mEnableLateUpdate)
            {
                TimeMachine.I.RemoveLateUpdate(Self_LateUpdate, mLateUpdateOrder);
            }

            //注销事件
            foreach(var item in mEventPool)
            {
                XEvent.Remove(item);
            }

            XOnDestroy();
        }

        protected virtual void XOnDestroy()
        {

        }

        //用私有方法给时间机，防止有什么奇怪的幺蛾子
        private void Self_Update()
        {
            XUpdate();
        }
        private void Self_LateUpdate()
        {
            XLateUpdate();
        }

        protected virtual void XUpdate()
        {

        }

        protected virtual void XLateUpdate()
        {

        }

        /// <summary>
        /// 打开UI时收到的消息
        /// </summary>
        /// <param name="param"></param>
        public virtual void OnOpenUIMessage(System.Object param)
        {

        }

        public virtual void OnCloseUIMessage(System.Object param)
        {

        }


        #endregion

        #region 安全的事件


        private List<int> mEventPool = new List<int>();

        /// <summary>
        /// 注册事件
        /// </summary>
        /// <param name="eventName">事件名</param>
        /// <param name="callback">事件回调</param>
        protected void EventRegister(string eventName,System.Action callback)
        {
            mEventPool.Add(XEvent.Register(eventName, callback));
        }

        /// <summary>
        /// 注册事件
        /// </summary>
        /// <param name="eventName">事件名</param>
        /// <param name="callback">事件回调</param>
        protected void EventRegister(string eventName, System.Action<System.Object> callback)
        {
            mEventPool.Add(XEvent.Register(eventName, callback));
        }


        #endregion

        #region UI操作



        /// <summary>
        /// 关掉自己
        /// </summary>
        protected void CloseMe()
        {
            mUIEntity?.Close();
        }



        #endregion

    }
}

