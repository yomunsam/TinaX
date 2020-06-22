using TinaX;
using TinaX.UIKit;
using TinaX.VFSKit;
using TinaX.XComponent;
using UnityEngine;
using UnityEngine.UI;

namespace Nekonya.UI
{
    public class FetchData : XUIBehaviour
    {
        [Binding("btn_exit")]
        public Button Btn_Close { get; set; }

        [Binding("txt_content")]
        public Text Txt_Content { get; set; }

        [Inject]
        public IVFS VFS { get; set; }

        public async override void Start()
        {
            using(var text_asset = await VFS.LoadAssetAsync<TextAsset>("Assets/App/Data/text/hello.txt"))
            {
                Txt_Content.text = text_asset.Get<TextAsset>().text;
            }

            Btn_Close.onClick.AddListener(() =>
            {
                this.Close();
            });
        }

    }
}
