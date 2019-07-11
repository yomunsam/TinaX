using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEditor;

namespace TinaXEditor
{
    public class DevOpsBuildWindow : EditorWindow
    {
        [MenuItem("TinaX/发布流程/编译发布",false,104)]
        public static void Open_AssetsPackageWindow()
        {
            EditorWindow.GetWindow<DevOpsBuildWindow>();
        }


        #region Runtime

        private DevOpsBuild buildMgr;
        
        #endregion

        private void OnEnable()
        {
            buildMgr = new DevOpsBuild();
        }

        private void OnDestroy()
        {
            buildMgr = null;
        }

        public DevOpsBuildWindow()
        {
            this.titleContent = new GUIContent("编译发布");
            this.minSize = new Vector2(550, 400);
        }

        #region windowConfig
        private TinaX.Const.PlatformConst.E_Platform m_PlatformConf;
        private string OutPut_path;
        private bool Input_ABAsset;
        private string AB_Path;
        private bool Rebuild_AB;
        private BuildOptions opt = BuildOptions.None;
        #endregion



        private void OnGUI()
        {
            var origin_lable_font_size = GUI.skin.label.fontSize;
            var origin_lable_alignment = GUI.skin.label.alignment;

            EditorGUILayout.BeginVertical();

            GUILayout.Space(10);
            GUI.skin.label.fontSize = 18;
            GUI.skin.label.alignment = TextAnchor.MiddleLeft;
            GUILayout.Label("编译发布");
            GUI.skin.label.fontSize = origin_lable_font_size;
            m_PlatformConf = (TinaX.Const.PlatformConst.E_Platform)EditorGUILayout.EnumPopup("发布平台", m_PlatformConf);

            OutPut_path = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), TinaX.Setup.Framework_Build_Output_Path,m_PlatformConf.ToString());
            GUILayout.Label("输出位置：" + OutPut_path);
            GUILayout.Space(10);

            Input_ABAsset = GUILayout.Toggle(Input_ABAsset,"导入AssetBundle资源包");
            if (Input_ABAsset)
            {
                GUILayout.Space(4);
                AB_Path = TinaX.Setup.Framework_AssetSystem_Pack_Path + "/" + m_PlatformConf.ToString().ToLower();
                GUILayout.Label("导入路径：" + System.IO.Path.GetFullPath(AB_Path));
                if (!System.IO.Directory.Exists(System.IO.Path.GetFullPath(AB_Path)))
                {
                    GUILayout.Label("!路径对应的目录不存在");
                    Rebuild_AB = true;
                }
                Rebuild_AB = GUILayout.Toggle(Rebuild_AB, "重新构建资源包");
            }

            GUILayout.Space(20);
            opt = (BuildOptions)EditorGUILayout.EnumFlagsField("Build选项", opt);
            if(opt == BuildOptions.None)
            {
                GUILayout.Label(" - 不执行特殊操作");
            }
            var arr = Enum.GetValues(typeof(BuildOptions));
            for(int i =0; i < arr.Length; i++)
            {
                var e = (BuildOptions)arr.GetValue(i);
                uint select = (uint)e & (uint)opt;
                bool isSelect = select == (uint)e;

                if (isSelect)
                {
                    switch (e)
                    {
                        case BuildOptions.AllowDebugging:
                            GUILayout.Label(" - 允许远程调试");
                            break;
                        case BuildOptions.AutoRunPlayer:
                            GUILayout.Label(" - 编译成功自动运行");
                            break;
                        case BuildOptions.Development:
                            GUILayout.Label(" - [开发包]包含Debug标记并支持Profiler");
                            break;
                    }
                }
            }

            GUILayout.Space(20);
            GUILayout.Button("Build");

            EditorGUILayout.EndVertical();

            GUI.skin.label.fontSize = origin_lable_font_size;
            GUI.skin.label.alignment = origin_lable_alignment;
        }

    }
}

