using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace TinaX.TimeMachines
{
    [DisallowMultipleComponent]
    public class TimeMachineBehaviour : MonoBehaviour
    {
        #region runtime_计时器

        /// <summary>
        /// 总计时队列
        /// </summary>
        private List<S_TimerQueueItem> mTimerQueue = new List<S_TimerQueueItem>();

        /// <summary>
        /// 判断时间间隔
        /// </summary>
        private float m_interval = 0;
        /// <summary>
        /// 下次队列检查时间戳
        /// </summary>
        private long next_Inspect_time = 0;

        private IDisposable mdisposable;


        #endregion

        #region 时间刷新驱动
        //Update之类的全局驱动
        private Dictionary<int, Dictionary<uint, Action>> mUpdate = new Dictionary<int, Dictionary<uint, Action>>();    //key:order ( key: handle_id , value_action)
        private Dictionary<ulong, TimeUpdates> mUpdateIdCache = new Dictionary<ulong, TimeUpdates>();
        private List<int> mUpdateOrder = new List<int>();

        private Dictionary<int, Dictionary<uint, Action>> mLateUpdate = new Dictionary<int, Dictionary<uint, Action>>();
        private Dictionary<ulong, TimeUpdates> mLateUpdateIdCache = new Dictionary<ulong, TimeUpdates>();
        private List<int> mLateUpdateOrder = new List<int>();


        private Dictionary<int, Dictionary<uint, Action>> mFixedUpdate = new Dictionary<int, Dictionary<uint, Action>>();
        private Dictionary<ulong, TimeUpdates> mFixedUpdateIdCache = new Dictionary<ulong, TimeUpdates>();
        private List<int> mFixedUpdateOrder = new List<int>();

        #endregion

        #region Public 

        /// <summary>
        /// 添加计时器计划任务
        /// </summary>
        /// <param name="timeLater"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public int AddTimePlane(long timeLater,Action callback)
        {
            var cur_time = GetTimeStamp();
            var handle_id = queue_GetUsableID();
            var item = new S_TimerQueueItem()
            {
                handle_id = handle_id,
                timeType = E_TimeType.once,
                callback = callback
            };
            //计算下次触发时间
            item.nextTiggerTime = cur_time + timeLater;
            //Debug.Log("设置到时间戳 ：" + item.nextTiggerTime);
            //加入队列
            mTimerQueue.Add(item);
            if(m_interval <= 0)
            {
                InspectQueue();
            }
            else
            {
                if(next_Inspect_time <= 0)
                {
                    InspectQueue();
                }
                else
                {
                    if(item.nextTiggerTime < next_Inspect_time)
                    {
                        InspectQueue();
                    }
                }
            }

            return handle_id;
        }

        public void RemoveTimePlane(int id)
        {
            foreach(var item in mTimerQueue)
            {
                if(item.handle_id == id)
                {
                    mTimerQueue.Remove(item);
                    break;
                }
            }
            return;
        }


        /// <summary>
        /// 添加计时器计划任务（事件广播模式）
        /// </summary>
        /// <param name="timeLater"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public int AddTimePlane_Event(long timeLater, string eventName)
        {
            var cur_time = GetTimeStamp();
            var handle_id = queue_GetUsableID();
            var item = new S_TimerQueueItem()
            {
                handle_id = handle_id,
                timeType = E_TimeType.once,
                callback = () => {
                    //XEvent.Call(eventName);
                }
            };
            //计算下次触发时间
            item.nextTiggerTime = cur_time + timeLater;
            //Debug.Log("设置到时间戳 ：" + item.nextTiggerTime);
            //加入队列
            mTimerQueue.Add(item);
            if (m_interval <= 0)
            {
                InspectQueue();
            }
            else
            {
                if (next_Inspect_time <= 0)
                {
                    InspectQueue();
                }
                else
                {
                    if (item.nextTiggerTime < next_Inspect_time)
                    {
                        InspectQueue();
                    }
                }
            }

            return handle_id;
        }

        public ulong AddUpdate(Action callback,int Order = 0)
        {
            lock (this)
            {
                if (mUpdate.ContainsKey(Order))
                {
                    //找一个空的order_id
                    uint new_order_id = 0;
                    while (mUpdate[Order].ContainsKey(new_order_id))
                    {
                        new_order_id++;
                    }
                    //加入
                    mUpdate[Order].Add(new_order_id, callback);
                    //为其分配一个handle id
                    ulong handle_id = 0;
                    while (mUpdateIdCache.ContainsKey(handle_id))
                    {
                        handle_id++;
                    }
                    //存入
                    mUpdateIdCache.Add(handle_id, new TimeUpdates()
                    {
                        id_in_order = new_order_id,
                        order = Order
                    });

                    if (!mUpdateOrder.Contains(Order))
                    {
                        mUpdateOrder.Add(Order);
                        mUpdateOrder.Sort();
                    }

                    return handle_id;
                }
                else
                {
                    mUpdate.Add(Order, new Dictionary<uint, Action>());
                    //其实这儿没必要了，但还是走一遍吧
                    uint new_order_id = 0;
                    while (mUpdate[Order].ContainsKey(new_order_id))
                    {
                        new_order_id++;
                    }
                    mUpdate[Order].Add(new_order_id, callback);
                    //为其分配一个handle id
                    ulong handle_id = 0;
                    while (mUpdateIdCache.ContainsKey(handle_id))
                    {
                        handle_id++;
                    }
                    //存入
                    mUpdateIdCache.Add(handle_id, new TimeUpdates()
                    {
                        id_in_order = new_order_id,
                        order = Order
                    });

                    if (!mUpdateOrder.Contains(Order))
                    {
                        mUpdateOrder.Add(Order);
                        mUpdateOrder.Sort();
                    }

                    return handle_id;
                }
            }

            
            //if (mUpdate.ContainsKey(Order))
            //{
            //    mUpdate[Order] += callback;
            //}
            //else
            //{
            //    mUpdate.Add(Order, new Action(()=> { }));
            //    mUpdate[Order] += callback;
            //}
            //if (!mUpdateOrder.Contains(Order))
            //{
            //    mUpdateOrder.Add(Order);
            //}
            //mUpdateOrder.Sort();
        }

        public void RemoveUpdate(ulong handleId)
        {
            lock (this)
            {
                //根据HandleId找Order和order中对应的id
                if (mUpdateIdCache.TryGetValue(handleId, out var updateInfo))
                {
                    if (mUpdate.ContainsKey(updateInfo.order))
                    {
                        mUpdate[updateInfo.order].Remove(updateInfo.id_in_order);
                        if(mUpdate[updateInfo.order].Count <= 0)
                        {
                            mUpdate.Remove(updateInfo.order);
                            if (mUpdateOrder.Contains(updateInfo.order))
                            {
                                mUpdateOrder.Remove(updateInfo.order);
                                mUpdateOrder.Sort();
                            }
                        }

                    }
                    mUpdateIdCache.Remove(handleId);
                }
            }

            ////Debug.Log("移除：调用了");
            //if (mUpdate.ContainsKey(Order))
            //{
            //    //Debug.Log(mUpdate[Order].GetInvocationList().Length);
            //    mUpdate[Order] -= callback;
            //}
            //if (mUpdate[Order].GetInvocationList().Length <= 1)
            //{
            //    mUpdate.Remove(Order);
            //    mUpdateOrder.Remove(Order);
            //}
        }

        public ulong AddLateUpdate(Action callback, int Order = 0)
        {
            lock (this)
            {
                if (!mLateUpdate.ContainsKey(Order))
                {
                    mLateUpdate.Add(Order, new Dictionary<uint, Action>());
                }

                //找一个空的order_id
                uint new_order_id = 0;
                while (mLateUpdate[Order].ContainsKey(new_order_id))
                {
                    new_order_id++;
                }
                //加入
                mLateUpdate[Order].Add(new_order_id, callback);
                //登记handle_id
                ulong handle_id = 0;
                while (mLateUpdateIdCache.ContainsKey(handle_id))
                {
                    handle_id++;
                }
                mLateUpdateIdCache.Add(handle_id, new TimeUpdates() { order = Order, id_in_order = new_order_id });

                if (!mLateUpdateOrder.Contains(Order))
                {
                    mLateUpdateOrder.Add(Order);
                    mLateUpdateOrder.Sort();
                }

                return handle_id;
            }
        }

        public void RemoveLateUpdate(ulong handle_id)
        {
            lock (this)
            {
                if(mLateUpdateIdCache.TryGetValue(handle_id,out var uInfo))
                {
                    if (mLateUpdate.ContainsKey(uInfo.order))
                    {
                        mLateUpdate[uInfo.order].Remove(uInfo.id_in_order);
                        if(mLateUpdate[uInfo.order].Count <= 0)
                        {
                            mLateUpdate.Remove(uInfo.order);
                            mLateUpdateOrder.Remove(uInfo.order);
                            mLateUpdateOrder.Sort();
                        }
                    }
                    mLateUpdateIdCache.Remove(handle_id);
                }
                
            }
        }

        public ulong AddFixedUpdate(Action callback, int Order = 0)
        {
            lock (this)
            {
                if (!mFixedUpdate.ContainsKey(Order))
                {
                    mFixedUpdate.Add(Order, new Dictionary<uint, Action>());
                }

                //找一个空的order_id
                uint new_order_id = 0;
                while (mFixedUpdate[Order].ContainsKey(new_order_id))
                {
                    new_order_id++;
                }
                //加入
                mFixedUpdate[Order].Add(new_order_id, callback);
                //登记handle_id
                ulong handle_id = 0;
                while (mFixedUpdateIdCache.ContainsKey(handle_id))
                {
                    handle_id++;
                }
                mFixedUpdateIdCache.Add(handle_id, new TimeUpdates() { order = Order, id_in_order = new_order_id });

                if (!mFixedUpdateOrder.Contains(Order))
                {
                    mFixedUpdateOrder.Add(Order);
                    mFixedUpdateOrder.Sort();
                }

                return handle_id;
            }
        }

        public void RemoveFixedUpdate(ulong handle_id)
        {
            lock (this)
            {
                if (mFixedUpdateIdCache.TryGetValue(handle_id, out var uInfo))
                {
                    if (mFixedUpdate.ContainsKey(uInfo.order))
                    {
                        mFixedUpdate[uInfo.order].Remove(uInfo.id_in_order);
                        if (mFixedUpdate[uInfo.order].Count <= 0)
                        {
                            mFixedUpdate.Remove(uInfo.order);
                            mFixedUpdateOrder.Remove(uInfo.order);
                            mFixedUpdateOrder.Sort();
                        }
                    }
                    mFixedUpdateIdCache.Remove(handle_id);
                }

            }
        }

        #endregion


        #region Update

        private void Update()
        {
            foreach(var item in mUpdateOrder)
            {
                if (mUpdate.ContainsKey(item))
                {
                    foreach(var a in mUpdate[item])
                    {
                        a.Value?.Invoke();
                    }
                }
            }
        }

        private void LateUpdate()
        {
            foreach (var item in mLateUpdateOrder)
            {
                if (mLateUpdate.ContainsKey(item))
                {
                    foreach(var a in mLateUpdate[item])
                    {
                        a.Value?.Invoke();
                    }
                }
            }
        }

        private void FixedUpdate()
        {
            foreach (var item in mFixedUpdateOrder)
            {
                if (mFixedUpdate.ContainsKey(item))
                {
                    foreach (var a in mFixedUpdate[item])
                    {
                        a.Value?.Invoke();
                    }
                }
            }
        }

        #endregion

        /// <summary>
        /// 检查队列内容(单次)
        /// </summary>
        private void InspectQueue()
        {
            if(mdisposable != null)
            {
                mdisposable.Dispose();
            }
            
            //Debug.Log("一次队列判断");
            var cur_time = GetTimeStamp(); //当前时间
            //Debug.Log("当前时间:" + cur_time) ;
            if (mTimerQueue.Count <= 0)
            {
                //停掉计时器
                m_interval = 0;
            }
            else
            {
                //找到距离当前时间最近的一个
                S_TimerQueueItem near_item = new S_TimerQueueItem()
                {
                    nextTiggerTime = -1
                };
                for(int i = mTimerQueue.Count -1; i >=0; i--)
                {
                    var v = mTimerQueue[i];
                    if(v.nextTiggerTime <= cur_time)
                    {
                        //触发它，然后重新编排队列
                        v.callback();
                        if (v.timeType == E_TimeType.once)
                        {
                            //删掉
                            mTimerQueue.RemoveAt(i);
                        }
                        else
                        {
                            //TODO
                        }
                    }
                    else
                    {
                        if(near_item.nextTiggerTime == -1)
                        {
                            near_item = v;
                        }
                        else if(v.nextTiggerTime < near_item.nextTiggerTime)
                        {
                            near_item = v;
                        }
                    }
                }
                if (mTimerQueue.Count <= 0)
                {
                    m_interval = 0;
                }
                else
                {
                    long interval = near_item.nextTiggerTime - cur_time;
                    m_interval = interval;

                    //Debug.Log("interval:" + interval);
                    next_Inspect_time = cur_time + interval;
                    //根据时间间隔，设定下次调用
                    mdisposable = Observable.Timer(new TimeSpan(0, 0, (int)interval)).Subscribe(_ => InspectQueue());
                }

            }
        }




        #region 计时队列管理

        /// <summary>
        /// 队列中是否有这个ID
        /// </summary>
        private bool queue_IsIdExist(int id)
        {
            foreach(var item in mTimerQueue)
            {
                if (item.handle_id == id)
                {
                    return true;
                }
            }
            return false;
        }


        /// <summary>
        /// 获取一个可用的队列ID
        /// </summary>
        private int queue_GetUsableID()
        {
            int id = UnityEngine.Random.Range(100, 10000);
            while (queue_IsIdExist(id))
            {
                id++;
            }
            return id;
        }


        #endregion

        private static long GetTimeStamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return long.Parse(Convert.ToInt64(ts.TotalSeconds).ToString());
        }

        /// <summary>
        /// 计时器排队队列成员信息
        /// </summary>
        public struct S_TimerQueueItem
        {
            /// <summary>
            /// 计时成员句柄ID
            /// </summary>
            public int handle_id;

            /// <summary>
            /// 计时类型
            /// </summary>
            public E_TimeType timeType;
            /// <summary>
            /// 下次触发时间
            /// </summary>
            public long nextTiggerTime;

            /// <summary>
            /// 间隔类型（如果是每隔一段时间执行）
            /// </summary>
            public E_IntervalType intervalType;
            /// <summary>
            /// 间隔量
            /// </summary>
            public float intervalValue;

            public Action callback;
        }

        /// <summary>
        /// 计时器 计时类型
        /// </summary>
        public enum E_TimeType
        {
            /// <summary>
            /// 一次性即时（时间戳）
            /// </summary>
            once,
            /// <summary>
            /// 每隔一段时间
            /// </summary>
            every,
        }

        /// <summary>
        /// 计时器间隔类型
        /// </summary>
        public enum E_IntervalType
        {

        }


        public struct TimeUpdates
        {
            public uint id_in_order;
            public int order;
        }

    }
}

