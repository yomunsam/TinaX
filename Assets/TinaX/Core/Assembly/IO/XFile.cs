using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using ICSharpCode.SharpZipLib.Zip;

namespace TinaX.IO
{
    public static class XFile
    {
        public static string GetMD5(string fileName)
        {
            try
            {
                FileStream file = new FileStream(fileName, System.IO.FileMode.Open);
                MD5 md5 = new MD5CryptoServiceProvider();
                byte[] retVal = md5.ComputeHash(file);
                file.Close();
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < retVal.Length; i++)
                {
                    sb.Append(retVal[i].ToString("x2"));
                }
                return sb.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("GetMD5() fail,error:" + ex.Message);
            }
        }

        /// <summary>
        /// 解压缩zip
        /// </summary>
        /// <returns></returns>
        public static void Unzip(string filePath,string targetDir)
        {
            var fz = new FastZip();
            fz.ExtractZip(filePath, targetDir, FastZip.Overwrite.Always,(filename)=> { return true; },"","",true);
        }


    }
}