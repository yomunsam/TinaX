using Nekonya.Demo.Assets;

namespace TinaX.Services
{
    public static class AssetsExtend
    {
        public static IXCore UseDemoAssets(this IXCore core)
        {
            core.RegisterServiceProvider(new AssetsProvider());
            return core;
        }
    }
}
