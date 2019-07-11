using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


namespace TinaXGameKit.UITools
{

    /// <summary>
    /// @desc:      翻页容器，继承接口：IBeginDragHandler(开始拖拽),IEndDragHanler（结束拖拽）,IDragHandler(拖拽中)
    /// @author:    Rambo
    /// @use        UGUI创建Scroll View，并在Content添加组件content size fitter设置Hori fit为preferred size，
    ///             添加组件horizontal layout group，并在Content下添加翻页模板defPage，为defPage添加layout element，设置属性
    ///             preferred width和preferred height。格子翻页就为defPage添加layout group。
    /// @use        代码初始化InitPage，根据需求选择 格子LayoutGrid或者普通LayoutNorPage   
    /// </summary>
    [RequireComponent(typeof(ScrollRect))]
    [DisallowMultipleComponent]
    public class XPageView : MonoBehaviour , IBeginDragHandler,IEndDragHandler
    {
        private ScrollRect rect;                        //滑动组件
        private int currentPageIndex = -1;              //当前页下标
        private float targethorizontal = 0;             //滑动的起始坐标
        private bool isDrag = false;                    //是否拖拽结束

        private float startime = 0f;                    //开始时间 
        private float delay = 0.1f;                     //延时时间

        private int maxPageNum = 0;                     //最大页数
        private GameObject defPage;                     //默认翻页模板
        private int createPage = 0;                     //已创建页数

        private GameObject defGrid;                     //格子模板    
        private int row;                                //行
        private int column;                             //列
        private bool layoutGridItem = false;                        //是否为格子容器
        private List<GameObject> gridList = new List<GameObject>(); //格子翻页的每个格子存储

        private List<GameObject> pageList = new List<GameObject>(); //翻页容器存储
        private List<float> posList = new List<float>();            //求出每页的临界角，页索引从0开始

        /// <summary>
        /// 返回值为页面Index，-1说明没有数据，页码从0开始
        /// </summary>
        public Action<int> OnPageChanged;
        
        /// <summary>
        /// Item回调，index从0开始
        /// </summary>
        public Action<GameObject, int> CreateItemCall;

        public float smooting = 4;      //滑动速度

        #region 私有函数处理
        //重设页码坐标list
        private void UpdatePosList()
        {
            posList.Clear();
            for (int i = 0; i < createPage; i++)
            {
                float page = i / ((float)(createPage - 1));
                posList.Add(page);
            }
        }

        //是否要创建翻页容器
        private void IsCreatePage()
        {

            if (currentPageIndex + 1 == createPage)
            {
                if (layoutGridItem)
                {
                    CreateGridPage();
                }
                else
                {
                    CreateNorPage();
                }
            }

        }

        //设置当前容器所在页
        private void SetPageIndex(int index)
        {

            if (index != currentPageIndex)
            {
                currentPageIndex = index;
                if (OnPageChanged != null)
                {
                    OnPageChanged(currentPageIndex);
                }

                //StartCoroutine(DelayContinue());
                IsCreatePage();
            }

        }

        //创建翻页容器
        private void CreatePageItem()
        {
            GameObject sPage = GameObject.Instantiate<GameObject>(defPage);
            sPage.gameObject.SetActive(true);
            sPage.transform.SetParent(defPage.transform.parent);
            sPage.transform.localScale = Vector3.one;
            sPage.transform.localPosition = Vector3.zero;

            pageList.Insert(createPage, sPage);
            createPage += 1;
            UpdatePosList();
        }
        #endregion

        #region 滑动处理
        /// <summary>
        /// 开始滑动
        /// </summary>
        /// <param name="eventData"></param>
        public void OnBeginDrag(PointerEventData eventData)
        {
            isDrag = true;
        }

        /// <summary>
        /// 滑动结束
        /// </summary>
        /// <param name="eventData"></param>
        public void OnEndDrag(PointerEventData eventData)
        {

            float posX = rect.horizontalNormalizedPosition;
            int index = 0;
            //假设离第一位最近
            float offset = Mathf.Abs(posList[index] - posX);
            for (int i = 1; i < posList.Count; i++)
            {
                float temp = Mathf.Abs(posList[i] - posX);
                if (temp < offset)
                {
                    index = i;

                    //保存当前的偏移量
                    //如果到最后一页。反翻页。所以要保存该值
                    offset = temp;
                }
            }


            //因为这样效果不好。没有滑动效果。比较死板。所以改为插值
            //rect.horizontalNormalizedPosition = page[index];

            //动态创建小孩，并进行翻页通知
            SetPageIndex(index);

            targethorizontal = posList[index]; //设置当前坐标，更新函数进行插值
            isDrag = false;

        }
        #endregion

        #region 创建格子翻页
        /// <summary>
        /// 根据pos获取格子父节点GameObject
        /// </summary>
        /// <param name="pos">下标从0开始</param>
        /// <returns></returns>
        public GameObject GetGridParentByPos(int pos)
        {
            var num = row * column;
            GameObject curPage = defPage.gameObject;
            int page = Mathf.CeilToInt(pos / num);
            if (pageList.Count >= page && page >= 0)
            {
                curPage = pageList[page];

            }
            return curPage;
        }

        /// <summary>
        /// 初始格子翻页容器
        /// </summary>
        /// <param name="gridItem">格子模板</param>
        /// <param name="myRow">行</param>
        /// <param name="myColumn">列</param>
        public void LayoutGrid(GameObject gridItem, int myRow, int myColumn)
        {
            layoutGridItem = true;
            defGrid = gridItem;
            row = myRow;
            column = myColumn;

            for (int i = 0; i < 2; i++)
            {
                CreateGridPage();
            }
        }

        //创建格子翻页
        private void CreateGridPage()
        {
            if (createPage >= maxPageNum)
            {
                return;
            }
            CreatePageItem();

            var showNum = row * column;
            var sPos = (createPage - 1) * showNum;
            var endPos = createPage * showNum;

            for (int i = sPos; i < endPos; i++)
            {
                GameObject cGrid = GameObject.Instantiate<GameObject>(defGrid);
                GameObject obj = GetGridParentByPos(i);
                cGrid.SetActive(true);
                cGrid.transform.SetParent(obj.transform);
                cGrid.transform.localScale = Vector3.one;
                cGrid.transform.localPosition = Vector3.zero;

                gridList.Insert(i, cGrid);
                if (CreateItemCall != null)
                {
                    CreateItemCall(cGrid, i);
                }
            }
        }
        #endregion

        #region 创建普通翻页
        /// <summary>
        /// 初始化普通翻页
        /// </summary>
        public void LayoutNorPage()
        {
            for (int i = 0; i < 2; i++)
            {
                CreateNorPage();
            }
        }

        //创建普通翻页
        private void CreateNorPage()
        {
            if (createPage >= maxPageNum)
            {
                return;
            }
            CreatePageItem();
            if (CreateItemCall != null)
            {
                CreateItemCall(pageList[createPage - 1], createPage - 1);
            }
        }
        #endregion

        /// <summary>
        /// 初始化pageview
        /// </summary>
        /// <param name="defItem">翻页容器模板，构建时必须添加在content下，每次复制添加时是拿模板的父节点</param>
        /// <param name="maxPage">翻页容器的总页数</param>
        public void InitPage(GameObject defItem, int maxPage)
        {
            maxPageNum = maxPage;
            defPage = defItem;
        }

        // Use this for initialization
        void Start()
        {
            rect = transform.GetComponent<ScrollRect>();
            startime = Time.time;
        }

        //更新函数
        void Update()
        {
            if (Time.time < startime + delay) return;
            //如果不判断。当在拖拽的时候要也会执行插值，所以会出现闪烁的效果
            //这里只要在拖动结束的时候。在进行插值Mathf.Lerp，实现回弹效果
            if (!isDrag && posList.Count > 0)
            {
                rect.horizontalNormalizedPosition = Mathf.Lerp(rect.horizontalNormalizedPosition, targethorizontal, Time.deltaTime * smooting);

            }
        }
 

    }
}

