using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CatLib;

namespace TinaX
{
    
}

namespace TinaX.Cat
{
    public sealed class SceneProvide : IServiceProvider
    {
        public void Init()
        {

        }

        public void Register()
        {
            App.Singleton<XSceneManager>().Alias<ISceneManager>();
        }
    }
}

