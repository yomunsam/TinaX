using System;
using TinaX;
using TinaX.UIKit;
using TinaX.VFSKit;
using TinaX.XILRuntime;
using TinaX.XILRuntime.Registers;

namespace Nekonya.Example
{
    /// <summary>
    /// 推荐作为App (Game) 的程序入口。该class会被TinaX反射寻找，可以同时存在多个
    /// It is recommended to be the program entry of app (game). 
    /// This class will be reflection by tinax , and there can be multiple
    /// </summary>
    public class AppBootstrap : IXBootstrap
    {
        /// <summary>
        /// "OnInit" be called after "TinaX's Services" Inited and Registered, before "TinaX's Services" started.
        /// </summary>
        public void OnInit(IXCore core)
        {
            var xil = core.Services.Get<IXILRuntime>();
            this.RegisterILRuntimes(xil);
        }

        public void OnStart(IXCore core)
        {
            //move to hotfix project.
        }

        public void OnQuit() { }

        public void OnAppRestart() { }

        private void RegisterILRuntimes(IXILRuntime xil)
        {
            xil.RegisterXComponent();
            xil.RegisterUIKit();

            xil.DelegateManager.RegisterMethodDelegate<Int32, Int32>();

            xil.DelegateManager.RegisterMethodDelegate<TinaX.VFSKit.ISceneAsset, TinaX.XException>();

            xil.DelegateManager.RegisterDelegateConvertor<TinaX.UIKit.DataBinding.BindableProperty<System.Int32>.ValueChangedDalegate>((act) =>
            {
                return new TinaX.UIKit.DataBinding.BindableProperty<System.Int32>.ValueChangedDalegate((oldValue, newValue) =>
                {
                    ((Action<System.Int32, System.Int32>)act)(oldValue, newValue);
                });
            });
        }

    }
}
