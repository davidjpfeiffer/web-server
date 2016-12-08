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
            Console.WriteLine("Waiting for requests...");
            while (true) { }
        }

        public static void AcceptCallback(IAsyncResult asyncResult)
        {
            ServerSocket.BeginAccept(new AsyncCallback(AcceptCallback), ServerSocket);
            Socket client = GetDefaultSocket();

            try
            {
                client = ServerSocket.EndAccept(asyncResult);
                Console.WriteLine(string.Format("Connected with client {0}", GetClientAddress(client)));
                StateObject state = new StateObject(client);
                client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReceiveCallback), state);
            }
            catch (Exception)
            {
                client.Close();
            }
        }

        public static void ReceiveCallback(IAsyncResult asyncResult)
        {
            StateObject state = GetState(asyncResult);
            Socket client = state.client;

            try
            {
                int bytesRead = client.EndReceive(asyncResult);

                if (bytesRead > 0)
                {
                    Request request = new Request(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));
                    byte[] data = GetRequestedResource(request);

                    Console.WriteLine($@"Client {GetClientAddress(client)} requested: {request}");
                    client.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(SendCallback), state);
                }
            }
            catch(Exception)
            {
                Console.WriteLine(string.Format("Disconnected from client {0}", GetClientAddress(client)));
                client.Close();
            }
        }

        public static void SendCallback(IAsyncResult asyncResult)
        {
            StateObject state = GetState(asyncResult);
            Socket client = state.client;

            int bytesSent = client.EndSend(asyncResult);
            Console.WriteLine(string.Format("Sent {0} bytes to client {1}", bytesSent, GetClientAddress(client)));
            state = new StateObject(client);
            client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReceiveCallback), state);
        }

        private static void StartServer(string[] args)
        {
            if (args.Length > 1) throw new ArgumentException("Parameters: [<Port>]");

            int portNumber = args.Length == 1 ? int.Parse(args[0]) : 8080;
            ServerSocket = GetDefaultSocket();
            ServerSocket.Bind(new IPEndPoint(IPAddress.Any, portNumber));
            ServerSocket.Listen(10);
        }

        private static string GetClientAddress(Socket client)
        {
            IPEndPoint endPoint = (IPEndPoint)client.RemoteEndPoint;

            return string.Format("{0}:{1}", endPoint.Address, endPoint.Port);
        }

        private static StateObject GetState(IAsyncResult asyncResult)
        {
            return (StateObject)asyncResult.AsyncState;
        }

        private static Socket GetDefaultSocket()
        {
            return new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        private static byte[] GetRequestedResource(Request request)
        {
            string resourcePath;
            bool found = true;
            bool gif = false;

            switch (request.Url.ToLower())
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

            byte[] resource = gif ? File.ReadAllBytes(resourcePath) : Encoding.ASCII.GetBytes(File.ReadAllText(resourcePath));
            byte[] headers = Encoding.ASCII.GetBytes(GetHeaders(resource.Length, found, gif));

            return headers.Concat(resource).ToArray();
        }

        private static string GetHeaders(int contentLength, bool found, bool gif)
        {
            string newLine = "\r\n";
            string headers = string.Empty;

            headers += found ? "HTTP/1.1 200 OK" : "HTTP/1.1 404 Not Found";
            headers += newLine;
            headers += gif ? "Content-type: image/gif" : "Content-type: text/html";
            headers += newLine;
            headers += string.Format("Content-Length: {0}", contentLength);
            headers += newLine + newLine;

            return headers;
        }
    }
}