//因为AssetBundle是分平台的，所以啊，从网络加载的url得一一对应
using UnityEngine;
#if UNITY_EDITOR && ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif


namespace TinaX.VFSKit
{
    [System.Serializable]
    public struct WebVFSConfig
    {
        [Header("平台")]
        public TinaX.Const.PlatformConst.E_Platform Platform;

        [Header("Url")]
        public string Url;

        [Header("验证文件哈希值")]
        public bool VerifyFileHash;

#if UNITY_EDITOR && ODIN_INSPECTOR
        [InfoBox("使用服务器MD5列表进行验证时，获取md5列表的url")]
        [ShowIf("VerifyFileHash")]
#endif
        [Header("验证哈希值方式")]
        public WebLoadVerifyHashType WebLoadVerifyType;



#if UNITY_EDITOR && ODIN_INSPECTOR
        [InfoBox("使用服务器MD5列表进行验证时，获取md5列表的url")]
        [ShowIf("IsGetMD5List")]
        [ShowIf("VerifyFileHash")]
#endif
        [Header("获取MD5列表URL")]
        public string GetMD5ListUrl;

#if UNITY_EDITOR && ODIN_INSPECTOR
        [InfoBox("从服务器直接使用GET方式获取MD5信息的基础URL")]
        [ShowIf("IsGetMD5Info")]
        [ShowIf("VerifyFileHash")]
#endif
        [Header("获取MD5列表URL")]
        public string GetMD5Url;


#if UNITY_EDITOR && ODIN_INSPECTOR
        private bool IsGetMD5List() => WebLoadVerifyType == WebLoadVerifyHashType.GetFileMD5ListFromServer;

        private bool IsGetMD5Info() => WebLoadVerifyType == WebLoadVerifyHashType.GetMD5FromServer;
#endif

    }
}
