using UnityEngine;
using System;
using System.Collections.Generic;

namespace TinaX.Core
{

    public class XBaseConfig : ScriptableObject
    {
        #region 宏定义指令设置

        //共用宏定义串（所有平台都会有的定义
        public List<string> CommonDefineStr;


        public List<S_DefineWithBuildTargetGroup> DefineWithTarget;


        #endregion


        /// <summary>
        /// 结构体：宏定义 与 编译目标平台组的对应关系
        /// </summary>
        [Serializable]
        public struct S_DefineWithBuildTargetGroup
        {
            /// <summary>
            /// 目标平台组的名字
            /// </summary>
            public string TargetName;

            public List<string> DefineStr;

            /// <summary>
            /// 排除，这里会忽略全局设置
            /// </summary>
            public List<string> IgnoreDefine;
        }

    }
}
