using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CatLib;
using TinaX.VFS;
//using System;

namespace TinaX
{
    
    [System.Obsolete("AssetsMgr API is obsolete, please use VFSMgr instead.")]
    public class AssetsMgr : Facade<IVFS>
    {
        public static IVFS I
        {
            get
            {
                return Instance;
            }
        }
    }

    public class VFSMgr : Facade<IVFS>
    {
        public static IVFS I
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
    public class VFSProvide : IServiceProvider
    {
        public void Init()
        {

        }

        public void Register()
        {
            App.Singleton<XAssetsManager>().Alias<IVFS>();
        }
    }
}