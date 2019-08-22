using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR && ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace TinaX.VFSKit
{
    /// <summary>
    /// 目录加密定义
    /// </summary>
    [System.Serializable]
    public struct FolderEncryDefine 
    {
#if UNITY_EDITOR && ODIN_INSPECTOR
        [FolderPath( RequireExistingPath = true)]
#endif
        [Header("目录路径")]
        public string FolderPath;

#if UNITY_EDITOR && ODIN_INSPECTOR
        [Header("加密方式")]
#else
        [EnumLabel("加密方式")]
#endif

        public EncryptionType EncryType;

    }
}

