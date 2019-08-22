using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TinaX;

namespace TinaXEditor.DevOps
{
    /// <summary>
    /// 这是一个用来给TinaX Framework设置宏定义的窗口
    /// 窗口大概分为几部分
    /// 1. 无论什么平台都会有的定义（白名单）
    /// 2. 针对某个平台的单独定义
    /// 3. 针对平台的排除（黑名单
    /// 
    /// 
    /// !!!这个窗口操作的内容很敏感，不能即时保存哦，必须手动保存哦
    /// </summary>
    public class SharpDefineReadWriteWindow : EditorWindow
    {
        private static SharpDefineReadWriteWindow _window_obj;


        [MenuItem("TinaX/配置/项目宏定义设置",false,2)]
        public static void OpenWindow()
        {
            if (_window_obj != null)
            {
                _window_obj.Focus();
            }
            else
            {
                _window_obj = GetWindow<SharpDefineReadWriteWindow>("宏定义设置");

            }
        }

        private GUIStyle mStyle_Title = new GUIStyle();
        private GUIStyle mStyle_SelectedItem = new GUIStyle();

        private Vector2 mScroll_CommonDefine_Defaults;
        private Vector2 mScroll_CommonDefine_Custom;
        private string mSelect_CommonDefine_Default_Btn; //存放当前选中的项目：公共定义中的预设部分
        private string mStr_CommonDefine_Custom_Add;

        private Vector2 mScroll_Target_Defalut;
        private Vector2 mScroll_Target_Custom;
        private Vector2 mScroll_Target_Ignore;
        private BuildTargetGroup mSelect_Edit_BuildTargetGroup; //当前编辑的平台组
        private string mSelect_Target_Default_Btn; //存放当前选中的项目：平台定义中的预设部分
        private string mStr_Target_Custom_Add;
        private string mStr_Target_Ignore_Add;



        private TinaX.Core.XBaseConfig mBaseCfg;
        private List<string> mCommonSharpDefine = new List<string>();
        private TinaX.Core.XBaseConfig.S_DefineWithBuildTargetGroup mCur_DefineWithTarget;



        private void OnEnable()
        {

            this.minSize = new Vector2(900, 500);
            this.maxSize = new Vector2(1000, 800);

            mBaseCfg = TinaX.Config.GetTinaXConfig<TinaX.Core.XBaseConfig>(TinaX.Conf.ConfigPath.base_config);
            if(mBaseCfg != null)
            {
                mCommonSharpDefine = mBaseCfg.CommonDefineStr;
                
            }


            mStyle_Title.fontSize = 16;
            mStyle_Title.normal.textColor = TinaX.Core.XEditorStyleDefine.Color_Text_Normal;
            mStyle_Title.padding = new RectOffset(10, 10, 10, 10);

            mStyle_SelectedItem.fontStyle = FontStyle.Bold;
            mStyle_SelectedItem.normal.textColor = TinaX.Core.XEditorStyleDefine.Color_Text_Normal;

        }

        private void OnDestroy()
        {
            _window_obj = null;

            if(mBaseCfg != null)
            {
                //var ser_conf = new SerializedObject(mBaseCfg);
                //ser_conf.
                //ser_conf.ApplyModifiedProperties();
                EditorUtility.SetDirty(mBaseCfg);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
            
            //mBaseCfg.
        }


        private void OnGUI()
        {
            if (mBaseCfg != null)
            {
                GUILayout.BeginHorizontal(GUILayout.MinWidth(900),GUILayout.MaxWidth(1000));

                //通用白名单
                #region 通用白名单
                GUILayout.BeginVertical(GUILayout.Width(300));
                GUILayout.Label("通用定义",mStyle_Title);
                GUILayout.Label("无论在哪个平台，都会存在的定义哦");

                GUILayout.Space(15);
                GUILayout.Label("TinaX预设定义");
                //通用白名单 - 框架预定义部分
                mScroll_CommonDefine_Defaults = EditorGUILayout.BeginScrollView(mScroll_CommonDefine_Defaults, GUILayout.MaxHeight(200), GUILayout.MaxWidth(270));

                foreach(var item in SharpDefineDefault.DefineItems)
                {
                    var enable = mCommonSharpDefine.Contains(item.DefineStr);
                    if (mSelect_CommonDefine_Default_Btn == item.DefineStr)
                    {
                        //当前选中项
                        
                        GUILayout.Label(" - " + item.Name, mStyle_SelectedItem);
                        GUILayout.TextArea(item.Desc,500,GUILayout.MaxWidth(270),GUILayout.MaxHeight(60));
                        GUILayout.Label("定义字符：" + item.DefineStr);

                        //判断是否在配置的list中
                        if (enable)
                        {
                            //包含
                            GUILayout.BeginHorizontal();
                            GUILayout.Label("    定义状态：已启用",GUILayout.MaxWidth(125));
                            if(GUILayout.Button("停用定义", GUILayout.MaxWidth(55)))
                            {
                                mCommonSharpDefine.Remove(item.DefineStr);
                            }
                            GUILayout.EndHorizontal();
                        }
                        else
                        {
                            //不包含
                            GUILayout.BeginHorizontal();
                            GUILayout.Label("    定义状态：未启用",GUILayout.MaxWidth(125));
                            if (GUILayout.Button("启用定义", GUILayout.MaxWidth(55)))
                            {
                                mCommonSharpDefine.Add(item.DefineStr);
                            }
                            GUILayout.EndHorizontal();
                        }
                    }
                    else
                    {
                        string btn_name;
                        if (enable)
                        {
                            btn_name = "[√] " + item.Name;
                        }
                        else
                        {
                            btn_name = "[-] " + item.Name;
                        }
                        if (GUILayout.Button(btn_name))
                        {
                            mSelect_CommonDefine_Default_Btn = item.DefineStr;
                        }
                        
                    }
                    
                }

                EditorGUILayout.EndScrollView();

                //通用白名单 - 自定义
                GUILayout.Label("自定义#Define");
                mScroll_CommonDefine_Custom = EditorGUILayout.BeginScrollView(mScroll_CommonDefine_Custom,GUILayout.MaxHeight(200),GUILayout.MaxWidth(250));

                for(int i = mCommonSharpDefine.Count - 1; i >= 0; i--)
                {
                    var v = mCommonSharpDefine[i];
                    //判断下不是预定义内容
                    var flag = false;
                    foreach(var item in SharpDefineDefault.DefineItems)
                    {
                        if (item.DefineStr == v)
                        {
                            flag = true;
                            break;
                        }
                    }
                    if (!flag)
                    {
                        //绘制
                        GUILayout.BeginHorizontal(GUILayout.MaxWidth(250));
                        GUILayout.Label("  - " + v);
                        if (GUILayout.Button("移除", GUILayout.MaxWidth(35)))
                        {
                            mCommonSharpDefine.RemoveAt(i);
                        }
                        GUILayout.EndHorizontal();
                    }
                }

                //操作
                GUILayout.Space(10);
                GUILayout.BeginHorizontal();
                mStr_CommonDefine_Custom_Add = GUILayout.TextField(mStr_CommonDefine_Custom_Add, GUILayout.MaxWidth(190));
                if (GUILayout.Button("添加自定义宏",GUILayout.MaxWidth(75)))
                {
                    if (mStr_CommonDefine_Custom_Add.IsNullOrEmpty())
                    {
                        EditorUtility.DisplayDialog("不太对劲", "请输入有效的自定义宏 字符呀", "我错了");
                        
                    }
                    else if (mCommonSharpDefine.Contains(mStr_CommonDefine_Custom_Add))
                    {
                        EditorUtility.DisplayDialog("不太对劲", "不可以重复添加的哟", "我错了");
                    }
                    else
                    {
                        mCommonSharpDefine.Add(mStr_CommonDefine_Custom_Add);
                        
                    }
                }

                GUILayout.EndHorizontal();

                EditorGUILayout.EndScrollView();

                GUILayout.EndVertical();


                #endregion

                #region 针对平台的定义项
                GUILayout.BeginVertical(GUILayout.Width(300));

                GUILayout.Label("针对平台的定义", mStyle_Title);
                GUILayout.Label("针对通用定义的补充，只对某些特定目标平台生效。");

                GUILayout.BeginHorizontal();
                GUILayout.Label("当前编辑的平台：", GUILayout.MaxWidth(110));
                mSelect_Edit_BuildTargetGroup = (BuildTargetGroup)EditorGUILayout.EnumPopup(mSelect_Edit_BuildTargetGroup,GUILayout.MaxWidth(100));
                GUILayout.EndHorizontal();

                //确保列表
                if(mSelect_Edit_BuildTargetGroup.ToString() != mCur_DefineWithTarget.TargetName)
                {
                    var flag = false;
                    foreach(var item in mBaseCfg.DefineWithTarget)
                    {
                        if(item.TargetName == mSelect_Edit_BuildTargetGroup.ToString())
                        {
                            flag = true;
                            mCur_DefineWithTarget = item;
                            break;
                        }
                    }

                    if (!flag)
                    {
                        mCur_DefineWithTarget = new TinaX.Core.XBaseConfig.S_DefineWithBuildTargetGroup();
                        mCur_DefineWithTarget.TargetName = mSelect_Edit_BuildTargetGroup.ToString();

                        if (mCur_DefineWithTarget.DefineStr == null)
                        {
                            mCur_DefineWithTarget.DefineStr = new List<string>();
                        }
                        if(mCur_DefineWithTarget.IgnoreDefine == null)
                        {
                            mCur_DefineWithTarget.IgnoreDefine = new List<string>();
                        }

                        mBaseCfg.DefineWithTarget.Add(mCur_DefineWithTarget);
                    }

                    
                }

                GUILayout.Label("配置：" + mCur_DefineWithTarget.TargetName);

                //框架预定义部分
                GUILayout.Space(15);
                GUILayout.Label("TinaX预设定义");
                mScroll_Target_Defalut = EditorGUILayout.BeginScrollView(mScroll_Target_Defalut,GUILayout.MaxHeight(200),GUILayout.MaxWidth(270));

                foreach (var item in SharpDefineDefault.DefineItems)
                {
                    var enable = mCur_DefineWithTarget.DefineStr.Contains(item.DefineStr);
                    //检查下，在全局定义中没有的，这里才可以显示
                    if (mCommonSharpDefine.Contains(item.DefineStr))
                    {
                        //已经有了
                        GUILayout.Label(" -" + item.Name + " [全局定义]");
                    }
                    else
                    {
                        if (mSelect_Target_Default_Btn == item.DefineStr)
                        {
                            //当前选中项

                            GUILayout.Label(" - " + item.Name, mStyle_SelectedItem);
                            GUILayout.TextArea(item.Desc, 500, GUILayout.MaxWidth(270), GUILayout.MaxHeight(60));
                            GUILayout.Label("定义字符：" + item.DefineStr);

                            //判断是否在配置的list中
                            if (enable)
                            {
                                //包含
                                GUILayout.BeginHorizontal();
                                GUILayout.Label("    定义状态：已启用", GUILayout.MaxWidth(125));
                                if (GUILayout.Button("停用定义", GUILayout.MaxWidth(55)))
                                {
                                    mCur_DefineWithTarget.DefineStr.Remove(item.DefineStr);
                                }
                                GUILayout.EndHorizontal();
                            }
                            else
                            {
                                //不包含
                                GUILayout.BeginHorizontal();
                                GUILayout.Label("    定义状态：未启用", GUILayout.MaxWidth(125));
                                if (GUILayout.Button("启用定义", GUILayout.MaxWidth(55)))
                                {
                                    mCur_DefineWithTarget.DefineStr.Add(item.DefineStr);
                                }
                                GUILayout.EndHorizontal();
                            }
                        }
                        else
                        {
                            string btn_name;
                            if (enable)
                            {
                                btn_name = "[√] " + item.Name;
                            }
                            else
                            {
                                btn_name = "[-] " + item.Name;
                            }
                            if (GUILayout.Button(btn_name))
                            {
                                mSelect_Target_Default_Btn = item.DefineStr;
                            }

                        }
                    }
                    

                }

                EditorGUILayout.EndScrollView();
                //自定义部分【添加】
                GUILayout.Label("自定义#Define");
                mScroll_Target_Custom = EditorGUILayout.BeginScrollView(mScroll_Target_Custom, GUILayout.MaxHeight(150), GUILayout.MaxWidth(250));

                for (int i = mCur_DefineWithTarget.DefineStr.Count - 1; i >= 0; i--)
                {
                    var v = mCur_DefineWithTarget.DefineStr[i];
                    //判断下不是预定义内容
                    var flag = false;
                    foreach (var item in SharpDefineDefault.DefineItems)
                    {
                        if (item.DefineStr == v)
                        {
                            flag = true;
                            break;
                        }
                    }
                    if (!flag)
                    {
                        //绘制
                        GUILayout.BeginHorizontal(GUILayout.MaxWidth(250));
                        GUILayout.Label("  - " + v);
                        if (GUILayout.Button("移除", GUILayout.MaxWidth(35)))
                        {
                            mCur_DefineWithTarget.DefineStr.RemoveAt(i);
                        }
                        GUILayout.EndHorizontal();
                    }
                }

                //操作
                GUILayout.Space(10);
                GUILayout.BeginHorizontal();
                mStr_Target_Custom_Add = GUILayout.TextField(mStr_Target_Custom_Add, GUILayout.MaxWidth(190));
                if (GUILayout.Button("添加自定义宏", GUILayout.MaxWidth(75)))
                {
                    if (mStr_Target_Custom_Add.IsNullOrEmpty())
                    {
                        EditorUtility.DisplayDialog("不太对劲", "请输入有效的自定义宏 字符呀", "我错了");

                    }
                    else if (mCur_DefineWithTarget.DefineStr.Contains(mStr_Target_Custom_Add))
                    {
                        EditorUtility.DisplayDialog("不太对劲", "不可以重复添加的哟", "我错了");
                    }
                    else
                    {
                        mCur_DefineWithTarget.DefineStr.Add(mStr_Target_Custom_Add);

                    }
                }

                GUILayout.EndHorizontal();

                EditorGUILayout.EndScrollView();

                //自定义部分【排除】
                GUILayout.Space(10);
                GUILayout.Label("忽略列表");
                GUILayout.Label("该定义用于忽略全局定义和当前平台定义的宏符号");
                mScroll_Target_Ignore = EditorGUILayout.BeginScrollView(mScroll_Target_Ignore,GUILayout.MaxHeight(150), GUILayout.MaxWidth(250));

                for (int i = mCur_DefineWithTarget.IgnoreDefine.Count - 1; i >= 0; i--)
                {
                    var v = mCur_DefineWithTarget.IgnoreDefine[i];
                    

                    //绘制
                    GUILayout.BeginHorizontal(GUILayout.MaxWidth(250));
                    GUILayout.Label("  - " + v);
                    if (GUILayout.Button("移出",GUILayout.MaxWidth(35)))
                    {
                        mCur_DefineWithTarget.IgnoreDefine.RemoveAt(i);
                    }
                    GUILayout.EndHorizontal();
                }

                EditorGUILayout.EndScrollView();
                //操作
                GUILayout.Space(10);
                
                GUILayout.BeginHorizontal();
                mStr_Target_Ignore_Add = GUILayout.TextField(mStr_Target_Ignore_Add, GUILayout.MaxWidth(190));
                if (GUILayout.Button("添加忽略", GUILayout.MaxWidth(75)))
                {
                    if (mStr_Target_Ignore_Add.IsNullOrEmpty())
                    {
                        EditorUtility.DisplayDialog("不太对劲", "请输入有效的自定义宏 字符呀", "我错了");

                    }
                    else if (mCur_DefineWithTarget.IgnoreDefine.Contains(mStr_Target_Ignore_Add))
                    {
                        EditorUtility.DisplayDialog("不太对劲", "不可以重复添加的哟", "我错了");
                    }
                    else
                    {
                        mCur_DefineWithTarget.IgnoreDefine.Add(mStr_Target_Ignore_Add);

                    }
                }
                GUILayout.EndHorizontal();

                

                GUILayout.EndVertical();
                #endregion

                #region 最终配置预览和应用
                GUILayout.BeginVertical(GUILayout.Width(300));
                GUILayout.Label("预览与应用设置", mStyle_Title);
                GUILayout.Label("当前平台：" + mSelect_Edit_BuildTargetGroup.ToString());
                GUILayout.Label("在“针对平台的设置”那里切换平台哦");

                GUILayout.Space(10);
                GUILayout.Label("当前应有定义内容：");
                var define_str_arr = GetTotalDefines();
                var define_str = GetDefineStr(define_str_arr);

                foreach(var item in define_str_arr)
                {
                    GUILayout.Label("  -" + item);
                }

                GUILayout.Space(10);
                GUILayout.Label("定义字符串：");
                GUILayout.TextArea(define_str);

                GUILayout.Space(10);
                GUILayout.Label("当前Unity中有，但配置中没有的：");
                var unity_define_str = PlayerSettings.GetScriptingDefineSymbolsForGroup(mSelect_Edit_BuildTargetGroup);
                
                string[] unity_define_str_arr;
                if(unity_define_str.IsNullOrEmpty())
                {
                    unity_define_str_arr = new string[0];
                }
                else if (unity_define_str.IndexOf(';') < 0)
                {
                    unity_define_str_arr = new string[1] { unity_define_str };
                }
                else

                {
                    unity_define_str_arr = unity_define_str.Split(';');
                }
                foreach(var item in unity_define_str_arr)
                {
                    //Debug.Log(item);
                    var flag = false;
                    foreach(var i2 in define_str_arr)
                    {
                        if(i2 == item)
                        {
                            flag = true;
                            break;
                        }
                    }
                    if (!flag)
                    {

                        //没有
                        GUILayout.BeginHorizontal();
                        GUILayout.Label(" -" + item);
                        if (GUILayout.Button("添加到设置", GUILayout.MaxWidth(68)))
                        {
                            var i = EditorUtility.DisplayDialogComplex("添加到配置", "要添加窦啊什么地方呢", "全局定义", "算了", "当前平台定义");
                            switch (i)
                            {
                                case 0:
                                    //全局定义：
                                    mCommonSharpDefine.Add(item);
                                    break;
                                case 1:
                                    //取消
                                    break;
                                case 2:
                                    //添加到当前平台
                                    mCur_DefineWithTarget.DefineStr.Add(item);
                                    break;
                            }
                        }
                        GUILayout.EndHorizontal();
                    }
                }

                //操作

                if (GUILayout.Button("覆盖到Unity设置中"))
                {
                    if (EditorUtility.DisplayDialog("操作确认", "确定要将当前设定的内容覆盖进Unity设置吗？\n请仔细检查平台和字符串内容哇", "是", "算了"))
                    {
                        SharpDefineReadWriteUtils.CoverDefine(mSelect_Edit_BuildTargetGroup, define_str);
                        Debug.Log("已完成设置覆盖");
                    }
                }

                if (GUILayout.Button("增项到Unity设置中"))
                {
                    if (EditorUtility.DisplayDialog("操作确认", "确定要将当前设定的内容覆盖进Unity设置吗？\n请仔细检查平台和字符串内容哇", "是", "算了"))
                    {
                        foreach (var item in define_str_arr)
                        {
                            SharpDefineReadWriteUtils.AddDefineIfNotExist(mSelect_Edit_BuildTargetGroup, item);
                        }
                        Debug.Log("已完成设置增项");
                    }

                }

                GUILayout.EndVertical();

                #endregion


                GUILayout.EndHorizontal();
            }
            else
            {
                GUILayout.Label("暂时没法使用这个功能呢\n先执行一下Framework的安装流程呗\n=(:з」∠)_");
            }
            
        }

        
        private string[] GetTotalDefines()
        {
            List<string> arrs = new List<string>();
            //先把全局加进去
            arrs.AddRange(mCommonSharpDefine);
            //再把平台定义加进去
            foreach(var item in mCur_DefineWithTarget.DefineStr)
            {
                if (!arrs.Contains(item))
                {
                    arrs.Add(item);
                }
            }
            //减去忽略部分
            foreach(var item in mCur_DefineWithTarget.IgnoreDefine)
            {
                if (arrs.Contains(item))
                {
                    arrs.Remove(item);
                }
            }


            return arrs.ToArray();
        }

        /// <summary>
        /// 根据上面那个方法的数组，生成应用于Unity的字符串
        /// </summary>
        /// <param name="str_arr"></param>
        /// <returns></returns>
        private string GetDefineStr(string[] str_arr)
        {
            string str = "";
            for(int i = 0; i< str_arr.Length; i++)
            {
                str += str_arr[i];
                if(i < (str_arr.Length - 1))
                {
                    str += ";";
                }
            }
            return str;
        }


    }

}

