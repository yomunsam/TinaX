using System;

using UnityEngine;

namespace TinaX.UIKits
{
    public interface IUIEntity
    {


        #region 数据获取

        /// <summary>
        /// UI Path
        /// </summary>
        string UIPath { get; }
        
        /// <summary>
        /// UI name , if open UI by path , is null.
        /// </summary>
        string UIName { get; }

        /// <summary>
        /// UI Id
        /// </summary>
        ulong ID { get;}

        /// <summary>
        /// UI Layer  (sorting order)
        /// </summary>
        int LayerIndex { get; }

        Canvas UICanvas { get; }

        ulong GetChildID(int index);

        int childCount { get; }

        UIEntity.Injection GetInjectObject(string Name);

        UIEntity.Injection_Data GetInjectData(string Name);

        #endregion

        /// <summary>
        /// Close this UI
        /// </summary>
        void Close();


        void Hide();

        void Show();

        void SetUILayerTop();

        #region 操作

        #endregion




    }


}
