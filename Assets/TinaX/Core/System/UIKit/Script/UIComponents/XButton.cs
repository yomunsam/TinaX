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
    [AddComponentMenu("TinaX/UIKit/xButton")]
    public class XButton : Button
    {
#if UNITY_EDITOR
        [MenuItem("GameObject/UI/xButton", false, 12)]
        static void AddxText()
        {
            if (Selection.activeTransform != null)
            {
                var go_name = "xButton";
                if (Selection.activeTransform.Find(go_name) != null)
                {
                    var index = 1;

                    while (Selection.activeTransform.Find("xButton" + index.ToString()) != null)
                    {
                        index++;
                    }
                    go_name = "xButton" + index.ToString();
                }
                var BtnGo = new GameObject(go_name).SetLayerRecursive(5);
                var xImage = BtnGo.AddComponent<XImage>();
                var xButton = BtnGo.AddComponent<XButton>();
                BtnGo.transform.SetParent(Selection.activeTransform);
                var rect_trans = BtnGo.GetComponent<RectTransform>();
                rect_trans.anchoredPosition = Vector2.zero;
                rect_trans.localScale = Vector3.one;
                rect_trans.sizeDelta = new Vector2(200, 60);
                xImage.color = new Color(55f/255f, 55f/255f, 55f/255f);
                var GoText = new GameObject("Text").SetLayerRecursive(5);
                var xText = GoText.AddComponent<XText>();
                GoText.transform.SetParent(BtnGo.transform);
                var rect_txt = GoText.GetComponent<RectTransform>();
                
                rect_txt.anchorMin = Vector2.zero;
                rect_txt.anchorMax = Vector2.one;
                rect_txt.anchoredPosition = Vector2.zero;
                rect_txt.sizeDelta = Vector2.zero;
                xText.raycastTarget = false;
                xText.alignment = TextAnchor.MiddleCenter;
                xText.fontSize = 20;
                xText.color = Color.white;
                xText.text = "xButton";
            }
        }
#endif

    }
}

