// C# Translation of pytdx/errors.py

using System;

namespace Pytdx
{
    /// <summary>
    /// 当连接服务器出错的时候，会抛出的异常
    /// </summary>
    public class TdxConnectionException : Exception
    {
        public TdxConnectionException(string message) : base(message) { }
        public TdxConnectionException(string message, Exception innerException) : base(message, innerException) { }
    }

    /// <summary>
    /// 当函数调用出错的时候
    /// </summary>
    public class TdxFunctionCallException : Exception
    {
        /// <summary>
        /// The original exception that was caught.
        /// </summary>
        public Exception? OriginalException { get; set; }

        public TdxFunctionCallException(string message) : base(message) { }

        public TdxFunctionCallException(string message, Exception innerException) : base(message, innerException)
        {
            OriginalException = innerException;
        }
    }

    // Exceptions from pytdx/parser/base.py
    public class SocketClientNotReadyException : Exception { public SocketClientNotReadyException(string message) : base(message) { } }
    public class SendPkgNotReadyException : Exception { public SendPkgNotReadyException(string message) : base(message) { } }
    public class SendRequestPkgFailsException : Exception { public SendRequestPkgFailsException(string message) : base(message) { } }
    public class ResponseHeaderRecvFailsException : Exception { public ResponseHeaderRecvFailsException(string message) : base(message) { } }
    public class ResponseRecvFailsException : Exception { public ResponseRecvFailsException(string message) : base(message) { } }
}
