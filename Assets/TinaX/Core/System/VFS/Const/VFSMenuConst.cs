#if UNITY_EDITOR

namespace TinaX.VFSKit
{
    public static class VFSMenuConst
    {
        public const string MenuStr_AssetSimulation = "TinaX/Editor/Asset Simulation";

        public const string MenuStr_LoadAssetBundleFromTinaXWorkFolder = MenuStr_AssetSimulation + "/Load from TinaX work folder "; //从与工程同级的那个TinaXWorkFolder文件夹里加载资源

        public const string MenuStr_LoadAssetBundleFromStramingAssets = MenuStr_AssetSimulation + "/Load from StreamingAssets";

        public const string MenuStr_LoadAssetFromResources = MenuStr_AssetSimulation + "/Load from Resources";

        public const string MenuStr_LoadAssetByAssetDatabase = MenuStr_AssetSimulation + "/Use AssetDatabse";
    }
}

#endif