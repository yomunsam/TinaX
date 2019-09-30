using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TinaX;
using TinaX.UIKits;
#if UNITY_EDITOR && ODIN_INSPECTOR
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
#endif

namespace TinaXEditor.UIKit
{
    [CustomEditor(typeof(UIEntity))]
    [CanEditMultipleObjects]
#if UNITY_EDITOR && ODIN_INSPECTOR
    public class UIEntityEdit : OdinEditor
#else
    public class UIEntityEdit : Editor
#endif

    {
        private Rect windowRect = new Rect(8, 45, 145, 150);

        private void OnSceneGUI()
        {
            Handles.BeginGUI();

            GUILayout.Window(0, windowRect, Draw_UIKit_Main_Window, "UIKit");

            Handles.EndGUI();
        }


        private void Draw_UIKit_Main_Window(int windowID)
        {
            UIEntity entity = (UIEntity)target;
            
            GUILayout.Label("UI:" + target.name);

            GUILayout.Space(5);
            //GUILayout.BeginHorizontal();
            GUILayout.Label("Handle Type:", GUILayout.MaxWidth(85));
            entity.HandleType = (UIEntity.E_MainHandleType)EditorGUILayout.EnumPopup(entity.HandleType);

            //GUILayout.EndHorizontal();
        }



    }

}
