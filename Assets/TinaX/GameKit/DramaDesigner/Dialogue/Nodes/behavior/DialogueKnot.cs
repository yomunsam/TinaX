using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace TinaXGameKit.Drama.BP
{
    /// <summary>
    /// 对话系统 - 行为 - 节点入口
    /// </summary>
    [CreateNodeMenu("TinaX GameKit/剧情设计师/[对话] 节点")]
    public class DialogueKnot : DialogueBaseNode
    {
        [Header("节点名")]
        public string KnotName;

        [Output(ShowBackingValue.Unconnected, ConnectionType.Override)]
        public Connection Do;



        protected override void Init()
        {
            base.Init();
            this.name = "Knot";
        }

        // Return the correct value of an output port when requested
        public override object GetValue(NodePort port)
        {
            return null; // Replace this
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
