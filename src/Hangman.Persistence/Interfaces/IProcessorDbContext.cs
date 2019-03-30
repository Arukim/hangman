using Hangman.Persistence.Entities;
using MongoDB.Driver;
using System.Threading.Tasks;

namespace Hangman.Persistence.Interfaces
{
    /// <summary>
    /// TODO - add abstraction layer
    /// </summary>
    public interface IProcessorDbContext : IDbContext
    {
        IMongoCollection<TurnRegister> TurnRegisters { get; }
    }
}
