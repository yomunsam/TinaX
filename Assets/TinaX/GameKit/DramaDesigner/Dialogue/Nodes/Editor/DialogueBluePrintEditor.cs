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
    /// <summary>
    /// 剧情系统 对话系统 基础蓝图 编辑器类
    /// </summary>
    [CustomNodeGraphEditor(typeof(DialogueBluePrint))]
    public class DialogueBluePrintEditor : NodeGraphEditor
    {
        private bool mInited = false;
        private GUIStyle mTitleStyle = new GUIStyle();

        public override string GetNodeMenuName(Type type)
        {
            if (type.Namespace.StartsWith("TinaXGameKit.Drama"))
            {
                return base.GetNodeMenuName(type).Replace("TinaX GameKit/", "");
            }
            else
            {
                return null;
            }
            
        }

        public override void OnGUI()
        {
            base.OnGUI();

            if (!mInited)
            {
                Init();
                mInited = true;
            }
            //绘制一个标题
            GUILayout.Label("剧情对话蓝图", mTitleStyle);
        }

        private void Init()
        {
            mTitleStyle.margin = new RectOffset(10,10,10,10);
            mTitleStyle.normal.textColor = new Color(1, 1, 1, 0.6f);
            mTitleStyle.fontSize = 20;
        }
    }
}

