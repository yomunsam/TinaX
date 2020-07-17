using TinaX;
using TinaX.I18N;
using TinaX.Services;
using UnityEngine;


namespace Nekonya.Example
{
    public class AppStartup : MonoBehaviour
    {
        private async void Start()
        {
            var core = XCore.New()
            //.UseVFS()
            .RegisterServiceProvider(new XAsset.XAssetProvider())
            .UseUIKit()
            .UseI18N()
            .OnServicesStartException((service, err) =>
            {
                //
            });
            await core.RunAsync();

            //startup TinaX Framework 
        }
    }
}

