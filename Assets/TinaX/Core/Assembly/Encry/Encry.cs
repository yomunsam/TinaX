using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Security;
using System.Security.Cryptography;
using System;
using System.Text;


namespace TinaX
{
    /// <summary>
    /// 加解密类
    /// </summary>
    public static class Encry
    {

        /// <summary>
        /// 取MD5
        /// </summary>
        /// <param name="content">原文件</param>
        /// <returns>MD5文件</returns>
        public static string GetMD5(string content)
        {
            MD5 md5Hash = MD5.Create();
            var data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(content));
            StringBuilder str = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                str.Append(data[i].ToString("x2"));
            }
            return str.ToString();
            //var md5 = new MD5CryptoServiceProvider();
            //var data = System.Text.Encoding.Default.GetBytes(content);
            //var t_data = md5.ComputeHash(data);
            //return System.Text.Encoding.Default.GetString(t_data);
        }
    }
}

