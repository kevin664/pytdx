// C# Translation of pytdx/parser/setup_commands.py

using System.Net.Sockets;

namespace Pytdx
{
    public class SetupCommand1 : BaseCommand<bool>
    {
        public SetupCommand1(Socket socket) : base(socket) { }

        public override bool ParseResponse(byte[] body)
        {
            return true; // Response body is not used, just indicate success
        }

        public override void SetParams(params object[] args)
        {
            // Hex: 0c 00 00 00 00 00 02 00 02 00 15 00
            SendPkg = new byte[] { 0x0c, 0x00, 0x00, 0x00, 0x00, 0x00, 0x02, 0x00, 0x02, 0x00, 0x15, 0x00 };
        }
    }

    public class SetupCommand2 : BaseCommand<bool>
    {
        public SetupCommand2(Socket socket) : base(socket) { }

        public override bool ParseResponse(byte[] body)
        {
            return true; // Response body is not used, just indicate success
        }

        public override void SetParams(params object[] args)
        {
            // Hex: 0c 02 18 94 00 01 03 00 03 00 0d 00 01
            SendPkg = new byte[] { 0x0c, 0x02, 0x18, 0x94, 0x00, 0x01, 0x03, 0x00, 0x03, 0x00, 0x0d, 0x00, 0x01 };
        }
    }

    public class SetupCommand3 : BaseCommand<bool>
    {
        public SetupCommand3(Socket socket) : base(socket) { }

        public override bool ParseResponse(byte[] body)
        {
            return true; // Response body is not used, just indicate success
        }

        public override void SetParams(params object[] args)
        {
            // Hex: 0c 03 18 99 00 01 20 00 20 00 db 0f b9 fa bd f0 d6 a4 c8 af 00 00 00 f6 28 f4 40 00 00 00 00 00 00 00 00 00 00 00 00 00 00 04
            SendPkg = new byte[] {
                0x0c, 0x03, 0x18, 0x99, 0x00, 0x01, 0x20, 0x00, 0x20, 0x00, 0xdb, 0x0f, 0xb9, 0xfa, 0xbd, 0xf0,
                0xd6, 0xa4, 0xc8, 0xaf, 0x00, 0x00, 0x00, 0xf6, 0x28, 0xf4, 0x40, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x04
            };
        }
    }
}
