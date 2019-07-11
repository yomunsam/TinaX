using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinaXGameKit.Drama
{
    /// <summary>
    /// 给Node用的接口
    /// </summary>
    public interface IPlayerForNode
    {
        /// <summary>
        /// 执行一个普通内容对话
        /// </summary>
        /// <param name="content"></param>
        void DoContent(BP.DialogueContent content);


        /// <summary>
        /// 执行一个选择分支对话
        /// </summary>
        /// <param name="choose"></param>
        void DoChoose(BP.DialogueChoose choose);

        /// <summary>
        /// 结束
        /// </summary>
        /// <param name="param"></param>
        void DoFinish(string param);
    }
}
