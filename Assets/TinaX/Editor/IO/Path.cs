using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TinaXEditor
{
    public class XPath
    {
        /// <summary>
        /// Unity路径到系统绝对路径
        /// </summary>
        public static string UnityToOS(string path)
        {
            var full_path = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), path);
#if UNITY_EDITOR_WIN
            full_path = full_path.Replace("/", "\\");
#endif
            return full_path;
        }



    }
}
