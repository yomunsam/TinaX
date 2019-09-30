using System.Collections.Generic;

/*
 * 层级号分配规则
 * 
 * 每个UI占用10个layer数，
 * 
 * layer本体占用在10的整数倍上，并向前后各预留5个数位
 * 
 * 如，第一个UI，本体占用layer为10，向前后预留5位，则6到15号都属于该UI占用的layer index数。
 * 
 * 特殊分配：
 * 
 * 【UI遮罩】UI遮罩位于每个UI的下方，占用Layer为 UI本体 layer index - 2 
 * 
 */

namespace TinaX.UIKits
{
    /// <summary>
    /// UI层级管理器
    /// </summary>
    internal class UILayerManager
    {
        private UIManager mBaseMgr;
        private List<UIEntity> mUIPool = new List<UIEntity>();

#pragma warning disable IDE0051 // 删除未使用的私有成员
        private int CurMaxLayerIndex
#pragma warning restore IDE0051 // 删除未使用的私有成员
        {
            get
            {
                var index = 0;
                foreach(var item in mUIPool)
                {
                    if (item.LayerIndex > index)
                        index = item.LayerIndex;
                }
                return index;
            }
        }

        public UILayerManager(UIManager basemgr)
        {
            mBaseMgr = basemgr;
        }


        /// <summary>
        /// 为一个新的UI获取层级
        /// </summary>
        /// <returns></returns>
        public int GetNewLayerIndex(UIEntity entity)
        {
            mUIPool.Add(entity);

            //分配UI layer
            int cur_layer = mUIPool.Count * 10 + 10;
            entity.UICanvas.overrideSorting = true;
            entity.UICanvas.sortingOrder = cur_layer;



            return cur_layer;
        }


        public void OnUIClose(UIEntity entity)
        {
            //倒序查找
            for(var i = mUIPool.Count -1; i >= 0; i--)
            {
                if(mUIPool[i].ID != entity.ID)
                {
                    //只要能执行到这儿，说明这个不是最顶层的层级，那么把它上面的层级挨个缩小10
                    mUIPool[i].LayerIndex = mUIPool[i].LayerIndex - 10;
                }
                else
                {
                    mUIPool.RemoveAt(i);
                    break;
                }
            }
        }

        /// <summary>
        /// 置顶UI
        /// </summary>
        /// <param name=""></param>
        public void SetUITop(UIEntity entity)
        {
            if(entity.ID == mUIPool[mUIPool.Count - 1].ID)
            {
                //本来就是置顶的，不需要手动操作。
                return;
                //欸，发现了没，这个管理器里面只认准那个List的次序，这意味着：没事不要自己手动瞎几把调order, 如果要调的话，在关闭之前自己再手动调回去。
            }
            int cur_max_layer_index = mUIPool[mUIPool.Count - 1].LayerIndex;
            for(var i = mUIPool.Count - 1; i >= 0; i--)
            {
                if(mUIPool[i].ID != entity.ID)
                {
                    //把这个UI上面的UI层级依次减10
                    mUIPool[i].LayerIndex = mUIPool[i].LayerIndex - 10;
                }
                else
                {
                    entity.LayerIndex = cur_max_layer_index;
                    //把这个ui entity放在list 的最后面
                    var temp = mUIPool[mUIPool.Count - 1];
                    mUIPool[mUIPool.Count - 1] = mUIPool[i];
                    mUIPool[i] = temp;
                    break;
                }
            }
        }

    }
}
