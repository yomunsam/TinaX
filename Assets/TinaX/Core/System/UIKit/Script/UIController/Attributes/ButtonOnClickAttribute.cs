using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinaX.UIKits
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ButtonOnClickAttribute : Attribute
    {
        public string InjectButtonName { get; set; }

        public ButtonOnClickAttribute()
        {

        }

        public ButtonOnClickAttribute(string injectButtonName)
        {
            InjectButtonName = injectButtonName;
        }
    }
}
