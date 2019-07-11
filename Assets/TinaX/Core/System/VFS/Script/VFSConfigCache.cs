using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
//using System.Threading.Tasks;

namespace TinaX.VFS
{
    /// <summary>
    /// VFSConfigModel Cache
    /// </summary>
    public class VFSConfigCache
    {
        #region Property

        /// <summary>
        /// VFS 资源管理白名单
        /// </summary>
        public List<string> VFS_WhiteList = new List<string>();

        /// <summary>
        /// 特殊打包规则
        /// </summary>
        public List<S_Dir_PackInfo> VFS_SpecialFolder = new List<S_Dir_PackInfo>();

        /// <summary>
        /// 全局忽略路径（及其子目录
        /// </summary>
        public List<string> VFS_IgnorePath = new List<string>();

        /// <summary>
        /// 全局忽略的后缀名
        /// </summary>
        public List<string> VFS_IgnoreExt = new List<string>();

        /// <summary>
        /// VFS 全局忽略关键字（路径这有命中相关关键字的话，则忽略
        /// </summary>
        public List<string> VFS_IgnorePathKeyword = new List<string>();

        /// <summary>
        /// VFS 加密文件列表
        /// </summary>
        public List<string> VFS_EncryFileList = new List<string>();

        /// <summary>
        /// VFS 加密路径列表
        /// </summary>
        public List<string> VFS_EncryPathList = new List<string>();


        


        #endregion

        #region Static

        public static VFSConfigCache New(VFSConfigModel model)
        {
            var cache = new VFSConfigCache();
            cache.VFS_WhiteList.AddRange(model.Assets_system_whiteList);

            cache.VFS_SpecialFolder.AddRange(model.Special_Package_Folder);

            cache.VFS_IgnorePath.AddRange(model.Ignore_Path);
            cache.VFS_IgnorePath.AddRange(TinaX.AssetRuleConst.Ignore_Path);

            cache.VFS_IgnoreExt.AddRange(model.Ignore_Ext);
            cache.VFS_IgnoreExt.AddRange(TinaX.AssetRuleConst.Ignore_Ext);

            cache.VFS_IgnorePathKeyword.AddRange(model.Ignore_Path_keyword);
            cache.VFS_IgnorePathKeyword.AddRange(TinaX.AssetRuleConst.Ignore_Path_keyword);

            cache.VFS_EncryFileList.AddRange(model.EncryFileList);

            cache.VFS_EncryPathList.AddRange(model.EncryPathList);

            return cache;
        }

        #endregion
    }
}
