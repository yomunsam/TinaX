using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TinaX.UIKit
{
    public interface IUIKit
    {

        /// <summary>
        /// UI管理器是否为高级模式
        /// </summary>
        bool IsAdvanced { get; }

        /// <summary>
        /// UIRoot的RectTransform
        /// </summary>
        RectTransform UIKit_UIRoot_RectTrans { get; }

        /// <summary>
        /// UI安全区管理器
        /// </summary>
        XUISafeAreaMgr UISafeAreaManager { get; }

        /// <summary>
        /// UICamera's GameObject
        /// </summary>
        GameObject UIKit_UICamera_GameObject { get; }

        /// <summary>
        /// UIKit UI Camera
        /// </summary>
        Camera UIKit_UICamera { get; }


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
        IUIEntity OpenUI(string ui_name, System.Object ui_param);

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
        IUIEntity OpenUI(string ui_name, System.Object ui_param, bool use_mask, bool close_by_mask = false);


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
        IUIEntity OpenUIByPath(string ui_path, System.Object ui_param, bool use_mask, bool close_by_mask = false);

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

#if TinaX_CA_LuaRuntime_Enable

        /// <summary>
        /// 打开一个子级UI，给TinaX内部用的
        /// </summary>
        /// <param name="ui_name"></param>
        /// <param name="parent_id"></param>
        /// <param name="ui_param"></param>
        /// <param name="lua_param"></param>
        /// <param name="use_mask"></param>
        /// <param name="close_by_mask"></param>
        /// <returns></returns>
        IUIEntity OpenUIChild(string ui_name, int parent_id, System.Object ui_param, XLua.LuaTable lua_param, bool use_mask, bool close_by_mask = false);

#else
        /// <summary>
        /// 打开一个子级UI，给TinaX内部用的
        /// </summary>
        /// <param name="ui_name"></param>
        /// <param name="parent_id"></param>
        /// <param name="ui_param"></param>
        /// <param name="lua_param"></param>
        /// <param name="use_mask"></param>
        /// <param name="close_by_mask"></param>
        /// <returns></returns>
        IUIEntity OpenUIChild(string ui_name, int parent_id, System.Object ui_param, bool use_mask, bool close_by_mask = false);
#endif

        /// <summary>
        /// 关闭UI【高级模式不可用】
        /// </summary>
        /// <param name="ui_name">UI名</param>
        /// <param name="Param">UI关闭参数，可空</param>
        void CloseUI(string ui_name, System.Object Param = null);

        /// <summary>
        /// 关闭UI【仅高级模式】
        /// </summary>
        /// <param name="ui_id">UI实例 句柄ID</param>
        /// <param name="Param">UI关闭参数, 可空</param>
        void CloseUI(int ui_id, System.Object Param = null);

        /// <summary>
        /// 通过路径关闭UI【高级模式不可用】
        /// </summary>
        /// <param name="ui_path"></param>
        /// <param name="Param"></param>
        void CloseUIByPath(string ui_path, System.Object Param = null);

#if TinaX_CA_LuaRuntime_Enable

        /// <summary>
        /// 关闭UI并传递Lua参数【Lua封装调用】
        /// </summary>
        /// <param name="ui_name"></param>
        /// <param name="luaParam"></param>
        void CloseUIWithLuaParam(string ui_name, XLua.LuaTable luaParam);


        /// <summary>
        /// 关闭UI并传递Lua参数【Lua封装】【仅高级模式】
        /// </summary>
        /// <param name="ui_id"></param>
        /// <param name="luaParam"></param>
        void CloseUIWithLuaParam(int ui_id, XLua.LuaTable luaParam);

        /// <summary>
        /// 通过路径关闭UI并传递Lua参数【Lua封装用】
        /// </summary>
        /// <param name="ui_path"></param>
        /// <param name="luaParam"></param>
        void CloseUIByPathWithLuaParam(string ui_path, XLua.LuaTable luaParam);

#endif

        /// <summary>
        /// 隐藏UI【非高级模式】
        /// </summary>
        /// <param name="ui_name"></param>
        void HideUI(string ui_name);

        /// <summary>
        /// 通过路径隐藏UI【非高级模式】
        /// </summary>
        /// <param name="ui_path"></param>
        void HideUIByPath(string ui_path);

        /// <summary>
        /// 隐藏UI【高级模式】
        /// </summary>
        /// <param name="id"></param>
        void HideUI(int id);

        /// <summary>
        /// 显示UI【高级模式】
        /// </summary>
        /// <param name="ui_id"></param>
        void ShowUI(int ui_id);

        /// <summary>
        /// 显示UI
        /// </summary>
        /// <param name="ui_name"></param>
        void ShowUI(string ui_name);

        /// <summary>
        /// 显示UI
        /// </summary>
        /// <param name="ui_path"></param>
        void ShowUIByPath(string ui_path);

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

