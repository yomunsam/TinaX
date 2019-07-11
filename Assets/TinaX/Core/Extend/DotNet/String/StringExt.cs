using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using CatLib;

namespace TinaX
{
    public static class StringExt
    {
        /// <summary>
        /// 是否为邮箱地址
        /// </summary>
        /// <param name="_string"></param>
        /// <returns></returns>
        public static bool IsMail(this string _string)
        {
            return Regex.IsMatch(_string,
                @"^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$");
        }

        /// <summary>
        /// 高级比较，可设定是否忽略大小写
        /// </summary>
        /// <param name="source"></param>
        /// <param name="toCheck"></param>
        /// <param name="comp"></param>
        /// <returns></returns>
        public static bool Contains(this string source, string toCheck, StringComparison comp)
        {
            return source.IndexOf(toCheck, comp) >= 0;
        }

        /// <summary>
        /// 是否含有中文
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IncludeChinese(this string str)
        {
            bool flag = false;
            foreach (var a in str)
            {
                if (a >= 0x4e00 && a <= 0x9fbb)
                {
                    flag = true;
                    break;
                }
            }
            return flag;
        }

        /// <summary>
        /// 反转字符串
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string Reverse(this string str)
        {
            return Str.Reverse(str);
        }

        /// <summary>
        /// 是否为空字符串
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(this string str)
        {
            return string.IsNullOrEmpty(str);
        }

        /// <summary>
        /// string Base64加密
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ToBase64(this string str)
        {
            var b = System.Text.Encoding.Default.GetBytes(str);
            return Convert.ToBase64String(b);
        }

        public static string Base64ToStr(this string str)
        {
            var b = Convert.FromBase64String(str);
            return System.Text.Encoding.Default.GetString(b);
        }

    }
}
