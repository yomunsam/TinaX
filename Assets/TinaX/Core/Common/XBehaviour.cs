using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TinaX
{
    public class XBehaviour
    {
        #region Update
        public bool EnableUpdate { get; set; } = false;
        public int UpdateOrder { get; set; } = 0;
        private bool mEnableUpdate = false;
        internal ulong mUpdateHandleId { get; private set; }

        #endregion

        #region LateUpdate

        public bool EnableLateUpdate { get; set; } = false;
        public int LateUpdateOrder { get; set; } = 0;
        private bool mEnableLateUpdate = false;
        internal ulong mLateUpdateHandleId { get; private set; }

        #endregion

        #region FixedUpdate

        public bool EnableFixedUpdate { get; set; } = false;
        public int FixedUpdateOrder { get; set; } = 0;
        private bool mEnableFixedUpdate = false;
        internal ulong mFixedUpdateHandleId { get; private set; }
        #endregion

        #region 安全的事件注册

        List<ulong> mEventPool = new List<ulong>();

        #endregion

        #region 暴露给之类的生命周期

        protected void XAwake() { }

        protected void XEnable() { }

        protected void XStart() { }

        protected void XFixedUpdate() { }

        protected void XUpdate() { }

        protected void XLateUpdate() { }

        protected void XOnDisable() { }

        protected void XOnDestroy() { }

        #endregion

        /*
         * 我们发现，生命周期方法在实现的时候实际上分为了两层，
         * 比如Awake为例，
         * 用户继承XBehaviour时，是在之类中重载XAwake方法
         * 而Framework内部在控制生命周期时，是调用FrameworkPAwake方法，然后由FrameworkPAwake去调用XAwake方法
         * 这样做法的目的是，我们可以在FrameworkPAwake中添加一些私有的初始化代码，而开发者在重载XAwake方法时，即使不写base.Awake();
         * 也能保证初始化代码正常运行，避免了一定可能性的出错
         * 
         */

        internal void FrameworkPAwake()
        {
            XAwake();

            TryAddUpdate();
            TryAddLateUpdate();
            TryAddFixedUpdate();
        }
        
        internal void FrameworkPEnable()
        {
            XEnable();
        }

        internal void FrameworkPStart()
        {
            XStart();
        }

        internal void FrameworkPFixedUpdate()
        {
            XFixedUpdate();
        }

        internal void FrameworkPUpdate() 
        {
            XUpdate();
        }

        internal void FrameworkPLateUpdate()
        {
            XLateUpdate();
        }

        internal void FrameworkPOnDisable()
        {
            XOnDisable();
        }

        internal void FrameworkPOnDestroy()
        {
            TryRemoveFixedUpdate();
            TryRemoveLateUpdate();
            TryRemoveUpdate();

            //注销事件
            foreach (var item in mEventPool)
            {
                XEvent.Remove(item);
            }

            XOnDestroy();
        }



        private void TryAddUpdate()
        {
            mEnableUpdate = EnableUpdate;   //加一道私有变量的作用是防止该类生命周期途中，外部错误的变更了公开属性的值导致运行错误。
            if (mEnableUpdate)
            {
                mUpdateHandleId = TimeMachine.I.AddUpdate(FrameworkPUpdate, UpdateOrder);
            }
        }

        private void TryAddFixedUpdate()
        {
            mEnableFixedUpdate = EnableFixedUpdate;   //加一道私有变量的作用是防止该类生命周期途中，外部错误的变更了公开属性的值导致运行错误。
            if (mEnableFixedUpdate)
            {
                mFixedUpdateHandleId = TimeMachine.I.AddFixedUpdate(FrameworkPFixedUpdate, FixedUpdateOrder);
            }
        }

        private void TryAddLateUpdate()
        {
            mEnableLateUpdate = EnableLateUpdate;
            if (mEnableLateUpdate)
            {
                mLateUpdateHandleId = TimeMachine.I.AddLateUpdate(FrameworkPLateUpdate, LateUpdateOrder);
            }
        }

        private void TryRemoveUpdate()
        {
            if (mEnableUpdate)
                TimeMachine.I.RemoveUpdate(mUpdateHandleId);

        }

        private void TryRemoveLateUpdate()
        {
            if (mEnableLateUpdate)
                TimeMachine.I.RemoveLateUpdate(mLateUpdateHandleId);
        }

        private void TryRemoveFixedUpdate()
        {
            if (mEnableFixedUpdate)
                TimeMachine.I.RemoveLateUpdate(mFixedUpdateHandleId);
        }

        #region 安全的事件注册

        /// <summary>
        /// 注册事件
        /// </summary>
        /// <param name="eventName">事件名</param>
        /// <param name="callback">事件回调</param>
        protected void EventRegister(string eventName, System.Action callback)
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

    }
}

