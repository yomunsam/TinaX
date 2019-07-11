using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TinaX;

namespace TinaXEditor.DevOps
{
    /// <summary>
    /// 用来读写Unity中各个平台的#Define 宏定义
    /// </summary>
    public static class SharpDefineReadWriteUtils
    {
        public static string[] GetSharpDefineStrArr(BuildTargetGroup _BuildTarget)
        {
            var str = PlayerSettings.GetScriptingDefineSymbolsForGroup(_BuildTarget);
            if (str.IsNullOrEmpty())
            {
                return new string[0];
            }

            return str.Split(';'); ;
        }

        public static void AddDefineIfNotExist(BuildTargetGroup _BuildTarget,string DefineStr)
        {
            //判断是否存在
            var flag = false;
            var str = PlayerSettings.GetScriptingDefineSymbolsForGroup(_BuildTarget);
            if (!str.IsNullOrEmpty())
            {
                var str_arr = str.Split(';');
                foreach(var item in str_arr)
                {
                    if(item == DefineStr)
                    {
                        flag = true;
                        break;
                    }
                }
            }

            if (!flag)
            {
                //添加
                str += (";" + DefineStr);
                PlayerSettings.SetScriptingDefineSymbolsForGroup(_BuildTarget, str);
            }
        }

        /// <summary>
        /// 直接覆盖Unity定义
        /// </summary>
        /// <param name="_BuildTarget"></param>
        /// <param name="DefineStr">用分号处理好的字符串</param>
        public static void CoverDefine(BuildTargetGroup _BuildTarget, string DefineStr)
        {
            if (!DefineStr.IsNullOrEmpty())
            {
                PlayerSettings.SetScriptingDefineSymbolsForGroup(_BuildTarget, DefineStr);
            }

        }
    }

}
