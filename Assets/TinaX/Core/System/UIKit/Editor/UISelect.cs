

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityToolbarExtender;
using TinaX;
using TinaX.UIKit;

namespace TinaXEditor.UIKit
{
    [InitializeOnLoad]
    public class UISelect
    {
        static UISelect()
        {
            ToolbarExtender.LeftToolbarGUI.Add(OnToolbar);
        }

        static void OnToolbar()
        {
            if(UnityEngine.Application.isPlaying){
                GUILayout.FlexibleSpace();

                if(GUILayout.Button(new GUIContent("UI", "UI编辑区"), ToolbarStyles.commandButtonStyle))
                {
                    // SceneHelper.StartScene("Assets/ToolbarExtender/Example/Scenes/Scene1.unity");
                    var go = GameObject.Find(TinaX.Setup.Framework_Base_GameObject + "/" + UIKitConst.UIKit_RootGameObject_Name + "/" + UIKitConst.UIKit_UIRootGameObject_Name);
                    if (go != null){
                        Selection.activeGameObject = go;
                    }
                }

                GUILayout.Space(25);

            }

        }

        static class ToolbarStyles
        {
            public static readonly GUIStyle commandButtonStyle;

            static ToolbarStyles()
            {
                commandButtonStyle = new GUIStyle("Command")
                {
                    fontSize = 14,
                    alignment = TextAnchor.MiddleCenter,
                    imagePosition = ImagePosition.ImageAbove,
                    fontStyle = FontStyle.Normal
                };
            }
        }
    }
}




