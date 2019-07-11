
namespace TinaX
{
    /// <summary>
    /// TinaX 资源系统 内部规则
    /// </summary>
    public class AssetRuleConst
    {
        

        /// <summary>
        /// 忽略配置的路径及其子路径的文件
        /// </summary>
        public readonly static string[] Ignore_Path =
        {
            //TinaX自身目录不参与资源打包
            Setup.Framework_Path
        };

        /// <summary>
        /// 上面这个列表中的白名单
        /// </summary>
        public readonly static string[] Ignore_Path_whileList =
        {
#if TinaX_CA_LuaRuntime_Enable
            Setup.Framework_Path + "/Lua"
#endif
        };

        /// <summary>
        /// TinaX内部强制打包路径
        /// </summary>
        public readonly static string[] Tinax_Pack_Path =
        {
#if TinaX_CA_LuaRuntime_Enable
            Setup.Framework_Path + "/Lua"
#endif
        };

        /// <summary>
        /// 忽略配置中的后缀名（不加"."点）
        /// </summary>
        public readonly static string[] Ignore_Ext =
        {
            //代码文件
            "cs",
            "cpp",
            //文档
            "doc",
            "docx",
            "xls",
            "xlsx",
            "md",
            "wps",
            "pdf",
            "rtf",
            "ppt",
            "odf",
            //编辑中间件或包
            "dll",
            "so",
            "exe",
            "apk",
            "ipa",
            "appx",
            "deb",
            //美术原始素材
            "psd",
            //压缩包
            "zip",
            "7z",
            "rar",
            "iso",
            //Unity
            "meta",
            "unitypackage"
        };

        /// <summary>
        /// 如果文件路径中包含相关关键字，则忽略
        /// </summary>
        public readonly static string[] Ignore_Path_keyword =
        {
            "Editor",
            "Resources",
            "Plugins",
            "StreamingAssets",
            "Gizmos",
        };
    }
}