using UnityEngine;
#if UNITY_EDITOR && ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace TinaX
{
    /// <summary>
    /// 框架基础设置
    /// </summary>
    public class MainConfig : ScriptableObject
    {
        /// <summary>
        /// 框架是否启用
        /// </summary>
#if UNITY_EDITOR && ODIN_INSPECTOR
        [BoxGroup("TinaX Config")]
        [Header("启用TinaX")]
#else
        [Header("[TinaX]")]
#endif
        [SerializeField]
        public bool TinaX_Enable = true;

        /// <summary>
        /// 框架初始启动Scene
        /// </summary>
#if UNITY_EDITOR && ODIN_INSPECTOR
        [Header("启动场景")]
        [BoxGroup("TinaX Config")]
        [InfoBox("框架启动后会自动载入此处配置的Scene")]

#else
        [Tooltip("如配置内容不为空，则框架启动后会自动载入此处配置的Scene")]
#endif
        public string Startup_Scene;


        /// <summary>
        /// 当前母包版本号
        /// </summary>
#if UNITY_EDITOR && ODIN_INSPECTOR
        [Header("当前母包版本号")]
        [BoxGroup("TinaX Config")]
#else
        
#endif
        public int Version_Code = 0;


        /// <summary>
        /// 框架与网络等系统的唯一识别名，非显示名
        /// </summary>
#if UNITY_EDITOR && ODIN_INSPECTOR
        [Header("应用ID")]
        [BoxGroup("TinaX Config")]
        [InfoBox("框架与网络等系统的唯一识别名，非显示名")]
#else
        [Tooltip("框架与网络等系统的唯一识别名，非显示名")]
#endif
        public string App_Name ="";




    }

}
