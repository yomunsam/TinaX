using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR && ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace TinaX.UIKit
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Canvas))]
    public class UIEntity : MonoBehaviour, IUIEntity
    {
        #region 配置


#if UNITY_EDITOR && ODIN_INSPECTOR
        private void OnHandleTypeChanged()
        {
            print("喵");
            if (!Application.isPlaying)
            {
#if TinaX_CA_LuaRuntime_Enable
                if (HandleType == E_MainHandleType.LuaBehaviour)
                {
                    gameObject.GetComponentOrAdd<Lua.LuaBehaviour>().UIBehaviour = true;
                }
#endif
            }
        }
        [Header("主处理方式")]
        [OnValueChanged("OnHandleTypeChanged",true)]
#endif
        public E_MainHandleType HandleType = E_MainHandleType.UIBase;

        [Header("全屏UI")]
        [Tooltip("打开全屏UI时会隐藏上一个全屏UI")]
        public bool IsFullScreenUI;

        [Header("在隐藏时不要暂停")]
        [Tooltip("如果为true,当本UI被隐藏时不会被暂停（SetActive(false)")]
        public bool DontPauseWhenHide;

#if UNITY_EDITOR && ODIN_INSPECTOR
        [FoldoutGroup("高级模式")]
        [Header("可同时打开多个")]
        [InfoBox("如果勾选，在高级模式下可同时打开多个该UI")]
#endif
        public bool MultiUI = false;

        #endregion

        private Canvas mCanvas;
        private RectTransform rectTrans;
        private string m_uiPath;
        private string m_uiName;

        #region 高级模式
        
        private int m_ID;   //UI句柄ID
        private UIEntity m_Parent;  //父UI
        private List<UIEntity> m_childs = new List<UIEntity>() ;    //子UI

        #endregion

        /// <summary>
        /// [高级模式]分配句柄ID
        /// </summary>
        /// <returns></returns>
        public void SetID(int id)
        {
            m_ID = id;
        }
        /// <summary>
        /// [高级模式]获取句柄ID
        /// </summary>
        /// <returns></returns>
        public int GetID()
        {
            return m_ID;
        }

        public void AddChild(UIEntity child)
        {
            
            m_childs.Add(child);
        }


        public int LayerIndex
        {
            get
            {
                return mCanvas.sortingOrder;
            }
        }


        public int SetLayer(int layer)
        {
            mCanvas.sortingOrder = layer;
            return mCanvas.sortingOrder;
        }


        public GameObject GetGameObject()
        {
            return gameObject;
        }

        public string GetUIName()
        {
            return m_uiName;
        }

        public string GetUIPath()
        {
            return m_uiPath;
        }


        #region UI操作

        /// <summary>
        /// 隐藏本UI
        /// </summary>
        /// <returns></returns>
        public IUIEntity Hide()
        {
            if (!UIKit.I.IsAdvanced)
            {
                UIKit.I.HideUIByPath(m_uiPath);
            }
            else
            {
                UIKit.I.HideUI(m_ID);
            }
            return this;
        }

        public IUIEntity Show()
        {
            if (!UIKit.I.IsAdvanced)
            {
                UIKit.I.ShowUIByPath(m_uiPath);
            }
            else
            {
                UIKit.I.ShowUI(m_ID);
            }
            return this;
        }

        public void Close()
        {
            if (!UIKit.I.IsAdvanced)
            {
                UIKit.I.CloseUIByPath(m_uiPath);
            }
            else
            {
                UIKit.I.CloseUI(m_ID);
            }
            
        }


        #endregion


        #region 高级模式的UI操作

        /// <summary>
        /// 打开UI,在高级模式下，被打开的UI被认为是当前UI的子级UI
        /// </summary>
        /// <param name="ui_name">UI名</param>
        /// <param name="Param">附加参数</param>
        public IUIEntity OpenUI(string ui_name,System.Object Param = null)
        {
            if (UIKit.I.IsAdvanced)
            {
#if TinaX_CA_LuaRuntime_Enable
                return UIKit.I.OpenUIChild(ui_name, m_ID, Param, null, false, false);
#else
                return UIKit.I.OpenUIChild(ui_name, m_ID, Param, false, false);

#endif
            }
            else
            {
                return UIKit.I.OpenUI(ui_name, Param);
            }
        }

#endregion



#region 事件们


        /// <summary>
        /// 当UI创建后，由UI管理系统调用
        /// </summary>
        public void OnUICreated(string ui_path, string ui_name = "unknow")
        {
            mCanvas = gameObject.GetComponent<Canvas>();
            rectTrans = gameObject.GetComponent<RectTransform>();

            transform.localScale = Vector3.one;

            rectTrans.anchorMax = Vector2.one;
            rectTrans.anchorMin = Vector2.zero;
            rectTrans.anchoredPosition = Vector2.zero;

            mCanvas.overrideSorting = true;
            m_uiPath = ui_path;
            m_uiName = ui_name;

        }

        /// <summary>
        /// UI打开的附带参数
        /// </summary>
        /// <param name="param"></param>
        public void OnUIOpenMessage(System.Object param)
        {
            if (HandleType == E_MainHandleType.UIBase)
            {
                var uiBase = gameObject.GetComponent<UIBase>();
                if (uiBase != null)
                {
                    uiBase.OnOpenUIMessage(param);
                }
            }
#if TinaX_CA_LuaRuntime_Enable
            else if (HandleType == E_MainHandleType.LuaBehaviour)
            {
                var luabehaviour = gameObject.GetComponent<Lua.LuaBehaviour>();
                if (luabehaviour != null)
                {
                    luabehaviour.OnUIOpenMessage(param);
                }
            }
#endif
        }

#if TinaX_CA_LuaRuntime_Enable
        /// <summary>
        /// UI打开的附带参数（lua）
        /// </summary>
        /// <param name="table"></param>
        public void OnUIOpenMessage_LuaTable(XLua.LuaTable table)
        {
            var luabehaviour = gameObject.GetComponent<Lua.LuaBehaviour>();
            if (luabehaviour != null)
            {
                luabehaviour.OnUIOpenMessage_LuaTable(table);
            }
        }
#endif

        /// <summary>
        /// 关闭UI的附带参数
        /// </summary>
        /// <param name="param"></param>
        public void OnUICloseMessage(System.Object param)
        {
            if (HandleType == E_MainHandleType.UIBase)
            {
                var uiBase = gameObject.GetComponent<UIBase>();
                if (uiBase != null)
                {
                    uiBase.OnCloseUIMessage(param);
                }
            }
#if TinaX_CA_LuaRuntime_Enable
            else if (HandleType == E_MainHandleType.LuaBehaviour)
            {
                var luabehaviour = gameObject.GetComponent<Lua.LuaBehaviour>();
                if (luabehaviour != null)
                {
                    luabehaviour.OnUICloseMessage(param);
                }
            }
#endif
        }

#if TinaX_CA_LuaRuntime_Enable
        /// <summary>
        /// 关闭UI的附带参数 Lua table
        /// </summary>
        /// <param name="table"></param>
        public void OnUICloseMessage_LuaTable(XLua.LuaTable table)
        {
            var luabehaviour = gameObject.GetComponent<Lua.LuaBehaviour>();
            if (luabehaviour != null)
            {
                luabehaviour.OnUICloseMessage_LuaTable(table);
            }
        }

#endif

#endregion


        /// <summary>
        /// UI主处理方法类型
        /// </summary>
        public enum E_MainHandleType
        {
            UIBase,
#if TinaX_CA_LuaRuntime_Enable
            LuaBehaviour
#endif
        }

    }



}

