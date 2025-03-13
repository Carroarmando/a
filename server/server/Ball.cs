using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SFML.System;

namespace server
{
    internal class Ball
    {
        public Vector2f pos = new Vector2f();
        public int r = 1;
        int vel = 1;

        int inclinazzione = 35;

        public Ball()
        {
            Sposta();
        }

        void Sposta()
        {
            Thread.Sleep(3000);

            double angoloRad = inclinazzione * (Math.PI / 180.0);

            while (true)
            {
                pos += new Vector2f((float)Math.Cos(angoloRad) * vel, (float)Math.Sin(angoloRad) * vel);
                Thread.Sleep(16);
            }
        }

    }
}
