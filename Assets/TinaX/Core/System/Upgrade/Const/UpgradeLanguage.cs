using System;
using UnityEngine;

namespace TinaX.Upgrade
{



    /// <summary>
    /// 系统层更新的语言表
    /// </summary>
    public static class UpgradeLanguage
    {
        public static SystemLanguage Cur_lang
        {
            get
            {
                return Application.systemLanguage;
            }
        } 


        /// <summary>
        /// 检查更新
        /// </summary>
        public static string CheckUpgrade
        {
            get
            {
                if(Cur_lang == SystemLanguage.Chinese || Cur_lang == SystemLanguage.ChineseSimplified)
                {
                    return "正在检查更新";
                }
                if(Cur_lang == SystemLanguage.ChineseTraditional)
                {
                    return "正在檢查更新";
                }
                if(Cur_lang == SystemLanguage.Japanese)
                {
                    return "更新をチェックしています";
                }
                return "Checking for updates";
            }
        }

        /// <summary>
        /// 检查更新失败
        /// </summary>
        public static string CheckUpgrade_Fail
        {
            get
            {
                if (Cur_lang == SystemLanguage.Chinese || Cur_lang == SystemLanguage.ChineseSimplified)
                {
                    return "检查更新失败";
                }
                if (Cur_lang == SystemLanguage.ChineseTraditional)
                {
                    return "檢查更新失敗";
                }
                if (Cur_lang == SystemLanguage.Japanese)
                {
                    return "検査更新失败";
                }
                return "Check update failed";
            }
        }

        /// <summary>
        /// 检查更新失败,网络故障
        /// </summary>
        public static string CheckUpgrade_Fail_Connect_Lost
        {
            get
            {
                if (Cur_lang == SystemLanguage.Chinese || Cur_lang == SystemLanguage.ChineseSimplified)
                {
                    return "当前网络不稳定，无法连接到更新索引服务器";
                }
                if (Cur_lang == SystemLanguage.ChineseTraditional)
                {
                    return "當前網絡不穩定，無法連接到更新索引服務器";
                }
                if (Cur_lang == SystemLanguage.Japanese)
                {
                    return "現在のネットワークが不安定で、インデックスサーバを更新することができません。";
                }
                return "The current network is unstable and unable to connect to the update index server";
            }
        }

        /// <summary>
        /// Confirm
        /// </summary>
        public static string Confirm
        {
            get
            {
                if (Cur_lang == SystemLanguage.Chinese || Cur_lang == SystemLanguage.ChineseSimplified)
                {
                    return "确定";
                }
                if (Cur_lang == SystemLanguage.ChineseTraditional)
                {
                    return "確定";
                }
                if (Cur_lang == SystemLanguage.Japanese)
                {
                    return "確定する";
                }
                return "confirm";
            }
        }

        /// <summary>
        /// 新的版本
        /// </summary>
        public static string NewVersion
        {
            get
            {
                if (Cur_lang == SystemLanguage.Chinese || Cur_lang == SystemLanguage.ChineseSimplified)
                {
                    return "新的版本";
                }
                if (Cur_lang == SystemLanguage.ChineseTraditional)
                {
                    return "新的版本";
                }
                if (Cur_lang == SystemLanguage.Japanese)
                {
                    return "新しいバージョン";
                }
                return "new version";
            }
        }

        public static string Upgrade_With_Browser
        {
            get
            {
                if (Cur_lang == SystemLanguage.Chinese || Cur_lang == SystemLanguage.ChineseSimplified)
                {
                    return "当前App版本已不提供更新补丁，请在浏览器中下载最新的安装包。";
                }
                if (Cur_lang == SystemLanguage.ChineseTraditional)
                {
                    return "當前App版本已不提供更新補丁，請在瀏覽器中下載最新的安裝包。";
                }
                if (Cur_lang == SystemLanguage.Japanese)
                {
                    return "現在のAppバージョンは、パッチを更新していないので、ブラウザで最新のインストールバッグをダウンロードしてください。";
                }
                return "The current version of App does not provide an update patch. Please download the latest installation package in the browser.";
            }
        }


        /// <summary>
        /// 正在下载
        /// </summary>
        public static string Upgrade_Downloading
        {
            get
            {
                if (Cur_lang == SystemLanguage.Chinese || Cur_lang == SystemLanguage.ChineseSimplified)
                {
                    return "正在下载更新补丁";
                }
                if (Cur_lang == SystemLanguage.ChineseTraditional)
                {
                    return "正在下載更新";
                }
                if (Cur_lang == SystemLanguage.Japanese)
                {
                    return "ダウンロード更新中";
                }
                return "Downloading updates";
            }
        }

        /// <summary>
        /// 更新完成
        /// </summary>
        public static string Upgrade_Success
        {
            get
            {
                if (Cur_lang == SystemLanguage.Chinese || Cur_lang == SystemLanguage.ChineseSimplified)
                {
                    return "更新完成";
                }
                if (Cur_lang == SystemLanguage.ChineseTraditional)
                {
                    return "更新完成";
                }
                if (Cur_lang == SystemLanguage.Japanese)
                {
                    return "更新する";
                }
                return "Completed";
            }
        }


        /// <summary>
        /// 更新完成，请重启App
        /// </summary>
        public static string Upgrade_Success_Restart
        {
            get
            {
                if (Cur_lang == SystemLanguage.Chinese || Cur_lang == SystemLanguage.ChineseSimplified)
                {
                    return "更新完成，App将尝试自行重启";
                }
                if (Cur_lang == SystemLanguage.ChineseTraditional)
                {
                    return "更新完成，App將嘗試自行重啓";
                }
                if (Cur_lang == SystemLanguage.Japanese)
                {
                    return "更新完了、Appは自分で再起動してみてください";
                }
                return "Update completed, App will attempt to restart itself";
            }
        }

        /// <summary>
        /// 文件校验错误
        /// </summary>
        public static string File_Check_Error
        {
            get
            {
                if (Cur_lang == SystemLanguage.Chinese || Cur_lang == SystemLanguage.ChineseSimplified)
                {
                    return "下载的更新补丁无法通过验证，请确认您的网络是否安全，或联系客服获得帮助";
                }
                if (Cur_lang == SystemLanguage.ChineseTraditional)
                {
                    return "下載的更新補丁無法通過驗證，請確認您的網絡是否安全，或聯系客服獲得幫助";
                }
                if (Cur_lang == SystemLanguage.Japanese)
                {
                    return "ダウンロードの更新パッチを検証することはできませんので、ネットワークが安全かどうかを確認してください。";
                }
                return "The downloaded update patch cannot be verified. Please confirm that your network is secure or contact customer service for help.";
            }
        }

    }
}
