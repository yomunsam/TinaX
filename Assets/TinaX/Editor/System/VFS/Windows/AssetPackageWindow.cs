using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


namespace TinaXEditor.Assets
{
    public class AssetPackageWindow : EditorWindow
    {
        [MenuItem("TinaX/发布流程/打包资源",false,100)]
        public static void Open_AssetsPackageWindow()
        {
            EditorWindow.GetWindow<AssetPackageWindow>();
        }

        #region Runtime

        private AssetPackageMgr am_apmgr;

        #endregion

        #region Conf
        private TinaX.Const.PlatformConst.E_Platform m_PlatformConf;
        private bool m_full_reBuild;
        private bool m_re_sign_ab = true; //重新遍历AB标记
        private bool m_cancel_ab_sign;  //在打包后取消AB标记
        private bool m_log_to_console;  //把打包过程输出在控制台
        private string m_cur_dir_path;    //当前路径
        #endregion

        public AssetPackageWindow()
        {
            this.titleContent = new GUIContent("资源系统-打包");
            this.maxSize = new Vector2(400, 300);
            this.minSize = new Vector2(400, 300);
            m_cur_dir_path = System.IO.Directory.GetCurrentDirectory();

            am_apmgr = new AssetPackageMgr();
        }

        ~AssetPackageWindow()
        {
            am_apmgr = null;
        }

        private void OnGUI()
        {
            var origin_lable_font_size = GUI.skin.label.fontSize;
            var origin_lable_alignment = GUI.skin.label.alignment;

            EditorGUILayout.BeginVertical();

            GUILayout.Space(10);
            GUI.skin.label.fontSize = 18;
            GUI.skin.label.alignment = TextAnchor.MiddleLeft;
            GUILayout.Label("资源打包");
            GUI.skin.label.fontSize = origin_lable_font_size;

            m_PlatformConf = (TinaX.Const.PlatformConst.E_Platform)EditorGUILayout.EnumPopup("打包平台",m_PlatformConf);
            GUILayout.Space(10);
            GUILayout.Label("输出位置：" + System.IO.Path.Combine(m_cur_dir_path, TinaX.Setup.Framework_AssetSystem_Pack_Path)+ "/"
                + m_PlatformConf.ToString().ToLower() + "/");
            GUILayout.Label("路径："+ "StreamingAssets/vfs/" + m_PlatformConf.ToString().ToLower() + "/");
            GUILayout.Space(10);
            m_full_reBuild = GUILayout.Toggle(m_full_reBuild," 完全重构");
            m_re_sign_ab = GUILayout.Toggle(m_re_sign_ab, " 重新编辑AssetBundle打包标记");
            m_cancel_ab_sign = GUILayout.Toggle(m_cancel_ab_sign, "在打包完成后取消AssetBundle标记");
            m_log_to_console = GUILayout.Toggle(m_log_to_console, "输出打包Log到控制台");

            GUILayout.Space(10);
            if (GUILayout.Button("开始打包"))
            {
                am_apmgr.PackGlobal(TinaX.Setup.Framework_AssetSystem_Pack_Path + "/" + m_PlatformConf.ToString().ToLower() + "/", TinaX.Platform.GetBuildTarget(m_PlatformConf), m_full_reBuild,m_re_sign_ab,m_cancel_ab_sign,m_log_to_console);
            }
            GUILayout.Space(20);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("管理VFS 资源"))
            {
                VFSFolderMgrWindow.Open_VFSWindow();
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();

            GUI.skin.label.fontSize = origin_lable_font_size;
            GUI.skin.label.alignment = origin_lable_alignment;
        }


        


    }
}
