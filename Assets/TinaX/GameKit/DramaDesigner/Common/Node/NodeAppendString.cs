using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using XNode;

namespace TinaXGameKit.Drama.BP
{
    /// <summary>
    /// 对话系统 - 行为 - 节点入口
    /// </summary>
    [CreateNodeMenu("TinaX GameKit/通用/拼接字符串")]
    public class NodeAppendString : DialogueBaseNode
    {
        [Input] public string Text1;
        [Input] public string Text2;

        [Output(ShowBackingValue.Always, ConnectionType.Override)]
        public Connection Output;



        protected override void Init()
        {
            
            this.name = "Append String";
        }

        // Return the correct value of an output port when requested
        public override object GetValue(NodePort port)
        {
            if (port.fieldName == "Output")
            {
                var t1 = GetInputValue<string>("Text1", this.Text1);
                var t2 = GetInputValue<string>("Text2", this.Text2);
                return t1 + t2;
            }
            return null;
        }


        public override void Trigger(IPlayerForNode player)
        {
            var output = GetOutputPort("Do");
            if (output.ConnectionCount >= 1)
            {
                var next = output.GetConnection(0).node as DialogueBaseNode;
                next.Trigger(player);
            }
        }

        public override void DoNext(IPlayerForNode player)
        {

        }

        public override void DoNextWhitParam(IPlayerForNode player, System.Object param)
        {

        }

    }

}
