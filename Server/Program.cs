using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Server
{
    class Server
    {
        private static Socket ServerSocket;
        private static List<string> Log;
        private static uint DataSize = 1024;

        public static void Main(string[] args)
        {
            StartServer(args);

            while (true)
            {
                Log = new List<string>();
                byte[] data = new byte[DataSize];

                LogMessage("Waiting for a client...");
                Socket client = ServerSocket.Accept();
                LogMessage($@"Connected with {GetClientAddress(client)} at port {GetClientPort(client)}");

                LogMessage("Waiting for request from client...");
                Request request = GetRequestFromClient(client);
                if (request == null) continue;
                LogMessage($@"Received client request: {request}");

                data = Encoding.ASCII.GetBytes(GetRequestedResource(request));
                client.Send(data, SocketFlags.None);
                LogMessage("Resource sent to client");

                LogMessage(String.Format($@"Disconnecting from client {GetClientAddress(client)}"));
                client.Close();

                File.AppendAllLines("../../log.txt", Log);
            }
        }

        private static Request GetRequestFromClient(Socket client)
        {
            byte[] data = new byte[DataSize];
            int dataLength = client.Receive(data);

            if (dataLength == 0) return null;
            return new Request(Encoding.ASCII.GetString(data, 0, dataLength));
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

        private static int GetClientPort(Socket client)
        {
            return ((IPEndPoint)client.RemoteEndPoint).Port;
        }

        private static string GetRequestedResource(Request request)
        {
            string resourcePath;

            switch (request.Url)
            {
                case "/":
                case "/index":
                case "/index.html":
                    resourcePath = "../../Pages/index.html";
                    break;

                case "/about":
                case "/about.html":
                    resourcePath = "../../Pages/about.html";
                    break;

                default:
                    resourcePath = "../../Pages/404.html";
                    break;
            }

            return File.ReadAllText(resourcePath);
        }

        private static void LogMessage(string message)
        {
            Console.WriteLine(message);
            lock(Log)
            {
                Log.Add(message);
            }
        }
    }
}