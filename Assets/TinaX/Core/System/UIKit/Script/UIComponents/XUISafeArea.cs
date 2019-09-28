using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TinaX.UIKits
{
    [DisallowMultipleComponent]
    [AddComponentMenu("TinaX/UIKit/xUISafeArea")]
    public class XUISafeArea : MonoBehaviour
    {
#if UNITY_EDITOR
        [MenuItem("GameObject/UI/xUISafeArea", false, 12)]
        static void AddxUISafeArea()
        {
            var go_name = "xUISafeArea";
            if (Selection.activeTransform.Find(go_name) != null)
            {
                var index = 1;

                while (Selection.activeTransform.Find("xUISafeArea" + index.ToString()) != null)
                {
                    index++;
                }
                go_name = "xUISafeArea" + index.ToString();
            }
            if (Selection.activeTransform != null)
            {
                
                var SafeAreaGo = new GameObject(go_name).SetLayerRecursive(5);

                var xSarea = SafeAreaGo.AddComponent<XUISafeArea>();

                SafeAreaGo.transform.SetParent(Selection.activeTransform);
                var rect_trans = SafeAreaGo.GetComponentOrAdd<RectTransform>();
                rect_trans.anchoredPosition3D = Vector3.zero;
                rect_trans.localScale = Vector3.one;
                rect_trans.anchorMin = Vector2.zero;
                rect_trans.anchorMax = Vector2.one;
                rect_trans.sizeDelta = Vector2.zero;

            }
            
        }
#endif

        [Header("背景图")]
        public RectTransform BackGround;

        [Header("缩放背景")]
        public bool ScaleBackground = false;
        [Header("背景原始横纵比")]
        public Vector2Int BgAspectRationDefine;

        private void Awake()
        {
            //检查UISafeArea
            //UIKit.I.UISafeAreaManager.GetOffsetInfo();

            //背景图ScaleBg
            ScaleBg();
        }


        private void ScaleBg()
        {
            if (BackGround == null) return;
            if (BgAspectRationDefine.x == 0 || BgAspectRationDefine.y == 0) return;

            float screenAspectRation = (float)Screen.width / Screen.height;
            float bgAspectRation = (float)BgAspectRationDefine.x / BgAspectRationDefine.y;


            BackGround.anchorMin = new Vector2(0.5f, 0.5f);
            BackGround.anchorMax = new Vector2(0.5f, 0.5f);
            BackGround.anchoredPosition = Vector2.zero;



            var ui_root_rect = UIKit.I.UIKit_UIRoot_RectTrans;
            var cur_ui_size = ui_root_rect.sizeDelta;

            if (screenAspectRation > bgAspectRation)
            {
                //屏幕比背景图要宽，优先用背景图宽度填满屏幕
                BackGround.sizeDelta = new Vector2(cur_ui_size.x, cur_ui_size.x / bgAspectRation);
            }
            else
            {
                //屏幕比背景图要狭窄，优先用背景图高度填满拼命
                BackGround.sizeDelta = new Vector2(cur_ui_size.y * bgAspectRation, cur_ui_size.y);

            }



        }

    }
}