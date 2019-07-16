using UnityEditor;
using UnityEngine;

namespace TinaX.VFS.ProjectSetting
{
    static class PSVFSConfig
    {

        private static VFS.VFSConfigModel mConfig;
        private static SerializedObject mSer_Config;

        private static GUIStyle mTitle = new GUIStyle();

        private static SerializedProperty mConf_VFS_WhiteList; //VFS白名单
        private static SerializedProperty mConf_VFS_SpecialFolder; //文件夹特殊打包规则
        private static SerializedProperty mConf_VFS_IgnorePath; //忽略配置的路径及其子路径的文件
        private static SerializedProperty mConf_VFS_IgnoreExt; //忽略配置中的后缀名（不加"."点）
        private static SerializedProperty mConf_VFS_IgnorePathKeyword; //如果文件路径中包含相关关键字，则忽略
        private static SerializedProperty mConf_VFS_EncryFileList; //如果文件路径中包含相关关键字，则忽略
        private static SerializedProperty mConf_VFS_EncryPathList; //如果文件路径中包含相关关键字，则忽略

        private static SerializedProperty mConf_VFS_EnableChineseFileInput; //如果文件路径中包含相关关键字，则忽略
        private static SerializedProperty mConf_VFS_ChineseHandleType; //导入文件名包含中文时的处理方式
        private static SerializedProperty mConf_VFS_EnableTextureToSpriteInUIAssetsFolder; //导入图片到UI目录时自动设置为Sprite


        [SettingsProvider]
        public static SettingsProvider Projects_VFS()
        {
            var provider = new SettingsProvider("TinaX/VFS", SettingsScope.Project)
            {
                label = "虚拟文件系统",
                activateHandler = (searchContext, rootElement) =>
                {

                    mTitle.normal.textColor = TinaX.Core.XEditorStyleDefine.Color_Blue;
                    mTitle.fontSize = 15;


                    mConfig = TinaX.Config.GetTinaXConfig<VFSConfigModel>(TinaX.Conf.ConfigPath.vfs);
                    if(mConfig != null)
                    {
                        mSer_Config = new SerializedObject(mConfig);
                        if (mSer_Config != null)
                        {
                            mConf_VFS_WhiteList = mSer_Config.FindProperty("Assets_system_whiteList");
                            mConf_VFS_SpecialFolder = mSer_Config.FindProperty("Special_Package_Folder");
                            mConf_VFS_IgnorePath = mSer_Config.FindProperty("Ignore_Path");
                            mConf_VFS_IgnoreExt = mSer_Config.FindProperty("Ignore_Ext");
                            mConf_VFS_IgnorePathKeyword = mSer_Config.FindProperty("Ignore_Path_keyword");
                            mConf_VFS_EncryFileList = mSer_Config.FindProperty("EncryFileList");
                            mConf_VFS_EncryPathList = mSer_Config.FindProperty("EncryPathList");

                            mConf_VFS_EnableChineseFileInput = mSer_Config.FindProperty("EnableChineseFileInput");
                            mConf_VFS_ChineseHandleType = mSer_Config.FindProperty("ChineseHandleType");
                            mConf_VFS_EnableTextureToSpriteInUIAssetsFolder = mSer_Config.FindProperty("EnableTextureToSpriteInUIAssetsFolder");
                        }

                    }

                },
                guiHandler = (searchContext) =>
                {
                    if(mSer_Config != null && mConfig != null)
                    {
                        EditorGUILayout.BeginVertical(GUILayout.MaxWidth(500));

                        EditorGUILayout.LabelField("  资源目录设定", mTitle);
                        EditorGUILayout.Space();

                        EditorGUILayout.PropertyField(mConf_VFS_WhiteList, new GUIContent("文件管理白名单"),true);
                        EditorGUILayout.PropertyField(mConf_VFS_SpecialFolder, new GUIContent("文件夹特殊打包规则"),true);
                        EditorGUILayout.PropertyField(mConf_VFS_IgnorePath, new GUIContent("打包忽略路径"),true);
                        EditorGUILayout.PropertyField(mConf_VFS_IgnoreExt, new GUIContent("打包忽略后缀名"),true);
                        EditorGUILayout.PropertyField(mConf_VFS_IgnorePathKeyword, new GUIContent("打包忽略路径关键字"),true);
                        EditorGUILayout.PropertyField(mConf_VFS_EncryFileList, new GUIContent("资源[文件]加密名单"),true);
                        EditorGUILayout.PropertyField(mConf_VFS_EncryPathList, new GUIContent("资源[路径]加密名单"),true);

                        EditorGUILayout.Space();
                        EditorGUILayout.LabelField("  资源处理规则",mTitle);
                        EditorGUILayout.Space();

                        EditorGUILayout.PropertyField(mConf_VFS_EnableChineseFileInput, new GUIContent("处理导入文件中的中文"), true);
                        if (mConfig.EnableChineseFileInput)
                        {
                            EditorGUILayout.PropertyField(mConf_VFS_ChineseHandleType, new GUIContent("导入文件名包含中文时的处理方式"), true);

                        }
                        EditorGUILayout.PropertyField(mConf_VFS_EnableTextureToSpriteInUIAssetsFolder, new GUIContent("导入图片到UI目录时自动设置为Sprite"), true);


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


