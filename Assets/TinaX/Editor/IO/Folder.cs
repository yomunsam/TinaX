using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using UnityEditor;

namespace TinaXEditor
{
    public class Folder
    {

        

        public static void CreateFolder(string path)
        {
            System.IO.Directory.CreateDirectory(path);
        }
    }
}

