using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using SFML.System;

namespace server
{
    internal class Player
    {
        public int n;

        public int distance = 300;
        public TcpClient client;
        public Vector2f pos = new Vector2f();
        public int width = 1;

        public Player(TcpClient client, int n)
        {
            this.client = client;
            this.n = n;
            if (n == 0)
                pos = new Vector2f(0, distance);
            else
                pos = new Vector2f(0, -distance);
        }
    }
}
