using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TinaX.UIKits;
using System;

namespace TinaX.Common
{
    public class TinaXMsgbox : MonoBehaviour
    {
        [Header("标题")]
        public Text txt_title;
        [Header("正文")]
        public Text txt_Content;
        [Header("按钮们")]
        public Button btn_yes;
        public Button btn_no;

        public void OnOpenUI(string title, string content, string str_btn_yes, Action btn_yes_callback = null, string str_btn_no = null, Action btn_no_callback = null)
        {
            txt_title.text = title;
            txt_Content.text = content;
            btn_yes.gameObject.GetComponentInChildren<XText>().text = str_btn_yes;
            btn_yes.onClick.RemoveAllListeners();
            btn_yes.onClick.AddListener(()=> {
                btn_yes_callback();
            });

            if (str_btn_no == null)
            {
                btn_no.gameObject.SetActive(false);
            }
            else
            {
                btn_no.gameObject.GetComponentInChildren<XText>().text = str_btn_no;
                btn_no.onClick.RemoveAllListeners();
                btn_no.onClick.AddListener(() =>
                {
                    btn_no_callback();
                });
            }

        }

    }
}

namespace TinaX.Utils
{
    public static class MsgBox
    {
        private const string MsgBox_Hor_Name = "TinaX_MsgBox_Hor"; //GameObject识别名
        private const string MsgBox_Ver_Name = "TinaX_MsgBox_Ver"; //

        private const string MsgBox_Hor_Path = "TinaX/Common/MsgBox_hor"; //prefab - resources
        private const string MsgBox_Ver_Path = "TinaX/Common/MsgBox_ver"; //prefab - resources

        private static bool mIsMsgBoxShow; //MsgBox是否正常显示
        private static GameObject MsgBox_Go_Hor;
        private static GameObject MsgBox_Go_Ver;

        /// <summary>
        /// 打开MessageBox
        /// </summary>
        /// <param name="title">标题</param>
        /// <param name="content">正文</param>
        /// <param name="btn_yes">确认按钮文本</param>
        /// <param name="btn_yes_callback">确认按钮回调</param>
        /// <param name="btn_no">取消按钮文本</param>
        /// <param name="btn_no_callback">取消按钮回调</param>
        public static void ShowMsgBox(string title,string content,string btn_yes,Action btn_yes_callback = null, string btn_no = null,Action btn_no_callback = null)
        {
            if (mIsMsgBoxShow) return;

            Action finish = new Action(() => {
                mIsMsgBoxShow = false;
                if(MsgBox_Go_Hor != null)
                {
                    GameObject.Destroy(MsgBox_Go_Hor);
                }
                if (MsgBox_Go_Ver != null)
                {
                    GameObject.Destroy(MsgBox_Go_Ver);
                }
            });


            Action _yes_callback;
            if(btn_yes_callback == null)
            {
                _yes_callback = finish;
            }
            else
            {
                _yes_callback = btn_yes_callback;
                _yes_callback += finish;
            }

            Action _no_callback;
            if (btn_no_callback == null)
            {
                _no_callback = finish;
            }
            else
            {
                _no_callback = btn_no_callback;
                _no_callback += finish;
            }

            //判断当前屏幕长宽，以启用对应的UI
            if (Screen.width < Screen.height)
            {
                //竖屏
                var prefab = Resources.Load<GameObject>(MsgBox_Ver_Path);
                MsgBox_Go_Ver = GameObject.Instantiate(prefab)
                    .Name(MsgBox_Ver_Name);
                var ui_script = MsgBox_Go_Ver.GetComponent<TinaX.Common.TinaXMsgbox>();
                

                if(btn_no == null)
                {
                    ui_script.OnOpenUI(title, content, btn_yes, _yes_callback);
                }
                else
                {
                    ui_script.OnOpenUI(title, content, btn_yes, _yes_callback,btn_no,_no_callback);
                }
            }
            else
            {
                //横屏
                var prefab = Resources.Load<GameObject>(MsgBox_Hor_Path);
                MsgBox_Go_Hor = GameObject.Instantiate(prefab)
                    .Name(MsgBox_Hor_Name);
                var ui_script = MsgBox_Go_Hor.GetComponent<TinaX.Common.TinaXMsgbox>();


                if (btn_no == null)
                {
                    ui_script.OnOpenUI(title, content, btn_yes, _yes_callback);
                }
                else
                {
                    ui_script.OnOpenUI(title, content, btn_yes, _yes_callback, btn_no, _no_callback);
                }
            }

            mIsMsgBoxShow = true;

        }
        
    }
}