using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

class Client
{
    private static Socket ServerSocket;
    private static bool Connected;

    public static void Main(string[] args)
    {
        ConnectToServer(args);

        if (Connected)
        {
            Console.WriteLine("Successfully connected to server");
            Console.WriteLine("Type exit to quit");
        }

        while (Connected)
        {
            Console.Write("Enter requested URL: ");
            string message = Console.ReadLine().ToLower();
            if (message.Length == 0) continue;
            if (message == "exit") break;

            RequestResourceFromServer(message);
            ReceiveResourceFromServer();
        }

        DisconnectFromServer();
    }

    private static void DisconnectFromServer()
    {
        try
        {
            ServerSocket.Shutdown(SocketShutdown.Both);
            ServerSocket.Close();
            Connected = false;
        }
        catch { }
    }

    private static void RequestResourceFromServer(string url)
    {
        try
        {
            ServerSocket.Send(Encoding.ASCII.GetBytes(string.Format("GET {0} HTTP/1.1\r\n\r\n", url)));
        }
        catch(Exception)
        {
            Console.WriteLine("Unable to connect to server");
            Connected = false;
        }
    }

    private static void ReceiveResourceFromServer()
    {
        try
        {
            byte[] data = new byte[1024];
            int dataLength = ServerSocket.Receive(data);
            string message = Encoding.ASCII.GetString(data, 0, dataLength);
            Console.WriteLine(message);
        }
        catch(Exception)
        {
            Console.WriteLine("An error occured when requesting this resource");
        }
    }

    private static string GetServerName(string[] args)
    {
        return args.Length > 0 ? args[0] : "localhost";
    }

    private static int GetPortNumber(string[] args)
    {
        return args.Length > 1 ? int.Parse(args[1]) : 8080;
    }

    private static void ConnectToServer(string[] args)
    {
        try
        {
            if (args.Length > 2) throw new ArgumentException("Parameters: <Server> <Port>");

            string serverName = GetServerName(args);
            int portNumber = GetPortNumber(args);
            ServerSocket = GetDefaultSocket();

            IPHostEntry ipHostEntry = Dns.GetHostEntry(serverName);
            IPAddress ipAddress = ipHostEntry.AddressList[1];
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddress, portNumber);

            ServerSocket.Connect(ipEndPoint);
            Connected = true;
        }
        catch(Exception)
        {
            Console.WriteLine("Unable to connect to server");
            Connected = false;
        }
    }

    private static Socket GetDefaultSocket()
    {
        return new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    }
}