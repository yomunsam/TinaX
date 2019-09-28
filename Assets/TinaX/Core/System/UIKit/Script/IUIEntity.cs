using System;

using UnityEngine;

namespace TinaX.UIKits
{
    public interface IUIEntity
    {
        /// <summary>
        /// 手动设定层级，改设置不会记录进UI管理器
        /// </summary>
        /// <param name="layer"></param>
        /// <returns></returns>
        int SetLayer(int layer);

        /// <summary>
        /// 获取当前UI的GameObject
        /// </summary>
        /// <returns></returns>
        GameObject GetGameObject();

        /// <summary>
        /// 获取UI名，如果未使用UI组系统载入的UI则显示“Unknow”
        /// </summary>
        /// <returns></returns>
        string GetUIName();

        /// <summary>
        /// 获取UI路径（在可寻址系统中）
        /// </summary>
        /// <returns></returns>
        string GetUIPath();

        
        int GetID();



        #region UI操作

        /// <summary>
        /// 隐藏自身UI
        /// </summary>
        /// <returns></returns>
        IUIEntity Hide();

        /// <summary>
        /// 显示被隐藏的自己
        /// </summary>
        /// <returns></returns>
        IUIEntity Show();

        /// <summary>
        /// 关掉自己
        /// </summary>
        void Close();

        #endregion


        #region 高级模式 UI操作

        /// <summary>
        /// 打开UI,在高级模式下，被打开的UI被认为是当前UI的子级UI
        /// </summary>
        /// <param name="ui_name">UI名</param>
        /// <param name="Param">附加参数</param>
        /// <returns>子UI的实体对象</returns>
        IUIEntity OpenUI(string ui_name, System.Object Param = null);

        #endregion

    }
}
