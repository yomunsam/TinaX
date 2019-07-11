using TinaX;
using UnityEditor;
using UnityEngine;

namespace TinaXEditor
{
    static class TinaXConfWithoutOdin
    {



        #region 封面页

        [SettingsProvider]
        public static SettingsProvider ProjectSetting_Index()
        {
            return new SettingsProvider("TinaX", SettingsScope.Project)
            {
                label = "TinaX Framework",
                guiHandler = (searchStr) =>
                {
                    //检查配置
                    var conf = Config.GetTinaXConfig<TinaX.Core.XBaseConfig>(TinaX.Conf.ConfigPath.base_config);
                    if (conf == null)
                    {
                        //没有安装
                        GUILayout.Label("没有初始化TinaX Framework, 请先执行初始化");
                        if (GUILayout.Button("点击此处开始"))
                        {
                            TinaXEditor.Setup.FrameworkSetup.OpenSetupWindow();
                        }
                    }
                    else
                    {
                        GUILayout.Label("TinaX Framework 项目设置");
                    }
                },
            };
        }

        #endregion


        #region MainConfig

        private static MainConfig mMainConfig;
        private static SerializedObject mSer_MainConfig;

        private static SerializedProperty mMainConfig_EnableTinaX;
        private static SerializedProperty mMainConfig_StartupScene;
        private static SerializedProperty mMainConfig_VersionCode;
        private static SerializedProperty mMainConfig_AppName;

        [SettingsProvider]
        public static SettingsProvider ProjectSetting_TinaXMainConfig()
        {
            

            var provider = new SettingsProvider("TinaX/Main", SettingsScope.Project)
            {
                label = "基础设置",
                activateHandler = (searchContext, rootElement) =>
                {
                    mMainConfig = TinaX.Config.CreateIfNotExist<MainConfig>(TinaX.Conf.ConfigPath.main);
                    if (mMainConfig != null)
                    {
                        mSer_MainConfig = new SerializedObject(mMainConfig);
                        if(mSer_MainConfig != null)
                        {
                            mMainConfig_EnableTinaX = mSer_MainConfig.FindProperty("TinaX_Enable");
                            mMainConfig_StartupScene = mSer_MainConfig.FindProperty("Startup_Scene");
                            mMainConfig_VersionCode = mSer_MainConfig.FindProperty("Version_Code");
                            mMainConfig_AppName = mSer_MainConfig.FindProperty("App_Name");

                        }
                    }

                },
                guiHandler = (searchContext) =>
                {
                    if(mSer_MainConfig != null && mMainConfig != null)
                    {

                        EditorGUILayout.BeginVertical(GUILayout.MaxWidth(500));

                        EditorGUILayout.PropertyField(mMainConfig_EnableTinaX, new GUIContent("是否启用TinaX Framework"));
                        EditorGUILayout.PropertyField(mMainConfig_StartupScene, new GUIContent("初始化启动场景"));
                        EditorGUILayout.PropertyField(mMainConfig_VersionCode, new GUIContent("当前母包版本号"));
                        EditorGUILayout.PropertyField(mMainConfig_AppName, new GUIContent("应用ID"));


                        EditorGUILayout.EndVertical();

                        mSer_MainConfig.ApplyModifiedProperties();
                    }
                    else
                    {
                        GUILayout.Label("未成功加载TinaX 配置文件的有效数据\n可能是文件丢失、损坏、被占用，或者尚未初始化TinaX Framework");
                    }
                    
                    
                },
                deactivateHandler = () =>
                {
                    
                }
            };

            return provider;
        }

        #endregion
    }
}

