

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TinaX.UIKit;
#if TinaX_CA_LuaRuntime_Enable
using XLua;
#endif
#if UNITY_EDITOR && ODIN_INSPECTOR
using UnityEditor;
using Sirenix.OdinInspector;
#endif

namespace TinaX.Lua
{
    [DisallowMultipleComponent]
#if TinaX_CA_LuaRuntime_Enable
    [AddComponentMenu("TinaX/Lua/LuaBehaviour")]
#endif
    public class LuaBehaviour : MonoBehaviour
    {
#if TinaX_CA_LuaRuntime_Enable
        #region 配置
#if UNITY_EDITOR && ODIN_INSPECTOR
        [BoxGroup("对象数据绑定")]
        [Header("Lua脚本")]
#endif
        public TextAsset LuaScript;

#if UNITY_EDITOR && ODIN_INSPECTOR
        [BoxGroup("对象数据绑定")]
        [Header("更新次序")]
#endif
        public int UpdateOrder = 0;
#if UNITY_EDITOR && ODIN_INSPECTOR
        [BoxGroup("对象数据绑定")]
        [Header("注入绑定对象")]
        [TableList]
#endif
        public Injection[] Injections;
#if UNITY_EDITOR && ODIN_INSPECTOR
        [BoxGroup("对象数据绑定")]
        [Header("注入绑定字符串")]
        [TableList]
#endif
        public Injection_String[] Injections_str;
#if UNITY_EDITOR && ODIN_INSPECTOR
        [BoxGroup("对象数据绑定")]
        [Button("绑定编辑窗口")]
        static void OpenBindEditor()
        {
            EditorWindow.GetWindow<LuaBehaviourBindEditor>().Show();
        }
#endif
        //#if UNITY_EDITOR
        //        [BoxGroup("运行模式")]
        //        [Header("独立虚拟机")]
        //        [Tooltip("慎用")]
        //#endif
        //        public bool standaloneVM = false;

#if UNITY_EDITOR && ODIN_INSPECTOR
        [BoxGroup("运行模式")]
        [Header("UI模式")]
#endif
        public bool UIBehaviour = false;



#endregion

        [HideInInspector]
        public LuaTable scriptData;

        protected System.Action luaStart;
        protected System.Action luaUpdate;
        protected System.Action luaLateUpdate;
        protected System.Action luaOnDestroy;
        protected LuaFunction luaOnUIOpenMessage;
        protected LuaFunction luaOnUICloseMessage;


        private bool lua_runed = false;
        private bool enable_update = false;
        private bool enable_lateupdate = false;
        private UIEntity mUIEntity;

#region 生命周期

        private void Awake()
        {
            if(LuaScript == null)
            {
                return;
            }
            if (!LuaManager.Inited)
            {
                return;
            }
            scriptData = LuaManager.LuaVM.NewTable();
            LuaTable meta = LuaManager.LuaVM.NewTable();
            meta.Set("__index", LuaManager.LuaVM.Global);
            scriptData.SetMetaTable(meta);
            meta.Dispose();

            scriptData.Set("self", this);
            //scriptData.Set<LuaFunction>("", EventRegister);

            if (Injections != null && Injections.Length > 0)
            {
                foreach (var item in Injections)
                {
                    if (item.Name != "" && item.Name != null && item.Object != null)
                    {
                        scriptData.Set(item.Name, item.Object);
                    }
                }
            }

            if (Injections_str != null && Injections_str.Length > 0)
            {
                foreach (var item in Injections_str)
                {
                    if (item.Name != "" && item.Name != null && item.strValue != null)
                    {
                        scriptData.Set(item.Name, item.strValue);
                    }
                }
            }

            //Debug.Log("文件名：" + LuaScript.name);
            LuaManager.LuaVM.DoString(LuaScript.text, LuaScript.name, scriptData);
            Action luaAwake = scriptData.Get<Action>("Awake");
            scriptData.Get("Start", out luaStart);
            scriptData.Get("Update", out luaUpdate);
            scriptData.Get("OnDestroy", out luaOnDestroy);
            scriptData.Get("LateUpdate", out luaLateUpdate);
            scriptData.Get("OnUIOpenMessage", out luaOnUIOpenMessage);
            scriptData.Get("OnUICloseMessage", out luaOnUICloseMessage);
            //luaOnUIOpenMessage = scriptData.Get<LuaFunction>("OnUIOpenMessage");


            if(luaAwake != null)
            {
                luaAwake();
            }
            if (luaUpdate != null)
            {
                if (luaUpdate.GetInvocationList().Length >= 1)
                {
                    enable_update = true;
                    TimeMachine.I.AddUpdate(xUpdate, UpdateOrder);
                }
            }
            if (luaLateUpdate != null)
            {
                if (luaLateUpdate.GetInvocationList().Length >= 1)
                {
                    enable_lateupdate = true;
                    TimeMachine.I.AddUpdate(xLateUpdate, UpdateOrder);
                }
            }
                

            lua_runed = true;
            mUIEntity = gameObject.GetComponent<UIEntity>();
            if(mUIEntity== null)
            {
                UIBehaviour = false;
            }
        }

        private void Start()
        {
            if(luaStart != null && lua_runed)
            {
                luaStart();
            }
        }

        private void OnDestroy()
        {
            if (lua_runed)
            {
                if(luaOnDestroy != null)
                {
                    luaOnDestroy();
                }
                luaOnDestroy = null;
                luaUpdate = null;
                luaStart = null;
                luaLateUpdate = null;
                scriptData.Dispose();
                Injections = null;
                Injections_str = null;
            }
            if (enable_update)
            {
                TimeMachine.I.RemoveUpdate(xUpdate, UpdateOrder);
            }
            if (enable_lateupdate)
            {
                TimeMachine.I.RemoveLateUpdate(xLateUpdate, UpdateOrder);
            }
            if (mEventIds.Count > 0)
            {
                foreach(var i in mEventIds)
                {
                    XEvent.Remove(i);
                }
            }
        }

        private void xUpdate()
        {
            luaUpdate();
        }

        private void xLateUpdate()
        {
            luaLateUpdate();
        }

#endregion

        public void OnUIOpenMessage(System.Object param)
        {
            if(luaOnUIOpenMessage != null && UIBehaviour)
            {
                luaOnUIOpenMessage.Call(param);
            }
        }

        

        public void OnUIOpenMessage_LuaTable(LuaTable param)
        {
            if (luaOnUIOpenMessage != null && UIBehaviour)
            {
                luaOnUIOpenMessage.Call(param);
            }
        }

        public void OnUICloseMessage(System.Object param)
        {
            if (luaOnUICloseMessage != null && UIBehaviour)
            {
                luaOnUICloseMessage.Call(param);
            }
        }

        public void OnUICloseMessage_LuaTable(LuaTable param)
        {
            if (luaOnUICloseMessage != null && UIBehaviour)
            {
                luaOnUICloseMessage.Call(param);
            }
        }

#region 安全的事件系统

        private List<int> mEventIds = new List<int>();

        public int EventRegister(string eventName,LuaFunction callback)
        {
            var id = XEvent.RegisterFromLua(eventName, callback);
            mEventIds.Add(id);
            return id;
        }


#endregion

#region UI扩展


        public RectTransform GetRectTransform()
        {
            return gameObject.GetComponent<RectTransform>();
        }

        /// <summary>
        /// 关掉自己
        /// </summary>
        public void CloseMe()
        {
            if (UIBehaviour)
            {
                mUIEntity?.Close();
            }
        }

#endregion

#region 编辑器Debug

#if UNITY_EDITOR && ODIN_INSPECTOR


        [Button("热重载")]
        public void HotOverload()
        {
            
        }


#endif
        #endregion


#endif

    }


#if TinaX_CA_LuaRuntime_Enable
    /// <summary>
    /// 注入LuaBehavior的对象
    /// </summary>
    [System.Serializable]
    public class Injection
    {

        public UnityEngine.Object Object;

        public string Name;
    }

    /// <summary>
    /// 注入LuaBehavior的文本
    /// </summary>
    [System.Serializable]
    public class Injection_String
    {
        [Header("注入变量名")]
        public string Name;
        [Header("注入值")]
        public string strValue;
    }

#endif
}



