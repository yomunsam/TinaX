using UnityEngine;
using System;
using UniRx;

namespace TinaX.Task
{
    public interface ITaskManager
    {




        /// <summary>
        /// [Unirx]运行一个异步方法（多线程）
        /// </summary>
        /// <param name="action"></param>
        /// <param name="RunFinish"></param>
        void RxRun(Action action, Action<Unit> RunFinish = null);
    }
}
