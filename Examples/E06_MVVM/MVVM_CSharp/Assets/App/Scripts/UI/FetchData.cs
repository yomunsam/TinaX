using TinaX;
using TinaX.UIKit;
using TinaX.UIKit.DataBinding;
using TinaX.UIKit.MVVM;
using TinaX.VFSKit;
using UnityEngine;

namespace Nekonya.Example
{
    public class FetchData : XUIBehaviour
    {
        [Bindable] public BindableProperty<string> Content { get; set; } = new BindableProperty<string>("loading...");

        [Inject] public IVFS VFS { get; set; }

        public async override void Start()
        {
            using(var text_asset = await VFS.LoadAssetAsync<TextAsset>("Assets/App/Data/text/hello.txt"))
            {
                Content.Value = text_asset.Get<TextAsset>().text;
            }
        }

        [Command]
        public void DoExit()
        {
            this.Close();
        }
    }
}
