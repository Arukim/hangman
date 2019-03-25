using System.Threading.Tasks;

namespace Hangman.Core
{
    /// <summary>
    /// Mimic windows-service
    /// This is used for workload implementations
    /// </summary>
    public interface IService
    {
        Task StartAsync();
        Task StopAsync();
    }
}
