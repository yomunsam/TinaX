using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TinaX.TimeMachines
{
    public class XTimeMachine :ITimeMachine
    {

        #region runtime

        private TimeMachineBehaviour mTimeMachineBehaviour;


        #endregion

        public XTimeMachine()
        {

            //MonoBehaviour部分
            mTimeMachineBehaviour = GameObjectHelper
                .FindOrCreateGo(Setup.Framework_Base_GameObject)
                .GetComponentOrAdd<TimeMachineBehaviour>();
        }

        /// <summary>
        /// 添加时间计划任务
        /// </summary>
        public int AddTimerPlane(long timeLater, Action callback)
        {
            return mTimeMachineBehaviour.AddTimePlane(timeLater, callback);
        }

        public void RemoveTimerPlane(int id)
        {
            mTimeMachineBehaviour.RemoveTimePlane(id);
        }

        public int AddTimePlane_Event(long timeLater, string eventName)
        {
            return mTimeMachineBehaviour.AddTimePlane_Event(timeLater, eventName);
        }

        public ulong AddUpdate(Action callback, int Order = 0)
        {
            return mTimeMachineBehaviour.AddUpdate(callback, Order);
        }


        public void RemoveUpdate(ulong handle_id)
        {
            mTimeMachineBehaviour.RemoveUpdate(handle_id);
        }

        public ulong AddLateUpdate(Action callback, int Order = 0)
        {
            return mTimeMachineBehaviour.AddLateUpdate(callback, Order);
        }

        public void RemoveLateUpdate(ulong handle_id)
        {
            mTimeMachineBehaviour.RemoveLateUpdate(handle_id);
        }

        public ulong AddFixedUpdate(Action callback, int Order = 0)
        {
            return mTimeMachineBehaviour.AddFixedUpdate(callback, Order);
        }

        public void RemoveFixedUpdate(ulong handle_id)
        {
            mTimeMachineBehaviour.RemoveFixedUpdate(handle_id);
        }

    }
}

