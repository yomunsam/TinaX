using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TinaX
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

        public void AddUpdate(Action callback, int Order = 0)
        {
            mTimeMachineBehaviour.AddUpdate(callback, Order);
        }


        public void RemoveUpdate(Action callback, int Order = 0)
        {
            mTimeMachineBehaviour.RemoveUpdate(callback, Order);
        }

        public void AddLateUpdate(Action callback, int Order = 0)
        {
            mTimeMachineBehaviour.AddLateUpdate(callback, Order);
        }

        public void RemoveLateUpdate(Action callback, int Order = 0)
        {
            mTimeMachineBehaviour.RemoveLateUpdate(callback, Order);
        }

    }
}

