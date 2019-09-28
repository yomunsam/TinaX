using System;
//using UnityEngine;


namespace TinaX.TimeMachines
{
    /// <summary>
    /// 时间机
    /// </summary>
    public interface ITimeMachine
    {
        /// <summary>
        /// 添加定时计划任务
        /// </summary>
        /// <param name="timeLater">延迟时间（秒）</param>
        /// <param name="callback">时间回调</param>
        /// <returns>任务句柄</returns>
        int AddTimerPlane(long timeLater, Action callback);

        /// <summary>
        /// 添加定时计划任务 广播方式
        /// </summary>
        /// <param name="timeLater">延迟时间（秒）</param>
        /// <param name="eventName">事件名 字符</param>
        /// <returns>任务句柄</returns>
        int AddTimePlane_Event(long timeLater, string eventName);

        /// <summary>
        /// 移除定时计划任务
        /// </summary>
        /// <param name="id">注册时获得的ID</param>
        void RemoveTimerPlane(int id);

        /// <summary>
        /// 登记全局Update事件管理
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="Order">调用顺序，值越大，调用顺序越靠后</param>
        /// <returns>handle id</returns>
        ulong AddUpdate(Action callback, int Order = 0);

        /// <summary>
        /// 移除全局Update事件管理
        /// </summary>
        /// <param name="handle_id">handle_id | 登记时分配的句柄id</param>
        void RemoveUpdate(ulong handle_id);

        /// <summary>
        /// 登记全局LateUpdate事件管理
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="Order">调用顺序，值越大，调用顺序越靠后</param>
        /// <returns>handle id</returns>
        ulong AddLateUpdate(Action callback, int Order = 0);

        /// <summary>
        /// 移除全局LateUpdate事件管理
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="Order">登记时用的值</param>
        void RemoveLateUpdate(ulong handle_id);


        ulong AddFixedUpdate(Action callback, int Order = 0);

        void RemoveFixedUpdate(ulong handle_id);

    }
}

