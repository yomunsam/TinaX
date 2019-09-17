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



        private readonly Dictionary<string, string> mHashKV = new Dictionary<string, string>(); //key:path value:hash

        private bool mInited = false;


        public void Init()
        {
            if (mInited) return;
            if (Files == null) Files = new FileHashInfo[0];
            foreach(var item in Files)
            {
                if (!mHashKV.ContainsKey(item.Path))
                {
                    mHashKV.Add(item.Path, item.Hash);
                }
            }
            mInited = true;
        }

        public string GetMD5ByPath(string path)
        {
            if (!mInited) Init();
            if (mHashKV.ContainsKey(path))
            {
                return mHashKV[path];
            }
            else
            {
                return string.Empty;
            }
        }

        public bool IsFileExists(string path)
        {
            if (!mInited) Init();
            return mHashKV.ContainsKey(path);
        }

    }
    [System.Serializable]
    public struct FileHashInfo
    {
        public string Path;
        public string Hash;
    }
}
