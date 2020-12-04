using TinaX.UIKit;
using TinaX.UIKit.DataBinding;
using TinaX.UIKit.MVVM;

namespace Nekonya.Example
{
    public class CounterScreen : XUIBehaviour
    {
        [Bindable]
        public BindableProperty<int> Counter { get; set; } = new BindableProperty<int>(0);

        [Command]
        public void OnBtnClicked()
        {
            Counter.Value++;
        }
    }
}
