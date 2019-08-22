using System;

namespace TinaX.Exceptions
{ 
    public class XWebException :ApplicationException
    {
        public ErrorType ErrType { get; private set; } = ErrorType.Unknow;
        public System.Net.HttpStatusCode Http_StatusCode { get; private set; } = default;
        public XWebException(string msg) : base(msg)
        {

        }

        public XWebException(string msg, System.Net.HttpStatusCode statusCode):base(msg)
        {
            Http_StatusCode = statusCode;
            ErrType = ErrorType.HttpError;
        }

        public XWebException(string msg, ErrorType errType, System.Net.HttpStatusCode statusCode = default) :base (msg)
        {
            ErrType = errType;
            Http_StatusCode = statusCode;
        }

        public enum ErrorType
        {
            Unknow,
            HttpError,
            NetworkError
        }
    }
}
