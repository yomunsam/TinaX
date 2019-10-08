#if UNITY_EDITOR
using UnityEngine;

namespace TinaX.VFSKit
{
    public enum ImportNotLettersOrNumsHandleType 
    {
        [Header("Warning | 警告")]
        Warning,
        [Header("Rename | 自动改名")]
        Rename,
        [Header("Delete | 删除文件")]
        Delete,
        

    }

}



#endif