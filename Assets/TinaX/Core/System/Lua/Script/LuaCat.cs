#if TinaX_CA_LuaRuntime_Enable

using TinaX.Lua;
using CatLib;

namespace TinaX
{
    


    public class LuaScript : Facade<ILuaMgr>
    {
        public static ILuaMgr I
        {
            get
            {
                return Instance;
            }
        }
    }

}

namespace TinaX.Cat
{
    public class LuaProvide : IServiceProvider
    {
        public void Init()
        {

        }

        public void Register()
        {
            App.Singleton<LuaManager>().Alias<ILuaMgr>();
        }
    }
}


#endif