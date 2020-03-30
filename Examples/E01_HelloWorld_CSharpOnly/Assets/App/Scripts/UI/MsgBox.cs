using TinaX.XComponent;
using TinaX.UIKit;
using UnityEngine.UI;
using UnityEngine;

namespace Nekonya.Example
{
    public class MsgBox : XBehaviour
    {
        [Binding("txt_title")]
        public Text mTitle { get; set; }

        [Binding("txt_content")]
        public Text mContent { get; set; }


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
                mContent.text = (string)args[0];
            else if(args.Length > 1)
            {
                mContent.text = (string)args[0];
                mTitle.text = (string)args[1];
            }
        }

    }
}
