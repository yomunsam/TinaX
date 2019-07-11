using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TinaX.I18N
{
    /// <summary>
    /// 国际化系统
    /// </summary>
    public interface IXI18N
    {
        void Start();

        /// <summary>
        /// 设置地区
        /// </summary>
        /// <param name="region"></param>
        /// <returns></returns>
        IXI18N UseRegion(string region);
        /// <summary>
        /// 根据Key获取相应的字符
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        string GetString(string key);
    }

}
