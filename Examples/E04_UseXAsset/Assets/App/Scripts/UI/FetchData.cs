using System.Threading.Tasks;
using TinaX;
using TinaX.UIKit;
//using TinaX.VFSKit;
using TinaX.XComponent;
using UnityEngine;
using UnityEngine.UI;
using libx;
using XAsset;

namespace Nekonya.Example
{
    public class FetchData : XUIBehaviour
    {
        [Binding("btn_exit")]
        public Button Btn_Close { get; set; }

        [Binding("txt_content")]
        public Text Txt_Content { get; set; }

        //[Inject]
        //public IVFS VFS { get; set; }

        public async override void Start()
        {
            //using(var text_asset = await VFS.LoadAssetAsync<TextAsset>("Assets/App/Data/text/hello.txt"))
            //{
            //    Txt_Content.text = text_asset.Get<TextAsset>().text;
            //}
            var req = Assets.LoadAssetAsync("Assets/App/Data/text/hello.txt", typeof(TextAsset));
            await req;
            Txt_Content.text = ((TextAsset)req.asset).text;
            req.Release();


            Btn_Close.onClick.AddListener(() =>
            {
                this.Close();
            });
        }

    }
}
