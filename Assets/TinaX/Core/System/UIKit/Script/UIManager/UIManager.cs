using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;
using TinaX;
using UnityEngine;
using System.Reflection;


namespace TinaX.UIKits
{
    public class UIManager : IUIKit
    {
        /// <summary>
        /// UI背景遮罩管理
        /// </summary>
        private UIMaskManager mMaskMgr;

        /// <summary>
        /// UI层级管理器
        /// </summary>
        private UILayerManager mLayerMgr;

        /// <summary>
        /// 全屏UI调度管理器
        /// </summary>
        private FullScreenUIManager mFullScreenMgr;

        private IVFS mVFS;  //依赖注入

        private UIKitConfig mConfig;
        private UIGroupConf mCurUIGroup;

        #region UGUI GameObjects
        public GameObject UIRootGameObject { get; private set; }
        public GameObject UICameraGameObject { get; private set; }
        public Camera UICamera { get; private set; }

        public Canvas UIRootCanvas { get; private set; }
        public RectTransform UIRootRectTransform { get; private set; }

        public Transform UIRootTransform { get; private set; }

        public UIKitConfig UIConfig => mConfig;
        #endregion

        /// <summary>
        /// 已加载的UI存储处
        /// </summary>
        private List<UIEntity> mLoadedUIPool = new List<UIEntity>();
        private Dictionary<ulong, UIEntity> mDict_UIs = new Dictionary<ulong, UIEntity>(); // key ui_id,

        public UIManager(IVFS vfs)
        {
            mVFS = vfs;

            //配置
            mConfig = TinaX.Config.GetTinaXConfig<UIKitConfig>(TinaX.Conf.ConfigPath.uikit);
            if(mConfig == null)
            {
                throw new Exception("[TinaX.UIKits]not valid config. | 无效的配置");
            }

            mCurUIGroup = mConfig.Default_UIGroup;

            CtorGameobjects();

            //Manager
            mMaskMgr = new UIMaskManager(this);
            mLayerMgr = new UILayerManager(this);
            mFullScreenMgr = new FullScreenUIManager(this);
        }


        private void CtorGameobjects()
        {
            var tinax_root_go = GameObjectHelper.FindOrCreateGo(Setup.Framework_Base_GameObject);
            var go_uikit = tinax_root_go.FindOrCreateGameObject(UIKitConst.UIKit_RootGameObject_Name);
            UIRootGameObject = go_uikit.FindOrCreateGameObject(UIKitConst.UIKit_UIRootGameObject_Name);
            UICameraGameObject = go_uikit.FindOrCreateGameObject(UIKitConst.UIKit_UICameraGameObject_Name);

            #region UICamera

            UICamera = UICameraGameObject.GetComponentOrAdd<Camera>();
            UICamera.clearFlags = CameraClearFlags.Depth;
            UICamera.cullingMask = 1 << 5;
            UICamera.orthographic = true;
            UICamera.depth = 99;
            UICamera.allowHDR = false;
            UICamera.allowMSAA = false;

            #endregion

            #region UIRoot
            UIRootGameObject.SetLayerRecursive(5);

            UIRootCanvas = UIRootGameObject.GetComponentOrAdd<Canvas>();
            UIRootCanvas.renderMode = RenderMode.ScreenSpaceCamera;
            UIRootCanvas.worldCamera = UICamera;
            UIRootCanvas.pixelPerfect = mConfig.PixelPerfect;


            var _canvas_scaler = UIRootGameObject.GetComponentOrAdd<CanvasScaler>();
            _canvas_scaler.uiScaleMode = mConfig.Canvas_Scale_Mode;
            _canvas_scaler.referenceResolution = mConfig.UISize;
            _canvas_scaler.screenMatchMode = mConfig.ScreenMatchMode;
            _canvas_scaler.matchWidthOrHeight = mConfig.Match;

            _canvas_scaler.scaleFactor = mConfig.ScaleFactor;
            _canvas_scaler.referencePixelsPerUnit = mConfig.referencePixelsPerUnit;
            _canvas_scaler.physicalUnit = mConfig.physicalUnit;

            var _gr = UIRootGameObject.GetComponentOrAdd<GraphicRaycaster>();
            _gr.ignoreReversedGraphics = true;
            #endregion

            UIRootTransform = UIRootGameObject.transform;
            UIRootRectTransform = UIRootGameObject.GetComponent<RectTransform>();

            #region EventSystem
            var ESGo = GameObjectHelper.FindOrCreateGo("EventSystem");
            var esObj = ESGo.GetComponentOrAdd<UnityEngine.EventSystems.EventSystem>();
            esObj.sendNavigationEvents = true;
            esObj.pixelDragThreshold = 10;
            var sim = ESGo.GetComponentOrAdd<UnityEngine.EventSystems.StandaloneInputModule>();
            ESGo.DontDestroy();

            #endregion

        }


        #region 公开的接口方法们

        //------一长串的OpenUI----------------------------------------------------------------------------------------------------------------

        public IUIEntity OpenUI(string ui_name)
        {
            return OpenUINoLua(ui_name, null, null, false, false, null, null);
        }

        public IUIEntity OpenUI(string ui_name, object ui_param)
        {
            return OpenUINoLua(ui_name, null, null, false, false, null, ui_param);
        }

        public IUIEntity OpenUI(string ui_name, bool use_mask, bool close_by_mask)
        {
            return OpenUINoLua(ui_name, null, null, use_mask, close_by_mask, null, null);
        }

        public IUIEntity OpenUI(string ui_name, System.Object ui_param, bool use_mask, bool close_by_mask = false)
        {
            return OpenUINoLua(ui_name, null, null, use_mask, close_by_mask, null, ui_param);
        }

        public IUIEntity OpenUI(string ui_name,Type ControllerType)
        {
            return OpenUINoLua(ui_name, null, null, false, false, ControllerType, null);
        }

        public IUIEntity OpenUI(Type ControllerType, OpenUIParam datas)
        {
            //Debug.Log("被调用打开UI了啊");
            string ui_name;
            string ui_path;
            if (datas.UIName.IsNullOrEmpty() && datas.UIPath.IsNullOrEmpty())
            {
                //Debug.Log("没有传入UIName和Path，需要反射获取");
                try
                {
                    GetUINameAndPathFromUIController(ControllerType,out ui_name, out ui_path);
                }
                catch (Exceptions.UIKitException e)
                {
                    throw e;
                }
            }
            else
            {
                //Debug.Log("看来传入了UIName或方法");
                ui_name = datas.UIName;
                ui_path = datas.UIPath;

            }
            return OpenUINoLua(ui_name, ui_path, null, datas.UseMask, datas.CloseByMask, ControllerType, datas.Param);
        }

        public IUIEntity OpenUI<TController>(string ui_name) where TController: UIController
        {
            return OpenUINoLua(ui_name, null, null, false, false, typeof(TController), null);
        }

        public IUIEntity OpenUI<TController>() where TController : UIController
        {
            try
            {
                GetUINameAndPathFromUIController<TController>(out var uiName, out var uiPath);
                return OpenUINoLua(uiName, uiPath, null, false, false, typeof(TController), null);
            }
            catch(Exceptions.UIKitException e)
            {
                throw e;
            }
            
        }

        public IUIEntity OpenUI<TController>(object param) where TController : UIController
        {
            try
            {
                GetUINameAndPathFromUIController<TController>(out var uiName, out var uiPath);
                return OpenUINoLua(uiName, uiPath, null, false, false, typeof(TController), param);
            }
            catch (Exceptions.UIKitException e)
            {
                throw e;
            }
        }

        public IUIEntity OpenUI<TController>(OpenUIParam datas) where TController : UIController
        {
            string ui_name;
            string ui_path;
            if(datas.UIName.IsNullOrEmpty() && datas.UIPath.IsNullOrEmpty())
            {
                try
                {
                    GetUINameAndPathFromUIController<TController>(out ui_name, out ui_path);
                }
                catch (Exceptions.UIKitException e)
                {
                    throw e;
                }
            }
            else
            {
                ui_name = datas.UIName;
                ui_path = datas.UIPath;

            }
            return OpenUINoLua(ui_name, ui_path, null, datas.UseMask, datas.CloseByMask, typeof(TController), datas.Param);
        }

        public IUIEntity OpenUIByPath(string ui_path, object ui_param, bool use_mask, bool close_by_mask = false)
        {
            return OpenUINoLua(null, ui_path, null, use_mask, close_by_mask, null, ui_param);
        }

        public IUIEntity OpenUIByPath<TController>(string ui_path, object ui_param, bool use_mask, bool close_by_mask = false) where TController : UIController
        {
            return OpenUINoLua(null, ui_path, null, use_mask, close_by_mask, typeof(TController), ui_param);
        }


#if TinaX_CA_LuaRuntime_Enable

        /// <summary>
        /// 打开UI 供Lua调用
        /// </summary>
        /// <param name="ui_name">UI名</param>
        /// <param name="lua_param">lua table参数</param>
        /// <param name="use_mask">使用遮罩</param>
        /// <param name="close_by_mask">点击遮罩关闭UI</param>
        /// <returns></returns>
        public IUIEntity OpenUIWhitLuaParam(string ui_name, XLua.LuaTable lua_param, bool use_mask, bool close_by_mask) //只有luabehaviour能接收lua table参数，所以不会有uiController的情况
        {
            return OpenUIForLua(ui_name, null, null, use_mask, close_by_mask, lua_param); 
        }

        public IUIEntity OpenUIByPathWhitLuaParam(string ui_path, XLua.LuaTable lua_param, bool use_mask, bool close_by_mask = false)
        {
            return OpenUIForLua(null, ui_path, null, use_mask, close_by_mask, lua_param);
        }

#endif



        //------又是一长串的CloseUI----------------------------------------------------------------------------------------------------------------

        //过气方法
        public void CloseUI(string UIName, object Param = null)
        {
            var uis = mLoadedUIPool.Where(ui => !ui.UIName.IsNullOrEmpty() && ui.UIName == UIName);
            if(uis.Count() > 0)
            {
                foreach(var entity in uis)
                {
                    CloseUINoLua(entity, Param);
                }
            }
        }

        public void CloseUI(ulong UI_ID,object Param = null)
        {
            if (mDict_UIs.ContainsKey(UI_ID))
            {
                CloseUINoLua(mDict_UIs[UI_ID], Param);
            }
        }

        public void CloseUIByPath(string ui_path, object Param = null)
        {
            var uis = mLoadedUIPool.Where(ui => !ui.UIPath.IsNullOrEmpty() && ui.UIPath == ui_path);
            if (uis.Count() > 0)
            {
                foreach (var entity in uis)
                {
                    CloseUINoLua(entity, Param);
                }
            }
        }


#if TinaX_CA_LuaRuntime_Enable

        public void CloseUIWithLuaParam(string ui_name, XLua.LuaTable luaParam)
        {
            var uis = mLoadedUIPool.Where(ui => !ui.UIName.IsNullOrEmpty() && ui.UIName == ui_name);
            if (uis.Count() > 0)
            {
                foreach (var entity in uis)
                {
                    CloseUIForLua(entity, luaParam);
                }
            }
        }

        public void CloseUIWithLuaParam(ulong ui_id, XLua.LuaTable luaParam)
        {
            if (mDict_UIs.ContainsKey(ui_id))
            {
                CloseUIForLua(mDict_UIs[ui_id], luaParam);
            }
        }


        public void CloseUIByPathWithLuaParam(string ui_path, XLua.LuaTable luaParam)
        {
            var uis = mLoadedUIPool.Where(ui => !ui.UIPath.IsNullOrEmpty() && ui.UIPath == ui_path);
            if (uis.Count() > 0)
            {
                foreach (var entity in uis)
                {
                    CloseUIForLua(entity, luaParam);
                }
            }
        }

#endif


        //------又是一长串的HideUI----------------------------------------------------------------------------------------------------------------

        public void HideUI(string UIName , bool IncludeChild = true)  //过气 + 1
        {
            var uis = mLoadedUIPool.Where(ui => !ui.UIName.IsNullOrEmpty() && ui.UIName == UIName);
            if(uis.Count() > 0)
            {
                foreach(var entity in uis)
                {
                    HideUIHandle(entity, IncludeChild);
                }
            }
        }

        public void HideUIByPath(string ui_path, bool IncludeChild = true) //过气 + 1
        {
            var uis = mLoadedUIPool.Where(ui => !ui.UIPath.IsNullOrEmpty() && ui.UIPath == ui_path);
            if (uis.Count() > 0)
            {
                foreach (var entity in uis)
                {
                    HideUIHandle(entity, IncludeChild);
                }
            }
        }

        public void HideUI(ulong id, bool IncludeChild = true)
        {
            if (mDict_UIs.ContainsKey(id))
            {
                HideUIHandle(mDict_UIs[id], IncludeChild);
            }
        }

        //------又是一长串的ShowUI----------------------------------------------------------------------------------------------------------------

        public void ShowUI(string UIName, bool IncludeChild = true) //过气
        {
            var uis = mLoadedUIPool.Where(ui => !ui.UIName.IsNullOrEmpty() && ui.UIName == UIName);
            if (uis.Count() > 0)
            {
                foreach (var entity in uis)
                {
                    ShowUIHandle(entity, IncludeChild);
                }
            }
        }

        public void ShowUI(ulong id, bool IncludeChild = true)
        {
            if (mDict_UIs.ContainsKey(id))
            {
                HideUIHandle(mDict_UIs[id], IncludeChild);
            }
        }

        public void ShowUIByPath(string ui_path, bool IncludeChild = true) //过气 + 1
        {
            var uis = mLoadedUIPool.Where(ui => !ui.UIPath.IsNullOrEmpty() && ui.UIPath == ui_path);
            if (uis.Count() > 0)
            {
                foreach (var entity in uis)
                {
                    HideUIHandle(entity, IncludeChild);
                }
            }
        }

        //------置顶UI------------------------------------------------------------------------------------------------------

        /// <summary>
        /// 置顶UI
        /// </summary>
        /// <param name="entity"></param>
        public void SetUILayerTop(UIEntity entity)
        {
            mLayerMgr.SetUITop(entity);
        }

        public void SetUIlayerTop(ulong id)
        {
            if (mDict_UIs.ContainsKey(id))
            {
                mLayerMgr.SetUITop(mDict_UIs[id]);
            }
        }

        //-------------------------------------------------------------------------------------------------------------------

        public void SetUIGroup(UIGroupConf ui_group)
        {
            mCurUIGroup = ui_group;
        }

        public Vector2 GetUIScreenLocalPoint(Transform trans)
        {
            Vector2 point;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                UIRootRectTransform,
                UICamera.WorldToScreenPoint(trans.position),
                UICamera,
                out point))
            {
                return point;
            }
            else
            {
                return Vector2.zero;
            }
        }


        #endregion




        public UIEntity GetUIEntityByID(ulong ID)
        {
            if (mDict_UIs.ContainsKey(ID))
            {
                return mDict_UIs[ID];
            }
            else
            {
                return null;
            }
        }

        private UIEntity OpenUINoLua(string ui_name, string ui_path, UIEntity parent_entity, bool use_mask, bool close_by_mask, Type uiControllerType, System.Object param)
        {
#if TinaX_CA_LuaRuntime_Enable
            return OpenUIHandle(ui_name, ui_path, parent_entity, use_mask, close_by_mask, uiControllerType, false, param, null);
#else
            return OpenUIHandle(ui_name, ui_path, parent_entity, use_mask, close_by_mask, uiControllerType, param);
#endif
        }

        private void CloseUINoLua(UIEntity entity,object param = null)
        {
#if TinaX_CA_LuaRuntime_Enable
            CloseUIHandle(entity, false, param, null);
#else
            CloseUIHandle(entity, param);
#endif
        }

#if TinaX_CA_LuaRuntime_Enable
        private UIEntity OpenUIForLua(string ui_name, string ui_path, UIEntity parent_entity, bool use_mask, bool close_by_mask, XLua.LuaTable param)
        {
            return OpenUIHandle(ui_name, ui_path, parent_entity, use_mask, close_by_mask, null, true, null, param);
        }


        private void CloseUIForLua(UIEntity entity,XLua.LuaTable lua_param)
        {
            CloseUIHandle(entity, true, null, lua_param);
        }

#endif

        private void GetUINameAndPathFromUIController<T>(out string _ui_name , out string _ui_path) where T : UIController
        {
            string ui_name;
            string ui_path;

            Type ctrl_type = typeof(T);
            var ctrl_attr = ctrl_type.GetCustomAttribute<UIControllerAttribute>(true);
            if (ctrl_attr != null)
            {
                ui_name = ctrl_attr.UIName;
                ui_path = ctrl_attr.UIPath;
            }
            else
            {
                ui_name = null;
                ui_path = null;
            }

            if (ui_name.IsNullOrEmpty() && ui_path.IsNullOrEmpty())
            {
                throw new Exceptions.UIKitException(Exceptions.UIKitException.ErrorType.UINameOrPathInvalid, "Can't got ui name or ui path by UIController");
            }

            _ui_name = ui_name;
            _ui_path = ui_path;
        }

        private void GetUINameAndPathFromUIController(Type type, out string _ui_name, out string _ui_path) 
        {
            string ui_name;
            string ui_path;

            Type ctrl_type = type;
            var ctrl_attr = ctrl_type.GetCustomAttribute<UIControllerAttribute>(true);
            if (ctrl_attr != null)
            {
                ui_name = ctrl_attr.UIName;
                ui_path = ctrl_attr.UIPath;
            }
            else
            {
                ui_name = null;
                ui_path = null;
            }

            if (ui_name.IsNullOrEmpty() && ui_path.IsNullOrEmpty())
            {
                throw new Exceptions.UIKitException(Exceptions.UIKitException.ErrorType.UINameOrPathInvalid, "Can't got ui name or ui path by UIController");
            }

            _ui_name = ui_name;
            _ui_path = ui_path;
        }



#if TinaX_CA_LuaRuntime_Enable
        private UIEntity OpenUIHandle(string ui_name, string ui_path, UIEntity parent_entity, bool use_mask, bool close_by_mask, Type uiControllerType, bool lua_table_param, System.Object param, XLua.LuaTable lua_param)
#else
        private UIEntity OpenUIHandle(string ui_name ,string ui_path, UIEntity parent_entity,bool use_mask , bool close_by_mask , Type uiControllerType , System.Object param)
#endif

        {
            /*
             * 步骤
             * 1. 加载文件
             * 2. 调整层级
             * 3. 全屏UI顶替规则
             * 4. 处理遮罩
             * 5. 管理生命周期（如果需要的话
             * 6. 传递参数
             * 7. 登记和完成
             * 
             */

            //错误判断
            if(uiControllerType != null)
            {
                if (!uiControllerType.IsSubclassOf(typeof(UIController)))
                {
                    throw new Exceptions.UIKitException(Exceptions.UIKitException.ErrorType.UIControllerInvalid, $"type:\"{uiControllerType.FullName}\" is not UIController Class.");
                }
            }

            //加载
            try
            {
                var ui_vfs_path = GetUIVFSPath(ui_name, ui_path);
                var ui_prefab = LoadUIFileByVFS(ui_vfs_path);
                var prefab_uientity = ui_prefab.GetComponent<UIEntity>();
                //检查这个Prefab是否包含UIKit
                if (prefab_uientity == null)
                {
                    throw new Exceptions.UIKitException(Exceptions.UIKitException.ErrorType.UIEntityInvalid, "Can't found \"UIEntity\" in ui prefab, UI vfs path:" + ui_vfs_path);
                }

                //检查重复加载
                if (!prefab_uientity.MultiUI)
                {
                    //不可以加载多个
                    //判断是否有加载多个
                    if (mLoadedUIPool.Any(ui => ui.UIPath == ui_vfs_path))
                    {
                        //抛异常
                        mVFS.RemoveUse(ui_vfs_path);
                        throw new Exceptions.UIKitException(Exceptions.UIKitException.ErrorType.UIAlreadyLoaded, "UI already loaded and this ui not allow open multipe entity." + (ui_name.IsNullOrEmpty() ? "UI name: " + ui_name : "UI Path: " + ui_vfs_path));
                    }
                }

                //加载就是了
                prefab_uientity = null;
                var ui_gameObject = GameObject.Instantiate(ui_prefab, UIRootTransform);
                UIEntity entity = ui_gameObject.GetComponent<UIEntity>();
                entity.UIPath = ui_vfs_path;
                entity.UIName = ui_name;
                entity.OnUICreated();

                //分配id
                ulong ui_id = 10;
                while (mDict_UIs.ContainsKey(ui_id))
                {
                    ui_id++;
                }
                entity.ID = ui_id;

                //当前加载的ID是否是某个ID的子类？
                if (parent_entity != null)
                {
                    //还真是，那就把这个UI的信息交给父Entity
                    parent_entity.AddChild(ui_id);
                }

                //层级调整
                if(ui_gameObject.GetComponent<Canvas>()!= null)
                {
                    mLayerMgr.GetNewLayerIndex(entity);
                }
                //全屏UI顶替规则
                mFullScreenMgr.OnUIOpen(entity);    //无脑调用即可，内部会做判断
                //处理遮罩
                if (use_mask)
                {
                    mMaskMgr.UseMask(entity, close_by_mask);
                }

                //头疼大事来了，管理生命周期
                if(entity.HandleType == UIEntity.E_MainHandleType.UIController)
                {
                    //反射出UIController
                    InitUIController(uiControllerType, entity);
                }

                //其他的不用管理生命周期，继续往下：
                //传递参数
#if TinaX_CA_LuaRuntime_Enable
                if(lua_table_param)
                {
                    if(lua_param != null)
                    {
                        entity.OnUIOpenMessage_LuaTable(lua_param);
                    }
                }
                else
                {
                    //普通类型的参数
                    if (param != null)
                        entity.OnUIOpenMessage(param);
                }
#else
                //没有lua（真tm清爽
                //普通类型的参数
                if (param != null)
                    entity.OnUIOpenMessage(param);
#endif


                //登记（最后异步
                mDict_UIs.Add(ui_id, entity);
                mLoadedUIPool.Add(entity);

                return entity;



            }
            catch (Exceptions.UIKitException e)
            {
                throw e;
            }
            catch (Exceptions.VFSException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                throw e;
            }


        }


#if TinaX_CA_LuaRuntime_Enable
        private void CloseUIHandle(UIEntity entity, bool lua_table_param, object param, XLua.LuaTable lua_param)
#else
        private void CloseUIHandle(UIEntity entity, object param)
#endif
        {
            /*
             * UI关闭流程
             * 1. 释放层级
             * 2. 处理全屏UI顶替
             * 3. 处理遮罩
             * 4. 管理生命周期（如果需要
             * 5. 传递参数（如果需要
             * 6. 移除登记
             * 7. 销毁GameObject
             * 8. GC登记
             */

            //释放层级
            mLayerMgr.OnUIClose(entity);
            //处理全屏顶替
            mFullScreenMgr.OnUIClose(entity);
            //处理遮罩
            mMaskMgr.RemoveMask(entity);
            //管理生命周期
            //TODO 关闭UI的生命周期
            //传递参数？
#if TinaX_CA_LuaRuntime_Enable
            if (lua_table_param)
            {
                if(lua_param != null)
                {
                    entity.OnUICloseMessage_LuaTable(lua_param);
                }
            }
            else
            {
                if(param != null)
                {
                    entity.OnUICloseMessage(param);
                }
            }
#else
            if(param != null)
            {
                entity.OnUICloseMessage(param);
            }
#endif
            //移除登记
            if (mDict_UIs.ContainsKey(entity.ID))
            {
                mDict_UIs.Remove(entity.ID);
            }
            if (mLoadedUIPool.Contains(entity))
            {
                mLoadedUIPool.Remove(entity);
            }

            //销毁GameObject
            entity.gameObject.DestroySelf();
            //VFS GC登记
            mVFS.RemoveUse(entity.UIPath);

            entity = null;

        }

        private void HideUIHandle(UIEntity entity, bool IncludeChild = true)
        {
            entity.gameObject.Hide();
            if (IncludeChild)
            {
                if (entity.childCount > 0)
                {
                    for (var i = 0; i < entity.childCount; i++)
                    {
                        var child = GetUIEntityByID(entity.GetChildID(i));
                        if(child != null)
                        {
                            HideUIHandle(child,IncludeChild);
                        }
                    }
                }
            }
            
        }

        private void ShowUIHandle(UIEntity entity, bool IncludeChild = true)
        {
            entity.gameObject.Show();
            if (IncludeChild)
            {
                if (entity.childCount > 0)
                {
                    for (var i = 0; i < entity.childCount; i++)
                    {
                        var child = GetUIEntityByID(entity.GetChildID(i));
                        if (child != null)
                        {
                            ShowUIHandle(child, IncludeChild);
                        }
                    }
                }
            }

        }

        private void InitUIController(Type ctrlType, UIEntity entity)
        {
            var instance = Activator.CreateInstance(ctrlType); //先不管构造函数的功能了
            

            var fields = ctrlType.GetFields(BindingFlags.Public | BindingFlags.Instance);
            foreach(var item in fields)
            {
                //找（UIInject）
                var attr = item.GetCustomAttribute<UIInjectAttribute>();
                if (attr != null && !attr.InjectName.IsNullOrEmpty())
                {
                    bool injected = false;

                    //绑定信息
                    var inject_obj = entity.GetInjectObject(attr.InjectName);
                    if(inject_obj != null)
                    {
                        try
                        {
                            item.SetValue(instance, inject_obj.Object);
                        }
                        catch (ArgumentException e)
                        {
                            //多做一步尝试
                            if(inject_obj.Object.GetType() == typeof(GameObject) && (item.FieldType.IsSubclassOf(typeof(UnityEngine.Component)) || (item.FieldType.IsSubclassOf(typeof(UnityEngine.MonoBehaviour)))))
                            {
                                //Debug.Log("被注入的类是GameObject，而且尝试直接注入失败了，我们看看能不能在这个GameObject里面获取到想要被注入的类");
                                var tryObj = ((GameObject)inject_obj.Object).GetComponent(item.FieldType);
                                if(tryObj != null)
                                {
                                    try
                                    {
                                        item.SetValue(instance, tryObj);
                                    }
                                    catch(ArgumentException e2)
                                    {
                                        Debug.LogError($"[TinaX.UIkits] Inject UI object \"{inject_obj.Name}\" to UIController field \"{item.Name}\" Error: {e2.Message}");
                                    }
                                    finally
                                    {
                                        injected = true;
                                    }
                                }
                                else
                                {
                                    Debug.LogError($"[TinaX.UIkits] Inject UI object \"{inject_obj.Name}\" to UIController field \"{item.Name}\" Error: {e.Message}");
                                }
                            }
                            else
                            {
                                Debug.LogError($"[TinaX.UIkits] Inject UI object \"{inject_obj.Name}\" to UIController field \"{item.Name}\" Error: {e.Message}");
                            }
                        }
                        finally
                        {
                            injected = true;
                        }

                        
                    }

                    if (!injected)
                    {
                        var inject_data = entity.GetInjectData(attr.InjectName);
                        if(inject_data != null)
                        {
                            try
                            {
                                switch (inject_data.DataType)
                                {
                                    default:
                                        XLog.PrintE("Framework没有实现该类型的UIController数据注入："+ inject_data.DataType.ToString());
                                        break;

                                    case UIEntity.InjectDataType.Boolean:
                                        item.SetValue(instance, inject_data.BoolData);
                                        break;
                                    case UIEntity.InjectDataType.Float:
                                        item.SetValue(instance, inject_data.FloatData);
                                        break;
                                    case UIEntity.InjectDataType.Int:
                                        item.SetValue(instance, inject_data.IntData);
                                        break;
                                    case UIEntity.InjectDataType.Text:
                                        item.SetValue(instance, inject_data.TextData);
                                        break;
                                    case UIEntity.InjectDataType.Long:
                                        item.SetValue(instance, inject_data.LongData);
                                        break;
                                    case UIEntity.InjectDataType.Double:
                                        item.SetValue(instance, inject_data.DoubleData);
                                        break;


                                    case UIEntity.InjectDataType.TextArray:
                                        item.SetValue(instance, inject_data.StringArrayData);
                                        break;
                                    case UIEntity.InjectDataType.IntArray:
                                        item.SetValue(instance, inject_data.IntArrayData);
                                        break;
                                    case UIEntity.InjectDataType.FloatArray:
                                        item.SetValue(instance, inject_data.FloatArrayData);
                                        break;


                                    case UIEntity.InjectDataType.Color:
                                        item.SetValue(instance, inject_data.ColorData);
                                        break;
                                    case UIEntity.InjectDataType.Color32:
                                        item.SetValue(instance, inject_data.Color32Data);
                                        break;
                                    case UIEntity.InjectDataType.Vector2:
                                        item.SetValue(instance, inject_data.Vector2Data);
                                        break;
                                    case UIEntity.InjectDataType.Vector2Int:
                                        item.SetValue(instance, inject_data.Vector2IntData);
                                        break;
                                    case UIEntity.InjectDataType.Vector3:
                                        item.SetValue(instance, inject_data.Vector3Data);
                                        break;
                                    case UIEntity.InjectDataType.Vector3Int:
                                        item.SetValue(instance, inject_data.Vector3IntData);
                                        break;
                                    case UIEntity.InjectDataType.Quaternion:
                                        item.SetValue(instance, inject_data.QuaternionData);
                                        break;
                                    case UIEntity.InjectDataType.Sprite:
                                        item.SetValue(instance, inject_data.SpriteData);
                                        break;

                                }
                            }
                            catch (ArgumentException e)
                            {
                                Debug.LogError($"[TinaX.UIkits] Inject UI object \"{inject_obj.Name}\" to UIController field \"{item.Name}\" Error: {e.Message}");
                            }
                            finally
                            {
                                injected = true;
                            }
                        }
                    }

                    if (!injected)
                    {
                        Debug.LogWarning($"[TinaX.UIkits] Inject UI object \"{attr.InjectName}\" failed. Can't found Inject Object From UIEntity by this injectName.\n注入UI对象\"{attr.InjectName}\"失败，因为在UIEntity里没找到叫这个名字的可注入对象");
                    }
                }
            
                //TinaX System组件注入
                //先不管了，C# 8好像才能用泛型Attribute
            
            }

            var props = ctrlType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach(var prop in props)
            {

                //找 UIInject
                var attr = prop.GetCustomAttribute<UIInjectAttribute>();

                if (attr != null && !attr.InjectName.IsNullOrEmpty())
                {
                    bool injected = false;

                    //绑定信息
                    var inject_obj = entity.GetInjectObject(attr.InjectName);
                    if (inject_obj != null)
                    {
                        try
                        {
                            prop.SetValue(instance, inject_obj.Object);
                        }
                        catch (ArgumentException e)
                        {
                            //多做一步尝试
                            if (inject_obj.Object.GetType() == typeof(GameObject) && (prop.PropertyType.IsSubclassOf(typeof(UnityEngine.Component)) || (prop.PropertyType.IsSubclassOf(typeof(UnityEngine.MonoBehaviour)))))
                            {
                                //Debug.Log("被注入的类是GameObject，而且尝试直接注入失败了，我们看看能不能在这个GameObject里面获取到想要被注入的类");
                                var tryObj = ((GameObject)inject_obj.Object).GetComponent(prop.PropertyType);
                                if (tryObj != null)
                                {
                                    try
                                    {
                                        prop.SetValue(instance, tryObj);
                                    }
                                    catch (ArgumentException e2)
                                    {
                                        Debug.LogError($"[TinaX.UIkits] Inject UI object \"{inject_obj.Name}\" to UIController field \"{prop.Name}\" Error: {e2.Message}");
                                    }
                                    finally
                                    {
                                        injected = true;
                                    }
                                }
                                else
                                {
                                    Debug.LogError($"[TinaX.UIkits] Inject UI object \"{inject_obj.Name}\" to UIController field \"{prop.Name}\" Error: {e.Message}");
                                }
                            }
                            else
                            {
                                Debug.LogError($"[TinaX.UIkits] Inject UI object \"{inject_obj.Name}\" to UIController field \"{prop.Name}\" Error: {e.Message}");
                            }
                        }
                        finally
                        {
                            injected = true;
                        }


                    }

                    if (!injected)
                    {
                        var inject_data = entity.GetInjectData(attr.InjectName);
                        if (inject_data != null)
                        {
                            try
                            {
                                switch (inject_data.DataType)
                                {
                                    default:
                                        XLog.PrintE("Framework没有实现该类型的UIController数据注入：" + inject_data.DataType.ToString());
                                        break;

                                    case UIEntity.InjectDataType.Boolean:
                                        prop.SetValue(instance, inject_data.BoolData);
                                        break;
                                    case UIEntity.InjectDataType.Float:
                                        prop.SetValue(instance, inject_data.FloatData);
                                        break;
                                    case UIEntity.InjectDataType.Int:
                                        prop.SetValue(instance, inject_data.IntData);
                                        break;
                                    case UIEntity.InjectDataType.Text:
                                        prop.SetValue(instance, inject_data.TextData);
                                        break;
                                    case UIEntity.InjectDataType.Long:
                                        prop.SetValue(instance, inject_data.LongData);
                                        break;
                                    case UIEntity.InjectDataType.Double:
                                        prop.SetValue(instance, inject_data.DoubleData);
                                        break;


                                    case UIEntity.InjectDataType.TextArray:
                                        prop.SetValue(instance, inject_data.StringArrayData);
                                        break;
                                    case UIEntity.InjectDataType.IntArray:
                                        prop.SetValue(instance, inject_data.IntArrayData);
                                        break;
                                    case UIEntity.InjectDataType.FloatArray:
                                        prop.SetValue(instance, inject_data.FloatArrayData);
                                        break;


                                    case UIEntity.InjectDataType.Color:
                                        prop.SetValue(instance, inject_data.ColorData);
                                        break;
                                    case UIEntity.InjectDataType.Color32:
                                        prop.SetValue(instance, inject_data.Color32Data);
                                        break;
                                    case UIEntity.InjectDataType.Vector2:
                                        prop.SetValue(instance, inject_data.Vector2Data);
                                        break;
                                    case UIEntity.InjectDataType.Vector2Int:
                                        prop.SetValue(instance, inject_data.Vector2IntData);
                                        break;
                                    case UIEntity.InjectDataType.Vector3:
                                        prop.SetValue(instance, inject_data.Vector3Data);
                                        break;
                                    case UIEntity.InjectDataType.Vector3Int:
                                        prop.SetValue(instance, inject_data.Vector3IntData);
                                        break;
                                    case UIEntity.InjectDataType.Quaternion:
                                        prop.SetValue(instance, inject_data.QuaternionData);
                                        break;
                                    case UIEntity.InjectDataType.Sprite:
                                        prop.SetValue(instance, inject_data.SpriteData);
                                        break;

                                }
                            }
                            catch (ArgumentException e)
                            {
                                Debug.LogError($"[TinaX.UIkits] Inject UI object \"{inject_obj.Name}\" to UIController field \"{prop.Name}\" Error: {e.Message}");
                            }
                            finally
                            {
                                injected = true;
                            }
                        }
                    }

                    if (!injected)
                    {
                        Debug.LogWarning($"[TinaX.UIkits] Inject UI object \"{attr.InjectName}\" failed. Can't found Inject Object From UIEntity by this injectName.\n注入UI对象\"{attr.InjectName}\"失败，因为在UIEntity里没找到叫这个名字的可注入对象");
                    }
                }
            }

            var methods = ctrlType.GetMethods(BindingFlags.Public | BindingFlags.Instance);
            foreach(var method in methods)
            {
                //Button OnClick
                var btn_attr = method.GetCustomAttribute<ButtonOnClickAttribute>();
                if(btn_attr != null && method.GetParameters().Length == 0)
                {
                    //找有没有
                    var inject_obj = entity.GetInjectObject(btn_attr.InjectButtonName);
                    if(inject_obj != null && inject_obj.Object is UnityEngine.UI.Button)
                    {
                        var btn = inject_obj.Object as UnityEngine.UI.Button;
                        btn.onClick.RemoveAllListeners();
                        //Debug.Log("添加监听:" + method.Name);
                        btn.onClick.AddListener(() =>
                        {
                            method.Invoke(instance, null);
                        });
                    }
                }
            }

            var ctrl = (UIController)instance;

            ctrl.entity = entity;

            #region Updates 方法

            var class_attr = ctrlType.GetCustomAttribute<UIControllerAttribute>();
            if(class_attr != null)
            {
                ctrl.EnableUpdate = class_attr.EnableUpdate;
                ctrl.UpdateOrder = class_attr.UpdateOrder;

                ctrl.EnableFixedUpdate = class_attr.EnableFixedUpdate;
                ctrl.FixedUpdateOrder = class_attr.FixedUpdateOrder;

                ctrl.EnableLateUpdate = class_attr.EnableLateUpdate;
                ctrl.LateUpdateOrder = class_attr.LateUpdateOrder;

                ctrl.PauseUpdatesWhenDisable = class_attr.PauseUpdatesWhenDisable;
            }

            #endregion


            //把类交给UIEntity
            entity.SetupUIController(ctrl);
        }

        private string GetUIVFSPath(string ui_name, string ui_path)
        {
            if (ui_name.IsNullOrEmpty() && ui_path.IsNullOrEmpty())
            {
                throw new ArgumentNullException("UI name and UI path is invalid.");
            }

            string ui_vfs_path = ui_path;

            if (!ui_name.IsNullOrEmpty() && mCurUIGroup != null)
            {
                ui_vfs_path = mCurUIGroup.GetPathByName(ui_name);
                if (ui_vfs_path.IsNullOrEmpty())
                {
                    throw new Exceptions.UIKitException(Exceptions.UIKitException.ErrorType.UINameInvalid, $"UI name invalid in current ui group config: \"{ui_name}\"");
                }
            }
            else if (!ui_path.IsNullOrEmpty())
            {
                ui_vfs_path = ui_path;
            }
            else if (!ui_name.IsNullOrEmpty() && ui_path.IsNullOrEmpty() && mCurUIGroup == null)
            {
                throw new Exceptions.UIKitException(Exceptions.UIKitException.ErrorType.UIGroupInvalid, $"UI group object is null");
            }

            return ui_vfs_path;
        }

        private GameObject LoadUIFileByVFS(string vfs_path) //返回prefab
        {
            //加载尝试
            try
            {
                var prefab = mVFS.LoadAsset<GameObject>(vfs_path);
                return prefab;
            }
            catch(Exceptions.VFSException e)
            {
                if(e.ErrorType == Exceptions.VFSException.VFSErrorType.FileNotExist)
                {
                    throw new Exceptions.UIKitException(Exceptions.UIKitException.ErrorType.UIFileNotFound, "ui path not found in vfs :" + vfs_path);
                }
                else if(e.ErrorType == Exceptions.VFSException.VFSErrorType.PathNotValid)
                {
                    throw new Exceptions.UIKitException(Exceptions.UIKitException.ErrorType.UIPathInvalid, "ui path invalid:" + vfs_path);
                }
                else
                {
                    throw e;
                }
            }
            catch(Exception e)
            {
                throw e;
            }

        }


        private async Task<GameObject> LoadUIFileByVFSAsync(string ui_name, string ui_path) //返回prefab
        {
            if (ui_name.IsNullOrEmpty() && ui_path.IsNullOrEmpty())
            {
                throw new ArgumentNullException("UI name and UI path is invalid.");
            }

            string ui_vfs_path = ui_path;

            if (!ui_name.IsNullOrEmpty() && mCurUIGroup != null)
            {
                ui_vfs_path = mCurUIGroup.GetPathByName(ui_name);
                if (ui_vfs_path.IsNullOrEmpty())
                {
                    throw new Exceptions.UIKitException(Exceptions.UIKitException.ErrorType.UINameInvalid, $"UI name invalid in current ui group config: \"{ui_name}\"");
                }
            }
            else if (!ui_path.IsNullOrEmpty())
            {
                ui_vfs_path = ui_path;
            }
            else if (!ui_name.IsNullOrEmpty() && ui_path.IsNullOrEmpty() && mCurUIGroup == null)
            {
                throw new Exceptions.UIKitException(Exceptions.UIKitException.ErrorType.UIGroupInvalid, $"UI group object is null");
            }


            //加载尝试
            try
            {
                var prefab = await mVFS.LoadAssetLocalOrWebAsync<GameObject>(ui_vfs_path);
                return prefab;
            }
            catch (Exceptions.VFSException e)
            {
                if (e.ErrorType == Exceptions.VFSException.VFSErrorType.FileNotExist)
                {
                    throw new Exceptions.UIKitException(Exceptions.UIKitException.ErrorType.UIFileNotFound, "ui path not found in vfs :" + ui_vfs_path);
                }
                else if (e.ErrorType == Exceptions.VFSException.VFSErrorType.PathNotValid)
                {
                    throw new Exceptions.UIKitException(Exceptions.UIKitException.ErrorType.UIPathInvalid, "ui path invalid:" + ui_vfs_path);
                }
                else
                {
                    throw e;
                }
            }
            catch (Exception e)
            {
                throw e;
            }

        }


    }
}
