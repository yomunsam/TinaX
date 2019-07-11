using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinaXGameKit.Drama.BP
{
    /// <summary>
    /// 对话系统 普通正文 接口
    /// </summary>
    public interface IDialogContent
    {
        /// <summary>
        /// 获取正文内容
        /// </summary>
        /// <returns></returns>
        string GetContent();

        /// <summary>
        /// 获取讲述者
        /// </summary>
        /// <returns></returns>
        string GetSpeaker();

        /// <summary>
        /// 获取文本绑定
        /// </summary>
        /// <returns></returns>
        DialogueBaseNode.TextBindTpl[] GetTextBind();

        /// <summary>
        /// 获取数字绑定
        /// </summary>
        /// <returns></returns>
        DialogueBaseNode.NumberBindTpl[] GetNumberBind();
    }
}
