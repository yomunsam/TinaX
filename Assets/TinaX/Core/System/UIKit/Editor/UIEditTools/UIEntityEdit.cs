using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TinaX;
using TinaX.UIKit;


namespace TinaXEditor.UIKit
{
    [CustomEditor(typeof(UIEntity))]
    [CanEditMultipleObjects]
    public class UIEntityEdit : Editor
    {
        private Rect windowRect = new Rect(8, 45, 120, 150);

        private void OnSceneGUI()
        {
            Handles.BeginGUI();

            GUILayout.Window(0, windowRect, Draw_UIKit_Main_Window, "UIKit");

            Handles.EndGUI();
        }


        private void Draw_UIKit_Main_Window(int windowID)
        {
            GUILayout.Label("UI:" + target.name);
            
        }
    }

}
