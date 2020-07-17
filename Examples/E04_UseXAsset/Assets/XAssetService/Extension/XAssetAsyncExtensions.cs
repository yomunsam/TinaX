using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XAsset.Awaiter;

namespace XAsset
{
    public static class XAssetAsyncExtensions
    {
        public static AssetRequestAwaiter GetAwaiter(this libx.AssetRequest assetreq)
        {
            return new AssetRequestAwaiter(assetreq);
        }
    }
}
