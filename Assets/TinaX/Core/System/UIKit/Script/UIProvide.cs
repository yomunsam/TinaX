using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TinaX.UIKits;
using CatLib;

namespace TinaX
{
    

    public class UIKit : Facade<IUIKit>
    {
        public static IUIKit I
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
            App.Singleton<XUIMgrGateway>().Alias<IUIKit>();
        }
    }
}
