using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TinaX.VFSKit;

namespace TinaXEditor.VFSKit.ProjectSetting
{
    static class PSUIKitConfig
    {
        private static VFSConfigModel mConfig;
        private static SerializedObject mSer_Config;

        private static GUIStyle mTitle = new GUIStyle();

        private static SerializedProperty mConf_VFS_FileMode; //文件处理模式
        private static SerializedProperty mConf_VFS_WhiteList; //文件白名单
        private static SerializedProperty mConf_VFS_FolderPackRule; //文件特殊打包规则
        private static SerializedProperty mConf_VFS_Ignore_Path; //文件忽略路径
        private static SerializedProperty mConf_VFS_Ignore_Path_WhiteList; //文件忽略路径中的白名单
        private static SerializedProperty mConf_VFS_Ignore_ExtName; //全局后缀名忽略挥着
        private static SerializedProperty mConf_VFS_Ignore_Path_Item_Keyword; //全局后缀路径项
        private static SerializedProperty mConf_VFS_Ignore_Path_Keyword; //全局忽略路径关键字

        private static SerializedProperty mConf_VFS_EnableVFS; //是否启用WebVFS
        private static SerializedProperty mConf_VFS_InitVFSOnFramewrokStart; //在框架启动时初始化WebVFS
        private static SerializedProperty mConf_VFS_WebVFSConfigs; 

        private static SerializedProperty mConf_VFS_VFS_EncryFolder; 
        private static SerializedProperty mConf_VFS_Encry_OffsetHandleType; 

        private static SerializedProperty mConf_VFS_HandlerInputNotLettersOrNums; 
        private static SerializedProperty mConf_VFS_NotLettersOrNumsHandleType; 


        [SettingsProvider]
        public static SettingsProvider Projects_UIKit()
        {
            var provider = new SettingsProvider("TinaX/VFS", SettingsScope.Project)
            {
                label = "虚拟文件系统",
                activateHandler = (searchContext, rootElement) =>
                {
                    mTitle.normal.textColor = TinaX.Core.XEditorStyleDefine.Color_Blue;
                    mTitle.fontSize = 15;

                    mConfig = TinaX.Config.CreateIfNotExist<VFSConfigModel>(TinaX.Conf.ConfigPath.vfs);
                    if (mConfig != null)
                    {
                        mSer_Config = new SerializedObject(mConfig);
                        if (mSer_Config != null)
                        {
                            mConf_VFS_FileMode = mSer_Config.FindProperty("FileMode");
                            mConf_VFS_WhiteList = mSer_Config.FindProperty("VFS_WhiteList");
                            mConf_VFS_FolderPackRule = mSer_Config.FindProperty("FolderPackRule");
                            mConf_VFS_Ignore_Path = mSer_Config.FindProperty("Ignore_Path");
                            mConf_VFS_Ignore_Path_WhiteList = mSer_Config.FindProperty("Ignore_Path_WhiteList");
                            mConf_VFS_Ignore_ExtName = mSer_Config.FindProperty("Ignore_ExtName");
                            mConf_VFS_Ignore_Path_Item_Keyword = mSer_Config.FindProperty("Ignore_Path_Item_Keyword");
                            mConf_VFS_Ignore_Path_Keyword = mSer_Config.FindProperty("Ignore_Path_Keyword");

                            mConf_VFS_EnableVFS = mSer_Config.FindProperty("EnableWebVFS");
                            mConf_VFS_InitVFSOnFramewrokStart = mSer_Config.FindProperty("InitWebVFSOnFrameworkStart");
                            mConf_VFS_WebVFSConfigs = mSer_Config.FindProperty("ConfigWebVFS");

                            mConf_VFS_VFS_EncryFolder = mSer_Config.FindProperty("VFS_EncryFolder");
                            mConf_VFS_Encry_OffsetHandleType = mSer_Config.FindProperty("Encry_OffsetHandleType");

                            mConf_VFS_HandlerInputNotLettersOrNums = mSer_Config.FindProperty("HandlerInputNotLettersOrNums");
                            mConf_VFS_NotLettersOrNumsHandleType = mSer_Config.FindProperty("NotLettersOrNumsHandleType");

                        }

                    }

                },
                guiHandler = (searchContext) =>
                {
                    if (mSer_Config != null && mConfig != null)
                    {
                        EditorGUILayout.BeginVertical(GUILayout.MaxWidth(500));

                        EditorGUILayout.LabelField("  资源管理",mTitle);
                        EditorGUILayout.Space();

                        EditorGUILayout.PropertyField(mConf_VFS_FileMode, new GUIContent("VFS文件模式"), true);
                        EditorGUILayout.PropertyField(mConf_VFS_WhiteList, new GUIContent("VFS 文件管理白名单"), true);
                        EditorGUILayout.PropertyField(mConf_VFS_FolderPackRule, new GUIContent("文件特殊打包规则"), true);
                        EditorGUILayout.PropertyField(mConf_VFS_Ignore_Path, new GUIContent("文件路径忽略规则"), true);
                        EditorGUILayout.PropertyField(mConf_VFS_Ignore_Path_WhiteList, new GUIContent("文件路径忽略下的白名单"), true);
                        EditorGUILayout.PropertyField(mConf_VFS_Ignore_ExtName, new GUIContent("全局忽略后缀名"), true);
                        EditorGUILayout.PropertyField(mConf_VFS_Ignore_Path_Item_Keyword, new GUIContent("全局忽略路径项"), true);
                        EditorGUILayout.PropertyField(mConf_VFS_Ignore_Path_Keyword, new GUIContent("全局忽略路径关键字"), true);



                        EditorGUILayout.Space();
                        EditorGUILayout.LabelField("  Web VFS" , mTitle);
                        EditorGUILayout.Space();

                        EditorGUILayout.HelpBox("WebVFS 目前属于实验性功能， 可能不稳定， 请谨慎用于生产环境", MessageType.Warning);
                        EditorGUILayout.PropertyField(mConf_VFS_EnableVFS, new GUIContent("启用WebVFS"), true);

                        if (mConfig.EnableWebVFS)
                        {
                            EditorGUILayout.PropertyField(mConf_VFS_InitVFSOnFramewrokStart, new GUIContent("在框架启动时初始化WebVFS"), true);
                            EditorGUILayout.PropertyField(mConf_VFS_WebVFSConfigs, new GUIContent("WebVFS 配置"), true);

                        }


                        EditorGUILayout.Space();
                        EditorGUILayout.LabelField("  Assets Encry", mTitle);
                        EditorGUILayout.Space();

                        EditorGUILayout.HelpBox("Assets Encry 目前属于实验性功能， 可能不稳定， 请谨慎用于生产环境", MessageType.Warning);

                        EditorGUILayout.PropertyField(mConf_VFS_VFS_EncryFolder, new GUIContent("资源加密文件夹"), true);
                        EditorGUILayout.PropertyField(mConf_VFS_Encry_OffsetHandleType, new GUIContent("Offset加密 处理方式"), true);


                        EditorGUILayout.Space();
                        EditorGUILayout.LabelField("  Assets Import Rule", mTitle);
                        EditorGUILayout.Space();

                        EditorGUILayout.PropertyField(mConf_VFS_HandlerInputNotLettersOrNums, new GUIContent("处理导入的文件中的非字母或数字的命名"), true);
                        EditorGUILayout.PropertyField(mConf_VFS_NotLettersOrNumsHandleType, new GUIContent("当导入了非字母、数字或下划线的命名的文件的处理方式"), true);




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
