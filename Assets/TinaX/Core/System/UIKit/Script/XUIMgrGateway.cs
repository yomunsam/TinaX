using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using TinaX.Conf;
using TinaX;

namespace TinaX.UIKit
{
    /// <summary>
    /// UI管理器 入口
    /// </summary>
    public class XUIMgrGateway:IUIKit
    {
        /// <summary>
        /// 管理器模式; 0: 简单模式      1：高级模式
        /// </summary>
        private int m_ManagerMode = 0;

        private XUIManager m_UIMgr_Normal;  //简单模式，UI管理器
        private XUIManagerAdv m_UIMgr_Adv;  //高级模式，UI管理器
        private UIMaskMgr m_UIMaskMgr;      //UI遮罩管理
        private UILayerMgr m_UILayerMgr;    //UI层级管理器
        private Canvas mUIRootCanvas;   //UIRoot Canvas
        private Transform mUIRootTrans;
        private RectTransform mUIRootRectTrans;
        private Camera mUICamera;
        private GameObject mUICameraGameObject;
        

        private UIKitConfig mConfig;
        private UIGroupConf mDefaultUIGroup;

        private IVFS mVFS;

        #region UISafeArea

        private XUISafeAreaMgr mUISafeAreaMgr;

        #endregion

        /// <summary>
        /// 初始化
        /// </summary>
        public XUIMgrGateway(IVFS _vfs)
        {
            mVFS = _vfs;
            mConfig = TinaX.Config.GetTinaXConfig<UIKitConfig>(ConfigPath.uikit);
            if (mConfig == null)
            {
                mConfig = new UIKitConfig();
                mConfig.Use_AdvancedMode = false;
            }

            mDefaultUIGroup = mConfig.Default_UIGroup;

            //初始化GameObject相关
            Init_GameObjects();
            //初始化UIMask
            m_UIMaskMgr = null;
            m_UIMaskMgr = new UIMaskMgr(mUIRootTrans,mConfig.MaskColor,this);
            m_UILayerMgr = null;
            m_UILayerMgr = new UILayerMgr(this);

            if (mConfig.Use_AdvancedMode)
            {
                //高级模式
                m_ManagerMode = 1;
                m_UIMgr_Adv = null;
                m_UIMgr_Adv = new XUIManagerAdv(this,mVFS);
            }
            else
            {
                //简单模式
                m_ManagerMode = 0;
                m_UIMgr_Normal = null;
                m_UIMgr_Normal = new XUIManager(this,mVFS);
            }

            #region UISafeArea

            if (mConfig.Enable_UISafeArea)
            {
                mUISafeAreaMgr = new XUISafeAreaMgr()
                    .Enable();
                
            }

            #endregion

        }

        ~XUIMgrGateway()
        {
            if(mUISafeAreaMgr != null)
            {
                mUISafeAreaMgr.Disable();
                mUISafeAreaMgr = null;
            }
        }


        private void Init_GameObjects()
        {
            var TinaXRoot = GameObjectHelper.FindOrCreateGo(Setup.Framework_Base_GameObject);
            var Go_UIKit = TinaXRoot.FindOrCreateGo(UIKitConst.UIKit_RootGameObject_Name);
            var Go_UIKit_Root = Go_UIKit.FindOrCreateGo(UIKitConst.UIKit_UIRootGameObject_Name);
            var Go_UIKit_Camera = Go_UIKit.FindOrCreateGo(UIKitConst.UIKit_UICameraGameObject_Name);

            #region UICamera_Create

            var camera = Go_UIKit_Camera.GetComponentOrAdd<Camera>();
            camera.clearFlags = CameraClearFlags.Depth;
            camera.cullingMask = 1 << 5;
            camera.orthographic = true;
            camera.depth = 99;
            camera.allowHDR = false;
            camera.allowMSAA = false;

            mUICameraGameObject = Go_UIKit_Camera;
            mUICamera = camera;
            #endregion


            #region UIRoot_Create
            Go_UIKit_Root.layer = 5;

            var canvas = Go_UIKit_Root.GetComponentOrAdd<Canvas>();
            mUIRootCanvas = canvas;
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = camera;
            canvas.pixelPerfect = mConfig.PixelPerfect;

            var _canvas_scaler = Go_UIKit_Root.GetComponentOrAdd<CanvasScaler>();
            _canvas_scaler.uiScaleMode = mConfig.Canvas_Scale_Mode;
            _canvas_scaler.referenceResolution = mConfig.UISize;
            _canvas_scaler.screenMatchMode = mConfig.ScreenMatchMode;
            _canvas_scaler.matchWidthOrHeight = mConfig.Match;

            _canvas_scaler.scaleFactor = mConfig.ScaleFactor;
            _canvas_scaler.referencePixelsPerUnit = mConfig.referencePixelsPerUnit;
            _canvas_scaler.physicalUnit = mConfig.physicalUnit;


            var _gr = Go_UIKit_Root.GetComponentOrAdd<GraphicRaycaster>();
            _gr.ignoreReversedGraphics = true;

            #endregion


            mUIRootTrans = Go_UIKit_Root.transform;
            mUIRootRectTrans = Go_UIKit_Root.GetComponent<RectTransform>();

            #region EventSystem
            var ESGo = GameObjectHelper.FindOrCreateGo("EventSystem");
            var esObj = ESGo.GetComponentOrAdd<UnityEngine.EventSystems.EventSystem>();
            esObj.sendNavigationEvents = true;
            esObj.pixelDragThreshold = 10;
            var sim = ESGo.GetComponentOrAdd<UnityEngine.EventSystems.StandaloneInputModule>();
            ESGo.DontDestroy();

            #endregion

        }



        /// <summary>
        /// UI管理器是否为高级模式
        /// </summary>
        public bool IsAdvanced
        {
            get
            {
                return m_ManagerMode == 1;
            }
        }

        public UIKitConfig UIKit_Config
        {
            get
            {
                return mConfig;
            }
        }

        public UILayerMgr UIKit_LayerMgr
        {
            get
            {
                return m_UILayerMgr;
            }
        }

        public Transform UIKit_UIRoot_Trans
        {
            get
            {
                //if (mUIRootTrans == null)
                //{
                //    Debug.Log("诶呀");
                //}
                return mUIRootTrans;
            }
        }

        public UIGroupConf UIKit_DefaultUIGroup
        {
            get
            {
                return mDefaultUIGroup;
            }
        }

        public UIMaskMgr UIKit_MaskMgr
        {
            get
            {
                return m_UIMaskMgr;
            }
        }


        public RectTransform UIKit_UIRoot_RectTrans
        {
            get
            {
                return mUIRootRectTrans;
            }
        }

        public GameObject UIKit_UICamera_GameObject
        {
            get
            {
                return mUICameraGameObject;
            }
        }

        public Camera UIKit_UICamera
        {
            get
            {
                return mUICamera;
            }
        }



        public XUISafeAreaMgr UISafeAreaManager
        {
            get
            {
                return mUISafeAreaMgr;
            }
        }


        public IUIEntity OpenUI(string ui_name)
        {
            return this.OpenUI(ui_name, null);
        }


        public IUIEntity OpenUI(string ui_name, System.Object ui_param)
        {
            if (IsAdvanced)
            {
                //高级模式
                return m_UIMgr_Adv.OpenUI(ui_name, false, false,OpenParam:ui_param);
            }
            else
            {
                return m_UIMgr_Normal.OpenUI(ui_name, false, false,OpenParam:ui_param);
            }
        }

        /// <summary>
        /// 打开UI
        /// </summary>
        public IUIEntity OpenUI(string ui_name,bool use_mask, bool close_by_mask)
        {
            if (IsAdvanced)
            {
                //高级模式
                return m_UIMgr_Adv.OpenUI(ui_name,use_mask,close_by_mask);
            }
            else
            {
                return m_UIMgr_Normal.OpenUI(ui_name, use_mask, close_by_mask);
            }
        }

        public IUIEntity OpenUI(string ui_name, System.Object ui_param, bool use_mask, bool close_by_mask = false)
        {
            if (IsAdvanced)
            {
                //高级模式
                return m_UIMgr_Adv.OpenUI(ui_name, use_mask, close_by_mask,OpenParam:ui_param);
            }
            else
            {
                return m_UIMgr_Normal.OpenUI(ui_name, use_mask, close_by_mask, OpenParam: ui_param);
            }
        }

#if TinaX_CA_LuaRuntime_Enable
        
        //Lua层会有封装，所以这里传入的参数可以搞的很多，不用考虑用户体验

        [XLua.LuaCallCSharp]
        public IUIEntity OpenUIWhitLuaParam(string ui_name, XLua.LuaTable lua_param, bool use_mask, bool close_by_mask)   
        {
            if (IsAdvanced)
            {
                //高级模式
                return m_UIMgr_Adv.OpenUI(ui_name, use_mask, close_by_mask, LuaParam:lua_param);
            }
            else
            {
                return m_UIMgr_Normal.OpenUI(ui_name, use_mask, close_by_mask, LuaParam: lua_param);
            }
        }
#endif

        //用Path打开，目前版本是不推荐的，只是遵循传统留下这么个接口，不太考虑用户体验的，所有参数都堆过来呗
        public IUIEntity OpenUIByPath(string ui_path, System.Object ui_param, bool use_mask, bool close_by_mask = false)
        {
            if (IsAdvanced)
            {
                //高级模式
                return m_UIMgr_Adv.OpenUIByPath(ui_path, use_mask, close_by_mask,OpenParam: ui_param);
            }
            else
            {
                return m_UIMgr_Normal.OpenUIByPath(ui_path, use_mask, close_by_mask, OpenParam: ui_param);
            }
        }

#if TinaX_CA_LuaRuntime_Enable
        public IUIEntity OpenUIByPathWhitLuaParam(string ui_path, XLua.LuaTable lua_param, bool use_mask, bool close_by_mask = false)
        {
            if (IsAdvanced)
            {
                //高级模式
                return m_UIMgr_Adv.OpenUIByPath(ui_path, use_mask, close_by_mask, LuaParam: lua_param);
            }
            else
            {
                return m_UIMgr_Normal.OpenUIByPath(ui_path, use_mask, close_by_mask, LuaParam: lua_param);
            }
        }

#endif

#if TinaX_CA_LuaRuntime_Enable
        public IUIEntity OpenUIChild(string ui_name,int parent_id, System.Object ui_param, XLua.LuaTable lua_param, bool use_mask, bool close_by_mask = false)
        {
            if (IsAdvanced)
            {
                return m_UIMgr_Adv.OpenUI(ui_name, use_mask, close_by_mask, parent_id, ui_param, lua_param);
            }
            else
            {
                XLog.PrintW("[TinaX][UIKit]当前UI系统不是高级模式，但被尝试用高级模式父子级UI功能");
                return null;
            }
        }

#else
        public IUIEntity OpenUIChild(string ui_name, int parent_id, System.Object ui_param, bool use_mask, bool close_by_mask = false)
        {
            if (IsAdvanced)
            {
                return m_UIMgr_Adv.OpenUI(ui_name, use_mask, close_by_mask, parent_id, ui_param);
            }
            else
            {
                XLog.PrintW("[TinaX][UIKit]当前UI系统不是高级模式，但被尝试用高级模式父子级UI功能");
                return null;
            }
        }
#endif

        public void CloseUI(string ui_name,System.Object Param = null)
        {
            if (IsAdvanced)
            {
                //高级模式下，不可用UI名关闭UI
                XLog.PrintW("[TinaX][UIKit]当前UI系统为高级模式，不可使用UI名称关闭UI, 请使用UI实例的句柄ID");
                return;
            }
            else
            {
                m_UIMgr_Normal.CloseUI(ui_name, close_param: Param);
            }
        }

        public void CloseUI(int ui_id, System.Object Param = null)
        {
            if (IsAdvanced)
            {
                m_UIMgr_Adv.CloseUI(ui_id,close_param:Param);
            }
            else
            {
                XLog.PrintW("[TinaX][UIKit]当前UI系统不是高级模式，但被尝试用高级模式的 ID 形式关闭UI");
                return;
            }
        }

        public void CloseUIByPath(string ui_path, System.Object Param = null)
        {
            if (IsAdvanced)
            {
                //高级模式下，不可用UI名关闭UI
                XLog.PrintW("[TinaX][UIKit]当前UI系统为高级模式，不可使用UI路径关闭UI, 请使用UI实例的句柄ID");
                return;
            }
            else
            {
                m_UIMgr_Normal.CloseUIByPath(ui_path, close_param: Param);
            }
        }

#if TinaX_CA_LuaRuntime_Enable
        public void CloseUIWithLuaParam(string ui_name,XLua.LuaTable luaParam)
        {
            if (IsAdvanced)
            {
                //高级模式下，不可用UI名关闭UI
                XLog.PrintW("[TinaX][UIKit]当前UI系统为高级模式，不可使用UI名称操作UI, 请使用UI实例的句柄ID");
                return;
            }
            else
            {
                m_UIMgr_Normal.CloseUI(ui_name, LuaParam: luaParam);
            }
        }



        public void CloseUIByPathWithLuaParam(string ui_path, XLua.LuaTable luaParam)
        {
            if (IsAdvanced)
            {
                //高级模式下，不可用UI名关闭UI
                XLog.PrintW("[TinaX][UIKit]当前UI系统为高级模式，不可使用UI名称操作UI, 请使用UI实例的句柄ID");
                return;
            }
            else
            {
                m_UIMgr_Normal.CloseUIByPath(ui_path, LuaParam: luaParam);
            }
        }

        public void CloseUIWithLuaParam(int ui_id, XLua.LuaTable luaParam)
        {
            if (IsAdvanced)
            {
                m_UIMgr_Adv.CloseUI(ui_id, LuaParam:luaParam);
            }
            else
            {
                XLog.PrintW("[TinaX][UIKit]当前UI系统不是高级模式，但被尝试用高级模式的 ID 形式操作UI");
                return;
            }
        }

#endif

        public void HideUI(string ui_name)
        {
            if (IsAdvanced)
            {
                XLog.PrintW("[TinaX][UIKit]当前UI系统为高级模式，不可使用UI名称操作UI, 请使用UI实例的句柄ID");
                return;
            }
            else
            {
                m_UIMgr_Normal.HideUI(ui_name);
            }
        }

        public void HideUIByPath(string ui_path)
        {
            if (IsAdvanced)
            {
                XLog.PrintW("[TinaX][UIKit]当前UI系统为高级模式，不可使用UI路径操作UI, 请使用UI实例的句柄ID");
                return;
            }
            else
            {
                m_UIMgr_Normal.HideUIByPath(ui_path);
            }
        }


        public void HideUI(int ui_id)
        {
            if (IsAdvanced)
            {
                m_UIMgr_Adv.HideUI(ui_id);
            }
            else
            {
                XLog.PrintW("[TinaX][UIKit]仅在高级模式下，可使用句柄ID操作UI");
                return;
            }
        }


        public void ShowUI(string ui_name)
        {
            if (IsAdvanced)
            {
                XLog.PrintW("[TinaX][UIKit]当前UI系统为高级模式，不可使用UI名称操作UI, 请使用UI实例的句柄ID");
                return;
            }
            else
            {
                m_UIMgr_Normal.ShowUI(ui_name);
            }
        }

        public void ShowUIByPath(string ui_path)
        {
            if (IsAdvanced)
            {
                XLog.PrintW("[TinaX][UIKit]当前UI系统为高级模式，不可使用UI路径操作UI, 请使用UI实例的句柄ID");
                return;
            }
            else
            {
                m_UIMgr_Normal.ShowUIByPath(ui_path);
            }
        }


        public void ShowUI(int ui_id)
        {
            if (IsAdvanced)
            {
                m_UIMgr_Adv.ShowUI(ui_id);
            }
            else
            {
                XLog.PrintW("[TinaX][UIKit]仅在高级模式下，可使用句柄ID操作UI");
                return;
            }
        }

        public void SetUIGroup(UIGroupConf ui_group)
        {
            mDefaultUIGroup = ui_group;
        }

        /// <summary>
        /// 置顶UI
        /// </summary>
        public void SetUILayerTop(UIEntity entity)
        {
            m_UILayerMgr.SetUILayerTop(entity);
        }


        public Vector2 GetUIScreenLocalPoint(Transform trans)
        {
            Vector2 point;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                mUIRootRectTrans,
                mUICamera.WorldToScreenPoint(trans.position),
                mUICamera,
                out point))
            {
                return point;
            }
            else
            {
                return Vector2.zero;
            }
        }

    }


    /// <summary>
    /// UI窗口下方的遮罩层
    /// </summary>
    public class UIMaskMgr
    {
        private GameObject MaskGo;
        private Canvas MaskCanvas;
        private XUIMgrGateway mGateway;

        /// <summary>
        /// 记录所有需要Mask的UI对象
        /// </summary>
        private List<UIItem> UIMaskList = new List<UIItem>();
        private UIItem cur_MaskItem;

        /// <summary>
        /// 初始化
        /// </summary>
        public UIMaskMgr(Transform TransUIRoot,Color maskColor, XUIMgrGateway _gateway)
        {
            mGateway = _gateway;
            MaskGo = TransUIRoot.gameObject.FindOrCreateGo("UIMask");
            MaskCanvas = MaskGo.GetComponentOrAdd<Canvas>();
            MaskCanvas.overrideSorting = true;
            MaskCanvas.sortingOrder = 0;


            var mask_gr = MaskGo.GetComponentOrAdd<GraphicRaycaster>();
            mask_gr.ignoreReversedGraphics = true;

            var rect_mask_trans = MaskGo.GetComponent<RectTransform>();
            rect_mask_trans.anchorMin = Vector2.zero;
            rect_mask_trans.anchorMax = Vector2.one;
            rect_mask_trans.anchoredPosition3D = Vector3.zero;
            rect_mask_trans.localScale = Vector3.one;
            rect_mask_trans.sizeDelta = Vector2.zero;

            var go_img_mask = MaskGo.FindOrCreateGo("Mask");
            var img_mask = go_img_mask.GetComponentOrAdd<XImage>();
            img_mask.color = maskColor;

            var rect_img = go_img_mask.GetComponent<RectTransform>();
            rect_img.anchorMin = Vector2.zero;
            rect_img.anchorMax = Vector2.one;
            rect_img.anchoredPosition3D = Vector3.zero;
            rect_img.localScale = Vector3.one;
            rect_img.sizeDelta = Vector2.zero;

            go_img_mask.GetComponentOrAdd<Button>().onClick.AddListener(OnMaskClicked);


            MaskGo.SetLayerRecursive(5);
            MaskGo.Hide();
        }

        /// <summary>
        /// 使用遮罩
        /// </summary>
        public void UseMask(UIItem item)
        {
            //加到列表
            UIMaskList.Add(item);
            //把Mask置于这个UI下
            MaskGo.Show();
            MaskCanvas.sortingOrder = item.LayerIndex - 1;

            //
            cur_MaskItem = item;
        }

        /// <summary>
        /// UI关闭时，移除该遮罩
        /// </summary>
        /// <param name="uiitem"></param>
        public void RemoveMask(UIItem uiitem)
        {
            if(cur_MaskItem == uiitem)
            {
                //当前显示的遮罩被关了
                cur_MaskItem = null;
            }
            UIMaskList.Remove(uiitem);
            //还有没有其他用到遮罩的UI呢？
            if(UIMaskList.Count > 0)
            {
                //有
                //找到层级最前的那个
                UIItem last_ui = UIMaskList[0];
                foreach (var item in UIMaskList)
                {
                    if(item.LayerIndex > last_ui.LayerIndex)
                    {
                        last_ui = item;
                    }
                }
                MaskCanvas.sortingOrder = last_ui.LayerIndex - 1;
                MaskGo.Show();

                cur_MaskItem = last_ui;
            }
            else
            {
                //没有
                MaskCanvas.sortingOrder = 0;
                MaskGo.Hide();
            }

        }

        private void OnMaskClicked()
        {
            //被点了，看看当前的mask要不要相应点击
            if (cur_MaskItem.Mask_SmartClose)
            {
                //要
                if (mGateway.IsAdvanced)
                {
                    //高级模式，使用ID关UI
                    mGateway.CloseUI(cur_MaskItem.GetID());
                }
                else
                {
                    mGateway.CloseUIByPath(cur_MaskItem.UIPath);
                }
            }
        }
    }


    /// <summary>
    /// UI层级管理器
    /// </summary>
    public class UILayerMgr
    {
        /*
         * 该管理器用于管理UI层级的数值运算
         * 以及多个UI在打开后的覆盖关系（如全屏UI的隐藏逻辑）
         * 
         * layeroder:
         * 
         * 每个UI占用10个layer数
         * 
         * 如第一个UI,占用 6 - 15,UI本体位于10
         * 
         */

        private XUIMgrGateway mGateway;

        private int mCurMaxLayer = 0;   //这里以UI本体所在layer记录

        private List<UIEntity> mUIPool = new List<UIEntity>(); //存放所有UI
        private List<UIEntity> mFullScreen_GameObject_List = new List<UIEntity>();
        private UIEntity mCur_Active_FullScreenUI; //当前活动状态的全屏界面


        /// <summary>
        /// 当新的UI被打开
        /// </summary>
        /// <param name="IsFullScreen">是否为全屏UI</param>
        /// <param name="DontPauseWhenHide">在全屏UI隐藏时，不要暂停</param>
        /// <param name="uiLayer">UI本体Layer序列</param>
        public void OnUIOpen(out int uiLayer, bool IsFullScreen, bool DontPauseWhenHide, UIEntity _Entity)
        {
            //先存入UI
            mUIPool.Add(_Entity);
            ////最大UI层级数 + 10 （也就是一个UI占用的数量）
            //mCurMaxLayer = mCurMaxLayer + 10;]
            mCurMaxLayer = mUIPool.Count * 10 + 1;
            
            uiLayer = mCurMaxLayer;

            //全屏UI处理
            if (IsFullScreen)
            {
                //记录
                mFullScreen_GameObject_List.Add(_Entity);
                if(mCur_Active_FullScreenUI != null)
                {

                    //顶替流程

                        //从视口中移走先
                    var rect_trans = mCur_Active_FullScreenUI.gameObject.GetComponent<RectTransform>();
                    rect_trans.anchoredPosition = new Vector2(rect_trans.anchoredPosition.x, rect_trans.anchoredPosition.y + (mGateway.UIKit_UIRoot_RectTrans.rect.size.y * (mFullScreen_GameObject_List.Count) + 1) + 100);

                    if (!mCur_Active_FullScreenUI.DontPauseWhenHide)
                    {
                        mCur_Active_FullScreenUI.gameObject.Hide(); //隐藏之后，这里的运算会停掉，省资源
                    }

                }

                mCur_Active_FullScreenUI = _Entity;
            }

            

        }

        public void OnUIClose(UIItem ui_item)
        {
            //if(ui_item.LayerIndex == mCurMaxLayer)
            //{
            //    //将被关掉的是最顶级的UI;
            //    mCurMaxLayer -= 10;
            //}
            var entity = ui_item.GetUIEntity();
            for (var i = mUIPool.Count -1; i >=0; i--)
            {
                if(mUIPool[i] != entity)
                {
                    //被关掉的不是最顶层的UI
                    //把这个UI上面的UI层级依次减10
                    mUIPool[i].SetLayer(mUIPool[i].LayerIndex - 10);
                }
                else
                {
                    mUIPool.RemoveAt(i);
                    break;
                }
            }

            //var entity = ui_item.GetUIEntity();
            if (entity.IsFullScreenUI)
            {
                mFullScreen_GameObject_List.Remove(entity);
                //全屏
                if (mCur_Active_FullScreenUI == entity)
                {
                    //将被关掉的就是最顶级UI,找到上一个UI并放出来
                    if(mFullScreen_GameObject_List.Count > 0)
                    {
                        mCur_Active_FullScreenUI = mFullScreen_GameObject_List[mFullScreen_GameObject_List.Count - 1];
                        //放出来
                        mCur_Active_FullScreenUI.gameObject.Show();
                        var rect_trans = mCur_Active_FullScreenUI.gameObject.GetComponent<RectTransform>();
                        rect_trans.anchoredPosition3D = Vector3.zero;
                    }
                    else
                    {
                        mCur_Active_FullScreenUI = null;
                    }

                }
            }
        }

        /// <summary>
        /// 将某个UI置顶
        /// </summary>
        /// <param name="uientity"></param>
        public void SetUILayerTop(UIEntity uiEntity)
        {
            if(uiEntity == mUIPool[mUIPool.Count - 1])
            {
                return;
            }
            int cur_max_layer_index = mUIPool[mUIPool.Count - 1].LayerIndex;
            for(var i = mUIPool.Count -1; i >= 0; i -- )
            {
                if(mUIPool[i] != uiEntity)
                {
                    //把这个UI上面的UI层级依次减10
                    mUIPool[i].SetLayer(mUIPool[i].LayerIndex - 10);
                }
                else
                {
                    uiEntity.SetLayer(cur_max_layer_index);
                    //得把这个UI在list中放在最后面
                    var temp = mUIPool[mUIPool.Count -1];
                    mUIPool[mUIPool.Count -1] = mUIPool[i];
                    mUIPool[i] = temp;
                    break;
                }
            }

        }

        public UILayerMgr(XUIMgrGateway _gateway)
        {
            mGateway = _gateway;
        }

    }

    /// <summary>
    /// 这个方法用来在管理器中存放UI
    /// </summary>
    public class UIItem
    {
        /// <summary>
        /// 对应的UI实体
        /// </summary>
        private UIEntity mUIEntity;
        private string mUIPath;
        private GameObject mUIPrefab;
        private XUIMgrGateway mGateway;
        private GameObject mUIGameObject;
        private Canvas mUICanvas;

        //public Action OnUIClose;
        private bool mUseMask = false;
        private bool CanCloseByMaskClick = false; //点击Mask,可否关闭UI   

        public string UIPath
        {
            get
            {
                return mUIPath;
            }
        }

        
        public int LayerIndex
        {
            get
            {
                return mUICanvas.sortingOrder;
            }
        }

        /// <summary>
        /// 是否可以多个UI并存
        /// </summary>
        public bool IsMultiUI
        {
            get
            {
                if(mUIEntity == null)
                {
                    return false;
                }
                return mUIEntity.MultiUI;
            }
        }

        private int m_ID
        {
            get
            {
                return mUIEntity.GetID();
            }
            set
            {
                mUIEntity.SetID(value);
            }
        }

        public UIItem(GameObject _prefab,string _ui_path,XUIMgrGateway _gatewayMgr)
        {
            mUIPrefab = _prefab;
            mUIPath = _ui_path;
            mGateway = _gatewayMgr;
        }

        /// <summary>
        /// UI初始化
        /// </summary>
        public void InitUI()
        {
            if(mUIPrefab != null)
            {
                
                //spawn
                mUIGameObject = GameObject.Instantiate(mUIPrefab, mGateway.UIKit_UIRoot_Trans);
                //改名
                mUIGameObject.Name(mUIGameObject.name.Replace("(Clone)", ""));

                mUIEntity = mUIGameObject.GetComponentOrAdd<UIEntity>();

                //处理gameobject的recttransform
                var trans = mUIGameObject.GetComponent<RectTransform>();
                if(trans != null)
                {
                    trans.anchorMax = Vector2.one;
                    trans.anchorMin = Vector2.zero;
                    trans.localScale = Vector3.one;
                    trans.anchoredPosition3D = Vector3.zero;
                }

                mGateway.UIKit_LayerMgr.OnUIOpen(out int layerIndex, mUIEntity.IsFullScreenUI, mUIEntity.DontPauseWhenHide, mUIEntity);

                var canvas = mUIGameObject.GetComponent<Canvas>();
                if(canvas != null)
                {
                    canvas.overrideSorting = true;

                    //层级关系
                    canvas.sortingOrder = layerIndex;
                    
                }
                mUICanvas = canvas;

            }
            
        }

        public IUIEntity GetUIEntityAPI()
        {
            return mUIEntity;
        }

        public UIEntity GetUIEntity()
        {
            return mUIEntity;
        }

        /// <summary>
        /// 设置句柄ID
        /// </summary>
        public void SetID(int _id)
        {
            m_ID = _id;
        }

        public int GetID()
        {
            return m_ID;
        }

        /// <summary>
        /// 登记Child
        /// </summary>
        public void AddChild(UIEntity child)
        {
            mUIEntity.AddChild(child);
        }

        /// <summary>
        /// 使用遮罩
        /// </summary>
        public void UseMask(bool smartClose)
        {
            mUseMask = true;
            CanCloseByMaskClick = smartClose;
            mGateway.UIKit_MaskMgr.UseMask(this);
        }

        public bool Mask_SmartClose
        {
            get
            {
                return CanCloseByMaskClick;
            }
        }

        public void CloseUI() //调用这里就直接关了，要传参数的话，传完了再调用
        {
            //告诉遮罩管理器，如果有遮罩的话
            if (mUseMask)
            {
                mGateway.UIKit_MaskMgr.RemoveMask(this);
            }
            //告诉层级管理器
            mGateway.UIKit_LayerMgr.OnUIClose(this);
            //手起刀落
            GameObject.Destroy(mUIEntity.gameObject);
            //通知资源管理器，可以回收了
            VFS.I.RemoveUse(mUIPath);
            //打扫
            mUIEntity = null;
            mUIPrefab = null;
            mGateway = null;
            mUIGameObject = null;
            mUICanvas = null;
        }

    }


}
