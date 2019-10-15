using System;
using UnityEngine;

namespace TinaX.UIKits
{
    public interface IUIKit
    {

        GameObject UIRootGameObject { get; }
        GameObject UICameraGameObject { get; }
        Camera UICamera { get; }
        Canvas UIRootCanvas { get; }
        RectTransform UIRootRectTransform { get; }
        Transform UIRootTransform { get; }

        /// <summary>
        /// 打开UI
        /// </summary>
        /// <param name="ui_name">UI名称</param>
        /// <returns></returns>
        IUIEntity OpenUI(string ui_name);


        /// <summary>
        /// 打开UI
        /// </summary>
        /// <param name="ui_name">UI名称</param>
        /// <param name="ui_param">UI启动参数</param>
        /// <returns></returns>
        IUIEntity OpenUI(string ui_name, object ui_param);

        /// <summary>
        /// 打开UI
        /// </summary>
        /// <param name="ui_name">UI名</param>
        /// <param name="use_mask">是否使用UI遮罩</param>
        /// <param name="close_by_mask">点击遮罩关闭UI</param>
        /// <returns></returns>
        IUIEntity OpenUI(string ui_name, bool use_mask, bool close_by_mask);

        /// <summary>
        /// 打开UI
        /// </summary>
        /// <param name="ui_name">UI名称</param>
        /// <param name="ui_param">UI启动参数</param>
        /// <param name="use_mask">是否使用遮罩</param>
        /// <param name="close_by_mask">点击遮罩关闭UI</param>
        /// <returns></returns>
        IUIEntity OpenUI(string ui_name, object ui_param, bool use_mask, bool close_by_mask = false);

        /// <summary>
        /// Open UI
        /// </summary>
        /// <param name="ui_name"></param>
        /// <param name="ControllerType">ui controller type</param>
        /// <returns></returns>
        IUIEntity OpenUI(string ui_name, Type ControllerType);

        IUIEntity OpenUI(Type ControllerType, OpenUIParam datas);

        IUIEntity OpenUI<TController>(string ui_name) where TController : UIController;

        IUIEntity OpenUI<TController>() where TController : UIController;

        IUIEntity OpenUI<TController>(object param) where TController : UIController;

        IUIEntity OpenUI<TController>(OpenUIParam datas) where TController : UIController;

#if TinaX_CA_LuaRuntime_Enable

        /// <summary>
        /// 打开UI 供Lua调用
        /// </summary>
        /// <param name="ui_name">UI名</param>
        /// <param name="lua_param">lua table参数</param>
        /// <param name="use_mask">使用遮罩</param>
        /// <param name="close_by_mask">点击遮罩关闭UI</param>
        /// <returns></returns>
        IUIEntity OpenUIWhitLuaParam(string ui_name, XLua.LuaTable lua_param, bool use_mask, bool close_by_mask);

#endif

        /// <summary>
        /// 通过VFS路径打开UI
        /// </summary>
        /// <param name="ui_path">UI路径</param>
        /// <param name="ui_param">UI 启动参数</param>
        /// <param name="use_mask">是否启用遮罩</param>
        /// <param name="close_by_mask">点击遮罩关闭UI</param>
        /// <returns></returns>
        IUIEntity OpenUIByPath(string ui_path, object ui_param, bool use_mask, bool close_by_mask = false);

#if TinaX_CA_LuaRuntime_Enable

        /// <summary>
        /// 通过VFS路径打开UI 附带LuaTable参数
        /// </summary>
        /// <param name="ui_path">UI路径</param>
        /// <param name="lua_param">UI 启动参数</param>
        /// <param name="use_mask">是否启用遮罩</param>
        /// <param name="close_by_mask">点击遮罩关闭UI</param>
        /// <returns></returns>
        IUIEntity OpenUIByPathWhitLuaParam(string ui_path, XLua.LuaTable lua_param, bool use_mask, bool close_by_mask = false);

#endif


        /// <summary>
        /// 关闭UI
        /// </summary>
        /// <param name="ui_name">UI名</param>
        /// <param name="Param">UI关闭参数，可空</param>
        [Obsolete("It is no longer recommended to close UI by UI name or UI path, Please use UI handle ID, or close UI by IUIEntity")]
        void CloseUI(string ui_name, System.Object Param = null);

        /// <summary>
        /// 关闭UI
        /// </summary>
        /// <param name="UI_Id">UI实例 句柄ID</param>
        /// <param name="Param">UI关闭参数, 可空</param>
        void CloseUI(ulong UI_Id, System.Object Param = null);

        /// <summary>
        /// 通过路径关闭UI
        /// </summary>
        /// <param name="ui_path"></param>
        /// <param name="Param"></param>
        [Obsolete("It is no longer recommended to close UI by UI name or UI path, Please use UI handle ID, or close UI by IUIEntity")]
        void CloseUIByPath(string ui_path, object Param = null);

#if TinaX_CA_LuaRuntime_Enable

        /// <summary>
        /// 关闭UI并传递Lua参数
        /// </summary>
        /// <param name="ui_name"></param>
        /// <param name="luaParam"></param>
        [Obsolete("It is no longer recommended to close UI by UI name or UI path, Please use UI handle ID, or close UI by IUIEntity")]
        void CloseUIWithLuaParam(string ui_name, XLua.LuaTable luaParam);


        /// <summary>
        /// 关闭UI并传递Lua参数
        /// </summary>
        /// <param name="ui_id"></param>
        /// <param name="luaParam"></param>
        void CloseUIWithLuaParam(ulong ui_id, XLua.LuaTable luaParam);

        /// <summary>
        /// 通过路径关闭UI并传递Lua参数
        /// </summary>
        /// <param name="ui_path"></param>
        /// <param name="luaParam"></param>
        [Obsolete("It is no longer recommended to close UI by UI name or UI path, Please use UI handle ID, or close UI by IUIEntity")]
        void CloseUIByPathWithLuaParam(string ui_path, XLua.LuaTable luaParam);

#endif

        /// <summary>
        /// 隐藏UI
        /// </summary>
        /// <param name="ui_name"></param>
        [Obsolete("It is no longer recommended to hide UI by UI name or UI path, Please use UI handle ID, or close UI by IUIEntity")]
        void HideUI(string ui_name , bool IncludeChild = true);

        /// <summary>
        /// 通过路径隐藏UI
        /// </summary>
        /// <param name="ui_path"></param>
        [Obsolete("It is no longer recommended to hide UI by UI name or UI path, Please use UI handle ID, or close UI by IUIEntity")]
        void HideUIByPath(string ui_path, bool IncludeChild = true);

        /// <summary>
        /// 隐藏UI
        /// </summary>
        /// <param name="id"></param>
        void HideUI(ulong id, bool IncludeChild = true);

        /// <summary>
        /// 显示UI
        /// </summary>
        /// <param name="ui_id"></param>
        void ShowUI(ulong ui_id, bool IncludeChild = true);

        /// <summary>
        /// 显示UI
        /// </summary>
        /// <param name="ui_name"></param>
        [Obsolete("It is no longer recommended to show UI by UI name or UI path, Please use UI handle ID, or close UI by IUIEntity")]
        void ShowUI(string ui_name, bool IncludeChild = true);

        /// <summary>
        /// 显示UI
        /// </summary>
        /// <param name="ui_path"></param>
        [Obsolete("It is no longer recommended to show UI by UI name or UI path, Please use UI handle ID, or close UI by IUIEntity")]
        void ShowUIByPath(string ui_path, bool IncludeChild = true);



        void SetUIlayerTop(ulong id);

        /// <summary>
        /// 设置当前使用的UI组
        /// </summary>
        /// <param name="ui_group">UI组</param>
        void SetUIGroup(UIGroupConf ui_group);

        /// <summary>
        /// 获取某个UI元素相对UIKit根节点的相对坐标，成功返回坐标，失败返回Vector2.zero
        /// </summary>
        /// <param name="trans">UI元素的</param>
        /// <returns></returns>
        Vector2 GetUIScreenLocalPoint(Transform trans);

    }
}

