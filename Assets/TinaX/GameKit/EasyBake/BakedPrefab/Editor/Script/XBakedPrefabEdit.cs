using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TinaXGameKit.EasyBake;
using TinaX;
using UnityEditor;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
#endif

namespace TinaXGameKitEditor.EasyBake
{
    [CustomEditor(typeof(XBakedPrefab))]
#if ODIN_INSPECTOR
    public class XBakedPrefabEdit:OdinEditor
#else
    public class XBakedPrefabEdit:Editor
#endif
    {
        //private GUIStyle mStyle_Btn = EditorStyles.miniButton;

        

        //protected override void OnEnable()
        //{
        //    base.OnEnable();
        //    //mStyle_Btn.fontSize = 14;
        //    //mStyle_Btn.normal.textColor = TinaX.Core.XEditorStyleDefine.Color_Text_Normal;
        //}

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();


            GUILayout.Space(20);

            GUILayout.Button("Bake Prefab");
            GUILayout.Button("Bake Prefab (solo)");

            GUILayout.Button("使用Prefab Lightmap 刷新场景");

        }
    }

}
