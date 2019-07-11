using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinaXGameKit.Drama.BP
{
    /// <summary>
    /// 对话系统 选择 接口
    /// </summary>
    public interface IDialogChoose
    {
        /// <summary>
        /// 获取选择选项列表
        /// </summary>
        /// <returns></returns>
        string[] GetChooseList();

        /// <summary>
        /// 获取讲述者
        /// </summary>
        /// <returns></returns>
        string GetSpeaker();

        /// <summary>
        /// 获取标题
        /// </summary>
        /// <returns></returns>
        string GetTitle();
    }
}
