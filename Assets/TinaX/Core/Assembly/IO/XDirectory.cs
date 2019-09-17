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

        public static void CopyDir(string fromDir, string toDir)
        {
            if (!Directory.Exists(fromDir))
                return;

            if (!Directory.Exists(toDir))
            {
                Directory.CreateDirectory(toDir);
            }

            string[] files = Directory.GetFiles(fromDir);
            foreach (string formFileName in files)
            {
                string fileName = Path.GetFileName(formFileName);
                string toFileName = Path.Combine(toDir, fileName);
                File.Copy(formFileName, toFileName);
            }
            string[] fromDirs = Directory.GetDirectories(fromDir);
            foreach (string fromDirName in fromDirs)
            {
                string dirName = Path.GetFileName(fromDirName);
                string toDirName = Path.Combine(toDir, dirName);
                CopyDir(fromDirName, toDirName);
            }
        }

    }
}
