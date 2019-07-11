using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;
using TinaX;

namespace TinaXGameKit.Drama.BP
{
    /// <summary>
    /// 对话系统 - 行为 - 内容
    /// </summary>
    [CreateNodeMenu("TinaX GameKit/剧情设计师/[对话] 内容")]
    [NodeTint("#CCFFCC")]
    public class DialogueContent : DialogueBaseNode , IDialogContent
    {
        [Input] public Connection Input;
        [Output] public Connection output;

        [Input]
        [TextArea]
        [Header("显示内容")]
        public string Content;

        [Input]
        [Header("讲述者")]
        [Tooltip("默认留空")]
        public string Speaker;


        [Header("文本绑定")]
        public TextBindTpl[] TextBind = new TextBindTpl[] { };

        [Header("数值绑定")]
        public NumberBindTpl[] NumberBind = new NumberBindTpl[] { };

        /// <summary>
        /// 获取正文内容
        /// </summary>
        /// <returns></returns>
        public string GetContent()
        {
            return GetInputValue<string>("Content", this.Content);
        }

        public string GetSpeaker()
        {
            return GetInputValue<string>("Speaker", this.Speaker);
        }

        public TextBindTpl[] GetTextBind()
        {
            return TextBind;
        }

        public NumberBindTpl[] GetNumberBind()
        {
            return NumberBind;
        }
        
        protected override void Init()
        {
            base.Init();
            base.name = "Content";

            var basebp = (DialogueBluePrint)graph;
            if (TextBind.Length == 0 && basebp.Content_Text_Bind_List.Length > 0)
            {
                List<TextBindTpl> tempList = new List<TextBindTpl>();
                foreach (var item in basebp.Content_Text_Bind_List)
                {
                    tempList.Add(new TextBindTpl() {
                        key = item
                    });
                }
                TextBind = tempList.ToArray();
            }
            if (NumberBind.Length == 0 && basebp.Content_Number_Bind_List.Length > 0)
            {
                List<NumberBindTpl> tempList = new List<NumberBindTpl>();
                foreach (var item in basebp.Content_Number_Bind_List)
                {
                    tempList.Add(new NumberBindTpl()
                    {
                        key = item
                    });
                }
                NumberBind = tempList.ToArray();
            }
        }

        
        public override object GetValue(NodePort port)
        {
            return null; // Replace this
        }


        public override void DoNext(IPlayerForNode player)
        {
            var output = GetOutputPort("output");
            if (output.ConnectionCount >= 1)
            {
                var next = output.GetConnection(0).node as DialogueBaseNode;
                next.Trigger(player);
            }
            else
            {
                //意外结束
                player.DoFinish("");
            }
        }

        public override void DoNextWhitParam(IPlayerForNode player, System.Object param)
        {
            //throw new System.NotImplementedException();
        }

        public override void Trigger(IPlayerForNode player)
        {
            player.DoContent(this);
        }


    }
}
