/* TinaX Setup
 * 
*/
using System.IO;

namespace TinaX
{
    public static class Setup
    {
        /// <summary>
        /// 框架主目录在工程中的位置（Unity标准路径）
        /// </summary>
        public static readonly string Framework_Path = "Assets/TinaX";

        public static readonly string Framework_Lua_RootPath = Framework_Path + "/Lua";

        public static readonly string Framework_Lua_Init = Framework_Lua_RootPath + "/core/init";

        /// <summary>
        /// 框架配置文件目录（相对于Resources文件夹）
        /// </summary>
        public const string Framework_Config_Path = "TinaX/Config";

        /// <summary>
        /// 框架在场景中的基础GameObject名
        /// </summary>
        public const string Framework_Base_GameObject = "TinaX";

        public const string Framework_LocalStorage_TinaX = "tinaxCache";

        public const string Framework_LocalStorage_App = "appCache";


        public const string Framework_VFS_AssetBundle_Ext_Name = ".xab";

        /// <summary>
        /// 补丁包文件后缀名
        /// </summary>
        public const string Framework_Patch_Ext_Name = ".xpk";




#if UNITY_EDITOR

        /// <summary>
        /// Framework在工程Assets文件夹之外的工作目录
        /// </summary>
        public static readonly string EditorFrameworkOutsideFolderPath = "TinaXWorkFolder";

        /// <summary>
        /// 框架资源包打包路径
        /// </summary>
        public static string Framework_AssetSystem_Pack_Path = Path.Combine(EditorFrameworkOutsideFolderPath, "TinaX_VFS", "Platform");

        public static string Framework_Build_Output_Path = Path.Combine(EditorFrameworkOutsideFolderPath, "TinaX_VFS", "Build");

        public static string Framework_VFS_Patch_Path = Path.Combine(EditorFrameworkOutsideFolderPath,"TinaX_VFS","Patch");


        public static readonly string EditorFrameworkCacheFolder = Framework_Path + "/Editor/EditorCache";

#endif

    }
}