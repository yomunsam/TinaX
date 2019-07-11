using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TinaXGameKit.UITools
{
    /// <summary>
    /// @desc:       翻页容器高亮下标。创建一个panel，添加toggle group，添加hor layout group, 在panel添加一个
    ///              Toggle,并为Toggle添加layout element勾选preferred属性；
    /// @author:     Rambo
    /// </summary>
    public class PageViewMark : MonoBehaviour
    {
        public ToggleGroup toggleGroup; //图标组
        public Toggle togglePrefab;     //图标
        public XPageView pageView;
        public int count = 1;

        private List<Toggle> toggleList = new List<Toggle>();

        /// <summary>
        /// pageview回调
        /// </summary>
        /// <param name="currentPageIndex">当前page页面下标</param>
        private void OnScrollPageChanged(int currentPageIndex)
        {
            if (currentPageIndex >= 0)
            {
                toggleList[currentPageIndex].isOn = true;
            }
        }

        /// <summary>
        /// 创建图标
        /// </summary>
        /// <returns></returns>
        private Toggle CreateToggle()
        {
            Toggle t = GameObject.Instantiate<Toggle>(togglePrefab);
            t.gameObject.SetActive(true);
            t.transform.SetParent(toggleGroup.transform);
            t.transform.localScale = Vector3.one;
            t.transform.localPosition = Vector3.zero;
            return t;
        }


        // Use this for initialization
        void Start()
        {
            for (int i = 0; i < count; i++)
            {
                toggleList.Add(CreateToggle());
            }

            pageView.OnPageChanged = OnScrollPageChanged;
            OnScrollPageChanged(0);
        }

        // Update is called once per frame
        void Update()
        {

        }
    }

}

