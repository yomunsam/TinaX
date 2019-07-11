using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CatLib;

namespace TinaX
{
    /// <summary>
    /// TinaX 启动引导
    /// </summary>
    public class XBootstrap : IBootstrap
    {
        [Priority(100)]
        public void Bootstrap()
        {
            foreach(var item in ServiceRegister.Framework_Service_Reg)
            {
                if (!App.IsRegisted(item))
                {
                    App.Register(item);
                }
            }
        }
    }

}

