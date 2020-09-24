using TinaX;
using TinaX.UIKit;
using TinaX.XComponent;
using UnityEngine.UI;

namespace Nekonya.Example
{
    public class MainScreen : XUIBehaviour
    {
        [Inject] public IUIKit UIKit { get; set; }
        [Inject] public ICommandLineArgs CommandLineArgs { get; set; }
        

        public Button btn_1 { get; set; }
        public Button btn_counter { get; set; }
        public Button btn_fetchData { get; set; }
        
        [Binding("welcome")]
        public Text txt_welcome { get; set; }

        public override void Start()
        {
            //If you start the app/game from the command line and attach the "--Welcome = XXX" args, it will be displayed on the UI
            //In the editor, we can also set the simulation args that only take effect in the editor through the GUI "Project Settings -> Tinax framework -> XCore"
            if (CommandLineArgs.TryGetValue("welcome",out string _welcome))
            {
                txt_welcome.text = _welcome;
            }

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
