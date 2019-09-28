//针对CatLib的事件系统的封装,提供给业务逻辑使用
//TinaX内部使用CatLib的全局事件系统

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using CatLib;
#if TinaX_CA_LuaRuntime_Enable
using XLua;
#endif

namespace TinaX
{
    /// <summary>
    /// TinaX事件系统
    /// </summary>
    public static class XEvent
    {
        private static Dictionary<string, List<S_Event>> mEvents = new Dictionary<string, List<S_Event>>();
        private static Dictionary<ulong, S_Event> mEvent_id = new Dictionary<ulong, S_Event>();

        /// <summary>
        /// 注册事件
        /// </summary>
        /// <param name="eventName">事件名</param>
        /// <param name="callback">回调</param>
        public static ulong Register(string eventName, Action callback)
        {
            if (!mEvents.ContainsKey(eventName))
            {
                mEvents.Add(eventName, new List<S_Event>());
            }
            var event_info = new S_Event()
            {
                id = GetFreeId(),
                eventName = eventName,
                type = 1,
                callback = callback
            };
            mEvent_id.Add(event_info.id, event_info);
            mEvents[eventName].Add(event_info);
            return event_info.id;
        }

        /// <summary>
        /// 注册事件
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="callback"></param>
        public static ulong Register(string eventName, Action<System.Object> callback)
        {
            if (!mEvents.ContainsKey(eventName))
            {
                mEvents.Add(eventName, new List<S_Event>());
            }
            var event_info = new S_Event()
            {
                id = GetFreeId(),
                eventName = eventName,
                type = 3,
                callback_param = callback
            };
            mEvent_id.Add(event_info.id, event_info);
            mEvents[eventName].Add(event_info);
            return event_info.id;
        }

#if TinaX_CA_LuaRuntime_Enable

        /// <summary>
        /// 注册事件
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="callback"></param>
        public static ulong RegisterFromLua(string eventName, LuaFunction callback)
        {
            if (!mEvents.ContainsKey(eventName))
            {
                mEvents.Add(eventName, new List<S_Event>());
            }
            var event_info = new S_Event()
            {
                id = GetFreeId(),
                eventName = eventName,
                type = 2,
                callback_lua = callback
            };
            mEvent_id.Add(event_info.id, event_info);
            mEvents[eventName].Add(event_info);
            return event_info.id;
        }

#endif

        public static void Remove(ulong id)
        {
            if (mEvent_id.ContainsKey(id))
            {
                var event_Info = mEvent_id[id];
                mEvent_id.Remove(id);
                mEvents[event_Info.eventName].Remove(event_Info);
            }
        }

        public static void Call(string eventName,System.Object param = null)
        {
            if (mEvents.ContainsKey(eventName))
            {
                foreach(var item in mEvents[eventName])
                {
                    switch (item.type)
                    {
                        case 1:
                            if(item.callback != null)
                            {
                                item.callback();
                            }
                            break;
                        case 2:
#if TinaX_CA_LuaRuntime_Enable
                            if (item.callback_lua != null)
                            {
                                item.callback_lua.Call(param);
                            }
#endif
                            break;
                        case 3:
                            if (item.callback_param != null)
                            {
                                item.callback_param(param);
                            }
                            break;

                    }
                }
            }
        }

#if TinaX_CA_LuaRuntime_Enable

        public static void CallLua(string eventName, LuaTable param = null)
        {
            if (mEvents.ContainsKey(eventName))
            {
                foreach (var item in mEvents[eventName])
                {
                    switch (item.type)
                    {
                        case 1:
                            if (item.callback != null)
                            {
                                item.callback();
                            }
                            break;
                        case 2:
                            if (item.callback_lua != null)
                            {
                                item.callback_lua.Call(param);
                            }
                            break;
                        case 3:
                            if (item.callback_param != null)
                            {
                                item.callback_param(param);
                            }
                            break;

                    }
                }
            }
        }

#endif

        public struct S_Event
        {
            public ulong id;
            public string eventName;
            public int type;
            public Action callback; //type1
#if TinaX_CA_LuaRuntime_Enable
            public LuaFunction callback_lua; //type2
#endif
            public Action<System.Object> callback_param; //type3
        }

        private static ulong GetFreeId()
        {
            ulong id = (ulong)UnityEngine.Random.Range(999, 9999);
            while (mEvent_id.ContainsKey(id))
            {
                id++;
            }
            return id;
        }
    }
}
