using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using TinaX;


namespace TinaXEditor.Assets
{
    /// <summary>
    /// 虚拟文件系统 已打包的文件夹管理
    /// </summary>
    public class VFSFolderMgrWindow : EditorWindow
    {
        [MenuItem("TinaX/发布流程/VFS 目录管理",false,101)]
        public static void Open_VFSWindow()
        {
            EditorWindow.GetWindow<VFSFolderMgrWindow>();
        }

        public VFSFolderMgrWindow()
        {
            this.titleContent = new GUIContent("VFS目录管理");
            this.maxSize = new Vector2(540, 600);
            this.minSize = new Vector2(540, 300);
            m_cur_dir_path = System.IO.Directory.GetCurrentDirectory();
            m_VSF_Root_path = System.IO.Path.Combine(m_cur_dir_path, TinaX.Setup.Framework_AssetSystem_Pack_Path);
            int i_1 = m_VSF_Root_path.IndexOf("/");
            int i_2 = m_VSF_Root_path.IndexOf(@"\");
            if (i_2 > -1 && i_1 > -1)
            {
                if(i_2 < i_1)
                {
                    m_VSF_Root_path = m_VSF_Root_path.Replace("/", @"\");
                }
                else
                {
                    m_VSF_Root_path = m_VSF_Root_path.Replace(@"\","/");
                }
            }
            

            m_VSF_Stram_path = System.IO.Path.Combine(new string[] {
                m_cur_dir_path,
                "Assets/StreamingAssets/vfs/"
            });

        }
        string m_cur_dir_path;
        string m_VSF_Root_path;
        string m_VSF_Stram_path;
        string m_cur_selection_path;
        /*
         * 布局
         * 左边栏显示所有VFS目录的列表,宽度120
         *  
         *  右边栏显示工程StreamingAssets中所有目录的列表
         */

        private void OnGUI()
        {
            GUILayout.BeginHorizontal();

            //左边栏
            #region 左边栏
            GUILayout.BeginVertical(GUILayout.Width(120));
            GUILayout.Space(10);
            GUILayout.Label("VFS 打包存储目录：");
            GUILayout.Space(10);


            if (Directory.Exists(m_VSF_Root_path))
            {
                var folders = System.IO.Directory.GetDirectories(System.IO.Path.GetFullPath(m_VSF_Root_path));
                foreach (var folder in folders)
                {
                    if (m_cur_selection_path == folder)
                    {
                        GUILayout.Label("[◎]" + Path.GetFileName(folder), GUILayout.Width(119));
                    }
                    else
                    {
                        if (GUILayout.Button(Path.GetFileName(folder), GUILayout.Width(119)))
                        {
                            m_cur_selection_path = folder;
                        }
                    }

                }
            }
            else
            {
                GUILayout.Label("目录为空");
            }
            

            GUILayout.EndVertical();

            #endregion
            //中间
            #region 中间栏
            GUILayout.BeginVertical(GUILayout.Width(260));

            if (m_cur_selection_path.IsNullOrEmpty())
            {
                GUILayout.Label("当前选中：none");
            }
            else
            {
                GUILayout.Label("当前选中："+ Path.GetFileName(m_cur_selection_path));
                //GUILayout.Label("m_cur_selection_path:" + m_cur_selection_path);
                //GUILayout.Label("m_VSF_Root_path:" + m_VSF_Root_path);

                if (m_cur_selection_path.StartsWith(m_VSF_Root_path))
                {
                    GUILayout.Label("选中：VFS 打包存储");
                    GUILayout.Label("最后修改时间："+ Directory.GetLastWriteTime(m_cur_selection_path));

                    if (GUILayout.Button("移动到StreamingAssets"))
                    {
                        var target_folder_path = Path.Combine(m_VSF_Stram_path, Path.GetFileName(m_cur_selection_path));
                        //判断一下对面有没有文件
                        if (Directory.Exists(target_folder_path))
                        {
                            if(UnityEditor.EditorUtility.DisplayDialog("已存在","目标路径已存在，是否覆盖：\n"+ target_folder_path, "覆盖", "取消"))
                            {
                                //覆盖对面
                                Directory.Delete(target_folder_path);
                            }
                        }
                        Directory.CreateDirectory(Directory.GetParent(target_folder_path).ToString());
                        EditorUtility.DisplayProgressBar("正在复制","稍等",0.4f);
                        Folder.CopyDir(m_cur_selection_path, target_folder_path);
                        EditorUtility.ClearProgressBar();
                    }
                }
                else
                {
                    GUILayout.Label("选中：VFS StreamingAssets");
                    GUILayout.Label("最后修改时间：" + Directory.GetLastWriteTime(m_cur_selection_path));
                    if (GUILayout.Button("删除"))
                    {
                        if (Directory.Exists(m_cur_selection_path))
                        {
                            Directory.Delete(m_cur_selection_path,true);
                            m_cur_selection_path = "";
                        }
                    }
                }
            }


            GUILayout.EndVertical();
            #endregion



            //右边
            #region 右边
            GUILayout.BeginVertical(GUILayout.Width(120));
            GUILayout.Space(10);
            GUILayout.Label("VFS StreamingAssets：");
            GUILayout.Space(10);


            if (Directory.Exists(m_VSF_Stram_path))
            {
                var folders_Stream = System.IO.Directory.GetDirectories(System.IO.Path.GetFullPath(m_VSF_Stram_path));

                foreach (var folder in folders_Stream)
                {
                    if (m_cur_selection_path == folder)
                    {
                        GUILayout.Label("[◎]" + Path.GetFileName(folder), GUILayout.Width(119));
                    }
                    else
                    {
                        if (GUILayout.Button(Path.GetFileName(folder), GUILayout.Width(119)))
                        {
                            m_cur_selection_path = folder;
                        }
                    }
                }

            }
            else
            {
                GUILayout.Label("目录为空");
            }

            GUILayout.EndVertical();
            #endregion

            GUILayout.EndHorizontal();
        }
    }
}