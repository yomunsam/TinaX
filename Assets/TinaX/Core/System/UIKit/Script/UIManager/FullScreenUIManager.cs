using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Threading.Tasks;

namespace TinaX.UIKits
{
    /// <summary>
    /// 全屏UI调度管理器
    /// </summary>
    internal class FullScreenUIManager
    {
        private UIManager mBaseMgr;
        private List<UIEntity> mFSUIPool = new List<UIEntity>();
        private UIEntity mCurActiveFullScreenUI;

        public FullScreenUIManager(UIManager mgr)
        {
            mBaseMgr = mgr;
        }


        public void OnUIOpen(UIEntity entity)
        {
            //1. 检查是否要做全屏处理
            if (entity.IsFullScreenUI)
            {
                //是全屏UI，先给加入进列表
                if(!mFSUIPool.Any(e => e.ID == entity.ID))
                {
                    mFSUIPool.Add(entity);
                }

                //判断是否要顶替之前的UI
                if(mFSUIPool.Count > 1 && mCurActiveFullScreenUI != null)
                {
                    bool ignore = false;
                    if(entity.FullScreenIgnorePath.Contains(mCurActiveFullScreenUI.UIPath))
                    {
                        ignore = true;
                    }

                    if (!ignore && !mCurActiveFullScreenUI.UIName.IsNullOrEmpty())
                    {
                        if (entity.FullScreenIgnoreUIName.Contains(mCurActiveFullScreenUI.UIName))
                            ignore = true;
                    }

                    if (!ignore)
                    {
                        //顶替
                        MoveUI_ByIndex(mCurActiveFullScreenUI, mFSUIPool.Count + 1,true);
                        //登记
                        mCurActiveFullScreenUI = entity;
                    }


                }
            }
        }

        public void OnUIClose(UIEntity entity) //同样，所有UI都会调用过来一次，这里里面得处理好判断
        {
            if (mFSUIPool.Contains(entity))
            {
                mFSUIPool.Remove(entity);

                if (mCurActiveFullScreenUI != null && mCurActiveFullScreenUI.ID == entity.ID)
                {
                    //将被关掉的就是最顶级UI,找到上一个UI并放出来
                    if (mFSUIPool.Count > 0)
                    {
                        mCurActiveFullScreenUI = mFSUIPool[mFSUIPool.Count - 1];
                        ShowUIIfHide(mCurActiveFullScreenUI, true);
                        MoveUI_ByIndex(mCurActiveFullScreenUI,0, true);

                    }
                    else
                    {
                        mCurActiveFullScreenUI = null;
                    }
                }
            }
            
        }

        private void HideUIIfAllow(UIEntity entity , bool includeChild = true)
        {
            if (!entity.DontPauseWhenHide)
            {
                entity.gameObject.Hide();
            }

            if (includeChild && entity.childCount > 0)  //递龟（划掉   。递归
            {
                for (var i = 0; i < entity.childCount; i++)
                {
                    var _entity = mBaseMgr.GetUIEntityByID(entity.GetChildID(i));
                    if (_entity != null)
                    {
                        HideUIIfAllow(_entity, true);
                    }
                }
            }

        }

        private void ShowUIIfHide(UIEntity entity , bool includeChild = true)
        {
            entity.gameObject.Show();

            if (includeChild && entity.childCount > 0) 
            {
                for (var i = 0; i < entity.childCount; i++)
                {
                    var _entity = mBaseMgr.GetUIEntityByID(entity.GetChildID(i));
                    if (_entity != null)
                    {
                        ShowUIIfHide(_entity, true);
                    }
                }
            }
        }

        private void MoveUI_ByIndex(UIEntity entity,int index , bool includeChild = true)
        {
            var rect_trans = entity.gameObject.GetComponent<RectTransform>();
            if(index != 0)
            {
                rect_trans.anchoredPosition = new Vector2(rect_trans.anchoredPosition.x, rect_trans.anchoredPosition.y + (mBaseMgr.UIRootRectTransform.rect.size.y * index) + 100);
            }
            else
            {
                rect_trans.anchoredPosition = Vector2.zero;
            }

            if (includeChild && entity.childCount > 0)  //递龟（划掉   。递归
            {
                for(var i = 0; i < entity.childCount; i++)
                {
                    var _entity = mBaseMgr.GetUIEntityByID(entity.GetChildID(i));
                    if(_entity != null)
                    {
                        MoveUI_ByIndex(_entity, index, true);
                    }
                }
            }
        }
    }
}
