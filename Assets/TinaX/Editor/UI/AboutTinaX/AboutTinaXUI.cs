using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using TinaX;

namespace TinaXEditor.UI
{
    public class AboutTinaXUI : EditorWindow
    {

        private static AboutTinaXUI _Window;

        [MenuItem("Help/About TinaX",false,10)]
        public static void OpenUI()
        {
            if(_Window == null)
            {
                _Window = GetWindow<AboutTinaXUI>(true,"About TinaX");
            }
            else
            {
                _Window.Focus();
            }
        }


        private GUIStyle mStyle_Title;
        private GUIStyle mStyle_VersionLayout;

        private Vector2 mScrollV2 = new Vector2();


        AboutTinaXUI()
        {
            this.minSize = new Vector2(400, 450);
            this.maxSize = new Vector2(400, 450);

            
            
        }

        private void OnEnable()
        {
            mStyle_Title = new GUIStyle()
            {
                fontSize = 18,

            };
            mStyle_Title.normal.textColor = TinaX.Core.XEditorStyleDefine.Color_Text_Normal;
            mStyle_Title.padding = new RectOffset(15, 15, 10, 10);

            mStyle_VersionLayout = new GUIStyle();
            mStyle_VersionLayout.padding = new RectOffset(30, 30, 5, 5);
        }

        private void OnDestroy()
        {
            _Window = null;
        }

        void OnGUI()
        {
            //绘制标题
            GUILayout.Label("Tina X Framework", mStyle_Title);

            //绘制版本信息
            GUILayout.BeginVertical(mStyle_VersionLayout);
            GUILayout.Label("框架版本:" + FrameworkInfo.FrameworkVersion);
            GUILayout.Label("版本名：" + FrameworkInfo.FrameworkVersionName);
            GUILayout.Label("版本号：" + FrameworkInfo.FrameworkVersionCode);
            GUILayout.EndVertical();

            //绘制框架详情信息
            mScrollV2 = GUILayout.BeginScrollView(mScrollV2);

            DrawFrameworkDetailsInfo();

            GUILayout.EndScrollView();

            //绘制框架链接等
            GUILayout.Space(10);

            //一排链接按钮
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("TinaX in Github"))
            {
                Application.OpenURL(FrameworkInfo.Framework_Url_Github);
            }

            if (GUILayout.Button("Document"))
            {
                Application.OpenURL(FrameworkInfo.Framework_Url_Doc);
            }

            if (GUILayout.Button("Corala.Space"))
            {
                Application.OpenURL("https://corala.space");
            }


            GUILayout.EndHorizontal();

            //版权信息
            GUILayout.Label("Copy Right (c) 2019 Nekonya Studio . All rights reserved.  \nCorala.Space | https://nekonya.io");

        }


        private void DrawFrameworkDetailsInfo()
        {
            GUILayout.Label("TinaX Framework | Corala.Space 企划");

            string text_info1 = @"
TinaX Framework 开发参与人员：
    yomunsam(https://yomunchan.moe)

--------------------------------------

TinaX Framework 开源项目引用：
    CatLib - https://catlib.io
";

            GUILayout.Label(text_info1);
        }
    }
}

