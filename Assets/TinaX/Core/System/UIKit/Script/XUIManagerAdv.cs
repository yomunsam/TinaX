using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using TinaX;


namespace TinaX.UIKit
{
    /// <summary>
    /// Advanced 高级UI管理器
    /// </summary>
    public class XUIManagerAdv
    {
        private XUIMgrGateway mGateway;

        /// <summary>
        /// 简单模式的UI对象存储池
        /// 路径与UI不完全一一对应，
        /// 使用句柄ID作为Key
        /// </summary>
        private Dictionary<int, UIItem> mUIPool = new Dictionary<int, UIItem>();

        public XUIManagerAdv(XUIMgrGateway _Gateway)
        {
            mGateway = _Gateway;
        }

#if TinaX_CA_LuaRuntime_Enable

        public IUIEntity OpenUI(string ui_name, bool use_mask, bool close_by_mask,int ParentID = -1, System.Object OpenParam = null, XLua.LuaTable LuaParam = null)
        {
            //ui_name to ui_path
            var uiPath = mGateway.UIKit_DefaultUIGroup.GetPathByName(ui_name);
            if (uiPath.IsNullOrEmpty())
            {
                XLog.PrintE("[TinaX][UIKit]打开UI失败，无法通过UI名：" + ui_name + " 获取到对应的UI路径");
                return null;
            }

            return POpenUI(uiPath, use_mask, close_by_mask, ParentID, ui_name,OpenParam,LuaParam);

        }

#else
        public IUIEntity OpenUI(string ui_name, bool use_mask, bool close_by_mask, int ParentID = -1, System.Object OpenParam = null)
        {
            //ui_name to ui_path
            var uiPath = mGateway.UIKit_DefaultUIGroup.GetPathByName(ui_name);
            if (uiPath.IsNullOrEmpty())
            {
                XLog.PrintE("[TinaX][UIKit]打开UI失败，无法通过UI名：" + ui_name + " 获取到对应的UI路径");
                return null;
            }

            return POpenUI(uiPath, use_mask, close_by_mask, ParentID, ui_name, OpenParam);

        }

#endif


#if TinaX_CA_LuaRuntime_Enable
        public IUIEntity OpenUIByPath(string ui_path, bool use_mask, bool close_by_mask, int ParentID = -1, System.Object OpenParam = null, XLua.LuaTable LuaParam = null)
        {
            return POpenUI(ui_path, use_mask, close_by_mask, ParentID, "unknow", OpenParam, LuaParam);
        }

#else
        public IUIEntity OpenUIByPath(string ui_path, bool use_mask, bool close_by_mask, int ParentID = -1, System.Object OpenParam = null)
        {
            return POpenUI(ui_path, use_mask, close_by_mask, ParentID, "unknow", OpenParam);
        }
#endif


#if TinaX_CA_LuaRuntime_Enable
        public void CloseUI(int id, System.Object close_param = null, XLua.LuaTable LuaParam = null)
        {
            PCloseUI(id, close_param: close_param,lua_close_param:LuaParam);
        }
#else
        public void CloseUI(int id, System.Object close_param = null)
        {
            PCloseUI(id, close_param: close_param);
        }
#endif

        public void HideUI(int id)
        {
            PHideUI(id);
        }

        public void ShowUI(int id)
        {
            PShowUI(id);
        }

        #region 私有方法_UI操作

#if TinaX_CA_LuaRuntime_Enable
        private IUIEntity POpenUI(string ui_path, bool use_mask,bool close_by_mask, int ParentID = -1,string uiName = "unknow",System.Object OpenParam = null,XLua.LuaTable LuaParam = null)
        {
            //先找找这个UI在不在
            foreach(var item in mUIPool)
            {
                if(item.Value.UIPath == ui_path)
                {
                    //存在了
                    if (!item.Value.IsMultiUI)
                    {
                        //置顶UI并返回

                        return item.Value.GetUIEntityAPI();
                    }
                    break;
                }
            }

            //走到这儿就说明肯定得新建UI了
            var ui_prefab = AssetsMgr.I.LoadAsset<GameObject>(ui_path);
            //创建UI对象
            var ui_item = new UIItem(ui_prefab, ui_path, mGateway);
            //初始化UI
            ui_item.InitUI();   //UI对象内部自己处理层级
            //分配ID
            var ui_id = GetFreeUIPoolID();
            ui_item.SetID(ui_id);
            //如有父UI，记录
            if(ParentID != -1)
            {
                if (mUIPool.ContainsKey(ParentID))
                {
                    mUIPool[ParentID].AddChild(ui_item.GetUIEntity());
                }
            }

            //遮罩处理
            if (use_mask)
            {
                ui_item.UseMask(close_by_mask);
            }

            //记录UI
            mUIPool.Add(ui_id, ui_item);

            //处理Entity的事件
            ui_item.GetUIEntity().OnUICreated(ui_path, uiName);
            //处理打开UI参数传递
            if(OpenParam != null)
            {
                ui_item.GetUIEntity().OnUIOpenMessage(OpenParam);
            }

            if(LuaParam != null)
            {
                ui_item.GetUIEntity().OnUIOpenMessage_LuaTable(LuaParam);
            }



            //返回
            return ui_item.GetUIEntityAPI();

        }

#else
        private IUIEntity POpenUI(string ui_path, bool use_mask, bool close_by_mask, int ParentID = -1, string uiName = "unknow", System.Object OpenParam = null)
        {
            //先找找这个UI在不在
            foreach (var item in mUIPool)
            {
                if (item.Value.UIPath == ui_path)
                {
                    //存在了
                    if (!item.Value.IsMultiUI)
                    {
                        //置顶UI并返回

                        return item.Value.GetUIEntityAPI();
                    }
                    break;
                }
            }

            //走到这儿就说明肯定得新建UI了
            var ui_prefab = AssetsMgr.I.LoadAsset<GameObject>(ui_path);
            //创建UI对象
            var ui_item = new UIItem(ui_prefab, ui_path, mGateway);
            //初始化UI
            ui_item.InitUI();   //UI对象内部自己处理层级
            //分配ID
            var ui_id = GetFreeUIPoolID();
            ui_item.SetID(ui_id);
            //如有父UI，记录
            if (ParentID != -1)
            {
                if (mUIPool.ContainsKey(ParentID))
                {
                    mUIPool[ParentID].AddChild(ui_item.GetUIEntity());
                }
            }

            //遮罩处理
            if (use_mask)
            {
                ui_item.UseMask(close_by_mask);
            }

            //记录UI
            mUIPool.Add(ui_id, ui_item);

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
#endif

#if TinaX_CA_LuaRuntime_Enable
        //关闭UI
        private void PCloseUI(int id,System.Object close_param = null, XLua.LuaTable lua_close_param = null)
        {
            //UI是否存在
            if (mUIPool.ContainsKey(id))
            {
                //有，关吧
                //“你还有什么话要说吗”
                if(close_param != null)
                {
                    //"我有话说"
                    mUIPool[id].GetUIEntity().OnUICloseMessage(close_param);
                }

                if(lua_close_param != null)
                {
                    mUIPool[id].GetUIEntity().OnUICloseMessage_LuaTable(lua_close_param);
                }


                //“说完了，那就上路吧”
                mUIPool[id].CloseUI();

                //打扫一下现场
                mUIPool.Remove(id);
            }
        }

#else
        //关闭UI
        private void PCloseUI(int id, System.Object close_param = null)
        {
            //UI是否存在
            if (mUIPool.ContainsKey(id))
            {
                //有，关吧
                //“你还有什么话要说吗”
                if (close_param != null)
                {
                    //"我有话说"
                    mUIPool[id].GetUIEntity().OnUICloseMessage(close_param);
                }


                //“说完了，那就上路吧”
                mUIPool[id].CloseUI();

                //打扫一下现场
                mUIPool.Remove(id);
            }
        }
#endif


        private void PHideUI(int id)
        {
            PGetUIEntityAPI(id)?.GetGameObject().Hide();
        }

        private void PShowUI(int id)
        {
            PGetUIEntityAPI(id)?.GetGameObject().Show();
        }

        private IUIEntity PGetUIEntityAPI(int id)
        {
            if (mUIPool.ContainsKey(id))
            {
                return mUIPool[id].GetUIEntityAPI();
            }
            return null;
        }

#endregion

#region UIPool_ID处理

        private int GetFreeUIPoolID() //获取一个可用的空闲ID
        {
            int id = UnityEngine.Random.Range(9, 999);
            while (mUIPool.ContainsKey(id))
            {
                id++;
            }
            return id;
        }


#endregion

    }

}
