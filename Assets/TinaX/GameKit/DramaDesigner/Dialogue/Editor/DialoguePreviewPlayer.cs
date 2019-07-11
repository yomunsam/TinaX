using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TinaX;
using TinaXGameKit.Drama.BP;
using TinaXGameKit.Drama;
using UnityEditor;

namespace TinaXGameKitEditor.Drama
{
    /// <summary>
    /// 对话剧情预览播放器
    /// </summary>
    public class DialoguePreviewPlayer : EditorWindow
    {
        /// <summary>
        /// 要访问的入口BluePrint
        /// </summary>
        public static DialogueBluePrint ReadBP;

        [MenuItem("TGameKit/剧情设计师/对话蓝图预览",false,1)]
        public static void OpenWindow()
        {
            GetWindow<DialoguePreviewPlayer>().Show();
        }
        

        #region rumtime
        /// <summary>
        /// 播放器
        /// </summary>
        private DialoguePlayer mPlayer;
        private GUIStyle mTitleStyle = new GUIStyle();
        private GUIStyle mContentStyle = new GUIStyle();
        
        
        #endregion

        private void OnEnable()
        {
            this.titleContent = new GUIContent("对话预览");

            mTitleStyle.fontSize = 18;
            mTitleStyle.alignment = TextAnchor.UpperLeft;
            mTitleStyle.padding = new RectOffset(5, 5, 5, 5);
            mTitleStyle.normal.textColor = TinaX.Core.XEditorStyleDefine.Color_Text_Normal;
            

            mContentStyle.fontSize = 16;
            mContentStyle.alignment = TextAnchor.MiddleLeft;
            mContentStyle.padding = new RectOffset(5, 5, 5, 5);
            mContentStyle.normal.textColor = TinaX.Core.XEditorStyleDefine.Color_Text_Normal;

            //初始化播放器
            mPlayer = new DialoguePlayer();
            mPlayer.OnChoose = (data) =>
            {
                mTitle = data.GetTitle();
                mContent = "";
                mSpeaker = data.GetSpeaker();
                //按钮列表
                var chooseList = data.GetChooseList();
                List<Button> btns = new List<Button>();
                foreach(var item in chooseList)
                {
                    btns.Add(new Button() {
                        Title = item,
                        callback = () =>
                        {
                            mPlayer.NextWithString(item);
                        }
                    });
                }
                mButtonGroup = btns.ToArray();

                mCurStatus = GUIStatus.choose;
            };
            mPlayer.OnContent = (data) =>
            {
                mTitle = "";
                mContent = data.GetContent();
                mSpeaker = data.GetSpeaker();

                mCurStatus = GUIStatus.content;
            };
            mPlayer.OnFinish = (data) =>
            {
                mCurStatus = GUIStatus.finish;
                mContent = data;
            };
        }

        private void OnDestroy()
        {
            //释放播放器
            mPlayer = null;
        }


        #region GUI
        private enum GUIStatus
        {
            /// <summary>
            /// 选择播放的蓝图
            /// </summary>
            select,
            /// <summary>
            /// 显示内容对话
            /// </summary>
            content,
            /// <summary>
            /// 显示选项
            /// </summary>
            choose,
            /// <summary>
            /// 播放结束
            /// </summary>
            finish,
        }

        private struct Button
        {
            public string Title;
            public System.Action callback;
        }

        /// <summary>
        /// 当前状态记录
        /// </summary>
        private GUIStatus mCurStatus = GUIStatus.select;
        private string mTitle;      //标题
        private string mSpeaker;    //讲述者
        private string mContent;    //正文内容
        private Button[] mButtonGroup = new Button[] { };



        private void OnGUI()
        {
            switch (mCurStatus)
            {
                case GUIStatus.select:
                    Draw_select();
                    break;
                case GUIStatus.content:
                    Draw_Content();
                    break;
                case GUIStatus.choose:
                    Draw_Choose();
                    break;
                case GUIStatus.finish:
                    Draw_Finish();
                    break;
            }
        }


        private string mSelectPath;
        /// <summary>
        /// 绘制：选择要播放的蓝图
        /// </summary>
        private void Draw_select()
        {
            GUILayout.Label("输入路径：");
            mSelectPath = GUILayout.TextField(mSelectPath);
            if (GUILayout.Button("选择"))
            {
                var path  = EditorUtility.OpenFilePanel("选择对话蓝图", System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(),"Assets"), "asset");
                var cur_path = System.IO.Directory.GetCurrentDirectory();
                cur_path = cur_path.Replace("\\", "/");
                cur_path = cur_path + "/";
                path = path.Replace(cur_path, "");
                //path = path.Replace("\\", "/");
                mSelectPath = path;
            }

            GUILayout.Space(10);
            if (!mSelectPath.IsNullOrEmpty())
            {
                if (GUILayout.Button("播放"))
                {
                    AssetDatabase.Refresh();
                    AssetDatabase.ReleaseCachedFileHandles();
                    
                    var asset = AssetDatabase.LoadAssetAtPath<DialogueBluePrint>(mSelectPath);
                    if (asset == null)
                    {
                        EditorUtility.DisplayDialog("失败", "载入文件失败", "嗯");
                        return;
                    }
                    mPlayer.PlayDialogue(asset);
                    
                }
            }
        }

        /// <summary>
        /// 绘制普通对话内容
        /// </summary>
        private void Draw_Content()
        {
            GUILayout.BeginVertical();

            if (!mTitle.IsNullOrEmpty())
            {
                GUILayout.Label(mTitle, mTitleStyle);
            }
            if (!mContent.IsNullOrEmpty())
            {

                if (!mSpeaker.IsNullOrEmpty())
                {
                    GUILayout.Label("[" + mSpeaker + "]： ");
                }

                GUILayout.Label(mContent,mContentStyle);

            }
            GUILayout.Space(20);

            if (GUILayout.Button("next"))
            {
                mPlayer.Next();
            }

            GUILayout.EndVertical();
        }


        /// <summary>
        /// 绘制选择对话框
        /// </summary>
        private void Draw_Choose()
        {
            GUILayout.BeginVertical();
            if (!mSpeaker.IsNullOrEmpty())
            {
                GUILayout.Label("[" + mSpeaker + "]： ");
            }
            if (!mTitle.IsNullOrEmpty())
            {
                GUILayout.Label(mTitle, mTitleStyle);
            }
            
            if (!mContent.IsNullOrEmpty())
            {
                GUILayout.Label(mContent, mContentStyle);
            }
            GUILayout.Space(20);

            //绘制按钮组
            foreach(var item in mButtonGroup)
            {
                if (GUILayout.Button(item.Title))
                {
                    item.callback();
                }
            }

            GUILayout.EndVertical();
        }

        private void Draw_Finish()
        {
            GUILayout.Label("播放结束");
            if (!mContent.IsNullOrEmpty())
            {
                GUILayout.Label(mContent, mContentStyle);
            }
        }

        #endregion



    }

}
