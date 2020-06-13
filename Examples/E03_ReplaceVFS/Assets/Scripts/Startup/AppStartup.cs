using TinaX;
using TinaX.Services;
using UnityEngine;
using TinaX.UIKit;


namespace Nekonya
{
    public class AppStartup : MonoBehaviour
    {
        private async void Start()
        {
            var core = XCore.New()
                .UseDemoAssets()
                .RegisterServiceProvider(new UIKitProvider());
            await core.RunAsync();

            await UIKit.That.OpenUIAsync("helloUI");
        }
    }
}

