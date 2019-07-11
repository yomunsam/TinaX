using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TinaXGameKit.Drama.BP;
using XNode;
using XNodeEditor;
using UnityEditor;
using System;

namespace TinaXGameKitEditor.Drama.BP
{

    [CustomNodeEditor(typeof(DialogueContent))]
    public class DialogueContentEditor : NodeEditor
    {
        public override GUIStyle GetBodyStyle()
        {
            var style = base.GetBodyStyle();
            style.normal.textColor = Color.white;
            
            return style;
        }

        public override int GetWidth()
        {
            return 250;
        }

        

        public override void OnBodyGUI()
        {
            base.OnBodyGUI();

            var content = target as DialogueContent;

            //绘制文本绑定
            //if (content.TextInject.Count > 0)
            //{
            //    GUILayout.Space(10);
            //    GUILayout.Label("文本绑定:");

            //    //列出Key的list
            //    List<string> dictKeys = new List<string>();
            //    foreach(var item in content.TextInject)
            //    {
            //        dictKeys.Add(item.Key);
            //    }
            //    //GUILayout.Label("item number:" + dictKeys.Count);
            //    for (int i = 0; i < dictKeys.Count; i++)
            //    {
            //        var cur_key = dictKeys[i];
            //        GUILayout.BeginHorizontal();
            //        GUILayout.Label(cur_key + ":");
            //        content.TextInject[cur_key] = GUILayout.TextArea(content.TextInject[cur_key]);

            //        GUILayout.EndHorizontal();
            //    }
            //}
        }

        
    }
}


