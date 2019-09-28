using System.Collections;
using System.Collections.Generic;
using CatLib;

namespace TinaX
{
    

    public class XI18N : Facade<I18NKit.IXI18N>
    {
        public static I18NKit.IXI18N I
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
            App.Singleton<I18NKit.XI18NMgr>().Alias<I18NKit.IXI18N>();
        }
    }
}