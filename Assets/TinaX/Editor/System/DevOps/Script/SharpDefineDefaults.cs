using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TinaXEditor.DevOps
{
    /// <summary>
    /// 用来定义一些Framework预制的#Define 宏定义字符
    /// </summary>
    public static class SharpDefineDefault
    {

        public static S_Define[] DefineItems =
        {
            //Odin
            new S_Define()
            {
                Name = "Odin Inspector插件",
                DefineStr = "ODIN_INSPECTOR",
                Desc = "TinaX 推荐引入Odin插件以达到最佳体验，TinaX 自身开发也是以有Odin插件的效果为准。TinaX 也会做到在没有Odin插件时能运行，但体验不一定有保障。"
            },
            //Lua Runtime
            new S_Define()
            {
                Name = "LuaScript Runtime",
                DefineStr = TinaX.Const.SharpDefineConst.TinaX_LuaRuntime,
                Desc = "用来启用TinaX Lua语言环境的宏定义，需要配合xLua（https://github.com/Tencent/xlua）使用喵",
            },
            //XLua Hot fix
            new S_Define()
            {
                Name = "xLua Hotfix",
                DefineStr = "HOTFIX_ENABLE",
                Desc = "用于xLua库的Hotfix，需要配合xLua（https://github.com/Tencent/xlua）使用喵",
            },
            //文件系统
            new S_Define()
            {
                Name = "虚拟文件系统",
                DefineStr = TinaX.Const.SharpDefineConst.TinaX_VFS,
                Desc = "如果不启用的话，默认会使用UnityEngine.Resources 进行工程内文件的寻址加载。",
            },
        };
        
        public struct S_Define
        {
            /// <summary>
            /// 显示名
            /// </summary>
            public string Name;


            /// <summary>
            /// 定义字符串
            /// </summary>
            public string DefineStr;

            public string Desc;
        }
    }

}
