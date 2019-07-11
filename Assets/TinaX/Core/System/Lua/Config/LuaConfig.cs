

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


#if UNITY_EDITOR && ODIN_INSPECTOR
        [FoldoutGroup("Debug功能")]
        [Header("IDE调试扩展")]

#endif
        [EnumLabel("IDE调试模式")]
        public IDE_DebugExt IDE_Debug;



        /// <summary>
        /// IDE调试扩展
        /// </summary>
        [System.Serializable]
        public enum IDE_DebugExt
        {
            [Header("不启用")]
            None = 0,
            [Header("LuaIDE (VSCode)")]
            LuaIde = 1,
            [Header("LuaPanda (暂取消支持)")]
            LuaPanda = 2
        }

        public bool IsEnableLuaIDE()
        {
            return IDE_Debug == IDE_DebugExt.LuaIde;
        }
        public bool IsEnableLuaPanda()
        {
            return IDE_Debug == IDE_DebugExt.LuaPanda;
        }

#endif

    }

}


