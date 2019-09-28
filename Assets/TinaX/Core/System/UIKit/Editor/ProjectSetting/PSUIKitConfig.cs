using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


namespace TinaX.UIKits.ProjectSetting
{
    static class PSUIKitConfig
    {
        private static UIKitConfig mConfig;
        private static SerializedObject mSer_Config;

        private static GUIStyle mTitle = new GUIStyle();

        private static SerializedProperty mConf_UIKit_Default_UIGroup; //默认UI组
        private static SerializedProperty mConf_UIKit_Use_AdvancedMode; //高级模式


        [SettingsProvider]
        public static SettingsProvider Projects_UIKit()
        {
            var provider = new SettingsProvider("TinaX/UIKit", SettingsScope.Project)
            {
                label = "UIKit",
                activateHandler = (searchContext, rootElement) =>
                {
                    mTitle.normal.textColor = TinaX.Core.XEditorStyleDefine.Color_Blue;
                    mTitle.fontSize = 15;

                    mConfig = TinaX.Config.CreateIfNotExist<UIKitConfig>(TinaX.Conf.ConfigPath.uikit);
                    if (mConfig != null)
                    {
                        mSer_Config = new SerializedObject(mConfig);
                        if (mSer_Config != null)
                        {
                            mConf_UIKit_Default_UIGroup = mSer_Config.FindProperty("Default_UIGroup");
                            mConf_UIKit_Use_AdvancedMode = mSer_Config.FindProperty("Use_AdvancedMode");

                        }

                    }

                },
                guiHandler = (searchContext) =>
                {
                    if (mSer_Config != null && mConfig != null)
                    {
                        EditorGUILayout.BeginVertical(GUILayout.MaxWidth(500));

                        EditorGUILayout.LabelField("  UI系统",mTitle);
                        EditorGUILayout.Space();

                        EditorGUILayout.PropertyField(mConf_UIKit_Default_UIGroup, new GUIContent("默认UI组"), true);
                        EditorGUILayout.PropertyField(mConf_UIKit_Use_AdvancedMode, new GUIContent("启用UI高级模式"), true);



                        EditorGUILayout.Space();
                        EditorGUILayout.LabelField("  UI绘制" , mTitle);
                        EditorGUILayout.Space();

                        

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
