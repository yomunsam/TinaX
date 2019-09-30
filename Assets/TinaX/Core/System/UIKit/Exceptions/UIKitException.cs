using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinaX.Exceptions
{
    public class UIKitException : Exception
    {
        public ErrorType Error { get; private set; }
        
        public UIKitException(ErrorType errorType, string msg) : base(msg)
        {
            Error = errorType;

        }


        public enum ErrorType
        {
            UINameOrPathInvalid,
            /// <summary>
            /// UI名无效
            /// </summary>
            UINameInvalid,
            /// <summary>
            /// 当前UI组无效，未配置UI组或引用失效。
            /// </summary>
            UIGroupInvalid,
            /// <summary>
            /// UI路径无效
            /// </summary>
            UIPathInvalid,
            /// <summary>
            /// UI文件未找到
            /// </summary>
            UIFileNotFound,

            UIEntityInvalid,

            /// <summary>
            /// UI已经被加载了
            /// </summary>
            UIAlreadyLoaded,

            /// <summary>
            /// UIController无效
            /// </summary>
            UIControllerInvalid,
        }
    }
}
