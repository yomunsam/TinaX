using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CatLib;

namespace TinaX
{
    public sealed class SceneMgr : Facade<ISceneManager>
    {
        public static ISceneManager I
        {
            get
            {
                return SceneMgr.Instance;
            }
        }
    }

}
