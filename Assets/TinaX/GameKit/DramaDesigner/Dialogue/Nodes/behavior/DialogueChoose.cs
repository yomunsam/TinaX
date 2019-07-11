using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;
using TinaX;

namespace TinaXGameKit.Drama.BP
{
    /// <summary>
    /// 对话系统 - 行为 - 选择分支
    /// </summary>
    [CreateNodeMenu("TinaX GameKit/剧情设计师/[对话] 选择分支")]
    [NodeTint("#CCCCFF")]
    public class DialogueChoose : DialogueBaseNode, IDialogChoose
    {
        [Input] public DialogueContent.Connection Input;
        [Input] [TextArea] [Header("对话标题")][Tooltip("询问对话的标题，可留空")]public string Title;
        [Input] [Header("询问者")] [Tooltip("询问者，默认留空")] public string Speaker;
        
        public List<S_Choose> ChooseList;

        /// <summary>
        /// 如果某个选项只能选择一次的话，那么当它被选中之后，就会记录到这里，下次再获取选项列表的时候就可以排除这个选项了
        /// </summary>
        private List<string> mOnceSelectList = new List<string>();

        public string GetTitle()
        {
            return GetInputValue<string>("Title", this.Title);
        }

        public string GetSpeaker()
        {
            return GetInputValue<string>("Speaker", this.Speaker);
        }


        protected override void Init()
        {
            //base.Init();
            base.name = "Choose";
        }

        /// <summary>
        /// 获取选择选项列表
        /// </summary>
        /// <returns></returns>
        public string[] GetChooseList()
        {
            List<string> slist = new List<string>();
            foreach(var item in ChooseList)
            {
                if (item.Once)
                {
                    if (!mOnceSelectList.Contains(item.Content))
                    {
                        slist.Add(item.Content);
                    }
                }
                else
                {
                    slist.Add(item.Content);
                }
                
            }
            return slist.ToArray();
        }

        public override object GetValue(NodePort port)
        {
            return null; // Replace this
        }

        /// <summary>
        /// 选择分支
        /// </summary>
        [System.Serializable]
        public struct S_Choose
        {
            [Header("选择显示内容")]
            public string Content;
            [Header("单次选项")]
            public bool Once;
        }

        public override void DoNext(IPlayerForNode player)
        {
            //throw new System.NotImplementedException();
        }

        public override void DoNextWhitParam(IPlayerForNode player, System.Object param)
        {
            var select_info = (string)param;
            //Debug.Log("走到这里了，附带参数：" + select_info);
            if (!select_info.IsNullOrEmpty())
            {
                int index = -1;
                //Debug.Log("总数  " + ChooseList.Count);
                for (int i = 0; i < ChooseList.Count; i++)
                {
                    if(ChooseList[i].Content == select_info)
                    {
                        index = i;
                        //是否是单次选择节点
                        if (ChooseList[i].Once)
                        {
                            //记录它
                            mOnceSelectList.Add(ChooseList[i].Content);
                        }
                        break;
                    }
                }
                if (index == -1)
                {
                    player.DoFinish("");
                    return;
                } 

                var port = GetOutputPort("ChooseList " + index);
                if (port == null)
                {
                    player.DoFinish("");
                    return;
                }

                if (port.ConnectionCount >= 1)
                {
                    var next = port.GetConnection(0).node as DialogueBaseNode;
                    next.Trigger(player);
                }
            }
        }

        public override void Trigger(IPlayerForNode player)
        {
            player.DoChoose(this);
        }

        /// <summary>
        /// 重置变量
        /// </summary>
        public override void ReInit()
        {
            mOnceSelectList = new List<string>();
        }

    }
}
