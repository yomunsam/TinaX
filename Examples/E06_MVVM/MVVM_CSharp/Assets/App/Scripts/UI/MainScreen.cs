using TinaX;
using TinaX.I18N;
using TinaX.UIKit;
using TinaX.UIKit.DataBinding;
using TinaX.UIKit.MVVM;

namespace Nekonya.Example
{
    public class MainScreen : XUIBehaviour
    {
        [Inject] public IUIKit UIKit { get; set; }
        [Inject] public ICommandLineArgs CommandLineArgs { get; set; }
        [Inject] public II18N I18N { get; set; }
        
        [Bindable] public BindableProperty<string> WelcomeMsg { get; set; } = new BindableProperty<string>();

        private ILocalizerGroup L;

        public override void Start()
        {
            L = I18N.GetGroup("common");
            WelcomeMsg.Value = L["welcome"];

            //If you start the app/game from the command line and attach the "--Welcome = XXX" args, it will be displayed on the UI
            //In the editor, we can also set the simulation args that only take effect in the editor through the GUI "Project Settings -> Tinax framework -> XCore"
            if (CommandLineArgs.TryGetValue("welcome", out string _welcome))
                WelcomeMsg.Value = _welcome;
        }


        [Command]
        public async void ShowMsgBox()
        {
            await UIKit.OpenUIAsync("msgBox", new MsgBox(),
                    new OpenUIParam() { UseMask = true, CloseByMask = true },
                    "hello,world!", "hello");
        }

        [Command]
        public async void OpenCounter()
        {
            await UIKit.OpenUIAsync("counter",
                    new CounterScreen(),
                    new OpenUIParam
                    {
                        UseMask = true,
                        CloseByMask = true
                    });
        }

        [Command]
        public async void OpenFetchData()
        {
            await UIKit.OpenUIAsync("fetchData", new FetchData());
        }

    }

}
