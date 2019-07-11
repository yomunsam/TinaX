using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;
using TinaX;

namespace TinaXGameKit.Drama.BP
{
    /// <summary>
    /// 对话系统 - 描述 - 获取String
    /// </summary>
    [CreateNodeMenu("TinaX GameKit/剧情设计师/[对话] [描述] 获取String变量")]
    public class DialogueGetString : DialogueBaseNode
    {
        [Header("变量名")]
        public string Name;

        [Output( ShowBackingValue.Always, ConnectionType.Multiple)] public Connection Output;


        public override object GetValue(NodePort port)
        {
            if (port.fieldName == "Output")
            {
                var bp = graph as DialogueBluePrint;
                return bp.GetString(Name);
                
            }
            return "";
        }

        protected override void Init()
        {
            name = "GetString";
        }

        public override void DoNext(IPlayerForNode player)
        {
            
        }

        public override void DoNextWhitParam(IPlayerForNode player, object param)
        {
            
        }

        public override void Trigger(IPlayerForNode player)
        {
            
        }
    }
}

