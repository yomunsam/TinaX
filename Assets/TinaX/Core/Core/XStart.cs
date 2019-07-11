using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TinaX.Conf;

namespace TinaX
{
    public class XStart
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void OnGameStart()
        {
            var main_conf  = Config.GetTinaXConfig<MainConfig>(ConfigPath.main);
            if (main_conf == null)
            {
                Debug.LogWarning("[TinaX] Framework main config file not fount.");
                return;
            }
            if (!main_conf.TinaX_Enable) { return; }

            //启动XCore
            XCore.I.Init(main_conf);
        }

        public static void RestartFramework()
        {
            var main_conf = Config.GetTinaXConfig<MainConfig>(ConfigPath.main);
            if (main_conf == null)
            {
                Debug.LogWarning("[TinaX] Framework main config file not fount.");
                return;
            }
            if (!main_conf.TinaX_Enable) { return; }

            //启动XCore
            XCore.I.Init(main_conf);
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void OnSceneStart()
        {

        }
    }
}

