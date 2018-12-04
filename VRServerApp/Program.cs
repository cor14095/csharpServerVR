using System;
using System.Net;
using System.Net.Sockets;


namespace VRServerApp
{
    class serv
    {
        static void Main(string[] args)
        {
            IPAddress address = IPAddress.Parse("127.0.0.1");   // Localhost.
            TcpListener serverSocket = new TcpListener(address, 8081);
            Socket clientSocket = default(Socket);
            int counter = 0;

            serverSocket.Start();
            Console.WriteLine(" >> " + "Server Started");

            counter = 0;
            while (true)
            {
                counter += 1;
                clientSocket = serverSocket.AcceptSocket();
                Console.WriteLine(" >> " + "Client No:" + Convert.ToString(counter) + " started!");
                HandleClient client = new HandleClient();
                client.StartClient(clientSocket, Convert.ToString(counter));
            }
        }
    }
}
