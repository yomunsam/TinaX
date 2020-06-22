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
            .UseVFS()
            .UseUIKit()
            .UseI18N()
            .UseXILRuntime()
            .OnServicesStartException((service, err) =>
            {
                //
            });
            await core.RunAsync();

            //startup TinaX Framework 
        }
    }
}

