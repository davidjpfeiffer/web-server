using System.Net.Sockets;
using System.Text;

namespace Server
{
    public class StateObject
    {
        public Socket client = null;
        public const int BufferSize = 1024;
        public byte[] buffer = new byte[BufferSize];
        public StringBuilder data = new StringBuilder();
    }
}
