namespace TinaX.VFSKit
{
    public static class VFSPrivateConfig
    {

        /// <summary>
        /// VFS管理路径白名单
        /// </summary>
        public static readonly string[] VFS_WhiteList =
        {
#if TinaX_CA_LuaRuntime_Enable
            Setup.Framework_Lua_RootPath,
#endif
        };

        /// <summary>
        /// 特殊打包规则
        /// </summary>
        public static readonly FolderPackageRule[] VFS_FolderPackRule =
        {

        };

        /// <summary>
        /// 忽略路径
        /// </summary>
        public static readonly string[] VFS_Ignore_Path =
        {
            Setup.Framework_Path,
        };

        /// <summary>
        /// 针对忽略路径的白名单
        /// </summary>
        public static readonly string[] VFS_Ignore_Path_Whitelist =
        {
#if TinaX_CA_LuaRuntime_Enable
            Setup.Framework_Lua_RootPath,
#endif
        };

        /// <summary>
        /// 后缀名忽略规则
        /// </summary>
        public static readonly string[] Ignore_ExtName =    //加不加点都行，反正预处理的时候都会给加上点
        {
            //Code
            "cs",
            "cpp",
            "h",
            //Document
            "doc",
            "docx",
            "ppt",
            "pptx",
            "md",
            "wps",
            "pdf",
            "rtf",
            "odf",
            //编辑中间件或包
            "dll",
            "so",
            "exe",
            "apk",
            "ipa",
            "appx",
            "deb",
            //工程文件
            "psd",
            "afphoto",
            //Unity
            "meta",
            "unitypackage",

        };

        /// <summary>
        /// 忽略路径项
        /// </summary>
        public static readonly string[] Ignore_Path_Item =
        {
            "Editor",
            "Resources",
            //"Plugins",
            "StreamingAssets",
            "Gizmos",
        };


        /// <summary>
        /// 加密设置
        /// 
        /// 
        /// 


        /// </summary>
        public static readonly FolderEncryDefine[] VFS_EncryFolder =
        {

        };

    }
}

