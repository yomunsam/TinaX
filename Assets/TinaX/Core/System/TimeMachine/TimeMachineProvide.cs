using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CatLib;
using TinaX.TimeMachines;

namespace TinaX
{

    public class TimeMachine : Facade<ITimeMachine>
    {
        public static ITimeMachine I
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
    public class TimeMachineProvide : IServiceProvider
    {
        public void Init()
        {

        }

        public void Register()
        {
            App.Singleton<XTimeMachine>().Alias<ITimeMachine>();
        }
    }
}

