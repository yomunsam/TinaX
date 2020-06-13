using TinaX;
using TinaX.UIKit;
using UnityEngine.UI;

namespace Nekonya.Example
{
    public class MainScreen : XUIBehaviour
    {
        [Inject]
        public IUIKit UIKit { get; set; }
        public Button btn_1 { get; set; }
        public Button btn_counter { get; set; }
        public Button btn_fetchData { get; set; }

        public override void Start()
        {
            btn_1.onClick.AddListener(async () =>
            {
                await UIKit.OpenUIAsync("msgBox", new MsgBox(),
                    new OpenUIParam() { UseMask = true, CloseByMask = true }, 
                    "hello,world!", "hello");
            });

            btn_counter.onClick.AddListener(async () =>
            {
                await UIKit.OpenUIAsync("counter",
                    new CounterScreen(),
                    new OpenUIParam
                    {
                        UseMask = true,
                        CloseByMask = true
                    });
            });

            btn_fetchData.onClick.AddListener(() =>
            {
                UIKit.OpenUIAsync("fetchData", new FetchData());
            });
        }

    }

}
