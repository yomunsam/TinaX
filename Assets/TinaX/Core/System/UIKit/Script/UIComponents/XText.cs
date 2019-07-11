using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TinaX.UIKit
{
    [DisallowMultipleComponent]
    [AddComponentMenu("TinaX/UIKit/xText")]
    public class XText : Text
    {

#if UNITY_EDITOR
        [MenuItem("GameObject/UI/xText",false,10)]
        static void AddxText()
        {
            if (Selection.activeTransform != null)
            {
                var go_name = "xText";
                if (Selection.activeTransform.Find(go_name) != null)
                {
                    var index = 1;
                    
                    while (Selection.activeTransform.Find("xText" + index.ToString()) != null)
                    {
                        index++;
                    }
                    go_name = "xText" + index.ToString();
                }
                var TextGo = new GameObject(go_name).SetLayerRecursive(5);
                var xText = TextGo.AddComponent<XText>();
                TextGo.transform.SetParent(Selection.activeTransform);
                var rect_trans = TextGo.GetComponent<RectTransform>();
                rect_trans.anchoredPosition = Vector2.zero;
                rect_trans.localScale = Vector3.one;
                rect_trans.sizeDelta = new Vector2(200, 60);
                xText.text = "New xText";
                xText.fontSize = 20;
                xText.raycastTarget = false;
                Selection.activeTransform = TextGo.transform;
            }
        }
#endif

        /// <summary>
        /// 是否启用I18N
        /// </summary>
        public bool UseI18N = false;
        /// <summary>
        /// I18N的Key值
        /// </summary>
        public string I18NKey;

        public bool UseI18NInRumtime;




        protected override void Awake()
        {
            base.Awake();
#if UNITY_EDITOR
            if (Application.isPlaying)
            {
                if (UseI18N && UseI18NInRumtime && I18NKey.Length > 0)
                {
                    text = XI18N.I.GetString(I18NKey);
                }
            }
            
#else
            if (UseI18N && UseI18NInRumtime && I18NKey.Length >0)
            {
                text = XI18N.I.GetString(I18NKey);
            }
#endif

        }


        /// <summary>
        /// 通过I18N key设置文字
        /// </summary>
        /// <param name="key">key值</param>
        /// <param name="default_str">如果key不存在，使用此处缺省</param>
        public void SetI18NKey(string key,string default_str = null)
        {
            var i18n_str = XI18N.I.GetString(key);
            if (i18n_str == key && default_str != null)
            {
                text = default_str;
            }
            else{
                text = i18n_str;    
            }
        }

    }
}

