#if ODIN_INSPECTOR

using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;

namespace TinaXEditor
{
    public class TinaXConfWindow : OdinMenuEditorWindow
    {
        [MenuItem("TinaX/配置/项目设置",false,1)]
        public static void Open_ConfigWindow()
        {
            var window = GetWindow<TinaXConfWindow>();

        }
        

        TinaXConfWindow()
        {
            this.titleContent = new GUIContent("TinaX项目配置");
            this.minSize = new Vector2(750, 550);
            
        }


        protected override OdinMenuTree BuildMenuTree()
        {
            var tree = new OdinMenuTree(true);
            tree.DefaultMenuStyle.IconSize = 28.00f;
            tree.Config.DrawSearchToolbar = true;

            foreach (var item in TinaX.Conf.ConfigRegister.ConfigRegisters)
            {
                var obj = item.Action_Create();
                tree.Add(item.Title, obj);
            }
            

            return tree;
        }


    }
}



#endif