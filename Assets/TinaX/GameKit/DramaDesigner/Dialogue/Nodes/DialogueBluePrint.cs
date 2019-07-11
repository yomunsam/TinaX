using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;
using System;
using TinaX;
#if UNITY_EDITOR && ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace TinaXGameKit.Drama.BP
{
    /// <summary>
    /// 对话系统蓝图图表
    /// </summary>
    [CreateAssetMenu(menuName = "TinaX GameKit/剧情设计师/[对话系统] 对话设计蓝图" , fileName = "Dialogue")]
    public class DialogueBluePrint : NodeGraph
    {

        #region Config

        [Header("对话蓝图名")]
        public string DialogueName;

        [Header("默认节点名")]
        public string DefalueKnotsName;

#if UNITY_EDITOR && ODIN_INSPECTOR
        [Space(10)]
        [FoldoutGroup("蓝图全局变量")]
        [Header("文本变量")]
        [TableList]
#else
        [Header("文本变量")]
#endif
        public List<S_BP_VAR_Text> GlobalTextVar = new List<S_BP_VAR_Text>();

#if UNITY_EDITOR && ODIN_INSPECTOR
        [Space(10)]
        [FoldoutGroup("内容 节点绑定")]
#endif
        [Header("文本绑定")]
        public string[] Content_Text_Bind_List = new string[] { };

#if UNITY_EDITOR && ODIN_INSPECTOR
        [FoldoutGroup("内容 节点绑定")]
#endif
        [Header("数值绑定")]
        public string[] Content_Number_Bind_List = new string[] { };


        #endregion


        public override Node AddNode(Type type)
        {
            //Debug.Log("添加一个节点:" + type.Name);

            var node = base.AddNode(type);
            //特殊处理
            



            return node;
        }



        #region 逻辑部分

        private IPlayerForNode mPlayer;
        private DialogueStart mStartNode;
        private Dictionary<string, DialogueKnot> mKnots = new Dictionary<string, DialogueKnot>();

        /// <summary>
        /// 开始Node流程
        /// </summary>
        public void StartNode(IPlayerForNode player)
        {
            mPlayer = player;
            foreach (var item in nodes)
            {
                //Debug.Log("node:" + item.name);
                if (item.name == "Start" && mStartNode == null)
                {
                    mStartNode = (DialogueStart)item;
                }
                if(item.name == "Knot")
                {
                    var knot = (DialogueKnot)item;
                    if (!knot.KnotName.IsNullOrEmpty())
                    {
                        if (!mKnots.ContainsKey(knot.KnotName))
                        {
                            mKnots.Add(knot.KnotName, knot);
                        }
                        
                    }
                }
            }

            //如果有Start
            if (mStartNode != null)
            {
                mStartNode.Trigger(mPlayer);
            }
            else
            {
                //寻找有没有默认节点
                
            }

        }

        /// <summary>
        /// 重新初始化所有蓝图节点，和自身
        /// </summary>
        public void ReInit()
        {
            mStartNode = null;
            mKnots = new Dictionary<string, DialogueKnot>();
            foreach(var item in nodes)
            {
                var bpNode = (DialogueBaseNode)item;
                bpNode.ReInit();
            }
        }

        /// <summary>
        /// 设置String全局蓝图变量
        /// </summary>
        public void SetString(string key, string value)
        {
            foreach(var item in GlobalTextVar)
            {
                if(item.name == key)
                {
                    item.SetValue(value);
                    return;
                }
            }

            GlobalTextVar.Add(new S_BP_VAR_Text()
            {
                name = name,
                value = value,
            });
        }

        /// <summary>
        /// 获取String 全局蓝图变量
        /// </summary>
        /// <param name="key"></param>
        public string GetString(string key)
        {
            foreach(var item in GlobalTextVar)
            {
                return item.value;
            }
            return "";
        }

        public DialogueKnot GetKnot(string name)
        {
            if (mKnots.ContainsKey(name))
            {
                return mKnots[name];
            }
            return null;
        }

        #endregion




        /// <summary>
        /// 蓝图全局变量 TEXT
        /// </summary>
        [System.Serializable]
        public struct S_BP_VAR_Text
        {
            [Header("变量名")]
            public string name;

            [Header("默认值")]
            public string value;

            public void SetValue(string _value)
            {
                value = _value;
            }
        }
    }
}
