using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

class ChatServer
{
    private static TcpListener server;
    private static List<TcpClient> clients = new List<TcpClient>();

    static void Main(string[] args)
    {
        Console.WriteLine("Starting server...");
        server = new TcpListener(IPAddress.Any, 8888);
        server.Start();

        Console.WriteLine("Server is running. Waiting for connections...");
        while (true)
        {
            TcpClient client = server.AcceptTcpClient();
            clients.Add(client);
            Console.WriteLine("Client's connected!");

            Thread clientThread = new Thread(HandleClient);
            clientThread.Start(client);
        }
    }

    private static void HandleClient(object clientObject)
    {
        TcpClient client = (TcpClient)clientObject;
        NetworkStream stream = client.GetStream();
        byte[] buffer = new byte[1024];
        string username = "";

        try
        {
            while (true)
            {
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                if (bytesRead == 0) break;

                string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                if (string.IsNullOrEmpty(username))
                {
                    username = message;
                    Broadcast($"{username} joined the chat!");
                    continue;
                }

                Console.WriteLine($"{username}: {message}");
                Broadcast($"{username}: {message}");
            }
        }
        catch (Exception)
        {
            Console.WriteLine($"{username}'s client disconnected.");
        }
        finally
        {
            clients.Remove(client);
            Broadcast($"{username} leave the chat.");
            client.Close();
        }
    }

    private static void Broadcast(string message)
    {
        byte[] data = Encoding.UTF8.GetBytes(message);
        foreach (var client in clients)
        {
            try
            {
                client.GetStream().Write(data, 0, data.Length);
            }
            catch (Exception) { }
        }
    }
}
