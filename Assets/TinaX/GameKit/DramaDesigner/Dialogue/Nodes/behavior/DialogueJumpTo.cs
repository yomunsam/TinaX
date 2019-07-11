using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using UnityEngine;
using XNode;
using TinaX;

namespace TinaXGameKit.Drama.BP
{
    /// <summary>
    /// 对话系统 - 行为 - 结束
    /// </summary>
    [CreateNodeMenu("TinaX GameKit/剧情设计师/[对话] 跳转到节点")]
    public class DialogueJumpTo : DialogueBaseNode
    {
        [Input] public Connection Input;

        [Header("跳转到节点")]
        [Input] public string KnotName;

        public override void DoNext(IPlayerForNode player)
        {

        }

        public override void DoNextWhitParam(IPlayerForNode player, object param)
        {

        }

        public override void Trigger(IPlayerForNode player)
        {
            var _knotName = GetInputValue<string>("KnotName",this.KnotName);
            if (!_knotName.IsNullOrEmpty())
            {
                var bp = graph as DialogueBluePrint;
                var knot = bp.GetKnot(_knotName);
                if (knot == null)
                {
                    Debug.Log("没找到节点：" + _knotName);
                    player.DoFinish("");

                }
                else
                {
                    knot.Trigger(player);
                }
            }
        }
    }
}