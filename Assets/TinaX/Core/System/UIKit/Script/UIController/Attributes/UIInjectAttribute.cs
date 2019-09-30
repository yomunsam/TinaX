using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinaX.UIKits
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    class UIInjectAttribute : Attribute
    {
        public string InjectName { get; private set; }

        public UIInjectAttribute(string name)
        {
            InjectName = name;
        }
    }
}
