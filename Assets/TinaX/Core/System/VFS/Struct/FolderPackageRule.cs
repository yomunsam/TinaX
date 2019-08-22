using UnityEngine;
#if UNITY_EDITOR && ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace TinaX.VFSKit
{
    /// <summary>
    /// 文件夹打包规则
    /// </summary>
    [System.Serializable]
    public struct FolderPackageRule
    {
        [Header("文件夹打包路径")]
#if UNITY_EDITOR && ODIN_INSPECTOR
        [FolderPath(RequireExistingPath = true)]
#endif
        public string FolderPath;

        [Header("打包方法")]
        public FolderPackageType PackType;

    }
}
