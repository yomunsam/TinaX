using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TinaX;
using UnityEngine;

namespace XAsset
{
    [TinaX.Services.XServiceProviderOrder(80)]
    public class XAssetProvider : TinaX.Services.IXServiceProvider
    {
        public string ServiceName => "XAsset";

        public Task<XException> OnInit(IXCore core) => Task.FromResult<XException>(null);

        

        public void OnServiceRegister(IXCore core)
        {
            core.Services.BindBuiltInService<TinaX.Services.IAssetService, IXAssetService, XAssetService>();
        }

        public Task<XException> OnStart(IXCore core)
        {
            return core.GetService<IXAssetService>().Start();
        }

        public void OnQuit() { }

        public Task OnRestart() => Task.CompletedTask;
    }
}

