using System;
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
                    string request = Encoding.ASCII.GetString(state.buffer, 0, bytesRead);
                    Console.WriteLine($@"Client {GetClientAddress(client)} requested: {request}");
                    byte[] responseData = Controller.HandleRequest(request);
                    client.BeginSend(responseData, 0, responseData.Length, SocketFlags.None, new AsyncCallback(SendCallback), state);
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
    }
}