using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CatLib;
using TinaX.VFS;

namespace TinaX
{
    

    public class AssetsMgr : Facade<IAssetsManager>
    {
        public static IAssetsManager I
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
    public class AssetProvide : IServiceProvider
    {
        public void Init()
        {

        }

        public void Register()
        {
            App.Singleton<XAssetsManager>().Alias<IAssetsManager>();
        }
    }
}