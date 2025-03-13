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
        Dictionary<string, Func<string, Task>> actions;

        private object[] playerLocks = new object[2] { new object(), new object() };
        Player[] players = new Player[2];

        Ball ball = new Ball();
        public Gioco(TcpClient[] clients)
        {
            if(clients.Length == 2)
            {
                actions = new Dictionary<string, Func<string, Task>>
                {
                    { "pos", pos }
                };

                players[0] = new Player(clients[0], 0);
                players[1] = new Player(clients[1], 1);
                Task.Run(() => HandleClient(players[0]));
                Task.Run(() => HandleClient(players[1]));

                Task.Run(AggiornaPlayers);
            }
        }
        void Write(NetworkStream stream, string messageString)
        {
            try
            {
                byte[] messageByte = System.Text.Encoding.UTF8.GetBytes(messageString);

                int length = messageByte.Length;
                byte[] lengthByte = BitConverter.GetBytes(length);

                stream.Write(lengthByte, 0, 4);
                stream.Write(messageByte, 0, messageByte.Length);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Errore nell'invio del messaggio {messageString}: " + ex.Message);
            }
        }
        async Task HandleClient(Player player)
        {
            try
            {
                NetworkStream stream = player.client.GetStream();

                Write(stream, player.n.ToString());

                while (true)
                {
                    byte[] lunghezzaBytes = new byte[4];
                    int bytesRead = await stream.ReadAsync(lunghezzaBytes, 0, 4);

                    if (bytesRead == 0) // Client disconnesso
                        break;

                    int lunghezza = BitConverter.ToInt32(lunghezzaBytes, 0); // Converte i byte in int
                    byte[] dati = new byte[lunghezza];

                    // Assicura di leggere tutto il messaggio
                    int letti = 0;
                    while (letti < lunghezza)
                    {
                        int read = await stream.ReadAsync(dati, letti, lunghezza - letti);
                        if (read == 0) break; // Disconnessione
                        letti += read;
                    }

                    string message = Encoding.UTF8.GetString(dati);
                    Task.Run(() => AvviaAzione(message.Substring(0, 3), message.Substring(3)));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Errore: " + ex.Message);
            }
        }
        async Task AvviaAzione(string chiave, string parametro)
        {
            if (actions.TryGetValue(chiave, out var action))
                action(parametro);
            else
                Console.WriteLine($"Nessuna azione trovata per {chiave}");
        }
        async Task AggiornaPlayers()
        {
            while (true)
            {
                try
                {
                    string statoGiocatori = $"pos{players[0].pos.X}{players[0].width}{players[1].pos.X}{players[1].width}{ball.pos.X}{ball.pos.Y}{ball.r}";

                    foreach (var player in players)
                    {
                        if (player.client.Connected)
                        {
                            Write(player.client.GetStream(), statoGiocatori);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Errore nell'aggiornamento dello stato: " + ex.Message);
                }

                await Task.Delay(50); // 50ms per aggiornamenti fluidi
            }
        }
        async Task pos(string s)
        {
            int index = Convert.ToInt32(s[0].ToString());
            lock (playerLocks[index])
            {
                Player p = players[index];
                int x = Convert.ToInt32(s.Substring(1));
                p.pos = new SFML.System.Vector2f(x, p.pos.Y);
            }
        }
    }
}
