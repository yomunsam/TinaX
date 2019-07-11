using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TinaX
{
    public static class XLog
    {
        /// <summary>
        /// 输出
        /// </summary>
        /// <param name="msg"></param>
        public static void Print(object msg)
        {
            Debug.Log(msg);
        }

        /// <summary>
        /// 输出
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="go"></param>
        public static void Print(object msg,GameObject go)
        {
            Debug.Log(msg,go);
        }

        /// <summary>
        /// 输出警告
        /// </summary>
        /// <param name="msg"></param>
        public static void PrintW(object msg)
        {
            Debug.LogWarning(msg);
        }

        /// <summary>
        /// 输出警告
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="go"></param>
        public static void PrintW(object msg,GameObject go)
        {
            Debug.LogWarning(msg,go);
        }

        /// <summary>
        /// 输出错误
        /// </summary>
        /// <param name="msg"></param>
        public static void PrintE(object msg)
        {
            Debug.LogError(msg);
        }


        /// <summary>
        /// 输出错误
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="go"></param>
        public static void PrintE(object msg,GameObject go)
        {
            Debug.LogError(msg,go);
        }


        /// <summary>
        /// 获取红色富文本
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string GetColorString_Red(string content)
        {
#if UNITY_EDITOR
            //编辑器下，根据Unity Editor皮肤选择
            if (UnityEditor.EditorGUIUtility.isProSkin)
            {
                //黑皮肤
                return "<color=red>" + content + "</color>";
            }
            else
            {
                //白皮肤
                return "<color=red>" + content + "</color>";
            }
            
#else
            return "<color=red>" + content + "</color>"; //他们说语句中多个加号拼接字符串这种情况，C#的编译器会自动优化，那我就先不管了
#endif
        }


        /// <summary>
        /// 获取蓝色富文本
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string GetColorString_Blue(string content)
        {
#if UNITY_EDITOR
            //编辑器下，根据Unity Editor皮肤选择
            if (UnityEditor.EditorGUIUtility.isProSkin)
            {
                //黑皮肤
                return "<color=#00BFFF>" + content + "</color>";
            }
            else
            {
                //白皮肤
                return "<color=blue>" + content + "</color>";
            }

#else
            return "<color=blue>" + content + "</color>"; //他们说语句中多个加号拼接字符串这种情况，C#的编译器会自动优化，那我就先不管了
#endif
        }
    }
}

