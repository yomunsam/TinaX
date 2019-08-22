#if TinaX_CA_LuaRuntime_Enable
using System.Collections.Generic;
using XLua;

namespace TinaXGameKitEditor.EasyBake
{
    public static class EasyBakeLuaBuildConst
    {
        [BlackList]
        public static List<List<string>> BlackList = new List<List<string>>()  {
            ////Unity
            //new List<string>(){ "UnityEngine.UI.Text", "OnRebuildRequested"},


            //new List<string>(){ "TinaX.VFS.XAssetsManager", "Debug_GetABLoadedInfo"},

            //new List<string>(){ "TinaX.Setup", "EditorFrameworkOutsideFolderPath"},
            //new List<string>(){ "TinaX.Setup", "Framework_AssetSystem_Pack_Path"},
            //new List<string>(){ "TinaX.Setup", "Framework_Build_Output_Path"},
            //new List<string>(){ "TinaX.Setup", "Framework_VFS_Patch_Path"},
            //new List<string>(){ "TinaX.Setup", "EditorFrameworkCacheFolder"},

            //new List<string>(){ "TinaX.Lua.LuaBehaviour", "HotOverload"}

        };
    }
}

#endif