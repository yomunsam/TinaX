using TinaX;
using TinaX.UIKit;
using TinaX.VFSKit;
using UnityEngine;


namespace Nekonya.Example
{
    public class AppStartup : MonoBehaviour
    {
        private void Start()
        {
            var core = XCore.New()
            .RegisterServiceProvider(new VFSProvider())
            .RegisterServiceProvider(new UIKitProvider())
            .OnServicesStartException((service, err) =>
            {
                //
            });
            _ = core.RunAsync();

            //startup TinaX Framework 
        }
    }
}

