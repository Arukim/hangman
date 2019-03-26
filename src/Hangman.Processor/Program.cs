using System;
using System.Threading.Tasks;

namespace Hangman.Processor
{
    class Program
    {
        static async Task Main(string[] args) => await new Application().RunAsync(args);
    }
}
