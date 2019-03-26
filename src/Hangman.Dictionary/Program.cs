using System.Threading.Tasks;

namespace Hangman.Dictionary
{
    class Program
    {
        static async Task Main(string[] args) => await new Application().RunAsync(args);
    }
}
