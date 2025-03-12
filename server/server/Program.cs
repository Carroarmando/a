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
                Console.WriteLine("Client connesso!");
                Task.Run(() => HandleClient(client)); // Gestisce il client in un task separato
            }
        }

        static void HandleClient(TcpClient client)
        {
            try
            {
                NetworkStream stream = client.GetStream();
                byte[] buffer = new byte[1024];

                while (true)
                {
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);
                    if (bytesRead == 0) break; // Il client ha chiuso la connessione

                    string receivedData = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    Console.WriteLine("Messaggio ricevuto: " + receivedData);

                    // Risponde al client
                    string response = "Ricevuto: " + receivedData;
                    byte[] responseData = Encoding.UTF8.GetBytes(response);
                    stream.Write(responseData, 0, responseData.Length);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Errore: " + ex.Message);
            }
            finally
            {
                client.Close();
                Console.WriteLine("Client disconnesso.");
            }
        }
        static void AvviaAzione(string chiave, object parametro)
        {
            Dictionary<string, ParameterizedThreadStart> azioni = new Dictionary<string, ParameterizedThreadStart>
            {
                { "a", new ParameterizedThreadStart(a) }
            };

            if (azioni.ContainsKey(chiave))
            {
                Thread thread = new Thread(azioni[chiave]);
                thread.Start(parametro);
            }
            else
            {
                Console.WriteLine($"Nessuna azione trovata per '{chiave}'");
            }
        }

        public static void a(object b)
        {

        }
    }
}
