//2019.8.21 改： 这里应该是最终VFS对外的唯一异常类
using System;

namespace TinaX.Exceptions
{
    /// <summary>
    /// VFS Exception
    /// </summary>
    public class VFSException : ApplicationException
    {
        #region static

        private static Action<VFSException> mOnFrameworkInitExceptionCallback;

        public static void OnInitException(Action<VFSException> callback)
        {
            mOnFrameworkInitExceptionCallback += callback;
        }

        public static void CallInitException(VFSException exception)
        {
            mOnFrameworkInitExceptionCallback?.Invoke(exception);
        }

        #endregion

        private readonly VFSErrorType mErrorType;
        private VFSKit.AssetParseInfo mParseInfo;
        private readonly System.Net.HttpStatusCode mHttpStatusCode;

        public VFSErrorType ErrorType => mErrorType;
        public string LoadPath => mParseInfo.LoadPath;

        public System.Net.HttpStatusCode HttpStatusCode => mHttpStatusCode;

        #region 构造函数 们
        public VFSException(string message) : base(message)
        {
            mErrorType = VFSErrorType.Unknow;
        }

        public VFSException(string message, VFSErrorType errorType) : base(message)
        {
            mErrorType = errorType;
        }

        public VFSException(string message,VFSKit.AssetParseInfo ParseInfo,VFSErrorType errorType) : base(message)  
        {
            mErrorType = errorType;
            mParseInfo = ParseInfo;
        }


        public VFSException(string message, VFSErrorType errorType, System.Net.HttpStatusCode httpsStatusCode, VFSKit.AssetParseInfo ParseInfo) : base(message)
        {
            mErrorType = errorType;
            mHttpStatusCode = httpsStatusCode;
            mParseInfo = ParseInfo;
        }
        public VFSException(string message, VFSErrorType errorType, System.Net.HttpStatusCode httpsStatusCode) : base(message)
        {
            mErrorType = errorType;
            mHttpStatusCode = httpsStatusCode;
        }



        #endregion

        /// <summary>
        /// Framework Private
        /// </summary>
        /// <returns></returns>
        public VFSException SetParseInfo(VFSKit.AssetParseInfo ParseInfo)
        {
            mParseInfo = ParseInfo;
            return this;
        }


        public enum VFSErrorType
        {
            Unknow,
            #region VFS IO

            /// <summary>
            /// asset path not valid | 加载路径无效
            /// </summary>
            PathNotValid,

            /// <summary>
            /// not found file on disk | path对应的文件不存在
            /// </summary>
            FileNotExist,

            #endregion
            #region VFS Network

            Timeout,

            HttpError,

            NetworkError,

            /// <summary>
            /// Hash 不匹配
            /// </summary>
            HashMismatch,   

            #endregion

            WebVFS_NotEnable,

        }

    }
}

