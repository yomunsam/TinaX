using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace TinaX.UIKits
{
    [DisallowMultipleComponent]
    public class CtrlBehaviourMaster : MonoBehaviour
    {
        private UIController mController;

        private bool mAwaked = false;       //Unity有没有对本Behaviour执行过Awake
        private bool mStarted = false;      //Unity有没有对本Behaviour执行过Start

        private bool mDoAwaked = false;     //咱目前有没有往ctrl里面调用过awake
        private bool mDoStarted = false;        

        public void InitController(UIController ctrl)
        {
            mController = ctrl;

            if (mAwaked && !mDoAwaked)
            {
                //系统的Awake已经走过了，所以这里手动触发一下
                mController.FrameworkPAwake();
                mDoAwaked = true;
            }

            if (mStarted && !mDoStarted)
            {
                mController.FrameworkPStart();
                mDoStarted = true;
            }

        }

        public UIController GetUIController() => mController;

        //--------------------------------------------------------

        #region 接收Unity的生命周期调用

        private void Awake()
        {
            if(mController != null && !mDoAwaked)
            {
                mController.FrameworkPAwake();
                mDoAwaked = true;
            }
            mAwaked = true;
        }

        private void OnEnable()
        {
#if UNITY_EDITOR
            if (UnityEngine.Application.isPlaying)
            {
                mController?.FrameworkPEnable();
            }
#else
            mController?.FrameworkPEnable();
#endif
        }

        private void OnDisable()
        {
#if UNITY_EDITOR
            if (UnityEngine.Application.isPlaying)
            {
                mController?.FrameworkPOnDisable();
            }
#else
            mController?.FrameworkPOnDisable();
#endif
        }

        private void Start()
        {
            if(mController != null && !mDoStarted)
            {
                mController.FrameworkPStart();
                mDoStarted = true;
            }
            mStarted = true;
        }

        

        private void OnDestroy()
        {
            mController?.FrameworkPOnDestroy();
        }



        #endregion

    }
}
