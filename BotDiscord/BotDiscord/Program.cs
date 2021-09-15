using System;
using System.Threading.Tasks;

namespace BotDiscord
{
    class Program
    {
        public static async Task Main(string[] args) 
            => await Startup.RunAsync(args);
    }
}