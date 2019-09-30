#if UNITY_EDITOR && ODIN_INSPECTOR
using System;
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;


namespace TinaX.UIKits
{
    public class UIEntityInjectEditor : OdinEditorWindow
    {
        [TabGroup("Inject Object")]
        [TableList]
        [Title("注入对象编辑")]
        [OnValueChanged("OnInjectionChanged", true)]
        public UIEntity.Injection[] b_injection;

        [TabGroup("Inject Data")]
        [Title("注入数据编辑")]
        public UIEntity.Injection_Data[] b_injection_str;

        private UIEntity mTarget;

        protected override void OnEnable()
        {
            base.OnEnable();
            this.titleContent = new GUIContent("Inject Editor");
            mTarget = Selection.activeGameObject.GetComponent<UIEntity>();

            if (mTarget == null)
            {
                EditorUtility.DisplayDialog("Error", "编辑器初始化失败", "好");
                return;
            }
            this.UseScrollView = true;
            b_injection = mTarget.Injections;
            b_injection_str = mTarget.Injections_Data;

        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (mTarget != null)
            {
                mTarget.Injections = b_injection;
                mTarget.Injections_Data = b_injection_str;

                EditorUtility.SetDirty(mTarget.gameObject);
                EditorUtility.SetDirty(mTarget);


            }

        }

        private void OnInjectionChanged()
        {
            foreach (var item in b_injection)
            {
                if (item.Object != null && string.IsNullOrEmpty(item.Name))
                {
                    //Debug.Log("新拖上来一个东西：" + item.Object.name + " | " + item.Object.GetType().Name);
                    var typeName = item.Object.GetType().Name;
                    var objectName = item.Object.name;

                    void HandlePrefix(string prefix)
                    {
                        if (!objectName.StartsWith(prefix))
                        {
                            item.Name = prefix + objectName;
                        }
                        else
                        {
                            item.Name = objectName;
                        }
                    }

                    switch (typeName)
                    {
                        default:
                            item.Name = objectName;
                            break;
                        case "GameObject":
                            HandlePrefix("go_");
                            break;
                        case "Text":
                            HandlePrefix("txt_");
                            break;
                        case "XText":
                            HandlePrefix("txt_");
                            break;
                        case "Image":
                            HandlePrefix("img_");
                            break;
                        case "XImage":
                            HandlePrefix("img_");
                            break;
                        case "Button":
                            HandlePrefix("btn_");
                            break;
                        case "XButton":
                            HandlePrefix("btn_");
                            break;
                    }
                }
            }
        }
    }




}




#endif