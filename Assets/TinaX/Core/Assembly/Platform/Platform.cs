using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TinaX
{
    public static class Platform
    {
        public static string GetPlatformName(UnityEngine.RuntimePlatform platform)
        {
            switch (platform)
            {
                case UnityEngine.RuntimePlatform.WindowsPlayer:
                    return Const.PlatformConst.E_Platform.Windows64.ToString();
                case UnityEngine.RuntimePlatform.LinuxPlayer:
                    return Const.PlatformConst.E_Platform.Linux64.ToString();
                case UnityEngine.RuntimePlatform.OSXPlayer:
                    return Const.PlatformConst.E_Platform.OSX.ToString();
                case UnityEngine.RuntimePlatform.Android:
                    return Const.PlatformConst.E_Platform.Android.ToString();
                case UnityEngine.RuntimePlatform.IPhonePlayer:
                    return Const.PlatformConst.E_Platform.iOS.ToString();
                case UnityEngine.RuntimePlatform.XboxOne:
                    return Const.PlatformConst.E_Platform.XBox.ToString();
                case UnityEngine.RuntimePlatform.Switch:
                    return Const.PlatformConst.E_Platform.NSwitch.ToString();
                case UnityEngine.RuntimePlatform.WebGLPlayer:
                    return Const.PlatformConst.E_Platform.WebGL.ToString();
                case UnityEngine.RuntimePlatform.WSAPlayerARM:
                    return Const.PlatformConst.E_Platform.UWP.ToString();
                case UnityEngine.RuntimePlatform.WSAPlayerX64:
                    return Const.PlatformConst.E_Platform.UWP.ToString();
                case UnityEngine.RuntimePlatform.WSAPlayerX86:
                    return Const.PlatformConst.E_Platform.UWP.ToString();

                //EDITORS
                case UnityEngine.RuntimePlatform.WindowsEditor:
                    return Const.PlatformConst.E_Platform.Windows64.ToString();
                case UnityEngine.RuntimePlatform.LinuxEditor:
                    return Const.PlatformConst.E_Platform.Linux64.ToString();
                case UnityEngine.RuntimePlatform.OSXEditor:
                    return Const.PlatformConst.E_Platform.OSX.ToString();


                default:
                    return Const.PlatformConst.E_Platform.Windows64.ToString();
            }
        }

        /// <summary>
        /// 获取平台定义
        /// </summary>
        /// <param name="platform"></param>
        /// <returns></returns>
        public static TinaX.Const.PlatformConst.E_Platform GetPlatform(UnityEngine.RuntimePlatform platform)
        {
            switch (platform)
            {
                case UnityEngine.RuntimePlatform.WindowsPlayer:
                    return Const.PlatformConst.E_Platform.Windows64;
                case UnityEngine.RuntimePlatform.LinuxPlayer:
                    return Const.PlatformConst.E_Platform.Linux64;
                case UnityEngine.RuntimePlatform.OSXPlayer:
                    return Const.PlatformConst.E_Platform.OSX;
                case UnityEngine.RuntimePlatform.Android:
                    return Const.PlatformConst.E_Platform.Android;
                case UnityEngine.RuntimePlatform.IPhonePlayer:
                    return Const.PlatformConst.E_Platform.iOS;
                case UnityEngine.RuntimePlatform.XboxOne:
                    return Const.PlatformConst.E_Platform.XBox;
                case UnityEngine.RuntimePlatform.Switch:
                    return Const.PlatformConst.E_Platform.NSwitch;
                case UnityEngine.RuntimePlatform.WebGLPlayer:
                    return Const.PlatformConst.E_Platform.WebGL;
                case UnityEngine.RuntimePlatform.WSAPlayerARM:
                    return Const.PlatformConst.E_Platform.UWP;
                case UnityEngine.RuntimePlatform.WSAPlayerX64:
                    return Const.PlatformConst.E_Platform.UWP;
                case UnityEngine.RuntimePlatform.WSAPlayerX86:
                    return Const.PlatformConst.E_Platform.UWP;

                //EDITORS
                case UnityEngine.RuntimePlatform.WindowsEditor:
                    return Const.PlatformConst.E_Platform.Windows64;
                case UnityEngine.RuntimePlatform.LinuxEditor:
                    return Const.PlatformConst.E_Platform.Linux64;
                case UnityEngine.RuntimePlatform.OSXEditor:
                    return Const.PlatformConst.E_Platform.OSX;


                default:
                    return Const.PlatformConst.E_Platform.Windows64;
            }
        }



#if UNITY_EDITOR
        public static BuildTarget GetBuildTarget(TinaX.Const.PlatformConst.E_Platform platform)
            {
                switch (platform)
                {
                    default:
                        return BuildTarget.StandaloneWindows64;
                    case TinaX.Const.PlatformConst.E_Platform.Android:
                        return BuildTarget.Android;
                    case TinaX.Const.PlatformConst.E_Platform.iOS:
                        return BuildTarget.iOS;
                    case TinaX.Const.PlatformConst.E_Platform.Linux64:
                        return BuildTarget.StandaloneLinux64;
                    case TinaX.Const.PlatformConst.E_Platform.NSwitch:
                        return BuildTarget.Switch;
                    case TinaX.Const.PlatformConst.E_Platform.OSX:
                        return BuildTarget.StandaloneOSX;
                    case TinaX.Const.PlatformConst.E_Platform.WebGL:
                        return BuildTarget.WebGL;
                    case TinaX.Const.PlatformConst.E_Platform.Windows64:
                        return BuildTarget.StandaloneWindows64;
                    case TinaX.Const.PlatformConst.E_Platform.XBox:
                        return BuildTarget.XboxOne;
                    case TinaX.Const.PlatformConst.E_Platform.UWP:
                        return BuildTarget.WSAPlayer;

                }
            }
#endif
    }
}