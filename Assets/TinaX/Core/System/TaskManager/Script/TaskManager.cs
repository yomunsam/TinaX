using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UniRx;


namespace TinaX.Task
{
    /// <summary>
    /// 任务管理器
    /// </summary>
    public class TaskManager:ITaskManager
    {
        private CoroutineMgr mCoroutineMgr; //Unity原生协程

        public TaskManager()
        {

            //协程管理器初始化
            mCoroutineMgr = XCore.I.BaseGameObject
                .GetComponentOrAdd<CoroutineMgr>();

        }

        

        ~TaskManager()
        {
            mCoroutineMgr = null;
        }



        public void RxRun(Action action,Action<Unit> RunFinish = null)
        {
            var act = Observable.Start(action);
            if(RunFinish != null)
            {
                Observable.WhenAll(act)
                    .ObserveOnMainThread()
                    .Subscribe((xs) =>
                        {
                            RunFinish(xs);
                        });
            }
        }

    }
}
