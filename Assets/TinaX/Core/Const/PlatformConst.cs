namespace TinaX.Const
{
    /// <summary>
    /// TinaX平台定义
    /// </summary>
    public static class PlatformConst
    {
        public enum E_Platform
        {
            /// <summary>
            /// Microsoft Windows x64
            /// </summary>
            Windows64,
            /// <summary>
            /// GNU Linux
            /// </summary>
            Linux64,
            /// <summary>
            /// Apple OSX
            /// </summary>
            OSX,
            /// <summary>
            /// Google Android
            /// </summary>
            Android,
            /// <summary>
            /// Apple iOS
            /// </summary>
            iOS,
            /// <summary>
            /// Microsoft XBox
            /// </summary>
            XBox,
            /// <summary>
            /// Nintendo Switch
            /// </summary>
            NSwitch,
            /// <summary>
            /// WebGL
            /// </summary>
            WebGL,
            /// <summary>
            /// Microsoft UWP
            /// </summary>
            UWP
        }

        public static string GetPlatformName(UnityEngine.RuntimePlatform platform)
        {
            switch (platform)
            {
                case UnityEngine.RuntimePlatform.WindowsPlayer:
                    return E_Platform.Windows64.ToString();
                case UnityEngine.RuntimePlatform.LinuxPlayer:
                    return E_Platform.Linux64.ToString();
                case UnityEngine.RuntimePlatform.OSXPlayer:
                    return E_Platform.OSX.ToString();
                case UnityEngine.RuntimePlatform.Android:
                    return E_Platform.Android.ToString();
                case UnityEngine.RuntimePlatform.IPhonePlayer:
                    return E_Platform.iOS.ToString();
                case UnityEngine.RuntimePlatform.XboxOne:
                    return E_Platform.XBox.ToString();
                case UnityEngine.RuntimePlatform.Switch:
                    return E_Platform.NSwitch.ToString();
                case UnityEngine.RuntimePlatform.WebGLPlayer:
                    return E_Platform.WebGL.ToString();
                case UnityEngine.RuntimePlatform.WSAPlayerARM:
                    return E_Platform.UWP.ToString();
                case UnityEngine.RuntimePlatform.WSAPlayerX64:
                    return E_Platform.UWP.ToString();
                case UnityEngine.RuntimePlatform.WSAPlayerX86:
                    return E_Platform.UWP.ToString();

                //EDITORS
                case UnityEngine.RuntimePlatform.WindowsEditor:
                    return E_Platform.Windows64.ToString();
                case UnityEngine.RuntimePlatform.LinuxEditor:
                    return E_Platform.Linux64.ToString();
                case UnityEngine.RuntimePlatform.OSXEditor:
                    return E_Platform.OSX.ToString();


                default:
                    return E_Platform.Windows64.ToString();
            }
        }
    }


}