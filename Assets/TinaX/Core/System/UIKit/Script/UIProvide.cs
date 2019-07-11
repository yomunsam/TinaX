using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TinaX.UIKit;
using CatLib;

namespace TinaX.UIKit
{
    

    public class UIKit : Facade<IUIMgr>
    {
        public static IUIMgr I
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
    public class UIProvide : IServiceProvider
    {
        public void Init()
        {

        }

        public void Register()
        {
            App.Singleton<XUIMgrGateway>().Alias<IUIMgr>();
        }
    }
}
