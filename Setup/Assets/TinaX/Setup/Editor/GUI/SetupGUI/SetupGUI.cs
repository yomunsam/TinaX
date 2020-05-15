using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace TinaXEditor.Setup.Internal
{
    public class SetupGUI : EditorWindow
    {
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

        private int? mListByIndex;
        private List<PackageListModel.ListItem> ListData;

        private void OnGUI()
        {
            EditorGUILayout.BeginHorizontal(Styles.body);
            mToolbarIndex = GUILayout.Toolbar(mToolbarIndex, I18Ns.mToolbar_contents);

            //package list
            DrawPackageList();

            EditorGUILayout.EndHorizontal();
        }


        private void DrawPackageList()
        {

        }

        private void RefreshData(int toolbarIndex)
        {
            var listModel = SetupUtil.GetPackageList();
            mListByIndex = toolbarIndex;

            switch(toolbarIndex)
            {
                case 0:
                    //ListData = SetupUtil.GetPackages_Main(ref listModel);
                    break;
                case 1:
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
        }
    }
}

