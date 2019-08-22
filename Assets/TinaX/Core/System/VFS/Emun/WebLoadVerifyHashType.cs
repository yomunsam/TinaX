using UnityEngine;

namespace TinaX.VFSKit
{
    [System.Serializable]
    public enum WebLoadVerifyHashType
    {
        [Header("从服务器GET获取MD5")]
        GetMD5FromServer,

        [Header("从服务器GET获取所有文件的MD5列表")]
        GetFileMD5ListFromServer,

        [Header("使用本地MD5列表")]
        UseLocalMD5List,
    }
}
