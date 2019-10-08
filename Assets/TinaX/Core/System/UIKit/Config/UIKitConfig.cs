using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR && ODIN_INSPECTOR
using Sirenix.OdinInspector;
//using CatLib; //我也忘了这里为什么会引用CatLib了，先给注释掉看看会怎么样
#endif

namespace TinaX.UIKits
{
    public class UIKitConfig : ScriptableObject
    {

        #region 一些常规的设置

#if UNITY_EDITOR && ODIN_INSPECTOR
        [FoldoutGroup("UI系统")]
        [Header("默认UI组")]
#endif
        public UIGroupConf Default_UIGroup;


        #endregion


        #region 关于UI的绘制与缩放

#if UNITY_EDITOR && ODIN_INSPECTOR
        [FoldoutGroup("UI绘制")]
        [Header("缩放模式")]
#endif
        public CanvasScaler.ScaleMode Canvas_Scale_Mode = CanvasScaler.ScaleMode.ConstantPixelSize;

#if UNITY_EDITOR && ODIN_INSPECTOR
        private bool can_show_uisize()
        {
            if(Canvas_Scale_Mode == CanvasScaler.ScaleMode.ScaleWithScreenSize)
            {
                return true;
            }
            else { return false; }
        }
        [FoldoutGroup("UI绘制")]
        [Header("UI像素尺寸")]
        [ShowIf("can_show_uisize")]
#endif
        public Vector2 UISize = new Vector2Int(1920, 1080);


#if UNITY_EDITOR && ODIN_INSPECTOR
        [FoldoutGroup("UI绘制")]
        [Header("完美像素")]
#endif
        public bool PixelPerfect = false;

#if UNITY_EDITOR && ODIN_INSPECTOR
        [FoldoutGroup("UI绘制")]
        [Header("权重模式")]
        [ShowIf("can_show_uisize")]

#endif
        public CanvasScaler.ScreenMatchMode ScreenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;

#if UNITY_EDITOR && ODIN_INSPECTOR
        private bool can_show_match()
        {
            if (Canvas_Scale_Mode == CanvasScaler.ScaleMode.ScaleWithScreenSize && ScreenMatchMode == CanvasScaler.ScreenMatchMode.MatchWidthOrHeight)
            {
                return true;
            }
            else { return false; }
        }

        private Color GetProgressBarColor()
        {
            return TinaX.Core.XEditorStyleDefine.Color_Blue;
        }

        [FoldoutGroup("UI绘制")]
        [Header("缩放权重")]
        [ShowIf("can_show_match")]
        [MinValue(0)]
        [MaxValue(1)]
        [ProgressBar(0,1,Height = 25,ColorMember = "GetProgressBarColor")]
        //[CustomValueDrawer("Match | 权重")]
        [InfoBox("宽度：0 | 高度：1")]
#endif
        public float Match = 1;

#if UNITY_EDITOR && ODIN_INSPECTOR
        private bool can_if_constantPixelSize()
        {
            if (Canvas_Scale_Mode == CanvasScaler.ScaleMode.ConstantPixelSize)
            {
                return true;
            }
            else { return false; }
        }
        [FoldoutGroup("UI绘制")]

        [ShowIf("can_if_constantPixelSize")]
#endif
        public float ScaleFactor = 1;

#if UNITY_EDITOR && ODIN_INSPECTOR
        private bool show_if_phy()
        {
            if (Canvas_Scale_Mode == CanvasScaler.ScaleMode.ConstantPixelSize)
            {
                return true;
            }
            else { return false; }
        }
        [FoldoutGroup("UI绘制")]
        [ShowIf("show_if_phy")]
#endif
        public CanvasScaler.Unit physicalUnit = CanvasScaler.Unit.Points;


#if UNITY_EDITOR && ODIN_INSPECTOR
        [FoldoutGroup("UI绘制")]
#endif
        public float referencePixelsPerUnit = 100;

#if UNITY_EDITOR && ODIN_INSPECTOR
        [FoldoutGroup("UI绘制")]
        [Header("遮罩背景颜色")]
#endif
        public Color MaskColor = new Color(0, 0, 0, 0.5f);

        #endregion


        #region 导入和预处理

#if UNITY_EDITOR && ODIN_INSPECTOR
        [FoldoutGroup("资源导入")]
        [Header("UI散图根目录")]
        [FolderPath]
#endif
        public string UI_Img_Path;
#if UNITY_EDITOR && ODIN_INSPECTOR
        
        [FoldoutGroup("资源导入")]
        [Header("UI图集根目录")]
        [FolderPath]
        [InfoBox("注意,请将该目录配置进资源管理配置中的特殊路径列表，类型为Sub_dir")]
        //[InlineButton("AddFolderToAssetConf","帮我加入配置")]
#endif
        public string UI_Atlas_Path;


        #endregion


        #region UI安全区
#if UNITY_EDITOR && ODIN_INSPECTOR
        [FoldoutGroup("UI安全区")]
        [Header("启用UI安全区")]
#endif
        public bool Enable_UISafeArea;

#if UNITY_EDITOR && ODIN_INSPECTOR
        [FoldoutGroup("UI安全区")]
        [Header("UI安全区配置数据JSON")]
        [FilePath]
#endif
        public string UI_SafeArea_Json;

#if UNITY_EDITOR && ODIN_INSPECTOR
        [FoldoutGroup("UI安全区")]
        [Header("使用TinaX资源系统加载JSON")]
        [InfoBox("否则将使用Resources.Load方式加载")]
#endif
        public bool UISafeArea_LoadByVFS = true;


        #endregion


    }



}

