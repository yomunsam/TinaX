

using UnityEngine;
#if UNITY_EDITOR && ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace TinaX.Lua
{
    public class LuaConfig : ScriptableObject
    {

#if TinaX_CA_LuaRuntime_Enable

#if UNITY_EDITOR && ODIN_INSPECTOR
        [Title("Lua Script Config")]
        [Space(10)]
#endif
        public bool EnableLua;


#if UNITY_EDITOR && ODIN_INSPECTOR

        [Header("框架启动后运行:")]
        [Tooltip("不需要启动则留空")]
        [InfoBox("不需要启动则留空")]
        [FilePath(Extensions = "txt")]
        [ShowIf("EnableLua")]
#endif
        public string LuaScriptStartup;

#if UNITY_EDITOR && ODIN_INSPECTOR

        [Space(3)]
#endif
        [EnumLabel("Lua文件后缀名")]
        public LuaFileExten FileExten = LuaFileExten.txt;




        #region LuaDebug --> LuaIDE



#if UNITY_EDITOR && ODIN_INSPECTOR
        [FoldoutGroup("Debug功能")]
        [Header("启用LuaIDE扩展")]

#endif
        public bool Debug_LuaIDE_Enable = false;

#if UNITY_EDITOR && ODIN_INSPECTOR
        [FoldoutGroup("Debug功能")]
        [Header("LuaIDE 调试地址")]
        [ShowIf("Debug_LuaIDE_Enable")]
#endif
        public string Debug_LuaIDE_Addr = "localhost";

#if UNITY_EDITOR && ODIN_INSPECTOR
        [FoldoutGroup("Debug功能")]
        [Header("LuaIDE 调试端口")]
        [ShowIf("Debug_LuaIDE_Enable")]
#endif
        public int Debug_LuaIDE_Port = 7003;

        #endregion


        #region LuaDebug --> Tencent Lua Perfact

#if UNITY_EDITOR && ODIN_INSPECTOR
        [FoldoutGroup("Debug功能")]
        [Header("启用Tencent Lua Perfact扩展")]

#endif
        public bool Debug_LuaPerfact_Enable = false;

#if UNITY_EDITOR && ODIN_INSPECTOR
        [FoldoutGroup("Debug功能")]
        [Header("Tencent Lua Perfact 调试地址")]
        [ShowIf("Debug_LuaPerfact_Enable")]
#endif
        public string Debug_LuaPerfact_Addr = "127.0.0.1";

#if UNITY_EDITOR && ODIN_INSPECTOR
        [FoldoutGroup("Debug功能")]
        [Header("Tencent Lua Perfact 调试端口")]
        [ShowIf("Debug_LuaPerfact_Enable")]
#endif
        public int Debug_LuaPerfact_Port = 9826;

        #endregion


        //[EnumLabel("IDE调试模式")]
        //public bool IDE_Debug;



        ///// <summary>
        ///// IDE调试扩展
        ///// </summary>
        //[System.Serializable]
        //public enum IDE_DebugExt
        //{
        //    [Header("不启用")]
        //    None = 0,
        //    [Header("LuaIDE (VSCode)")]
        //    LuaIde = 1,
        //    [Header("LuaPanda (暂取消支持)")]
        //    LuaPanda = 2
        //}

        //public bool IsEnableLuaIDE()
        //{
        //    return IDE_Debug == IDE_DebugExt.LuaIde;
        //}
        //public bool IsEnableLuaPanda()
        //{
        //    return IDE_Debug == IDE_DebugExt.LuaPanda;
        //}

#endif // 启用lua的宏定义的endif ，别混了

    }

}


