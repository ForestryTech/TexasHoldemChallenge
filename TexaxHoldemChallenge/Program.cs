using System;
using System.Collections.Generic;
using System.Linq;

namespace TexasHoldemChallenge
{
    class Program
    {
        static void Main() {
            Game texasHoldem = new Game();
            texasHoldem.Start();
            texasHoldem.Play();
            Console.ReadLine();
            
        }
    }
}
