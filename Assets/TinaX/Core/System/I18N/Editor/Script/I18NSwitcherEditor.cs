using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityToolbarExtender;

namespace TinaXEditor.I18NKit
{
    [InitializeOnLoad]
    public class I18NSwitcherEditor
    {
        static string[] mRegionNames;

        static string[] mToolBar_RegionLists;
        static int mToolBar_Select;

        static int HandledIndex;

        static I18NSwitcherEditor()
        {
            mRegionNames = I18NIOEditor.GetRegionNames();
            if(mRegionNames != null)
            {
                List<string> tb_region_name = new List<string>();
                tb_region_name.Add("None");
                tb_region_name.AddRange(mRegionNames);
                mToolBar_RegionLists = tb_region_name.ToArray();

            }
            else
            {
                mToolBar_RegionLists = null;
            }

            ToolbarExtender.RightToolbarGUI.Add(OnToolBarGUI);

        }

        public static string GetCurSelectRegionName()
        {
            if(mToolBar_RegionLists != null)
            {
                if(mToolBar_RegionLists.Length > mToolBar_Select)
                {
                    return mToolBar_RegionLists[mToolBar_Select];
                }
            }

            return null;
        }

        static void OnToolBarGUI()
        {
            if (mToolBar_RegionLists != null)
            {
                GUILayout.FlexibleSpace();

                GUILayout.Label("I18N：");
                mToolBar_Select = GUILayout.Toolbar(mToolBar_Select, mToolBar_RegionLists);

                if( HandledIndex != mToolBar_Select )
                {
                    if (Application.isPlaying && mToolBar_Select != 0)
                    {
                        TinaX.XI18N.I.UseRegion(GetCurSelectRegionName());
                    }
                    HandledIndex = mToolBar_Select;
                }
            }

        }
    }
}


