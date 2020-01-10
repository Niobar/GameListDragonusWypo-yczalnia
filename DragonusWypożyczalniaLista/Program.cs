using System;

namespace DragonusWypożyczalniaLista
{
    class Program
    {
        static void Main(string[] args)
        {
            var dragonus = new DragonusDownloader();
            dragonus.Start();

        }
    }
}
