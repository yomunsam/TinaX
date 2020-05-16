using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/*
 * 这里全改成异步的了（
 * 
 */

namespace TinaXEditor.Setup.Internal
{
    public class SetupGUI : EditorWindow
    {
        private enum EDataState
        {
            NotReady        = 0,
            Loading         = 1,
            Ready           = 2,
        }

        private static SetupGUI _wnd;

        [MenuItem("TinaX/Setup/Manage TinaX Packages")]
        public static void OpenUI()
        {
            if(_wnd == null)
                _wnd = EditorWindow.GetWindow<SetupGUI>();
            else
            {
                _wnd.Show();
                _wnd.Focus();
            }

            _wnd.titleContent = new GUIContent("TinaX Package Manager");
        }

        private int mToolbarIndex;
        private Vector2 mListScrollViewV2;
        private Vector2 mInstallScrollViewV2;

        private EDataState mDataState = EDataState.NotReady;
        private int? mListByIndex;
        private List<PackageListInfo> mListData;

        private PackageListInfo mDetailData;

        private void OnGUI()
        {
            EditorGUILayout.BeginVertical(Styles.body);
            mToolbarIndex = GUILayout.Toolbar(mToolbarIndex, I18Ns.mToolbar_contents);

            //package list
            DrawPackageList();

            EditorGUILayout.EndVertical();
        }


        private void DrawPackageList()
        {
            
            if (mListByIndex == null || this.mDataState == EDataState.NotReady)
                RefreshData(mToolbarIndex);

            if (mListByIndex == null) return;
            if (mListByIndex != mToolbarIndex)
                RefreshData(mToolbarIndex);

            if(this.mDataState == EDataState.Loading)
            {
                GUILayout.Space(10);
                EditorGUILayout.LabelField(I18Ns.LoadingList,Styles.normal_label_center);
                return;
            }

            if(this.mDataState == EDataState.Ready)
            {
                if(mListData == null || mListData.Count == 0)
                {
                    GUILayout.Space(10);
                    EditorGUILayout.LabelField(I18Ns.DoData, Styles.normal_label_center);
                }
                else
                {
                    GUILayout.Space(10);
                    //布局是左右分栏
                    EditorGUILayout.BeginHorizontal();
                    //左边是列表
                    if (mDetailData != null)
                        EditorGUILayout.BeginVertical(Styles.package_list, GUILayout.MaxWidth(450));
                    else
                        EditorGUILayout.BeginVertical(Styles.package_list);

                    mListScrollViewV2 = EditorGUILayout.BeginScrollView(mListScrollViewV2);
                    foreach (var item in mListData)
                    {
                        EditorGUILayout.BeginVertical();
                        GUILayout.Space(3);
                        #region Content
                        EditorGUILayout.BeginHorizontal();

                        EditorGUILayout.BeginVertical();
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField(new GUIContent(item.BaseInfo.displayName,item.BaseInfo.packageName), Styles.title_label,GUILayout.MaxWidth(180));
                        GUILayout.Space(4);
                        //EditorGUILayout.LabelField(item.BaseInfo.packageName,GUILayout.MaxWidth(160));
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.LabelField(new GUIContent(item.BaseInfo.description, item.BaseInfo.description),GUILayout.MaxWidth(180));
                        EditorGUILayout.EndVertical();

                        GUILayout.FlexibleSpace();
                        //安装选项
                        EditorGUILayout.BeginVertical();
                        if(item.Installed)
                        {
                            if (GUILayout.Button("Remove"))
                            {

                            }
                        }
                        //else
                        //{
                        //    if (GUILayout.Button("Install"))
                        //    {

                        //    }
                        //}

                        //概述
                        if (GUILayout.Button("Detail"))
                        {
                            mDetailData = item;
                            Debug.Log(mDetailData.BaseInfo.displayName);
                        }

                        EditorGUILayout.EndVertical();

                        EditorGUILayout.EndHorizontal();
                        #endregion

                        GUILayout.Label("", GUI.skin.horizontalSlider);
                        EditorGUILayout.Space();
                        EditorGUILayout.EndVertical();
                    }
                    EditorGUILayout.EndScrollView();
                    EditorGUILayout.EndVertical();

                    //右边是细节面板
                    if(mDetailData != null)
                    {
                        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                        EditorGUILayout.LabelField(mDetailData.BaseInfo.displayName, Styles.title_label);


                        //操作
                        GUILayout.Space(10);
                        //安装？
                        if (!mDetailData.Installed && (!string.IsNullOrEmpty(mDetailData.BaseInfo.npmUrl) || !string.IsNullOrEmpty(mDetailData.BaseInfo.gitUrl)))
                        {
                            if(mDetailData.BaseInfo.versionTags != null && mDetailData.BaseInfo.versionTags.Count > 0)
                            {
                                mInstallScrollViewV2 = EditorGUILayout.BeginScrollView(mInstallScrollViewV2,GUILayout.MaxHeight(60));
                                foreach(var tag in mDetailData.BaseInfo.versionTags)
                                {
                                    if (GUILayout.Button($"Install {tag}"))
                                    {

                                    }
                                }
                                EditorGUILayout.EndScrollView();
                            }
                            else
                            {
                                GUILayout.Button("Install latest package");
                            }
                        }
                        EditorGUILayout.Space();

                        //从浏览器下载
                        if (!string.IsNullOrEmpty(mDetailData.BaseInfo.downloadUrl))
                        {
                            if(GUILayout.Button("Download from browser"))
                            {
                                Application.OpenURL(mDetailData.BaseInfo.downloadUrl);
                            }
                        }
                        //访问项目地址
                        if (!string.IsNullOrEmpty(mDetailData.BaseInfo.repoUrl))
                        {
                            if(GUILayout.Button("Package Homepage"))
                            {
                                Application.OpenURL(mDetailData.BaseInfo.repoUrl);
                            }
                        }

                        GUILayout.FlexibleSpace();
                        EditorGUILayout.EndVertical();
                    }
                    

                    EditorGUILayout.EndHorizontal();
                }
            }

        }

        private void RefreshData(int toolbarIndex)
        {
            mDetailData = null;
            var listModel = SetupUtil.GetPackageList();
            mListByIndex = toolbarIndex;

            this.mDataState = EDataState.Loading;
            switch(toolbarIndex)
            {
                case 0: //Main
                    SetupUtil.GetPackages_Main(ref listModel, data =>
                    {
                        this.mDataState = EDataState.Ready;
                        mListByIndex = toolbarIndex;
                        this.mListData = data;
                    });
                    break;
                case 1:
                    SetupUtil.GetPackages_Installed(ref listModel, data =>
                    {
                        this.mDataState = EDataState.Ready;
                        mListByIndex = toolbarIndex;
                        this.mListData = data;
                    });
                    break;
                case 2:
                    SetupUtil.GetPackages_Thirdparty(ref listModel, data =>
                    {
                        this.mDataState = EDataState.Ready;
                        mListByIndex = toolbarIndex;
                        this.mListData = data;
                    });
                    break;
                case 3:
                    SetupUtil.GetPackages_All(ref listModel, data =>
                    {
                        this.mDataState = EDataState.Ready;
                        mListByIndex = toolbarIndex;
                        this.mListData = data;
                    });
                    break;
            }

        }


        private void OnDestroy()
        {
            _wnd = null;
        }


        private static class Styles
        {
            private static GUIStyle _body;
            public static GUIStyle body
            {
                get
                {
                    if(_body== null)
                    {
                        _body = new GUIStyle();
                        _body.padding.left = 10;
                        _body.padding.right = 10;
                        _body.padding.top = 10;
                    }
                    return _body;
                }
            }

            private static GUIStyle _normal_label;
            public static GUIStyle normal_label
            {
                get
                {
                    if (_normal_label == null)
                    {
                        _normal_label = new GUIStyle(EditorStyles.label);
                        _normal_label.fontSize++;
                    }
                    return _normal_label;
                }
            }

            private static GUIStyle _title_label;
            public static GUIStyle title_label
            {
                get
                {
                    if (_title_label == null)
                    {
                        _title_label = new GUIStyle(EditorStyles.label);
                        _title_label.fontSize = 16;
                        _title_label.fontStyle = FontStyle.Bold;

                    }
                    return _title_label;
                }
            }

            private static GUIStyle _normal_label_center;
            public static GUIStyle normal_label_center
            {
                get
                {
                    if (_normal_label_center == null)
                    {
                        _normal_label_center = new GUIStyle(EditorStyles.label);
                        _normal_label_center.fontSize++;
                        _normal_label_center.alignment = TextAnchor.UpperCenter;
                    }
                    return _normal_label_center;
                }
            }

            private static GUIStyle _package_list;
            public static GUIStyle package_list
            {
                get
                {
                    if(_package_list == null)
                    {
                        _package_list = new GUIStyle();
                        _package_list.padding.left = 10;
                    }
                    return _package_list;
                }
            }

        }


        private static class I18Ns
        {
            private static bool? _isHans;
            private static bool IsHans
            {
                get
                {
                    if (_isHans == null)
                    {
                        _isHans = (Application.systemLanguage == SystemLanguage.Chinese || Application.systemLanguage == SystemLanguage.ChineseSimplified || Application.systemLanguage == SystemLanguage.ChineseTraditional);
                    }
                    return _isHans.Value;
                }
            }

            private static bool? _nihongo_desuka;
            private static bool NihongoDesuka
            {
                get
                {
                    if (_nihongo_desuka == null)
                        _nihongo_desuka = (Application.systemLanguage == SystemLanguage.Japanese);
                    return _nihongo_desuka.Value;
                }
            }

            private static GUIContent[] _toolbar_contents;
            public static GUIContent[] mToolbar_contents
            {
                get
                {
                    if(_toolbar_contents == null)
                    {
                        if (IsHans)
                        {
                            _toolbar_contents = new GUIContent[]
                            {
                                new GUIContent("主要包","适用于当前工程"),
                                new GUIContent("已安装",""),
                                new GUIContent("第三方",""),
                                new GUIContent("全部")
                            };
                        }
                        else
                        {
                            _toolbar_contents = new GUIContent[]
                            {
                                new GUIContent("Main","Applicable to current projects"),
                                new GUIContent("Installed",""),
                                new GUIContent("Third-party",""),
                                new GUIContent("All")
                            };
                        }
                    }
                    return _toolbar_contents;
                }
            }

            public static string LoadingList
            {
                get
                {
                    if(IsHans)
                        return "正在刷新列表";
                    if (NihongoDesuka)
                        return "リストの更新中";
                    return "Loading...";
                }
            }

            public static string DoData
            {
                get
                {
                    if (IsHans)
                        return "没有数据";
                    if (NihongoDesuka)
                        return "データはありません。";
                    return "No data.";
                }
            }
        }
    }
}

