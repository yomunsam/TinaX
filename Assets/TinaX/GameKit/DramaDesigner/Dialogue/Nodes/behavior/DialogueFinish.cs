using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace TinaXGameKit.Drama.BP
{
    /// <summary>
    /// 对话系统 - 行为 - 结束
    /// </summary>
    [CreateNodeMenu("TinaX GameKit/剧情设计师/[对话] 结束")]
    public class DialogueFinish : DialogueBaseNode
    {
        [Input] public Connection Input;

        [Header("结束消息")]
        [Input] public string Message;

        public override void DoNext(IPlayerForNode player)
        {
            
        }

        public override void DoNextWhitParam(IPlayerForNode player, object param)
        {
            
        }

        public override void Trigger(IPlayerForNode player)
        {
            player.DoFinish(Message);
        }
    }
}