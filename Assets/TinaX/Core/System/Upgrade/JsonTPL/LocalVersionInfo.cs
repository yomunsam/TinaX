using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinaX.Upgrade
{
    /// <summary>
    /// 本地Json信息存储
    /// </summary>
    public class LocalVersionInfo
    {
        public int base_version; //母包版本
        public int patch_version; //补丁包版本
    }
}
