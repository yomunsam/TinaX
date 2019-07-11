using UnityEngine;

namespace TinaX.VFS
{
    /// <summary>
    /// 更新页面的文字
    /// </summary>
    public static class UpdateScreenString
    {

        /// <summary>
        /// 检查更新
        /// </summary>
        public static string str_check_update
        {
            get
            {
                if (IsChineseLanguage())
                {
                    return "正在检查更新";
                }
                else
                {
                    return "Check Update";
                }
            }
        }

        /// <summary>
        /// 解析版本信息
        /// </summary>
        public static string str_parse_version_info
        {
            get
            {
                if (IsChineseLanguage())
                {
                    return "解析更新数据";
                }
                else
                {
                    return "Parse Update";
                }
            }
        }


        /// <summary>
        /// 更新失败
        /// </summary>
        public static string str_update_failed
        {
            get
            {
                if (IsChineseLanguage())
                {
                    return "更新失败";
                }
                else
                {
                    return "Update Failed.";
                }
            }
        }

        /// <summary>
        /// 确定按钮
        /// </summary>
        public static string str_btn_confirm
        {
            get
            {
                if (IsChineseLanguage())
                {
                    return "确定";
                }
                else
                {
                    return "Confirm";
                }
            }
        }


        private static bool mGetLanguaged = false;
        private static bool mIsChineseLang;

        private static bool IsChineseLanguage()
        {
            if (mGetLanguaged)
            {
                return mIsChineseLang;
            }
            else
            {
                if(Application.systemLanguage == SystemLanguage.Chinese || Application.systemLanguage == SystemLanguage.ChineseSimplified || Application.systemLanguage == SystemLanguage.ChineseTraditional)
                {
                    mIsChineseLang = true;
                }
                else
                {
                    mIsChineseLang = false;
                }
                mGetLanguaged = true;
                return mIsChineseLang;
            }
        }
    }
}
