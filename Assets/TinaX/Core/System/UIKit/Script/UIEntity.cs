using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR && ODIN_INSPECTOR
using Sirenix.OdinInspector;
using UnityEditor;
#endif

namespace TinaX.UIKits
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Canvas))]
    public class UIEntity : MonoBehaviour, IUIEntity
    {
        #region 配置


#if UNITY_EDITOR && ODIN_INSPECTOR
        [BoxGroup("Base Config")]
#endif
        [Header("主处理方式")]
        public E_MainHandleType HandleType = E_MainHandleType.UIBase;

#if UNITY_EDITOR && ODIN_INSPECTOR
        [BoxGroup("Base Config")]
#endif
        [Header("可同时打开多个")]
        public bool MultiUI = false;

#if UNITY_EDITOR && ODIN_INSPECTOR
        [FoldoutGroup("Full Screen")]
#endif
        [Header("全屏UI")]
        [Tooltip("打开全屏UI时会隐藏上一个全屏UI")]
        public bool IsFullScreenUI;

#if UNITY_EDITOR && ODIN_INSPECTOR
        [FoldoutGroup("Full Screen")]
#endif
        [Header("在隐藏时不要暂停")]
        [Tooltip("如果为true,当本UI被隐藏时不会被暂停（SetActive(false)")]
        public bool DontPauseWhenHide;


#if UNITY_EDITOR && ODIN_INSPECTOR
        [FoldoutGroup("Full Screen")]
#endif
        [Header("全屏规则忽略路径")]
        public string[] FullScreenIgnorePath;

#if UNITY_EDITOR && ODIN_INSPECTOR
        [InfoBox("需要注意仅在同一UI组且使用UI名加载时有效")]
        [FoldoutGroup("Full Screen")]
#endif
        [Header("全屏规则忽略UI名")]
        [Tooltip("需要注意仅在同一UI组且使用UI名加载时有效")]
        public string[] FullScreenIgnoreUIName;

#if UNITY_EDITOR && ODIN_INSPECTOR
        [FoldoutGroup("HandleType/Inject")]
        [ShowIfGroup("HandleType",E_MainHandleType.UIController)]
        [ShowIf("HandleType", E_MainHandleType.UIController)]
        [TableList]
#endif
        [Header("注入对象绑定")]
        public Injection[] Injections;

#if UNITY_EDITOR && ODIN_INSPECTOR
        [Button("Inject Editor",ButtonSizes.Medium)]
        [ShowIfGroup("HandleType", E_MainHandleType.UIController)]
        [ShowIf("HandleType", E_MainHandleType.UIController)]
        static void OpenBindEditor()
        {
            EditorWindow.GetWindow<UIEntityInjectEditor>().Show();
        }

        [FoldoutGroup("HandleType/Inject")]
        [ShowIfGroup("HandleType", E_MainHandleType.UIController)]
        [ShowIf("HandleType", E_MainHandleType.UIController)]
        //[TableList]
#endif
        [Header("基础数据注入绑定")]
        public Injection_Data[] Injections_Data;

        #endregion

        public string UIPath { get; internal set; }
        public string UIName { get; internal set; }
        public ulong ID { get; internal set; }

        public int LayerIndex
        {
            get
            {
                if (UICanvas == null)
                    return 0;
                else
                    return UICanvas.sortingOrder;
            }
            set
            {
                if(UICanvas != null)
                {
                    UICanvas.overrideSorting = true;
                    UICanvas.sortingOrder = value;
                }
            }
        }

        private RectTransform mRectTransform;
        public Canvas UICanvas { get; private set; }

        private List<ulong> mChildIds = new List<ulong>(); //只存储直接子级的id


        private CtrlBehaviourMaster mControllerBehaviourMaster;


        internal void AddChild(ulong child_id)
        {
            if (!mChildIds.Contains(child_id))
            {
                mChildIds.Add(child_id);
            }
        }



        internal bool EnableCloseByMaskClick { get; set; } = false; //点击遮罩能不能关闭它
        


        #region UI操作

        public void Close()
        {
            UIKit.I.CloseUI(ID);
        }



        public void Hide()
        {
            UIKit.I.HideUI(ID);
        }

        public void Show()
        {
            UIKit.I.ShowUI(ID);
        }

        public void SetUILayerTop()
        {
            UIKit.I.SetUIlayerTop(ID);
        }


        public int childCount => mChildIds.Count;

        public ulong GetChildID(int index)
        {
            if(mChildIds.Count <= index || index < 0)
            {
                return default;
            }
            else
            {
                return mChildIds[index];
            }

        }

        #endregion


        #region UI注入对象

        public Injection GetInjectObject(string Name)
        {
            foreach(var item in Injections)
            {
                if(item.Name == Name)
                {
                    return item;
                }
            }
            return null;
        }

        public Injection_Data GetInjectData(string Name)
        {
            foreach(var item in Injections_Data)
            {
                if(item.Name == Name)
                {
                    return item;
                }
            }
            return null;
        }

        #endregion


        #region 事件们


        /// <summary>
        /// 当UI创建后，由UI管理系统调用
        /// </summary>
        internal void OnUICreated()
        {
            mRectTransform = this.gameObject.GetComponent<RectTransform>();
            UICanvas = this.gameObject.GetComponent<Canvas>();

            transform.localScale = Vector3.one;

            mRectTransform.anchorMax = Vector2.one;
            mRectTransform.anchorMin = Vector2.zero;
            mRectTransform.anchoredPosition = Vector2.zero;

            UICanvas.overrideSorting = true;
            
        }

        /// <summary>
        /// UI打开的附带参数
        /// </summary>
        /// <param name="param"></param>
        internal void OnUIOpenMessage(object param)
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
            else if(HandleType == E_MainHandleType.UIController)
            {
                mControllerBehaviourMaster?.GetUIController()?.OnOpenUIMessage(param);
            }
        }

#if TinaX_CA_LuaRuntime_Enable
        /// <summary>
        /// UI打开的附带参数（lua）
        /// </summary>
        /// <param name="table"></param>
        internal void OnUIOpenMessage_LuaTable(XLua.LuaTable table)
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
        internal void OnUICloseMessage(System.Object param)
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
            else if( HandleType == E_MainHandleType.UIController)
            {
                mControllerBehaviourMaster?.GetUIController()?.OnCloseUIMessage(param);
            }
        }

#if TinaX_CA_LuaRuntime_Enable
        /// <summary>
        /// 关闭UI的附带参数 Lua table
        /// </summary>
        /// <param name="table"></param>
        internal void OnUICloseMessage_LuaTable(XLua.LuaTable table)
        {
            var luabehaviour = gameObject.GetComponent<Lua.LuaBehaviour>();
            if (luabehaviour != null)
            {
                luabehaviour.OnUICloseMessage_LuaTable(table);
            }
        }

#endif

        #endregion


        #region 内部操作

        internal void SetupUIController(UIController UI_Controller)
        {
            mControllerBehaviourMaster = this.gameObject.GetComponent<CtrlBehaviourMaster>();
            if(mControllerBehaviourMaster != null)
            {
                //这个情况在设计中是不应该出现的
                UnityEngine.Object.Destroy(mControllerBehaviourMaster);
            }

            mControllerBehaviourMaster = this.gameObject.AddComponent<CtrlBehaviourMaster>();
            mControllerBehaviourMaster.InitController(UI_Controller);
        }



        #endregion


        /// <summary>
        /// UI主处理方法类型
        /// </summary>
        public enum E_MainHandleType
        {
            UIBase,
#if TinaX_CA_LuaRuntime_Enable
            LuaBehaviour,
#endif
            UIController,
        }

        /// <summary>
        /// 可注入UI管理器的对象
        /// </summary>
        [System.Serializable]
        public class Injection
        {
            public UnityEngine.Object Object;

            public string Name;
        }

        /// <summary>
        /// 可注入UI管理器的对象
        /// </summary>
        [System.Serializable]
        public class Injection_Data
        {
            public string Name;

            public InjectDataType DataType;

#if UNITY_EDITOR && ODIN_INSPECTOR
            [ShowIf("DataType",InjectDataType.Int)]
#endif
            public int IntData;
#if UNITY_EDITOR && ODIN_INSPECTOR
            [ShowIf("DataType", InjectDataType.Text)]
#endif
            public string TextData;
#if UNITY_EDITOR && ODIN_INSPECTOR
            [ShowIf("DataType", InjectDataType.Float)]
#endif
            public float FloatData;
#if UNITY_EDITOR && ODIN_INSPECTOR
            [ShowIf("DataType", InjectDataType.Boolean)]
#endif
            public bool BoolData;

#if UNITY_EDITOR && ODIN_INSPECTOR
            [ShowIf("DataType", InjectDataType.Long)]
#endif
            public bool LongData;

#if UNITY_EDITOR && ODIN_INSPECTOR
            [ShowIf("DataType", InjectDataType.Double)]
#endif
            public bool DoubleData;







#if UNITY_EDITOR && ODIN_INSPECTOR
            [ShowIf("DataType", InjectDataType.TextArray)]
#endif
            public string[] StringArrayData;

#if UNITY_EDITOR && ODIN_INSPECTOR
            [ShowIf("DataType", InjectDataType.IntArray)]
#endif
            public int[] IntArrayData;

#if UNITY_EDITOR && ODIN_INSPECTOR
            [ShowIf("DataType", InjectDataType.FloatArray)]
#endif
            public float[] FloatArrayData;



#if UNITY_EDITOR && ODIN_INSPECTOR
            [ShowIf("DataType", InjectDataType.Color)]
            [ColorPalette]
#endif
            public UnityEngine.Color ColorData;

#if UNITY_EDITOR && ODIN_INSPECTOR
            [ShowIf("DataType", InjectDataType.Color32)]
#endif
            public UnityEngine.Color32 Color32Data;

#if UNITY_EDITOR && ODIN_INSPECTOR
            [ShowIf("DataType", InjectDataType.Vector3)]
#endif
            public UnityEngine.Vector3 Vector3Data;

#if UNITY_EDITOR && ODIN_INSPECTOR
            [ShowIf("DataType", InjectDataType.Vector3Int)]
#endif
            public UnityEngine.Vector3Int Vector3IntData;

#if UNITY_EDITOR && ODIN_INSPECTOR
            [ShowIf("DataType", InjectDataType.Vector2)]
#endif
            public UnityEngine.Vector2 Vector2Data;

#if UNITY_EDITOR && ODIN_INSPECTOR
            [ShowIf("DataType", InjectDataType.Vector2Int)]
#endif
            public UnityEngine.Vector2Int Vector2IntData;

#if UNITY_EDITOR && ODIN_INSPECTOR
            [ShowIf("DataType", InjectDataType.Quaternion)]
#endif
            public UnityEngine.Quaternion QuaternionData;

#if UNITY_EDITOR && ODIN_INSPECTOR
            [ShowIf("DataType", InjectDataType.Sprite)]
            [PreviewField(Alignment = ObjectFieldAlignment.Left,Height = 60)]
#endif
            public UnityEngine.Sprite SpriteData;


        }

        [System.Serializable]
        public enum InjectDataType
        {
            //C#
            Text,
            Int,
            Float,
            Boolean,
            Long,
            Double,

            //C#数组
            TextArray,
            IntArray,
            FloatArray,

            //Unity系列
            Color,
            Color32,
            Vector3,
            Vector3Int,
            Vector2,
            Vector2Int,
            Quaternion,
            Sprite,

            


        }

    }



}

