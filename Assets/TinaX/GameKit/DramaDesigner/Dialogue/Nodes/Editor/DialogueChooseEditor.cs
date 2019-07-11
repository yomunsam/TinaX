using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TinaXGameKit.Drama.BP;
using XNode;
using XNodeEditor;
using UnityEditor;

namespace TinaXGameKitEditor.Drama
{
    [CustomNodeEditor(typeof(DialogueChoose))]
    public class DialogueChooseEditor : NodeEditor
    {
        public override void OnBodyGUI()
        {
            //base.OnBodyGUI();
            var choose = target as DialogueChoose;
            GUILayout.BeginHorizontal();

            //输入------

            NodeEditorGUILayout.PortField(target.GetInputPort("Input"), GUILayout.Width(100));
            

            EditorGUILayout.Space();
            
            GUILayout.EndHorizontal();

            //标题
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("Title"));
            //询问者
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("Speaker"));

            //选项列表
            NodeEditorGUILayout.InstancePortList("ChooseList",typeof(DialogueChoose.S_Choose),serializedObject,NodePort.IO.Output, Node.ConnectionType.Override);

        }

        public override int GetWidth()
        {
            return 400 ;
        }
    }
}

