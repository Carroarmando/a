using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;

namespace server
{
    internal class Program
    {
        List<TcpClient> clients = new List<TcpClient>();

        static void Main()
        {
            TcpListener listener = new TcpListener(IPAddress.Any, 5000);
            listener.Start();
            Console.WriteLine("Server in ascolto sulla porta 5000...");

            while (true)
            {
                TcpClient client = listener.AcceptTcpClient();
                lock(clients)
                {
                    clients.Add(client);
                    if (clients.Count >= 2)
                    {
                        new Gioco(new TcpClient[2] { clients[0], clients[1] });
                        clients.RemoveAt(0);
                        clients.RemoveAt(0);
                    }
                }
            }
        }
    }
}
