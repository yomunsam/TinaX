using System;
using System.Collections.Generic;

namespace TinaX.VFSKit
{
    /// <summary>
    /// 文件哈希值存储
    /// </summary>
    public class VFSFileHashModel
    {
        public FileHashInfo[] Files;



        private Dictionary<string, string> mHashKV = new Dictionary<string, string>(); //key:path value:hash

        public void Init()
        {
            if (Files == null) Files = new FileHashInfo[0];
            foreach(var item in Files)
            {
                if (!mHashKV.ContainsKey(item.Path))
                {
                    mHashKV.Add(item.Path, item.Hash);
                }
            }
        }

        public string GetMD5ByPath(string path)
        {
            if (mHashKV.ContainsKey(path))
            {
                return mHashKV[path];
            }
            else
            {
                return string.Empty;
            }
        }

    }
    [System.Serializable]
    public struct FileHashInfo
    {
        public string Path;
        public string Hash;
    }
}
