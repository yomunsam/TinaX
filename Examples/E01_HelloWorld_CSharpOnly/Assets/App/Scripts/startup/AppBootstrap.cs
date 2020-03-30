using TinaX;
using TinaX.VFSKit;
using TinaX.UIKit;

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
        public void OnInit()
        {
        }

        public void OnStart()
        {
            //open ui
            var uikit = XCore.MainInstance.GetService<IUIKit>();
            _ = uikit.OpenUIAsync<MainScreen>("mainScreen");

            //load scene
            var vfs = XCore.MainInstance.GetService<IVFS>();
            vfs.LoadSceneAsync("Assets/App/Scenes/App.Main.unity",(scene, err) => 
            {
                scene.OpenScene();
            });
        }

        public void OnQuit()
        {
        }

        public void OnAppRestart()
        {
        }

    }
}
