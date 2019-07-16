using UnityEditor;
using UnityEngine;

namespace TinaX.Lua.ProjectSetting
{
    static class PSLuaConfig
    {

        private static Lua.LuaConfig mConfig;
        private static SerializedObject mSer_Config;

        private static GUIStyle mTitle = new GUIStyle();

        private static SerializedProperty mConf_EnableLua; //是否启用Lua
        private static SerializedProperty mConf_LuaScriptStartup; //启动入口
        private static SerializedProperty mConf_FileExten; //文件后缀名

        private static SerializedProperty mConf_Debug_LuaIDE_Enable; //LuaIDE enable
        private static SerializedProperty mConf_Debug_LuaIDE_Addr; //
        private static SerializedProperty mConf_Debug_LuaIDE_Port; //

        private static SerializedProperty mConf_Debug_LuaPerfact_Enable; //LuaPerfact enable
        private static SerializedProperty mConf_Debug_LuaPerfact_Addr; //
        private static SerializedProperty mConf_Debug_LuaPerfact_Port; //
        


        [SettingsProvider]
        public static SettingsProvider Projects_VFS()
        {
            var provider = new SettingsProvider("TinaX/Lua", SettingsScope.Project)
            {
                label = "Lua Script",
                activateHandler = (searchContext, rootElement) =>
                {

                    mTitle.normal.textColor = TinaX.Core.XEditorStyleDefine.Color_Blue;
                    mTitle.fontSize = 15;


                    mConfig = TinaX.Config.GetTinaXConfig<LuaConfig>(TinaX.Conf.ConfigPath.lua);
                    if (mConfig != null)
                    {
                        mSer_Config = new SerializedObject(mConfig);
                        if (mSer_Config != null)
                        {
                            mConf_EnableLua = mSer_Config.FindProperty("EnableLua");
                            mConf_LuaScriptStartup = mSer_Config.FindProperty("LuaScriptStartup");
                            mConf_FileExten = mSer_Config.FindProperty("FileExten");


                            mConf_Debug_LuaIDE_Enable = mSer_Config.FindProperty("Debug_LuaIDE_Enable");
                            mConf_Debug_LuaIDE_Addr = mSer_Config.FindProperty("Debug_LuaIDE_Addr");
                            mConf_Debug_LuaIDE_Port = mSer_Config.FindProperty("Debug_LuaIDE_Port");

                            mConf_Debug_LuaPerfact_Enable = mSer_Config.FindProperty("Debug_LuaPerfact_Enable");
                            mConf_Debug_LuaPerfact_Addr = mSer_Config.FindProperty("Debug_LuaPerfact_Addr");
                            mConf_Debug_LuaPerfact_Port = mSer_Config.FindProperty("Debug_LuaPerfact_Port");

                        }

                    }

                },
                guiHandler = (searchContext) =>
                {
#if !TinaX_CA_LuaRuntime_Enable
                    GUILayout.Label("您似乎未启用TinaX Framework的 Lua 功能，请先在宏定义设置中启用.");
#endif
                    if (mSer_Config != null && mConfig != null)
                    {
                        EditorGUILayout.BeginVertical(GUILayout.MaxWidth(500));

                        EditorGUILayout.LabelField("注意:在使用Lua语言之前，请先导入Tencent/xLua内容，并在TinaX设置中启用相关宏定义");
                        EditorGUILayout.LabelField("  Lua Script Config", mTitle);
                        EditorGUILayout.Space();

                        EditorGUILayout.PropertyField(mConf_EnableLua, new GUIContent("启用Lua环境"), true);
                        if (mConfig.EnableLua)
                        {
                            EditorGUILayout.PropertyField(mConf_LuaScriptStartup, new GUIContent("框架启动后运行代码:"), true);
                            EditorGUILayout.PropertyField(mConf_FileExten, new GUIContent("Lua文件后缀名:"), true);

                        }


                        GUILayout.Space(15);
                        EditorGUILayout.LabelField("  IDE Debug功能", mTitle);
                        GUILayout.Space(10);

                        EditorGUILayout.LabelField("LuaIDE 扩展");
                        EditorGUILayout.PropertyField(mConf_Debug_LuaIDE_Enable, new GUIContent("启用LuaIDE扩展:"), true);
                        if (mConfig.Debug_LuaIDE_Enable)
                        {
                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.PropertyField(mConf_Debug_LuaIDE_Addr, new GUIContent("调试地址:"), true);
                            EditorGUILayout.PropertyField(mConf_Debug_LuaIDE_Port, new GUIContent("调试端口:"), true);

                            EditorGUILayout.EndHorizontal();
                        }


                        EditorGUILayout.LabelField("LuaPerfact 扩展");
                        EditorGUILayout.PropertyField(mConf_Debug_LuaPerfact_Enable, new GUIContent("启用LuaPerfact扩展:"), true);
                        if (mConfig.Debug_LuaPerfact_Enable)
                        {
                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.PropertyField(mConf_Debug_LuaPerfact_Addr, new GUIContent("调试地址:"), true);
                            EditorGUILayout.PropertyField(mConf_Debug_LuaPerfact_Port, new GUIContent("调试端口:"), true);

                            EditorGUILayout.EndHorizontal();
                        }


                        EditorGUILayout.EndVertical();

                        mSer_Config.ApplyModifiedProperties();
                    }

                },
                deactivateHandler = () =>
                {

                }
            };

            return provider;
        }
    }
}
