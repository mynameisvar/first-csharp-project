using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

class ChatClient
{
    private static TcpClient client;
    private static NetworkStream stream;

    static void Main(string[] args)
    {
        Console.Write("Enter the username: ");
        string username = Console.ReadLine();

        client = new TcpClient("127.0.0.1", 8888);
        stream = client.GetStream();

        SendMessage(username);

        Thread receiveThread = new Thread(ReceiveMessages);
        receiveThread.Start();

        Console.WriteLine("Welcome to the chat!");
        while (true)
        {
            string message = Console.ReadLine();
            SendMessage(message);
        }
    }

    private static void SendMessage(string message)
    {
        byte[] data = Encoding.UTF8.GetBytes(message);
        stream.Write(data, 0, data.Length);
    }

    private static void ReceiveMessages()
    {
        byte[] buffer = new byte[1024];
        while (true)
        {
            try
            {
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                Console.WriteLine(message);
            }
            catch (Exception)
            {
                Console.WriteLine("Connection to the server lost.");
                break;
            }
        }
    }
}
