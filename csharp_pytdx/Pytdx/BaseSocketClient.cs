// C# Translation of pytdx/base_socket_client.py

using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Pytdx
{
    public abstract class BaseSocketClient : IDisposable
    {
        private readonly object _lock;
        private Socket _socket;
        private readonly bool _multithread;
        private readonly bool _heartbeat;
        private readonly bool _autoRetry;
        private readonly bool _raiseException;

        protected string Ip { get; private set; }
        protected int Port { get; private set; }
        protected IRetryStrategy RetryStrategy { get; set; }

        // Traffic Stats
        private long _sendPkgNum;
        private long _recvPkgNum;
        private long _sendPkgBytes;
        private long _recvPkgBytes;
        private DateTime? _firstPkgSendTime;


        protected BaseSocketClient(bool multithread = false, bool heartbeat = false, bool autoRetry = false, bool raiseException = false)
        {
            _multithread = multithread;
            if (_multithread || heartbeat)
            {
                _lock = new object();
            }

            _heartbeat = heartbeat;
            _autoRetry = autoRetry;
            _raiseException = raiseException;
            RetryStrategy = new DefaultRetryStrategy();
        }

        public bool Connect(string ip = "101.227.73.20", int port = 7709, int timeout = 5000)
        {
            Ip = ip;
            Port = port;
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
            {
                SendTimeout = timeout,
                ReceiveTimeout = timeout
            };

            try
            {
                _socket.Connect(ip, port);
            }
            catch (Exception ex)
            {
                if (_raiseException)
                {
                    throw new TdxConnectionException("Failed to connect to server.", ex);
                }
                return false;
            }

            Setup();

            // TODO: Implement Heartbeat logic
            // if (_heartbeat) { ... }

            return true;
        }

        public void Disconnect()
        {
            // TODO: Stop Heartbeat thread

            if (_socket != null)
            {
                try
                {
                    _socket.Shutdown(SocketShutdown.Both);
                    _socket.Close();
                    _socket = null;
                }
                catch (Exception ex)
                {
                    if (_raiseException)
                    {
                        throw new TdxConnectionException("Failed to disconnect.", ex);
                    }
                }
            }
        }

        protected abstract void Setup();

        protected T ApiRequest<T>(Func<Socket, T> sendAndRecv)
        {
            Func<T> apiCall = () =>
            {
                if (_multithread)
                {
                    Monitor.Enter(_lock);
                }

                try
                {
                    // Reset traffic stats for this call
                    // In the python version this seems to be handled inside the parser
                    // We will need to implement a similar logic
                    return sendAndRecv(_socket);
                }
                finally
                {
                    if (_multithread)
                    {
                        Monitor.Exit(_lock);
                    }
                }
            };

            if (!_autoRetry)
            {
                return apiCall();
            }

            // Auto-retry logic
            Exception lastException = null;
            foreach (var interval in RetryStrategy.GetIntervals())
            {
                try
                {
                    return apiCall();
                }
                catch (Exception ex)
                {
                    lastException = ex;
                    Task.Delay(interval).Wait();
                    Disconnect();
                    Connect(Ip, Port, _socket.SendTimeout);
                }
            }

            if (_raiseException)
            {
                throw new TdxFunctionCallException("Function call failed after multiple retries.", lastException);
            }

            return default(T);

        }

        public void Dispose()
        {
            Disconnect();
        }
    }
}
