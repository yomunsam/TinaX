using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEditor.UI;
using TinaX;
using TinaX.UIKits;

namespace TinaXEditor.UIKit
{
    [CustomEditor(typeof(XButton))]
    [CanEditMultipleObjects]
    public class XButtonEdit : ButtonEditor
    {
        private Rect windowRect = new Rect(8, 45, 120, 150);
        
        private void OnSceneGUI()
        {

            Handles.BeginGUI();

            GUILayout.Window(10, windowRect, Draw_UIKit_xText_window, "xButton");

            Handles.EndGUI();
        }

        private void Draw_UIKit_xText_window(int windowId)
        {
            var xButton = (XButton)target;
            //预设对象检索
            var xText = xButton.gameObject.GetComponentInChildren<XText>();
            var xImage = xButton.gameObject.GetComponentInChildren<XImage>();
            

            GUILayout.Label(target.name);
            
            //文字内容
            if(xText != null)
            {
                GUILayout.Space(8);
                //文字内容
                
                GUILayout.Label("按钮文字：");
                xText.text = GUILayout.TextField(xText.text);

                GUILayout.Space(5);
                //字号
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
            }

            


        }

        private enum E_xButton_format_type
        {
            none,
            text,
            img,
            img_text
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var xButton = (XButton)target;

            
        }

    }
}

