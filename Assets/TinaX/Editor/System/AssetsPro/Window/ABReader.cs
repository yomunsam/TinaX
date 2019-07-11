using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace TinaXEditor.AssetsPro
{
    /// <summary>
    /// AB包阅读器
    /// </summary>
    public class ABReader : EditorWindow
    {
        private const string m_MenuName = "Assets/TinaX/高级资源系统/浏览AssetBundle包";
        
        /// <summary>
        /// 当前处理的assetbundle
        /// </summary>
        private static string cur_ab_path;  //暂存
        private static ABReader cur_window_obj;

        [MenuItem(m_MenuName, true)]
        static bool MenuEnable()
        {
            if (Selection.activeObject == null)
            {
                return false;
            }
            var path = AssetDatabase.GetAssetPath(Selection.activeInstanceID);
            if(System.IO.Path.GetExtension(path).ToLower() != ".xab")
            {
                return false;
            }

            return true;
        }

        [MenuItem(m_MenuName)]
        static void Menu_ReadAB()
        {
            var path = AssetDatabase.GetAssetPath(Selection.activeInstanceID);
            ReadAB(path);
        }

        public static void ReadAB(string path)
        {
            cur_ab_path = path;
            cur_window_obj = EditorWindow.GetWindow<ABReader>(true);
            cur_window_obj.minSize = new Vector2(320, 500);
        }


        private string mCur_AB_Path;
        private string mCur_AB_FileName;
        private AssetBundle mCUr_AB;
        private string mCUr_Select_fileName = "";    //ab包中选中的文件名
        private Vector2 mCUr_Left_ScrollView_V2;
        private Vector2 mCUr_Right_ScrollView_V2;
        private bool mPreview_file = false;  //是否预览当前选中文件

        public ABReader()
        {
            mCur_AB_Path = cur_ab_path;
            mCur_AB_FileName = System.IO.Path.GetFileNameWithoutExtension(mCur_AB_Path);
            this.titleContent = new GUIContent("AssetBundle浏览器");
            
        }

        private void OnEnable()
        {
            //重复读取检测
            var abs = AssetBundle.GetAllLoadedAssetBundles();
            var ab_name = mCur_AB_Path;
            var ab_index = ab_name.IndexOf("assets/");
            ab_name = ab_name.Substring(ab_index, ab_name.Length - ab_index);
            //Debug.Log(ab_name);
            foreach (var item in abs)
            {
                if(item.name == ab_name.ToLower())
                {
                    if (Application.isPlaying)
                    {
                        return;
                    }
                    else
                    {
                        item.Unload(true);
                    }
                }
            }
            
            //读出ab包
            mCUr_AB = AssetBundle.LoadFromFile(mCur_AB_Path);
            
        }

        private void OnDestroy()
        {
            if(mCUr_AB != null)
            {
                mCUr_AB.Unload(true);
            }
        }

        private void OnGUI()
        {
            if (mCUr_AB == null)
            {
                Draw_No_file();
                return;
            }
            var origin_lable_font_size = GUI.skin.label.fontSize;
            var origin_lable_alignment = GUI.skin.label.alignment;

            

            EditorGUILayout.BeginHorizontal();

            #region 左边栏
            EditorGUILayout.BeginVertical(GUILayout.MinWidth(120),GUILayout.MaxWidth(220));
            //标题
            GUILayout.Space(10);
            GUI.skin.label.fontSize = 16;
            GUI.skin.label.alignment = TextAnchor.MiddleLeft;
            GUILayout.Label("资源包浏览器");
            GUI.skin.label.fontSize = 12;
            GUI.skin.label.alignment = TextAnchor.MiddleLeft;
            GUILayout.Label(mCur_AB_FileName);
            GUI.skin.label.fontSize = origin_lable_font_size;

            //绘制列表：这个包里面所有的文件
            GUILayout.Space(10);
            var file_name_in_ab = mCUr_AB.GetAllAssetNames();
            if (file_name_in_ab.Length > 0)
            {
                GUILayout.Label("Files in AB");
                GUILayout.Space(5);
                mCUr_Left_ScrollView_V2 = EditorGUILayout.BeginScrollView(mCUr_Left_ScrollView_V2,GUILayout.MaxWidth(220),GUILayout.MinWidth(120));
                for (int i = 0; i < file_name_in_ab.Length; i++)
                {
                    var name = file_name_in_ab[i];
                    if (mCUr_Select_fileName == name)
                    {
                        GUILayout.Label("【 "+ System.IO.Path.GetFileName(file_name_in_ab[i]) + " 】");
                    }
                    else
                    {
                        if (GUILayout.Button(System.IO.Path.GetFileName(file_name_in_ab[i])))
                        {
                            mPreview_file = false;
                            mCUr_Select_fileName = name;
                        }
                    }
                    
                }
                EditorGUILayout.EndScrollView();
            }
            else
            {
                mPreview_file = false;
                GUILayout.Label("该资源包中没有文件");
            }

            

            EditorGUILayout.EndVertical();
            #endregion

            #region 检视窗口
            EditorGUILayout.BeginVertical(GUILayout.MinWidth(120),GUILayout.MaxWidth(500));
            //当前选中项信息
            if(mCUr_Select_fileName != "")
            {
                GUILayout.Label("文件名：" + mCUr_Select_fileName);

                GUILayout.Space(20);
                if (!mPreview_file)
                {
                    if (GUILayout.Button("预览该文件"))
                    {
                        mPreview_file = true;
                    }
                }
            }
            else
            {
                GUILayout.Label("无选择文件");
            }

            if (mPreview_file)
            {
                Draw_Preview();
            }


            EditorGUILayout.EndVertical();


            #endregion


            EditorGUILayout.EndHorizontal();


            GUI.skin.label.fontSize = origin_lable_font_size;
            GUI.skin.label.alignment = origin_lable_alignment;
        }



        private void Draw_No_file()
        {
            GUILayout.Label("当前没有资源");
        }


        #region 绘制右侧检视面板

        private UnityEngine.Object loaded_Main_file;
        //private UnityEngine.Object[] loaded_files;
        private string mCur_Preview_fileName; //当前载入成功的预览文件名
        
        private void Draw_Preview()
        {
            if(mCur_Preview_fileName != mCUr_Select_fileName)
            {
                //载入文件
                loaded_Main_file = mCUr_AB.LoadAsset(mCUr_Select_fileName);
                mCur_Preview_fileName = mCUr_Select_fileName;
            }
            else
            {
                //根据文件后缀尝试预览
                var ext = System.IO.Path.GetExtension(mCur_Preview_fileName);
                if(ext == ".txt" || ext == ".json" || ext == ".html" || ext == ".xml" || ext == ".lua")
                {
                    Draw_Text();
                }else if (ext == ".jpg" || ext == ".png" || ext == ".tga")
                {
                    Draw_Image();
                }
                else
                {
                    GUILayout.Label("暂不支持预览该类型文件");
                }
            }
        }

        private void Draw_Text()
        {
            mCUr_Right_ScrollView_V2 = EditorGUILayout.BeginScrollView(mCUr_Right_ScrollView_V2);
            var txt_file = (TextAsset)loaded_Main_file;

            GUILayout.TextArea(txt_file.text);

            EditorGUILayout.EndScrollView();
        }

        private void Draw_Image()
        {
            mCUr_Right_ScrollView_V2 = EditorGUILayout.BeginScrollView(mCUr_Right_ScrollView_V2);
            var img_file = (Texture)loaded_Main_file;

            GUILayout.Label(img_file,GUILayout.MaxWidth(490));

            EditorGUILayout.EndScrollView();
        }

        #endregion



    }
}
