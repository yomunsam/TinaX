using TinaX;
using TinaX.UIKit;
using TinaX.VFSKit;
using TinaX.Lua;
using UnityEngine;
using System.Threading.Tasks;


namespace Nekonya.Example
{
    public class AppStartup : MonoBehaviour
    {
        private async void Start()
        {
            var core = XCore.New()
            .RegisterServiceProvider(new VFSProvider())
            .RegisterServiceProvider(new UIKitProvider())
            .RegisterServiceProvider(new LuaProvider())
            .OnServicesStartException((service, err) =>
            {
                //
            });
            await core.RunAsync();

            //startup TinaX Framework 
        }
    }
}

