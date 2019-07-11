using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinaX.Upgrade
{
    /// <summary>
    /// [枚举]检查升级结果反馈
    /// </summary>
    public enum EUpgradeCheckResults
    {
        /// <summary>
        /// 未知错误
        /// </summary>
        error,
        /// <summary>
        /// 未启用更新
        /// </summary>
        not_enable,
        /// <summary>
        /// 功能未实现
        /// </summary>
        expect,
        /// <summary>
        /// 连接服务器失败
        /// </summary>
        connect_error,
        /// <summary>
        /// 已经是最新版本，不需要更新
        /// </summary>
        newest,
        /// <summary>
        /// 需要升级
        /// </summary>
        upgrade,
    }
}
