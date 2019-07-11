using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


namespace TinaXEditor.VFS
{
    /// <summary>
    /// 存储每个补丁包的记录（每个补丁会生成一个
    /// </summary>
    public class VFSPatchInfoModel
    {
        /// <summary>
        /// 当前补丁距离上一个补丁删掉的文件
        /// </summary>
        public List<SFileHash> DeleteFile;

        /// <summary>
        /// 修改的文件
        /// </summary>
        public List<SFileHash> ModfiyFile;


        /// <summary>
        /// 增加的文件
        /// </summary>
        public List<SFileHash> AddFile;




        /// <summary>
        /// 记录每个文件
        /// </summary>
        [System.Serializable]
        public struct SFileHash
        {
            /// <summary>
            /// VFS文件名
            /// </summary>
            public string VFSFileName;

            public string MD5;
        }
    }
}


