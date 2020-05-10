using TinaX;
using TinaX.UIKit;
using TinaX.VFSKit;
using TinaX.I18N;
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
            .RegisterServiceProvider(new I18NProvider())
            .OnServicesStartException((service, err) =>
            {
                //
            });
            _ = core.RunAsync();

            //startup TinaX Framework 
        }
    }
}

