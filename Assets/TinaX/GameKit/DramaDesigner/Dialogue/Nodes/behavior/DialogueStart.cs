using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace TinaXGameKit.Drama.BP
{
    /// <summary>
    /// 对话系统 - 行为 - 开始
    /// </summary>
    [CreateNodeMenu("TinaX GameKit/剧情设计师/[对话] 开始")]
    public class DialogueStart : DialogueBaseNode
    {
        [Output( ShowBackingValue.Unconnected, ConnectionType.Override)]
        public Connection Do;
        
        public readonly string KnotName = "Start";


        protected override void Init()
        {
            base.Init();
            this.name = "Start";
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
            //throw new System.NotImplementedException();
        }

        public override void DoNextWhitParam(IPlayerForNode player, System.Object param)
        {
            //throw new System.NotImplementedException();
        }
    }

}
