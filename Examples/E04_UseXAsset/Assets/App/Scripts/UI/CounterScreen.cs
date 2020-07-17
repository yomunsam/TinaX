using TinaX;
using TinaX.I18N;
using TinaX.UIKit;
using TinaX.UIKit.DataBinding;
using UnityEngine.UI;

namespace Nekonya.Example
{
    public class CounterScreen : XUIBehaviour
    {
        public Text txt_counter { get; set; }
        public Button btn_click { get; set; }

        [Inject]
        public II18N m_I18N { get; set; }

        private BindableProperty<int> m_currentCounter = new BindableProperty<int>(-1);

        public override void Awake()
        {
            m_currentCounter.OnValueChanged((_, newValue) =>
            {
                txt_counter.text = string.Format(
                    m_I18N.GetText("txt_cur_counter", "common", "Current count: {0}"), newValue);
            });
            

            btn_click.onClick.AddListener(() =>
            {
                m_currentCounter.Value++;
            });
        }

        public override void Start()
        {
            m_currentCounter.Value = 0;
        }



    }
}
