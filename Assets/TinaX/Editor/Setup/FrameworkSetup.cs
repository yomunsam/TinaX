using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
#if  ODIN_INSPECTOR
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
#endif
using TinaX;

namespace TinaXEditor.Setup
{
    /// <summary>
    /// 框架安装与初始化窗口
    /// </summary>
    public class FrameworkSetup : EditorWindow
    {
        [MenuItem("TinaX/框架/初始化安装", false, 100)]
        public static void OpenSetupWindow()
        {
            EditorWindow.GetWindow<FrameworkSetup>().Show();

        }
        [MenuItem("TinaX/框架/初始化安装", true)]
        static bool CanOpenSetupWindow()
        {
            var conf = Config.GetTinaXConfig<TinaX.Core.XBaseConfig>(TinaX.Conf.ConfigPath.base_config);
            return conf == null;
        }
        public FrameworkSetup()
        {
            this.titleContent = new GUIContent("安装 " + TinaX.FrameworkInfo.FrameworkName);
            this.minSize = new Vector2(400, 350);
            this.maxSize = new Vector2(400, 350);
        }


        private enum E_PageIndex
        {
            splash,
            install,
            install_success,
        }
        E_PageIndex mPageIndex = E_PageIndex.splash;

#region Style

        GUIStyle mStyle_Title = new GUIStyle();
        GUIStyle mStyle_ButtonText = EditorStyles.miniButton;

#endregion

        private void OnEnable()
        {
            
#region Style

            //Body


            //Title
            mStyle_Title.fontSize = 20;
            mStyle_Title.fontStyle = FontStyle.Bold;
            mStyle_Title.normal.textColor = TinaX.Core.XEditorStyleDefine.Color_Text_Normal;
            mStyle_Title.alignment = TextAnchor.UpperCenter;
            mStyle_Title.padding = new RectOffset(0, 0, 10, 5);

            //Button Text
            //mStyle_ButtonText.fontSize = 14; //别用这句话，会影响编辑器的
            //mStyle_ButtonText.normal.textColor = TinaX.Core.XEditorStyleDefine.Color_Text_Normal;


#endregion
        }

        private void OnGUI()
        {
            
            GUILayout.BeginVertical();

            switch (mPageIndex)
            {
                case E_PageIndex.splash:
                    Draw_Splash();
                    break;
                case E_PageIndex.install:
                    Draw_Install();
                    break;
                case E_PageIndex.install_success:
                    Draw_Install_Success();
                    break;
            }


            GUILayout.EndVertical();

        }


        private void Draw_Splash()
        {
            GUILayout.Label("Tina X6 Framework", mStyle_Title);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("文档", mStyle_ButtonText))
            {
                Application.OpenURL(FrameworkInfo.Framework_Url_Doc);
            }
            if (GUILayout.Button("安装", mStyle_ButtonText))
            {
                mPageIndex = E_PageIndex.install;
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(200);
        }

        private Dictionary<string, bool> mDict_Install_status = new Dictionary<string, bool>();
        private Dictionary<string, bool> mDict_Install_Selectable_Tag = new Dictionary<string, bool>(); //如果一个项目是可选安装，就会存放在这里，来表示用户选择是否安装
        
        private void Draw_Install()
        {
            GUILayout.Space(10);
            //绘制安装项目
            foreach(var item in SystemInstallRegister.RegItems)
            {
                if (!mDict_Install_status.ContainsKey(item.Name))
                {
                    mDict_Install_status.Add(item.Name, false);

                    if (item.IsInstalled != null)
                    {
                        mDict_Install_status[item.Name] = item.IsInstalled();
                    }
                }
                

                if (item.Selectable)
                {

                    //可选安装

                    if (!mDict_Install_Selectable_Tag.ContainsKey(item.Name)) //初始值
                    {
                        mDict_Install_Selectable_Tag.Add(item.Name, item.DefaultSelect);
                    }

                    if (mDict_Install_status[item.Name])
                    {
                        //已安装
                        GUILayout.Label("-" + item.Name + " [√]");
                    }
                    else
                    {
                        //未安装
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("-" + item.Name + " [-]");

                        if (mDict_Install_Selectable_Tag[item.Name])
                        {
                            //选择了安装
                            GUILayout.Label("计划：安装");
                            if(GUILayout.Button("不要安装", GUILayout.MaxWidth(80)))
                            {
                                mDict_Install_Selectable_Tag[item.Name] = false;
                            }
                        }
                        else
                        {
                            //选择了不安装
                            GUILayout.Label("计划：不安装") ;
                            if (GUILayout.Button("安装", GUILayout.MaxWidth(80)))
                            {
                                mDict_Install_Selectable_Tag[item.Name] = true;
                            }
                        }

                        GUILayout.EndHorizontal();
                    }
                }
                else
                {
                    if (mDict_Install_status[item.Name])
                    {
                        //已安装
                        GUILayout.Label("-" + item.Name + " [√]");
                    }
                    else
                    {
                        //未安装
                        GUILayout.Label("-" + item.Name + " [-]");
                    }
                }
                
            }

            GUILayout.Space(20);
            if (GUILayout.Button("开始安装",mStyle_ButtonText))
            {
                Folder.CreateFolder("Assets/Resources/" + TinaX.Setup.Framework_Config_Path);
                foreach (var item in SystemInstallRegister.RegItems)
                {
                    //检查是否已安装
                    if (!mDict_Install_status[item.Name])
                    {
                        //没安装，看看是否是可选安装
                        if (item.Selectable)
                        {
                            //是
                            if (mDict_Install_Selectable_Tag[item.Name])
                            {
                                //要安装
                                item.DoInstall();
                                mDict_Install_status[item.Name] = true;
                            }
                        }
                        else
                        {
                            //不是，无脑装
                            item.DoInstall();
                            mDict_Install_status[item.Name] = true;
                        }
                    }
                }

                //然后我们来处理一下base_config
                Config.CreateIfNotExist<TinaX.Core.XBaseConfig>(TinaX.Conf.ConfigPath.base_config);
                

                mPageIndex = E_PageIndex.install_success;
            }


        }


        private void Draw_Install_Success()
        {
            GUILayout.Label("安装成功");
            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("看看文档"))
            {
                Application.OpenURL("https://tinax.corala.space/");
            }
            //if (GUILayout.Button("打开项目配置"))
            //{

            //}
            GUILayout.EndHorizontal();
        }
    }


    /// <summary>
    /// 在引擎编辑器启动的时候加载一次，确保一些框架关键性依赖和配置文件正常
    /// </summary>
    [UnityEditor.InitializeOnLoad]
    public class FrameworkAutoSetup
    {
        static FrameworkAutoSetup()
        {
            //Debug.Log("喵！");
            foreach(var item in TinaX.Conf.ConfigRegister.ConfigRegisters)
            {
                item.Action_Create();
            }
        }
    }
}

