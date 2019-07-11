#if TinaX_CA_LuaRuntime_Enable
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TinaX;

namespace TinaXEditor
{
    public class ChangeLuaFileExtName : EditorWindow
    {
        private static ChangeLuaFileExtName m_curWindow;

        private static string mCurPath; //当前路径（Unity）
        private static string mCurPathSys; //当前路径(系统绝对路径
        private static bool mIsDir; //是否为目录
        


        [MenuItem("Assets/TinaX/Lua/批量修改文件后缀名")]
        static void OpenWindow()
        {
            if (m_curWindow != null)
            {
                return;
            }
            //检查选中物品
            var path = AssetDatabase.GetAssetPath(Selection.activeInstanceID);
            if (path.IsNullOrEmpty())
            {
                XLog.PrintW("选中的路径为空");
                return;
            }
            //路径转为系统路径，判断是文件还是文件夹
            
            var path_sys = System.IO.Path.GetFullPath(path);
            mCurPath = path;
            mCurPathSys = path_sys;
            if (System.IO.Directory.Exists(path_sys))
            {
                mIsDir = true;
            }
            else
            {
                mIsDir = false;
            }

            m_curWindow = EditorWindow.GetWindow<ChangeLuaFileExtName>();
        }

        public ChangeLuaFileExtName()
        {
            this.titleContent = new GUIContent("修改Lua后缀");
        }


        private void OnGUI()
        {
            if (mIsDir)
            {
                Draw_Files();
            }
            else
            {
                Draw_SingleFile();
            }
        }

        /// <summary>
        /// 针对单个文件
        /// </summary>
        private void Draw_SingleFile()
        {
            GUILayout.Label("暂不支持针对单个文件的修改（就改一个文件你要工具干嘛）\n请手动修改，或者选择一个文件夹然后大概此工具");
        }

        private Vector2 files_Scroll;
        private TinaX.Lua.LuaFileExten mFiles_FromExt; //从 后缀名
        private TinaX.Lua.LuaFileExten mFiles_ToExt;    //到后缀名
        private List<string> mFiles_ChangeList = new List<string>(); //所有符合改名条件的文件列在这里

        private TinaX.Lua.LuaFileExten mFiles_CurRefreshExt; //当前刷新出来的列表是这个后缀

        /// <summary>
        /// 
        /// </summary>
        private void Draw_Files()
        {
            GUILayout.BeginVertical();

            GUILayout.Label("批量更改Lua文件后缀名 - 规则：");
            GUILayout.BeginHorizontal();
            GUILayout.Label("从后缀名：");
            mFiles_FromExt = (TinaX.Lua.LuaFileExten)EditorGUILayout.EnumPopup(mFiles_FromExt);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("修改为：");
            mFiles_ToExt = (TinaX.Lua.LuaFileExten)EditorGUILayout.EnumPopup(mFiles_ToExt);
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            GUILayout.Label("从：example" + Enum2Str(mFiles_FromExt) + " 修改为：example" + Enum2Str(mFiles_ToExt));

            GUILayout.Space(20);

            if (GUILayout.Button("刷新列表"))
            {
                if (mFiles_FromExt == mFiles_ToExt)
                {
                    XLog.PrintW("我替换我自己？");
                    return;
                }
                var fromExt = Enum2Str(mFiles_FromExt);
                var files_guid = AssetDatabase.FindAssets("", new string[] { mCurPath });
                foreach(var item in files_guid)
                {
                    var fileName = AssetDatabase.GUIDToAssetPath(item);
                    if (mFiles_FromExt== TinaX.Lua.LuaFileExten.txt)
                    {
                        //得分辨，仅仅获取.txt，不包含".lua.txt"
                        if(fileName.ToLower().EndsWith(".txt") && !fileName.ToLower().EndsWith(".lua.txt"))
                        {
                            mFiles_ChangeList.Add(fileName);
                        }
                    }
                    else if(mFiles_FromExt == TinaX.Lua.LuaFileExten.lua_txt)
                    {
                        if (fileName.ToLower().EndsWith(".lua.txt"))
                        {
                            mFiles_ChangeList.Add(fileName);
                        }
                    }else if(mFiles_FromExt == TinaX.Lua.LuaFileExten.lua)
                    {
                        if (fileName.ToLower().EndsWith(".lua"))
                        {
                            mFiles_ChangeList.Add(fileName);
                        }
                    }
                    
                }

                mFiles_CurRefreshExt = mFiles_FromExt;
            }

            files_Scroll = GUILayout.BeginScrollView(files_Scroll);

            if (mFiles_ChangeList .Count <= 0)
            {
                GUILayout.Label("没有符合条件的文件，请尝试刷新");
            }
            else
            {
                foreach(var item in mFiles_ChangeList)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(item);
                    if (GUILayout.Button("[移除]",GUILayout.MaxWidth(80)))
                    {
                        mFiles_ChangeList.Remove(item);
                        break;
                    }
                    GUILayout.EndHorizontal();
                }
            }

            GUILayout.EndScrollView();

            if (mFiles_ChangeList.Count > 0)
            {
                if (GUILayout.Button("开始替换"))
                {
                    if(mFiles_CurRefreshExt != mFiles_FromExt)
                    {
                        EditorUtility.DisplayDialog("错误", "需要再刷新一次列表","好");
                        return;
                    }

                    //开始替换
                    foreach (var item in mFiles_ChangeList)
                    {
                        Debug.Log("改名：" + item + "  -> " + System.IO.Path.GetFileName(item).Replace(Enum2Str(mFiles_FromExt), Enum2Str(mFiles_ToExt)));
                        AssetDatabase.RenameAsset(item, System.IO.Path.GetFileName(item).Replace(Enum2Str(mFiles_FromExt), Enum2Str(mFiles_ToExt)));
                    }
                }
            }

            GUILayout.EndVertical();
        }

        private string Enum2Str(TinaX.Lua.LuaFileExten emm) //这儿返回的就是最终后缀名，带小点点
        {
            switch (emm)
            {
                case TinaX.Lua.LuaFileExten.lua:
                    return ".lua";
                case TinaX.Lua.LuaFileExten.lua_txt:
                    return ".lua.txt";
                case TinaX.Lua.LuaFileExten.txt:
                    return ".txt";
                default:
                    return ".unknow";
            }
        }


        private void OnDestroy()
        {
            m_curWindow = null;   
        }
    }
}



#endif