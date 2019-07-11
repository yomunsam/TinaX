using System.Collections;
using System.Collections.Generic;
using CatLib;

namespace TinaX
{
    

    public class XI18N : Facade<I18N.IXI18N>
    {
        public static I18N.IXI18N I
        {
            get
            {
                return XI18N.Instance;
            }
        }
    }

}

namespace TinaX.Cat
{
    public class I18NProvide : IServiceProvider
    {
        public void Init()
        {

        }

        public void Register()
        {
            App.Singleton<I18N.XI18NMgr>().Alias<I18N.IXI18N>();
        }
    }
}