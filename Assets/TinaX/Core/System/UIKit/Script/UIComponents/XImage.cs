using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TinaX;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TinaX.UIKit
{
    [DisallowMultipleComponent]
    [AddComponentMenu("TinaX/UIKit/xImage")]
    public class XImage : Image
    {
#if UNITY_EDITOR
        [MenuItem("GameObject/UI/xImage", false, 11)]
        static void AddxText()
        {
            if (Selection.activeTransform != null)
            {
                var go_name = "xImage";
                if (Selection.activeTransform.Find(go_name) != null)
                {
                    var index = 1;

                    while (Selection.activeTransform.Find("xImage" + index.ToString()) != null)
                    {
                        index++;
                    }
                    go_name = "xImage" + index.ToString();
                }
                var TextGo = new GameObject(go_name).SetLayerRecursive(5);
                var xImage = TextGo.AddComponent<XImage>();
                TextGo.transform.SetParent(Selection.activeTransform);
                var rect_trans = TextGo.GetComponent<RectTransform>();
                rect_trans.anchoredPosition3D = Vector2.zero;
                rect_trans.localScale = Vector3.one;
                rect_trans.sizeDelta = new Vector2(100, 100);

                Selection.activeTransform = TextGo.transform;
            }
        }
#endif

        //[HideInInspector]
        private string mResPath;

        /// <summary>
        /// 设置Image图片资源的路径[by 资源管理系统]
        /// </summary>
        /// <param name="res_path">寻址路径</param>
        /// <param name="async">是否异步加载</param>
        public void SetResPath(string res_path,bool async = false)
        {
            //检查当前是否已有资源
            if (!mResPath.IsNullOrEmpty())
            {
                AssetsMgr.I.RemoveUse(mResPath);
            }

            if (async)
            {
                //异步
                AssetsMgr.I.LoadAssetAsync<Sprite>(res_path, (s) => {
                    sprite = s;
                });
            }
            else
            {
                sprite = AssetsMgr.I.LoadAsset<Sprite>(res_path);
            }

            mResPath = res_path;
        }


        protected override void OnDestroy()
        {
            base.OnDestroy();

            //释放资源
            if (!mResPath.IsNullOrEmpty())
            {
                AssetsMgr.I.RemoveUse(mResPath);
                mResPath = null;
            }
        }
    }
}

