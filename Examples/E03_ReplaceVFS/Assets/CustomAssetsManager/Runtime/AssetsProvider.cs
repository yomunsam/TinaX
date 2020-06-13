using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TinaX.Services;
using TinaX;
using System.Threading.Tasks;

namespace Nekonya.Demo.Assets
{
    [XServiceProviderOrder(60)] //Assets managment service should start before others.
    public class AssetsProvider : IXServiceProvider
    {
        public string ServiceName => "Nekonya.Demo.Assets";

        public Task<XException> OnInit(IXCore core) => Task.FromResult<XException>(null);

        public void OnServiceRegister(IXCore core)
        {
            core.Services.BindBuiltInService<TinaX.Services.IAssetService, IAssets, AssetsService>();
        }

        public Task<XException> OnStart(IXCore core) => Task.FromResult<XException>(null);

        public void OnQuit() { }

        public Task OnRestart() => Task.CompletedTask;

        
    }

}
