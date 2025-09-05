// C# Translation of pytdx/parser/get_security_count.py

using System.IO;
using System.Net.Sockets;
using System;

namespace Pytdx
{
    public class GetSecurityCountCommand : BaseCommand<int>
    {
        private readonly ushort _market;

        public GetSecurityCountCommand(Socket socket) : base(socket)
        {
        }

        public override int ParseResponse(byte[] body)
        {
            return BitConverter.ToUInt16(body, 0);
        }

        public override void SetParams(params object[] args)
        {
            if (args.Length != 1)
            {
                throw new ArgumentException("GetSecurityCountCommand requires one argument: market");
            }

            ushort market = Convert.ToUInt16(args[0]);

            using (var stream = new MemoryStream())
            using (var writer = new BinaryWriter(stream))
            {
                writer.Write(new byte[] { 0x0c, 0x0c, 0x18, 0x6c, 0x00, 0x01, 0x08, 0x00, 0x08, 0x00, 0x4e, 0x04 });
                writer.Write(market); // ushort is 2 bytes, little-endian by default
                writer.Write(new byte[] { 0x75, 0xc7, 0x33, 0x01 });
                SendPkg = stream.ToArray();
            }
        }
    }
}
