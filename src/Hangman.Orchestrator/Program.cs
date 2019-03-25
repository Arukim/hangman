using System.Threading.Tasks;

namespace Hangman.Orchestrator
{
    class Program
    {
        static async Task Main(string[] args) => await new Application().RunAsync(args);
    }
}
