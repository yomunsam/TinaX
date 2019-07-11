using System;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;

namespace TinaX.IO
{
    public class XFolder
    {
        /// <summary>
        /// 将文件夹压缩ZIP
        /// </summary>
        public static void ZipFolder(string folder,string zipName)
        {
            var fz = new FastZip();
            fz.CreateZip(zipName, folder, true, "");
        }
    }
}
