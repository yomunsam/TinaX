using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TinaX
{
    /// <summary>
    /// 针对枚举类在编辑器中显示自定义文本的扩展
    /// </summary>
    public class EnumLabelAttribute : HeaderAttribute
    {
        public EnumLabelAttribute(string header) : base(header)
        {

        }
    }
}


