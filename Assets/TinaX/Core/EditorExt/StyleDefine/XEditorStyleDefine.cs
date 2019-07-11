/*
 * 该文档是对编辑器风格的定义，如编辑器下的文本颜色 等
 * 之所以放在TinaX.Core下而非Editor下，是因为我们需要在Console输出等地方使用到
 * 
 * 
 */

using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
 #endif

namespace TinaX.Core
{
    public static class XEditorStyleDefine
    {
        /// <summary>
        /// 通常字体颜色
        /// </summary>
        public static Color Color_Text_Normal
        {
            get
            {
#if UNITY_EDITOR
                if (EditorGUIUtility.isProSkin)
                {
                    //黑皮肤
                    return Color.white;
                }
                else
                {
                    //白皮肤
                    return Color.black;
                }

#else
                return Color.black;
#endif
            }
        }

        public static Color Color_Blue
        {
            get
            {
#if UNITY_EDITOR
                if (EditorGUIUtility.isProSkin)
                {
                    //黑皮肤
                    return new Color(71f / 255f, 180f / 255f, 1, 1) ;
                }
                else
                {
                    //白皮肤
                    return Color.blue;
                }

#else
                return Color.blue;
#endif
            }
        }


    }
}
