using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

class Client
{
    private static Socket ServerSocket;

    public static void Main(string[] args)
    {
        Console.WriteLine("You may type exit to quit at any time.");

        while (true)
        {
            Console.WriteLine("Enter the requested URL");
            string message = Console.ReadLine();
            if (message.Length == 0) continue;
            if (message == "exit") break;

            ConnectToServer(args);
            RequestResourceFromServer(message);
            ReceiveResourceFromServer();
            DisconnectFromServer();
        }
    }

    private static void DisconnectFromServer()
    {
        ServerSocket.Shutdown(SocketShutdown.Both);
        ServerSocket.Close();
    }

    private static void RequestResourceFromServer(string url)
    {
        ServerSocket.Send(Encoding.ASCII.GetBytes($@"GET {url}"));
    }

    private static void ReceiveResourceFromServer()
    {
        byte[] data = new byte[1024];
        int dataLength = ServerSocket.Receive(data);
        string message = Encoding.ASCII.GetString(data, 0, dataLength);
        Console.WriteLine(message);
    }

    private static string GetServerName(string[] args)
    {
        return args.Length > 0 ? args[0] : "localhost";
    }

    private static void ConnectToServer(string[] args)
    {
        if (args.Length > 2) throw new ArgumentException("Parameters: <Server> <Port>");

        string serverName = args.Length > 0 ? args[0] : "localhost";
        int portNumber = args.Length > 1 ? int.Parse(args[1]) : 8080;

        ServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        IPHostEntry ipHostEntry = Dns.GetHostEntry(serverName);
        IPAddress ipAddress = ipHostEntry.AddressList[1];
        IPEndPoint ipEndPoint = new IPEndPoint(ipAddress, portNumber);

        ServerSocket.Connect(ipEndPoint);
    }
}