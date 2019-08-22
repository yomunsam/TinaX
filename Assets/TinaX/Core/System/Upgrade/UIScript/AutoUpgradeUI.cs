//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using TinaX;
//using TinaX.UIKit;
//using UnityEngine.UI;
//using System;


//namespace TinaX.Upgrade
//{
//    public class AutoUpgradeUI : MonoBehaviour
//    {
//        public Text txt_title;
//        //public Text txt_title;

//        /// <summary>
//        /// 设置标题文本
//        /// </summary>
//        /// <param name="str"></param>
//        public void SetTitleStr(string str)
//        {
//            txt_title.text = str;
//        }

//    }


//    public static class AutoUpgradeUI_Mgr
//    {

//        private const string Upgrade_Hor_Name = "TinaX_Upgrade_Hor"; //GameObject识别名
//        private const string Upgrade_Ver_Name = "TinaX_Upgrade_Ver"; //

//        private const string Upgrade_Hor_Path = "TinaX/Update/UpgradeScreen_Hor"; //prefab - resources
//        private const string Upgrade_Ver_Path = "TinaX/Update/UpgradeScreen_Ver"; //prefab - resources

//        private static bool mIsUpgrading; //当前正在有自动更新在工作
//        private static UpgradeMgr mUpgradeMgr;

//        private static AutoUpgradeUI mUIScript_Hor,mUIScript_Ver;
//        private static GameObject mGo_Hor, mGo_Ver;

//        /// <summary>
//        /// 调用这玩意自动更新
//        /// </summary>
//        /// <param name="OnFinish"></param>
//        public static void Start(Action<Upgrade.E_UpgradeResults> OnFinish)
//        {
//            if (mIsUpgrading)
//            {
//                return;
//            }
//            mIsUpgrading = true;

//            mUpgradeMgr = new UpgradeMgr();

//            //把UI打出来
//            if (Screen.width < Screen.height)
//            {
//                //竖屏
//                var prefab = Resources.Load<GameObject>(Upgrade_Ver_Path);
//                mGo_Ver = GameObject.Instantiate(prefab)
//                    .Name(Upgrade_Ver_Name);

//                mUIScript_Ver = mGo_Ver.GetComponent<AutoUpgradeUI>();


//            }
//            else
//            {
//                //横屏
//                var prefab = Resources.Load<GameObject>(Upgrade_Hor_Path);
//                mGo_Hor = GameObject.Instantiate(prefab)
//                    .Name(Upgrade_Hor_Name);

//                mUIScript_Hor = mGo_Hor.GetComponent<AutoUpgradeUI>();


//            }

//            SetTitleStr(UpgradeLanguage.CheckUpgrade);
//            mUpgradeMgr.CheckUpgrade((res)=> {
//                switch (res)
//                {
//                    case E_UpgradeCheckResults.connect_error:   //网络错误
//                        Utils.MsgBox.ShowMsgBox(UpgradeLanguage.CheckUpgrade_Fail, UpgradeLanguage.CheckUpgrade_Fail_Connect_Lost,UpgradeLanguage.Confirm,()=> {

//                            CloseUI_ClearStatus();
//                            OnFinish(E_UpgradeResults.connect_lost);
//                        });
//                        break;
//                    case E_UpgradeCheckResults.error:
//                        Utils.MsgBox.ShowMsgBox(UpgradeLanguage.CheckUpgrade_Fail, "Unknow Error", UpgradeLanguage.Confirm, () => {
//                            CloseUI_ClearStatus();
//                            OnFinish(E_UpgradeResults.error);
//                        });
//                        break;
//                    case E_UpgradeCheckResults.newest: //检查过了，不需要更新
//                        CloseUI_ClearStatus();
//                        OnFinish( E_UpgradeResults.success);
//                        break;
//                    case E_UpgradeCheckResults.not_enable:
//                        CloseUI_ClearStatus();
//                        OnFinish(E_UpgradeResults.success);
//                        break;
//                    case E_UpgradeCheckResults.upgrade:
//                        //需要更新了终于
//                        mUpgradeMgr.DoUpgrade((_res) =>
//                        {
//                            SetTitleStr("Finish");
//                            //Debug.Log("更新操作完成:" + _res);
//                            //更新完成
//                            if (_res == E_UpgradeResults.connect_lost)
//                            {
//                                Utils.MsgBox.ShowMsgBox(UpgradeLanguage.CheckUpgrade_Fail, UpgradeLanguage.CheckUpgrade_Fail_Connect_Lost, UpgradeLanguage.Confirm, () => {

//                                    CloseUI_ClearStatus();
//                                    OnFinish(E_UpgradeResults.connect_lost);
//                                });
//                            }
//                            if(_res == E_UpgradeResults.files_check_fail)
//                            {
//                                Utils.MsgBox.ShowMsgBox("Error", UpgradeLanguage.File_Check_Error, UpgradeLanguage.Confirm, () => {

//                                    CloseUI_ClearStatus();
//                                    XCore.I.RestartApp();   //尝试软重启框架
//                                });
//                            }
//                            if(_res == E_UpgradeResults.success)
//                            {
//                                Utils.MsgBox.ShowMsgBox(UpgradeLanguage.Upgrade_Success, UpgradeLanguage.Upgrade_Success_Restart, UpgradeLanguage.Confirm, () => {

//                                    CloseUI_ClearStatus();
//                                    XCore.I.RestartApp();   //尝试软重启框架
//                                });
//                            }

//                        },()=> {
//                            //开始更新
//                            SetTitleStr(UpgradeLanguage.Upgrade_Downloading);
//                        },(url) =>
//                        {
//                            //需要打开浏览器更新
//                            Utils.MsgBox.ShowMsgBox(UpgradeLanguage.NewVersion, UpgradeLanguage.Upgrade_With_Browser, UpgradeLanguage.Confirm, () =>
//                            {
//                                Application.OpenURL(url);
//                                //这里直接关掉App
//                                Application.Quit();
//#if UNITY_EDITOR
//                                UnityEditor.EditorApplication.isPlaying = false;
//#endif
//                            });
//                        });


//                        break;
//                }
//            });

//        }

//        /// <summary>
//        /// 关掉UI并清除状态变量
//        /// </summary>
//        private static void CloseUI_ClearStatus()
//        {
//            if(mGo_Hor != null)
//            {
//                GameObject.Destroy(mGo_Hor);
//            }
//            if (mGo_Ver != null)
//            {
//                GameObject.Destroy(mGo_Ver);
//            }
//            mIsUpgrading = false;
//            mUpgradeMgr = null;
//            mUIScript_Hor = null;
//            mUIScript_Ver = null;
//            mGo_Ver = null;
//            mGo_Hor = null;

//        }


//        /// <summary>
//        /// 设置标题字
//        /// </summary>
//        /// <param name="str"></param>
//        private static void SetTitleStr(string str)
//        {
//            if(mUIScript_Hor != null)
//            {
//                mUIScript_Hor.SetTitleStr(str);
//            }
//            if(mUIScript_Ver != null)
//            {
//                mUIScript_Ver.SetTitleStr(str);
//            }
//        }
//    }
//}
