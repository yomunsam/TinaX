using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UI;
using TinaX;
using TinaX.UIKit;
using TinaXEditor.I18NKit;


namespace TinaXEditor.UIKit
{
    [CustomEditor(typeof( XText))]
    public class XTextEdit : UnityEditor.UI.TextEditor
    {
        private Rect windowRect = new Rect(8, 45, 120, 220);

        private void OnSceneGUI()
        {
           
            Handles.BeginGUI();

            GUILayout.Window(10, windowRect, Draw_UIKit_xText_window, "xText");

            Handles.EndGUI();
        }

        private void Draw_UIKit_xText_window(int windowId)
        {
            var xText = (XText)target;
            GUILayout.Label(target.name);

            //内容
            GUILayout.Label("Text:");
            xText.text = GUILayout.TextField(xText.text);
            //字体大小
            
            GUILayout.BeginHorizontal();
            GUILayout.Label("字号：" + xText.fontSize.ToString());
            if (GUILayout.Button("+", GUILayout.Width(20)))
            {
                xText.fontSize += 2;
            }
            if (GUILayout.Button("-", GUILayout.Width(20)))
            {
                xText.fontSize -= 2;
            }
            GUILayout.EndHorizontal();
            xText.raycastTarget = GUILayout.Toggle(xText.raycastTarget, "可否交互");


            //I18N
            GUILayout.Space(10);
            GUILayout.Label("国际化设置");
            xText.UseI18N = GUILayout.Toggle(xText.UseI18N,"启用国际化");
            if (xText.UseI18N)
            {
                GUILayout.Label("I18N Key");
                xText.I18NKey = GUILayout.TextField(xText.I18NKey);
                GUILayout.Label("I18N Group");
                xText.I18NGroup = GUILayout.TextField(xText.I18NGroup);
                //xText.UseI18NInRumtime = GUILayout.Toggle(xText.UseI18NInRumtime, "运行时启用");

                if (GUILayout.Button("刷新文本"))
                {
                    var regionName = I18NSwitcherEditor.GetCurSelectRegionName();
                    if (!regionName.IsNullOrEmpty())
                    {
                        xText.text = I18NIOEditor.GetStringByKey(regionName, xText.I18NKey, xText.I18NGroup);

                    }
                    //xText.SetAllDirty();
                }
            }
            
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var xText = (XText)target;


            GUILayout.Space(10);
            GUILayout.Label("国际化设置");
            xText.UseI18N = GUILayout.Toggle(xText.UseI18N, "启用国际化");
            if (xText.UseI18N)
            {
                GUILayout.Label("I18N Key");
                xText.I18NKey = GUILayout.TextField(xText.I18NKey);
                GUILayout.Label("I18N Group");
                xText.I18NGroup = GUILayout.TextField(xText.I18NGroup);
                //xText.UseI18NInRumtime = GUILayout.Toggle(xText.UseI18NInRumtime, "运行时启用");

                GUILayout.BeginHorizontal();
                if (GUILayout.Button("刷新文本"))
                {
                    var regionName = I18NSwitcherEditor.GetCurSelectRegionName();
                    if (!regionName.IsNullOrEmpty())
                    {
                        xText.text = I18NIOEditor.GetStringByKey(regionName, xText.I18NKey, xText.I18NGroup);

                    }
                    //xText.SetAllDirty();
                }
                if (GUILayout.Button("I18N编辑器"))
                {
                    //xText.text = XI18NCacheEditor.GetGameStringByKey(xText.I18NKey);
                    //xText.SetAllDirty();
                }

                GUILayout.EndHorizontal();
            }
        }
    }
}

