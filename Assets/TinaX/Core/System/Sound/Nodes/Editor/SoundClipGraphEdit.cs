using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;
using XNodeEditor;
using UnityEditor;
using TinaX.BluePrint.Sound;
using System;

namespace TinaXEditor.BluePrint.Sound
{
    [CustomNodeGraphEditor(typeof(SmartClip), "TinaX.BP.Sound.Settings")]
    public class SoundClipResNodeEdit : NodeGraphEditor
    {
        private bool Inited = false;
        private SmartClip mTarget;
        private GUIStyle mDefaultStype;

        public override void OnGUI()
        {
            base.OnGUI();
            if (!Inited)
            {
                Inited = true;
                GUILayout.Label("初始化中");


                mTarget = (SmartClip)target;
                mDefaultStype = new GUIStyle();
                mDefaultStype.fontSize = 16;
                mDefaultStype.normal.textColor = new Color(1f, 1f, 1f, 0.6f);
                mDefaultStype.margin.left = 10;
                mDefaultStype.margin.top = 10;
                mDefaultStype.richText = false;
                
                


                //检查是否已存在
                //var flag = false;
                //foreach (var item in mTarget.nodes)
                //{
                //    if (item.name == "Sound Clip Res Node" || item.name == "XSound Clip Config")
                //    {
                //        flag = true;
                //    }
                //}
                //if (!flag)
                //{
                //    base.CreateNode(typeof(SoundClipResNode),Vector2.zero);
                //}
            }
            else
            {
                
                GUILayout.Label("蓝图-音频切片", mDefaultStype);
            }
        }


        
        

    }
}

