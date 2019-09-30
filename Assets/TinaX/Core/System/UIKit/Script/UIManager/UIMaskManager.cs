using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace TinaX.UIKits
{
    /// <summary>
    /// UI遮罩背景管理器
    /// </summary>
    internal class UIMaskManager
    {
        private GameObject mGoMask;
        private Canvas mMaskCanvas;
        private UIManager mBaseMgr;

        private List<UIEntity> mMaskedUIPool = new List<UIEntity>();

        private int CurMaxLayerIndex
        {
            get
            {
                var index = 0;
                foreach (var item in mMaskedUIPool)
                {
                    if (item.LayerIndex > index)
                        index = item.LayerIndex;
                }
                return index;
            }
        }


        public UIMaskManager(UIManager mgr)
        {
            mBaseMgr = mgr;

            //初始化遮罩UGUI
            mGoMask = mgr.UIRootGameObject.FindOrCreateGameObject("UIMask");
            mMaskCanvas = mGoMask.GetComponentOrAdd<Canvas>();
            mMaskCanvas.overrideSorting = true;
            mMaskCanvas.sortingOrder = 0;

            var mask_gr = mGoMask.GetComponentOrAdd<GraphicRaycaster>();
            mask_gr.ignoreReversedGraphics = true;

            var rect_mask_trans = mGoMask.GetComponent<RectTransform>();
            rect_mask_trans.anchorMin = Vector2.zero;
            rect_mask_trans.anchorMax = Vector2.one;
            rect_mask_trans.anchoredPosition3D = Vector3.zero;
            rect_mask_trans.localScale = Vector3.one;
            rect_mask_trans.sizeDelta = Vector2.zero;

            var go_img_mask = mGoMask.FindOrCreateGo("Mask");
            var img_mask = go_img_mask.GetComponentOrAdd<XImage>();
            img_mask.color = mgr.UIConfig.MaskColor;


            var rect_img = go_img_mask.GetComponent<RectTransform>();
            rect_img.anchorMin = Vector2.zero;
            rect_img.anchorMax = Vector2.one;
            rect_img.anchoredPosition3D = Vector3.zero;
            rect_img.localScale = Vector3.one;
            rect_img.sizeDelta = Vector2.zero;


            go_img_mask.GetComponentOrAdd<Button>().onClick.AddListener(OnMaskClicked);

            mGoMask.SetLayerRecursive(5)
                .Hide();

        }


        public void UseMask(UIEntity entity,bool closeByClickMask = false)
        {
            entity.EnableCloseByMaskClick = closeByClickMask;
            //登记
            mMaskedUIPool.Add(entity);

            mGoMask.Show();
            mMaskCanvas.sortingOrder = entity.LayerIndex - 2;

            //
        }

        /// <summary>
        /// 移除遮罩
        /// </summary>
        public void RemoveMask(UIEntity entity ) // 因为只有这里记录了一个UI是否使用遮罩的信息，所以所有UI关闭的时候都会过来调用一遍，这里得排除没有用遮罩的UI
        {
            lock (this)
            {
                if (mMaskedUIPool.Any(ui => ui.ID == entity.ID))
                {
                    //指定的UI有用到遮罩

                    //当前遮罩是否显示在这层UI上？
                    if (entity.LayerIndex == CurMaxLayerIndex)
                    {
                        mMaskedUIPool.Remove(entity);
                        //是的了
                        if (mMaskedUIPool.Count > 0)
                        {
                            //除了我之外还有别的使用了遮罩的UI
                            mGoMask.Show();
                            mMaskCanvas.sortingOrder = CurMaxLayerIndex - 2;
                        }
                        else
                        {
                            mGoMask.Hide();
                        }
                    }
                    else
                    {
                        //不是，不用管太多
                        mMaskedUIPool.Remove(entity);
                        //mGoMask.Hide();
                    }

                }
                else
                {
                    //没有，大概是不用管的
                }
            }
            
        }

        /// <summary>
        /// 查询某个UI使用使用了遮罩
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool IsUIUsedMask(ulong id)
        {
            return mMaskedUIPool.Any(ui => ui.ID == id);
        }

        /// <summary>
        /// 直接无脑Hide，其他的别管
        /// </summary>
        public void HideMask()
        {
            mGoMask.Hide();
        }

        private void OnMaskClicked()
        {
            if(mMaskedUIPool.Count > 0)
            {
                var cur_ui_entity = mMaskedUIPool[mMaskedUIPool.Count - 1];
                if (cur_ui_entity.EnableCloseByMaskClick)
                {
                    cur_ui_entity.Close();
                }
            }
        }

    }
}
