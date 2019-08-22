using UnityEngine;

namespace TinaX.VFSKit
{
    /// <summary>
    /// 文件夹打包类型
    /// </summary>
    [System.Serializable]
    public enum FolderPackageType
    {
        /// <summary>
        /// Normal (defalut)
        /// </summary>
        [Header("普通打包")]
        normal,
        /// <summary>
        /// 将指定的文件夹整体打包
        /// </summary>
        [Header("将指定的文件夹整体打包")]
        whole,
        /// <summary>
        /// 将指定文件夹的每一个子目录单独打包
        /// </summary>
        [Header("子目录整体打包")]
        sub_dir,

        /// <summary>
        /// 经在编辑器下可加载该路径，实际上不参与打包
        /// </summary>
        [Header("仅在编辑器下有效")]
        EditorOnly, 

    }
}
