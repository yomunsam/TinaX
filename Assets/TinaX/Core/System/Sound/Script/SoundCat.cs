using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CatLib;
using TinaX.Sound;

namespace TinaX.Cat
{
    public class XSoundProvide : IServiceProvider
    {
        public void Init()
        {
            
        }

        public void Register()
        {
            App.Singleton<IXSound, XSoundMgr>();
        }
    }
}
namespace TinaX
{
    public class XSound : Facade<IXSound>
    {
        public static IXSound I
        {
            get
            {
                return Instance;
            }
        }
    }
}
