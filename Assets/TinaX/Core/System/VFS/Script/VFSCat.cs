using CatLib;

namespace TinaX
{
    public class VFS : Facade<IVFS>
    {
        public static TinaX.IVFS I => VFS.Instance;
    }
}


namespace TinaX.Cat
{
    public class VFSProvide : IServiceProvider
    {
        public void Init()
        {

        }

        public void Register()
        {
            App.Singleton<IVFS,VFSKit.VFSKit>();
        }
    }
}