using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TinaX;
using System;

namespace TinaXGameKit.Drama
{
    /// <summary>
    /// 剧情 对话系统播放器
    /// </summary>
    public class DialoguePlayer: IPlayerForNode
    {
        #region 实现回调

        /// <summary>
        /// 执行 普通对话内容
        /// </summary>
        public Action<BP.IDialogContent> OnContent;
        /// <summary>
        /// 执行 选择对话
        /// </summary>
        public Action<BP.IDialogChoose> OnChoose;
        /// <summary>
        /// 结束对话
        /// </summary>
        public Action<string> OnFinish;

        /// <summary>
        /// 跳转到别的蓝图
        /// </summary>
        public Action<string> OnJumpBluePrint;

        /// <summary>
        /// 输入信息（如“来给这个角色起个名字吧”等等）
        /// </summary>
        public Action OnInputMessage;

        /// <summary>
        /// 实现特定行动
        /// </summary>
        public Action<string> OnAction;

        #endregion

        #region 给Node用的接口的实现

        /// <summary>
        /// 运行 内容
        /// </summary>
        /// <param name="content"></param>
        public void DoContent(BP.DialogueContent content)
        {
            mCur_Node = content;
            if(OnContent != null)
            {
                OnContent(content);
            }
        }

        public void DoChoose(BP.DialogueChoose choose)
        {
            mCur_Node = choose;
            if(OnChoose != null)
            {
                OnChoose(choose);
            }
        } 

        public void DoFinish(string param)
        {
            mCur_Node = null;
            if (OnFinish != null)
            {
                OnFinish(param);
            }
        }

        #endregion

        private List<BP.DialogueBluePrint> mBluePrintList = new List<BP.DialogueBluePrint>();
        private BP.DialogueBluePrint mCur_BluePrint;
        private BP.DialogueBaseNode mCur_Node;


        #region 供业务逻辑开发使用

        /// <summary>
        /// 播放对话
        /// </summary>
        public void PlayDialogue(BP.DialogueBluePrint bluePrint)
        {
            if (bluePrint.DialogueName.IsNullOrEmpty())
            {
                XLog.PrintW("[GameKit][Drama] 播放对话失败，指定的对话蓝图没有命名");
                //return;
            }
            mCur_BluePrint = bluePrint;
            if (!mBluePrintList.Contains(bluePrint))
            {
                mBluePrintList.Add(bluePrint);
            }

            mCur_BluePrint.ReInit();
            //Debug.Log("开始节点");
            mCur_BluePrint.StartNode(this);
        }

        /// <summary>
        /// 执行下一步
        /// </summary>
        public void Next()
        {
            if(mCur_Node != null)
            {
                mCur_Node.DoNext(this);
            }
        }

        /// <summary>
        /// 执行下一步，并传回字符串参数
        /// </summary>
        /// <param name="param"></param>
        public void NextWithString(string param)
        {
            if(mCur_Node != null)
            {
                mCur_Node.DoNextWhitParam(this, param);
            }
        }

        /// <summary>
        /// 执行下一步，并传回Object参数
        /// </summary>
        /// <param name="param"></param>
        public void Next(System.Object param)
        {
            if (mCur_Node != null)
            {
                mCur_Node.DoNextWhitParam(this, param);
            }
        }

        /// <summary>
        /// 设置蓝图全局参数（字符串）
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void SetString(string key, string value)
        {
            if (mCur_BluePrint != null)
            {
                mCur_BluePrint.SetString(key, value);
            }
        }

        #endregion

    }
}

