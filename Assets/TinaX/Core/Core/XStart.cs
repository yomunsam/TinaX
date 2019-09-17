using TinaX.Conf;
using UnityEngine;

namespace TinaX
{
    public class XStart
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static async void OnGameStart()
        {
            var curTime = System.DateTime.UtcNow;
            Debug.Log("TinaX Start :" + curTime.ToLongTimeString());
            var main_conf  = Config.GetTinaXConfig<MainConfig>(ConfigPath.main);
            if (main_conf == null)
            {
                Debug.LogWarning("[TinaX] Framework main config file not fount.");
                return;
            }
            if (!main_conf.TinaX_Enable) { return; }

            //启动XCore
            await XCore.I.Init(main_conf);
        }

        public static async void RestartFramework()
        {
            var main_conf = Config.GetTinaXConfig<MainConfig>(ConfigPath.main);
            if (main_conf == null)
            {
                Debug.LogWarning("[TinaX] Framework main config file not fount.");
                return;
            }
            if (!main_conf.TinaX_Enable) { return; }

            //启动XCore
            await XCore.I.Init(main_conf);
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void OnSceneStart()
        {

        }
    }
}

