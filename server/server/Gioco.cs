using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;

namespace server
{
    internal class Gioco
    {
        public Gioco(TcpClient[] players)
        {
            if(players.Lenght == 2)
            {
                Task.Run(() => HandleClient(players[0]));
                Task.Run(() => HandleClient(players[1]));
            }
        }
        void Read(NetworkStream stream)
        {

        }
        void Write(NetworkStream stream)
        {

        }
        void HandleClient(TcpClient client)
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
        void AvviaAzione(string chiave, object parametro)
        {
            Dictionary<string, ParameterizedThreadStart> azioni = new Dictionary<string, ParameterizedThreadStart>
            {
                //{ "a", new ParameterizedThreadStart(a) }
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
    }
}
