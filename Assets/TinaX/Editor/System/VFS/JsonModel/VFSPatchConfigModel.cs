


namespace TinaXEditor.VFS
{
    /// <summary>
    /// VFS增量更新配置文件的JSON对象映射模板
    /// </summary>
    public class VFSPatchConfigModel
    {
        public string PlatformName;
        public FileHash[] files;
        public EPatchType PatchType;
        
        [System.Serializable]
        public struct FileHash
        {
            public string FileName;
            public string MD5;
        }


        [System.Serializable]
        public enum EPatchType
        {
            /// <summary>
            /// 只更新最新包
            /// </summary>
            Newest = 0,

            /// <summary>
            /// 安装时依次安装多个包
            /// </summary>
            Each = 1,
        }

    }
}

