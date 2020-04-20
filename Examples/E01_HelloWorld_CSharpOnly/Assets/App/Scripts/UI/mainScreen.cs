using System.Threading.Tasks;
using TinaX;
using TinaX.UIKit;
using TinaX.XComponent;
using UnityEngine.UI;

namespace Nekonya.Example
{
    public class MainScreen : XBehaviour
    {
        [Inject]
        public IUIKit _UIKit { get; set; }
        public Button btn_1 { get; set; }

        public override void Start()
        {
            btn_1.onClick.AddListener(async () =>
            {
                await _UIKit.OpenUIAsync("msgBox", new MsgBox(),
                    new OpenUIParam() { UseMask = true, CloseByMask = true }, 
                    "hello,world!", "hello");
            });
        }

    }

}
