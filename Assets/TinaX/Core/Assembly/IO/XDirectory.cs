using System;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;

namespace TinaX.IO
{
    public class XDirectory
    {
        /// <summary>
        /// 将文件夹压缩ZIP
        /// </summary>
        public static void ZipDirectory(string folder,string zipName)
        {
            var fz = new FastZip();
            fz.CreateZip(zipName, folder, true, "");
        }

        /// <summary>
        /// If Director is not exists , create it. | 如果目录不存在，则创建
        /// </summary>
        /// <param name="path"></param>
        public static void CreateIfNotExists(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

    }
}
