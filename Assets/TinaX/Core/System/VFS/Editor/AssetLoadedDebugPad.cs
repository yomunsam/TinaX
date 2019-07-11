using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TinaXEditor.VFS
{
    public class AssetLoadedDebugPad : EditorWindow
    {
        [MenuItem("TinaX/编辑器/资源系统/AB资源加载检测")]
        public static void OpenWindow()
        {
            GetWindow<AssetLoadedDebugPad>().Show();
        }

        public AssetLoadedDebugPad()
        {
            this.titleContent = new GUIContent("已加载AB包查看");
        }


        private bool mInited = false;
        private TinaX.VFS.LoadedAssetBundle[] loaded_abList;

        private Vector2 scrollV2;
        private string cur_select_ab_path_name;

        private void OnGUI()
        {
            if (!Application.isPlaying)
            {
                GUILayout.Label("该功能只在运行时有效");
                mInited = false;
                return;
            }
            if (!Menu.GetChecked(TinaX.Const.AssetSystemConst.menu_editor_load_from_asset_pack_name))
            {
                GUILayout.Label("该功能仅在AB包加载模式下有效");
                mInited = false;
                return;
            }

            if (!mInited)
            {
                GUILayout.Label("初始化");
                Init();
                mInited = true;
                return;
            }

            if (GUILayout.Button("刷新列表"))
            {
                Refresh_List();
            }

            if (GUILayout.Button("GC"))
            {
                TinaX.AssetsMgr.I.GC();
            }


            if(loaded_abList.Length <= 0)
            {
                GUILayout.Label("列表是空空的");
            }
            else
            {
                scrollV2 =  EditorGUILayout.BeginScrollView(scrollV2);

                foreach(var item in loaded_abList)
                {
                    //if (item.ab_path == cur_select_ab_path_name)
                    //{
                    //    //显示详情
                    //    GUILayout.Label(" --> " + item.ab_path);
                    //    GUILayout.Label("引用数量：" + item.GetUsedNum());
                    //}
                    //else
                    //{
                    //    if (GUILayout.Button(item.ab_path))
                    //    {
                    //        cur_select_ab_path_name = item.ab_path;
                    //    }
                    //}

                    GUILayout.Label("[" + item.GetUsedNum() + "] " + item.ab_path);
                }
                

                EditorGUILayout.EndScrollView();
            }



        }

        private void Init()
        {
            Refresh_List();
        }

        private void Refresh_List()
        {
            loaded_abList = TinaX.AssetsMgr.I.Debug_GetABLoadedInfo();
        }

    }
}

