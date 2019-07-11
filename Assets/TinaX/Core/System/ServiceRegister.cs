using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CatLib;
using TinaX.Cat;

namespace TinaX
{
    /// <summary>
    /// 框架层服务注册
    /// </summary>
    internal class ServiceRegister
    {
        public static IServiceProvider[] Framework_Service_Reg
        {
            get
            {
                return new IServiceProvider[]
                {
                    new SceneProvide(),
                    //new LogProvide(),
                    new AssetProvide(),
                    new UIProvide(),
                    new I18NProvide(),
                    new TimeMachineProvide(),
#if TinaX_CA_LuaRuntime_Enable
                    new LuaProvide(),
#endif
                    new XSoundProvide(),
                    //new XDebugProvide(),
                };
            }
        }
    }
}

