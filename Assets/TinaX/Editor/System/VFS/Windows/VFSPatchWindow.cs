using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using TinaX;


namespace TinaXEditor.VFS
{
    public class VFSPatchWindow : EditorWindow
    {
        [MenuItem("TinaX/发布流程/VSF 更新包", false, 102)]
        public static void Open_AssetsPackageWindow()
        {
            EditorWindow.GetWindow<VFSPatchWindow>();
        }

        VFSPatchWindow()
        {
            mVFSPatch_Path = Path.Combine(Directory.GetCurrentDirectory(), TinaX.Setup.Framework_VFS_Patch_Path);
            if (!Directory.Exists(mVFSPatch_Path))
            {
                Directory.CreateDirectory(mVFSPatch_Path);
            }
            this.titleContent = new GUIContent("VFS 增量更新管理器");
        }

        //private string 
        public static string mVFSPatch_Path;

        private void OnGUI()
        {
            GUILayout.Space(10);
            //就列出个列表
            var folder = Directory.GetDirectories(mVFSPatch_Path);
            if (folder.Length <= 0)
            {
                GUILayout.Label("这儿啥都没有");
            }
            else
            {
                foreach(var item in folder)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(" - " + Path.GetFileName(item));
                    if (GUILayout.Button("打开",GUILayout.MaxWidth(80)))
                    {
                        VFSPatch_Edit_Window.edit_root_path = item;
                        GetWindow<VFSPatch_Edit_Window>().Show();
                    }
                    if (GUILayout.Button("删除", GUILayout.MaxWidth(60)))
                    {
                        if (EditorUtility.DisplayDialog("确认", "要删除这个母包记录吗？可能无法恢复哦！", "删除呀", "だめ!"))
                        {
                            Directory.Delete(item,true);
                        }
                    }
                    GUILayout.EndHorizontal();
                }
            }

            GUILayout.Space(20);
            if (GUILayout.Button("新建配置"))
            {
                GetWindow<VFSPatch_Create_Window>().Show();
            }
        }
    }

    /// <summary>
    /// 创建新配置用的窗口
    /// </summary>
    public class VFSPatch_Create_Window : EditorWindow
    {
        VFSPatch_Create_Window()
        {
            this.titleContent = new GUIContent("新建包更新配置");
            mVFS_Root_Path = Path.Combine(Directory.GetCurrentDirectory(), TinaX.Setup.Framework_AssetSystem_Pack_Path);
            if (!Directory.Exists(mVFS_Root_Path))
            {
                Directory.CreateDirectory(mVFS_Root_Path);
            }
        }

        private Vector2 mScroll_Platform;
        private string mVFS_Root_Path;

        private string mName;
        private string selectPlatform;
        private VFSPatchConfigModel.EPatchType mPatchType;


        private void OnEnable()
        {
            this.maxSize = new Vector2(300, 250);
            this.minSize = new Vector2(300, 350);
        }

        private void OnGUI()
        {
            GUILayout.Space(10);


            GUILayout.BeginHorizontal();
            GUILayout.Label("配置名：",GUILayout.MaxWidth(80));
            mName = GUILayout.TextField(mName);
            GUILayout.EndHorizontal();


            GUILayout.BeginHorizontal();
            GUILayout.Label("补丁类型：", GUILayout.MaxWidth(80));
            mPatchType = (VFSPatchConfigModel.EPatchType)EditorGUILayout.EnumPopup(mPatchType);
            GUILayout.EndHorizontal();
            if(mPatchType == VFSPatchConfigModel.EPatchType.Each)
            {
                GUILayout.Label("补丁类型：依次安装多个补丁");
            }else if(mPatchType == VFSPatchConfigModel.EPatchType.Newest)
            {
                GUILayout.Label("补丁类型：每次只安装单个基于母包最新的补丁");
            }


            GUILayout.Space(5);
            
            GUILayout.Label("基于平台：");
            //刷出平台列表
            var folder = Directory.GetDirectories(mVFS_Root_Path);
            if (folder.Length <= 0)
            {
                GUILayout.Label("没有找到已打包的资源\n先打包哦");
            }
            else
            {
                mScroll_Platform = EditorGUILayout.BeginScrollView(mScroll_Platform);
                foreach (var item in folder)
                {
                    var platform_name = Path.GetFileName(item);
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("  - " + platform_name);
                    if (selectPlatform == platform_name)
                    {
                        GUILayout.Label("- [已选中]");
                    }
                    else
                    {
                        if (GUILayout.Button("选中",GUILayout.MaxWidth(45)))
                        {
                            selectPlatform = platform_name;
                        }
                    }
                    
                    GUILayout.EndHorizontal();
                }

                EditorGUILayout.EndScrollView();
            }

            GUILayout.Space(15);
            if (mName.IsNullOrEmpty())
            {
                GUILayout.Label("请输入有效的配置名");
            }

            if (!mName.IsNullOrEmpty())
            {
                //名称是否重复
                if (Directory.Exists(Path.Combine(VFSPatchWindow.mVFSPatch_Path, mName)))
                {
                    GUILayout.Label("配置名已存在");
                }
                else
                {
                    if (selectPlatform.IsNullOrEmpty())
                    {
                        GUILayout.Label("请选择一个已打包的资源");
                    }
                    else
                    {
                        if (GUILayout.Button("创建"))
                        {
                            Directory.CreateDirectory(Path.Combine(VFSPatchWindow.mVFSPatch_Path, mName));
                            //写一个配置文件
                            var jsonObj = new VFSPatchConfigModel();
                            jsonObj.PlatformName = selectPlatform;
                            jsonObj.PatchType = mPatchType;
                            var jsonStr = JsonUtility.ToJson(jsonObj);
                            File.WriteAllText(Path.Combine(VFSPatchWindow.mVFSPatch_Path, mName, "config.json"),jsonStr);
                            this.Close();
                        }
                    }
                }
            }

            GUILayout.Space(10);
        }
    }

    public class VFSPatch_Edit_Window:EditorWindow
    {
        public static string edit_root_path;

        private string mEditPath;
        private VFSPatchConfigModel JsonObj;

        private string mPatchName;
        private bool mDeletable;

        private VFSPatchPackage mPatchMgr;

        VFSPatch_Edit_Window()
        {
            mEditPath = edit_root_path;
            this.titleContent = new GUIContent("增量更新:" + Path.GetFileName(mEditPath));
            //读配置文件
            var confPath = Path.Combine(edit_root_path, "config.json");
            if (File.Exists(confPath))
            {
                var jsonStr = File.ReadAllText(confPath);
                JsonObj = JsonUtility.FromJson<VFSPatchConfigModel>(jsonStr);
            }

        }

        private void OnEnable()
        {
            mPatchMgr = new VFSPatchPackage();
            mDeletable = true;
        }

        private void OnDestroy()
        {
            mPatchMgr = null;
        }

        private void OnGUI()
        {
            if(JsonObj == null)
            {
                GUILayout.Label("读取配置信息失败");
            }
            else
            {
                GUILayout.Space(10);

                GUILayout.Label("母包记录数：" + JsonObj.files.Length);

                GUILayout.Space(10);
                if (GUILayout.Button("更新母包记录"))
                {
                    if(JsonObj.files.Length > 0)
                    {
                        if (!EditorUtility.DisplayDialog("询问","当前已经有母包记录，继续操作会覆盖当前结果，要继续吗","继续","取消"))
                        {
                            return;
                        }
                    }
                    var read_root_path = Path.Combine(Directory.GetCurrentDirectory(), TinaX.Setup.Framework_AssetSystem_Pack_Path, JsonObj.PlatformName);
                    //尝试读配置中的路径
                    if (!Directory.Exists(read_root_path))
                    {
                        EditorUtility.DisplayDialog("错误", "读取配置中的VFS存储路径出错，路径不存在：\n" + read_root_path, "好");
                        return;
                    }

                    List<VFSPatchConfigModel.FileHash> files_info = new List<VFSPatchConfigModel.FileHash>();
                    var files = Directory.GetFiles(read_root_path,"*.*", SearchOption.AllDirectories);
                    EditorUtility.DisplayProgressBar("更新母包文件记录", "开始处理", 0);
                    for (int i = 0; i < files.Length; i++)
                    {
                        var file_path = files[i];
                        var file_path_xd = file_path.Replace(read_root_path, "");
                        file_path_xd = file_path_xd.Substring(1, file_path_xd.Length - 1);
                        EditorUtility.DisplayProgressBar("更新母包文件记录", "处理:" + file_path, (i / files.Length));
                        //Debug.Log("处理：" + file_path);
                        files_info.Add(new VFSPatchConfigModel.FileHash() {
                            FileName = file_path_xd,
                            MD5 = TinaX.IO.XFile.GetMD5(file_path)
                        });
                        
                    }
                    JsonObj.files = files_info.ToArray();
                    //写文件
                    if(File.Exists(Path.Combine(edit_root_path, "config.json")))
                    {
                        File.Delete(Path.Combine(edit_root_path, "config.json"));
                    }
                    File.WriteAllText(Path.Combine(edit_root_path, "config.json"), JsonUtility.ToJson(JsonObj));
                    EditorUtility.ClearProgressBar();
                }

                GUILayout.Space(10);
                GUILayout.Label("生成更新包:");
                GUILayout.BeginHorizontal();
                GUILayout.Label("更新包名称:");
                mPatchName = GUILayout.TextField(mPatchName);

                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("包含删除标记的补丁包:");
                mDeletable = EditorGUILayout.Toggle(mDeletable);
                GUILayout.EndHorizontal();


                if (!mPatchName.IsNullOrEmpty())
                {
                    if (GUILayout.Button("打更新包"))
                    {
                        var pack_dir = Path.Combine(mEditPath, "package");
                        var package_path = Path.Combine(pack_dir, mPatchName + TinaX.Setup.Framework_Patch_Ext_Name);
                        if (File.Exists(package_path))
                        {
                            EditorUtility.DisplayDialog("err", "文件名已存在", "好");
                            return;
                        }

                        mPatchMgr.SetBaseInfo(mEditPath)
                            .MakePatch(mPatchName, mDeletable);
                        

                        ////先把文件列成dict,方便处理
                        //Dictionary<string, VFSPatchConfigModel.FileHash> dict_origin_files = new Dictionary<string, VFSPatchConfigModel.FileHash>();
                        //for (int i = 0; i < JsonObj.files.Length; i++)
                        //{
                        //    EditorUtility.DisplayProgressBar("打更新包", "预处理", i / JsonObj.files.Length);
                        //    var path = Path.Combine(Directory.GetCurrentDirectory(), TinaX.Setup.Framework_AssetSystem_Pack_Path, JsonObj.PlatformName, JsonObj.files[i].FileName);
                        //    dict_origin_files.Add(path,  JsonObj.files[i]);
                        //}

                        ////遍历现在的文件
                        //var read_root_path = Path.Combine(Directory.GetCurrentDirectory(), TinaX.Setup.Framework_AssetSystem_Pack_Path, JsonObj.PlatformName);
                        //var files = Directory.GetFiles(read_root_path, "*.*", SearchOption.AllDirectories);
                        //EditorUtility.DisplayProgressBar("打更新包", "开始处理", 0);

                        ////所有变动和新增的文件存在这儿
                        //List<string> modify_path = new List<string>();
                        //for (int i = 0; i < files.Length; i++)
                        //{
                        //    var file_path = files[i];
                        //    EditorUtility.DisplayProgressBar("打更新包", "处理:" + file_path, (i / files.Length));
                        //    //处理
                        //    if (dict_origin_files.ContainsKey(file_path))
                        //    {
                        //        //已存在，对比md5
                        //        if(dict_origin_files[file_path].MD5 != TinaX.IO.XFile.GetMD5(file_path))
                        //        {
                        //            Debug.Log("<color=green>文件变动：" + file_path+"</color>");
                        //            modify_path.Add(file_path);
                        //        }
                        //        //else
                        //        //{
                        //        //    Debug.Log("无变动:" + file_path);
                        //        //}
                        //    }
                        //    else
                        //    {
                        //        //不存在
                        //        Debug.Log("<color=#FF83FA>文件增加：" + file_path + "</color>");
                        //        modify_path.Add(file_path);
                        //    }

                        //}

                        //EditorUtility.ClearProgressBar();
                        ////把所有文件复制
                        //var temp_dir = Path.Combine(mEditPath, "temp");
                        //if (Directory.Exists(temp_dir))
                        //{
                        //    Directory.Delete(temp_dir,true);
                        //}
                        //Directory.CreateDirectory(temp_dir);
                        //this.ShowNotification(new GUIContent("复制文件"));
                        //foreach(var file in modify_path)
                        //{
                        //    var file_path_xd = file.Replace(read_root_path, "");
                        //    file_path_xd = file_path_xd.Substring(1, file_path_xd.Length - 1);

                        //    var target_path = Path.Combine(temp_dir, file_path_xd);
                        //    Directory.CreateDirectory(Path.GetDirectoryName(target_path));
                        //    File.Copy(file, Path.Combine(temp_dir, file_path_xd));
                        //}
                        //this.RemoveNotification();


                        //if (!Directory.Exists(pack_dir))
                        //{
                        //    Directory.CreateDirectory(pack_dir);
                        //}

                        //TinaX.IO.XFolder.ZipFolder(temp_dir, package_path);
                        //var md5 = TinaX.IO.XFile.GetMD5(package_path);
                        //var md5_path = package_path + ".md5.txt";
                        //if (File.Exists(md5_path))
                        //{
                        //    File.Delete(md5_path);
                        //}
                        //File.WriteAllText(md5_path, md5);
                        //Directory.Delete(temp_dir,true);
                        //测试
                        //TinaX.IO.XFile.Unzip(package_path, pack_dir);
                    }
                }
            }
        }

        
    }

}