// C# Translation of pytdx/parser/base.py

using System;
using System.IO;
using System.IO.Compression;
using System.Net.Sockets;

namespace Pytdx
{
    public abstract class BaseCommand<T>
    {
        private const int RspHeaderLen = 0x10;

        protected byte[]? SendPkg;
        private readonly Socket? _socket;

        protected BaseCommand(Socket? socket)
        {
            _socket = socket;
        }

        public abstract T ParseResponse(byte[] body);

        public abstract void SetParams(params object[] args);

        public T CallApi()
        {
            if (_socket == null)
            {
                throw new SocketClientNotReadyException("Socket client not ready.");
            }

            if (SendPkg == null)
            {
                throw new SendPkgNotReadyException("Send package not ready. Call SetParams first.");
            }

            int bytesSent = _socket.Send(SendPkg);
            if (bytesSent != SendPkg.Length)
            {
                throw new SendRequestPkgFailsException("Failed to send the complete request package.");
            }

            byte[] headerBuf = new byte[RspHeaderLen];
            int bytesReceived = _socket.Receive(headerBuf, RspHeaderLen, SocketFlags.None);
            if (bytesReceived != RspHeaderLen)
            {
                throw new ResponseHeaderRecvFailsException("Failed to receive the complete response header.");
            }

            // Unpack header: <IIIHH
            // We only need zipsize and unzipsize for now
            uint zipsize = BitConverter.ToUInt32(headerBuf, 8);
            ushort unzipsize = BitConverter.ToUInt16(headerBuf, 12);

            byte[] bodyBuf = new byte[zipsize];
            int totalBodyReceived = 0;
            while (totalBodyReceived < zipsize)
            {
                int received = _socket.Receive(bodyBuf, totalBodyReceived, (int)zipsize - totalBodyReceived, SocketFlags.None);
                if (received == 0)
                {
                    throw new ResponseRecvFailsException("Connection closed while receiving response body.");
                }
                totalBodyReceived += received;
            }

            byte[] finalBody;
            if (zipsize == unzipsize)
            {
                finalBody = bodyBuf;
            }
            else
            {
                // Decompress using ZLib
                using (var memoryStream = new MemoryStream(bodyBuf))
                // Skip the first two bytes of the zlib header
                using (var zlibStream = new ZLibStream(memoryStream, CompressionMode.Decompress))
                using (var resultStream = new MemoryStream())
                {
                    zlibStream.CopyTo(resultStream);
                    finalBody = resultStream.ToArray();
                }
            }

            return ParseResponse(finalBody);
        }
    }
}
