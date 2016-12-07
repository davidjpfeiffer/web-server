using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Server
{
    class Server
    {
        private static Socket ServerSocket;

        public static void Main(string[] args)
        {
            StartServer(args);
            ServerSocket.BeginAccept(new AsyncCallback(AcceptCallback), ServerSocket);
            while (true) { }
        }

        public static void AcceptCallback(IAsyncResult asyncResult)
        {
            ServerSocket.BeginAccept(new AsyncCallback(AcceptCallback), ServerSocket);
            Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                client = ServerSocket.EndAccept(asyncResult);

                StateObject state = new StateObject();
                state.client = client;
                client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReceiveCallback), state);
            }
            catch (Exception)
            {
                client.Close();
            }
        }

        public static void ReceiveCallback(IAsyncResult asyncResult)
        {
            StateObject state = (StateObject)asyncResult.AsyncState;
            Socket client = state.client;
            int bytesRead = client.EndReceive(asyncResult);

            if (bytesRead > 0)
            {
                Request request = new Request(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));
                byte[] data = GetRequestedResource(request);

                Console.WriteLine($@"Received client {GetClientAddress(client)} request: {request}");
                client.BeginSend(data, 0, data.Length, SocketFlags.None, SendCallback, state);
            }
        }

        public static void SendCallback(IAsyncResult asyncResult)
        {
            StateObject state = (StateObject)asyncResult.AsyncState;
            Socket client = state.client;
            client.Close();
        }

        private static void StartServer(string[] args)
        {
            if (args.Length > 1) throw new ArgumentException("Parameters: [<Port>]");

            int portNumber = args.Length == 1 ? int.Parse(args[0]) : 8080;
            ServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            ServerSocket.Bind(new IPEndPoint(IPAddress.Any, portNumber));
            ServerSocket.Listen(10);
        }

        private static IPAddress GetClientAddress(Socket client)
        {
            return ((IPEndPoint)client.RemoteEndPoint).Address;
        }

        private static byte[] GetRequestedResource(Request request)
        {
            string resourcePath;
            bool found = true;
            bool gif = false;

            switch (request.Url)
            {
                case "/":
                case "/index":
                case "/index.html":
                    resourcePath = "../../Resources/index.html";
                    break;

                case "/about":
                case "/about.html":
                    resourcePath = "../../Resources/about.html";
                    break;

                case "/funny.gif":
                    resourcePath = "../../Resources/funny.gif";
                    gif = true;
                    break;

                default:
                    resourcePath = "../../Resources/404.html";
                    found = false;
                    break;
            }

            if (gif)
            {
                byte[] headers = Encoding.ASCII.GetBytes(GetHeaders(found, gif));
                byte[] resource = File.ReadAllBytes(resourcePath);
                return headers.Concat(resource).ToArray();
            }
            else
            {
                string headers = GetHeaders(found, gif);
                string resource = File.ReadAllText(resourcePath);
                return Encoding.ASCII.GetBytes(headers + resource);
            }
        }

        private static string GetHeaders(bool found, bool gif)
        {
            string newLine = "\r\n";
            string headers = string.Empty;

            headers += found ? "HTTP/1.1 200 OK" : "HTTP/1.1 404 Not Found";
            headers += newLine;
            headers += gif ? "Content-type: image/gif" : "text/html";
            headers += newLine + newLine;

            return headers;
        }
    }
}