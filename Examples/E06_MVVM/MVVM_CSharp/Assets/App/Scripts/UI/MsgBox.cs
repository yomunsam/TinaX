using TinaX.UIKit;
using TinaX.UIKit.DataBinding;
using TinaX.UIKit.MVVM;

namespace Nekonya.Example
{
    public class MsgBox : XUIBehaviour
    {
        [Bindable]
        public BindableProperty<string> Title { get; set; } = new BindableProperty<string>();
        
        [Bindable]
        public BindableProperty<string> Content { get; set; } = new BindableProperty<string>();

        public override void OnMessage(string msgName, params object[] msgParams)
        {
            switch (msgName)
            {
                case "OnOpenUIMessage":
                    OnOpenUIMessage(msgParams);
                    break;
            }
        }


        private void OnOpenUIMessage(params object[] args)
        {
            if (args == null) return;
            if (args.Length == 1)
                Content.Value = (string)args[0];
            else if(args.Length > 1)
            {
                Content.Value = (string)args[0];
                Title.Value = (string)args[1];
            }
        }

    }
}
