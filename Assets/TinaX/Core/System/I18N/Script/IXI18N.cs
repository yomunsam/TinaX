using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

namespace TinaX.I18NKit
{
    /// <summary>
    /// 国际化系统
    /// </summary>
    public interface IXI18N
    {

        System.Action<string, string> OnRegionSwitched { get; set; }

        void Start();

        /// <summary>
        /// Use Region | 设置地区
        /// </summary>
        /// <param name="region"></param>
        /// <returns></returns>
        IXI18N UseRegion(string regionName);

        /// <summary>
        /// Use Region async (WebVFS enable) | 异步 设置地区（WebVFS可用）
        /// </summary>
        /// <param name="regionName"></param>
        /// <returns></returns>
        Task UseRegionAsync(string regionName);

        /// <summary>
        /// 根据Key获取相应的字符
        /// </summary>
        /// <param name="key"></param>
        /// <param name="group">group name</param>
        /// <returns></returns>
        string GetString(string key, string group = I18NConst.DefaultGroupName);
    }

}
