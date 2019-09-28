using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TinaX.Conf;
using TinaX;

namespace TinaX.UIKits
{
    public class XUIManager
    {
        private XUIMgrGateway mGateway;
        private IVFS mVFS;

        /// <summary>
        /// 简单模式的UI对象存储池
        /// key为UI路径
        /// </summary>
        private Dictionary<string, UIItem> mUIPool = new Dictionary<string, UIItem>();



        public XUIManager(XUIMgrGateway _Gateway, IVFS _vfs)
        {
            mGateway = _Gateway;
            mVFS = _vfs;
        }


#if TinaX_CA_LuaRuntime_Enable
        /// <summary>
        /// 打开UI
        /// </summary>
        public IUIEntity OpenUI(string ui_name,bool use_mask, bool close_by_mask, System.Object OpenParam = null, XLua.LuaTable LuaParam = null)
        {
            //ui_name to ui_path
            var uiPath = mGateway.UIKit_DefaultUIGroup.GetPathByName(ui_name);
            if (uiPath.IsNullOrEmpty())
            {
                XLog.PrintE("[TinaX][UIKit]打开UI失败，无法通过UI名：" + ui_name + " 获取到对应的UI路径");
                return null;
            }

            return POpenUI(uiPath, use_mask, close_by_mask, ui_name,OpenParam,LuaParam);
        }

#else

        /// <summary>
        /// 打开UI
        /// </summary>
        public IUIEntity OpenUI(string ui_name, bool use_mask, bool close_by_mask, System.Object OpenParam = null)
        {
            //ui_name to ui_path
            var uiPath = mGateway.UIKit_DefaultUIGroup.GetPathByName(ui_name);
            if (uiPath.IsNullOrEmpty())
            {
                XLog.PrintE("[TinaX][UIKit]打开UI失败，无法通过UI名：" + ui_name + " 获取到对应的UI路径");
                return null;
            }

            return POpenUI(uiPath, use_mask, close_by_mask, ui_name, OpenParam);
        }
#endif

#if TinaX_CA_LuaRuntime_Enable
        public IUIEntity OpenUIByPath(string ui_path, bool use_mask, bool close_by_mask, System.Object OpenParam = null, XLua.LuaTable LuaParam = null)
        {
            return POpenUI(ui_path, use_mask, close_by_mask, "unknow", OpenParam, LuaParam);
        }
#else
        public IUIEntity OpenUIByPath(string ui_path, bool use_mask, bool close_by_mask, System.Object OpenParam = null)
        {
            return POpenUI(ui_path, use_mask, close_by_mask, "unknow", OpenParam);
        }

#endif


#if TinaX_CA_LuaRuntime_Enable
        public void CloseUI(string ui_name, System.Object close_param = null, XLua.LuaTable LuaParam = null)
        {
            var uiPath = mGateway.UIKit_DefaultUIGroup.GetPathByName(ui_name);
            if (uiPath.IsNullOrEmpty())
            {
                XLog.PrintE("[TinaX][UIKit]关闭UI失败，无法通过UI名：" + ui_name + " 获取到对应的UI路径");
                return;
            }

            PCloseUI(uiPath, close_param: close_param,lua_close_param:LuaParam);
        }

#else
        public void CloseUI(string ui_name, System.Object close_param = null)
        {
            var uiPath = mGateway.UIKit_DefaultUIGroup.GetPathByName(ui_name);
            if (uiPath.IsNullOrEmpty())
            {
                XLog.PrintE("[TinaX][UIKit]关闭UI失败，无法通过UI名：" + ui_name + " 获取到对应的UI路径");
                return;
            }

            PCloseUI(uiPath, close_param: close_param);
        }
#endif


#if TinaX_CA_LuaRuntime_Enable
        public void CloseUIByPath(string ui_path, System.Object close_param = null, XLua.LuaTable LuaParam = null)
        {
            PCloseUI(ui_path, close_param: close_param, lua_close_param: LuaParam);
        }
#else
        public void CloseUIByPath(string ui_path, System.Object close_param = null)
        {
            PCloseUI(ui_path, close_param: close_param);
        }
#endif

        public void ShowUI(string ui_name)
        {
            var uiPath = mGateway.UIKit_DefaultUIGroup.GetPathByName(ui_name);
            if (uiPath.IsNullOrEmpty())
            {
                XLog.PrintE("[TinaX][UIKit]操作UI失败，无法通过UI名：" + ui_name + " 获取到对应的UI路径");
                return;
            }

            PShowUI(uiPath);
        }

        public void ShowUIByPath(string ui_path)
        {
            PShowUI(ui_path);
        }

        public void HideUI(string ui_name)
        {
            var uiPath = mGateway.UIKit_DefaultUIGroup.GetPathByName(ui_name);
            if (uiPath.IsNullOrEmpty())
            {
                XLog.PrintE("[TinaX][UIKit]操作UI失败，无法通过UI名：" + ui_name + " 获取到对应的UI路径");
                return;
            }

            PHideUI(uiPath);
        }

        public void HideUIByPath(string ui_path)
        {
            PHideUI(ui_path);
        }

        #region 私有方法_UI操作

        

#if TinaX_CA_LuaRuntime_Enable
        /// <summary>
        /// 打开UI
        /// </summary>
        /// <param name="ui_path">UI路径</param>
        private IUIEntity POpenUI(string ui_path,bool use_mask, bool close_by_mask, string uiName = "unknow", System.Object OpenParam = null, XLua.LuaTable LuaParam = null)
        {
            //检查UI对象是否已存在
            if (mUIPool.ContainsKey(ui_path))
            {
                //存在，直接置顶
                mGateway.SetUILayerTop(mUIPool[ui_path].GetUIEntity());
                return mUIPool[ui_path].GetUIEntityAPI();
            }
            else
            {
                //不存在，打开流程
                var ui_prefab = mVFS.LoadAsset<GameObject>(ui_path);
                //创建UI对象
                var ui_item = new UIItem(ui_prefab, ui_path, mGateway);
                //初始化UI
                ui_item.InitUI();   //UI对象内部自己处理层级
                //遮罩处理
                if (use_mask)
                {
                    ui_item.UseMask(close_by_mask);
                }
                //记录UI
                mUIPool.Add(ui_path, ui_item);

                //处理Entity的事件
                ui_item.GetUIEntity().OnUICreated(ui_path, uiName);
                //处理打开UI参数传递
                if (OpenParam != null)
                {
                    ui_item.GetUIEntity().OnUIOpenMessage(OpenParam);
                }

                if (LuaParam != null)
                {
                    ui_item.GetUIEntity().OnUIOpenMessage_LuaTable(LuaParam);
                }


                //返回
                return ui_item.GetUIEntityAPI();
            }

            

        }

#else
        /// <summary>
        /// 打开UI
        /// </summary>
        /// <param name="ui_path">UI路径</param>
        private IUIEntity POpenUI(string ui_path, bool use_mask, bool close_by_mask, string uiName = "unknow", System.Object OpenParam = null)
        {
            //检查UI对象是否已存在
            if (mUIPool.ContainsKey(ui_path))
            {
                //存在，直接置顶
                mGateway.SetUILayerTop(mUIPool[ui_path].GetUIEntity());
                return mUIPool[ui_path].GetUIEntityAPI();
            }
            else
            {
                //不存在，打开流程
                var ui_prefab = mVFS.LoadAsset<GameObject>(ui_path);
                //创建UI对象
                var ui_item = new UIItem(ui_prefab, ui_path, mGateway);
                //初始化UI
                ui_item.InitUI();   //UI对象内部自己处理层级
                //遮罩处理
                if (use_mask)
                {
                    ui_item.UseMask(close_by_mask);
                }
                //记录UI
                mUIPool.Add(ui_path, ui_item);

                //处理Entity的事件
                ui_item.GetUIEntity().OnUICreated(ui_path, uiName);
                //处理打开UI参数传递
                if (OpenParam != null)
                {
                    ui_item.GetUIEntity().OnUIOpenMessage(OpenParam);
                }


                //返回
                return ui_item.GetUIEntityAPI();
            }



        }
#endif

#if TinaX_CA_LuaRuntime_Enable
        private void PCloseUI(string ui_path, System.Object close_param = null, XLua.LuaTable lua_close_param = null)
        {
            //UI是否存在
            if (mUIPool.ContainsKey(ui_path))
            {
                //存在
                if (close_param != null)
                {
                    mUIPool[ui_path].GetUIEntity().OnUICloseMessage(close_param);
                }

                if (lua_close_param != null)
                {
                    mUIPool[ui_path].GetUIEntity().OnUICloseMessage_LuaTable(lua_close_param);
                }


                mUIPool[ui_path].CloseUI();

                //打扫一下现场
                mUIPool.Remove(ui_path);
            }
        }
#else
        private void PCloseUI(string ui_path, System.Object close_param = null)
        {
            //UI是否存在
            if (mUIPool.ContainsKey(ui_path))
            {
                //存在
                if (close_param != null)
                {
                    mUIPool[ui_path].GetUIEntity().OnUICloseMessage(close_param);
                }

                


                mUIPool[ui_path].CloseUI();

                //打扫一下现场
                mUIPool.Remove(ui_path);
            }
        }
#endif

        private void PHideUI(string ui_path)
        {
            PGetUIEntityAPI(ui_path)?.GetGameObject().Hide();
        }

        private void PShowUI(string ui_path)
        {
            PGetUIEntityAPI(ui_path)?.GetGameObject().Show();
        }

        private IUIEntity PGetUIEntityAPI(string ui_path)
        {
            if (mUIPool.ContainsKey(ui_path))
            {
                return mUIPool[ui_path].GetUIEntityAPI();
            }
            else
            {
                return null;
            }
        }
#endregion




    }
}

